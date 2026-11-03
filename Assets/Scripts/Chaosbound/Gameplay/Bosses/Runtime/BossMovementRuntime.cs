using System;
using System.Collections.Generic;
using Chaosbound.Content.Enemy.Bosses;
using UnityEngine;
using UnityEngine.AI;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Sole movement and rotation authority for one Boss instance.
    /// Requires explicit Initialize; OnEnable never starts movement.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class BossMovementRuntime : MonoBehaviour
    {
        private const float DestinationRefreshDistance = 0.5f;
        private const float DirectionEpsilon = 0.0001f;

        private readonly Dictionary<int, BossMovementLockOwner> movementLocks =
            new Dictionary<int, BossMovementLockOwner>();

        private readonly Dictionary<int, BossMovementLockOwner> facingLocks =
            new Dictionary<int, BossMovementLockOwner>();

        private NavMeshAgent agent;
        private BossMovementSettings settings;
        private Transform target;
        private BossMovementState state = BossMovementState.Idle;

        private int nextLockId = 1;
        private float retreatCooldownRemaining;
        private float retreatRequestedDistance;
        private Vector3 retreatStartPosition;
        private Vector3 retreatDestination;
        private Vector3 lastApproachDestination;
        private bool hasApproachDestination;
        private bool isInitialized;
        private bool isFacingRequestActive;
        private FacingPolicy facingPolicy;
        private Transform facingTarget;
        private Vector3 fixedFacingDirection;
        private Vector3 capturedFacingDirection;
        private float originalAgentSpeed;
        private Vector3 activeDestination;
        private Vector3 lastProgressPosition;
        private bool hasActiveDestination;
        private float navigationNoProgressTimer;
        private float navigationRecoveryTimer;
        private int navigationRecoveryAttempts;

        public BossMovementState State => state;
        public bool IsInitialized => isInitialized;
        public bool IsAvailable =>
            isInitialized && agent != null && agent.isOnNavMesh &&
            state != BossMovementState.Unavailable;
        public bool HasMovementLocks => movementLocks.Count > 0;
        public bool HasFacingLocks => facingLocks.Count > 0;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                originalAgentSpeed = agent.speed;
                agent.updateRotation = false;
            }
        }

        public bool Initialize(BossRuntimeContext context)
        {
            if (context == null || !context.IsInitialized)
            {
                Debug.LogError(
                    "BossMovementRuntime requires an initialized BossRuntimeContext.",
                    this);
                SetUnavailable();
                return false;
            }

            if (agent == null)
                agent = GetComponent<NavMeshAgent>();

            if (agent == null)
            {
                Debug.LogError("Boss prefab is missing NavMeshAgent.", this);
                SetUnavailable();
                return false;
            }

            if (!agent.enabled || !agent.isOnNavMesh)
            {
                Debug.LogError(
                    "Boss NavMeshAgent must be enabled and placed on a NavMesh before initialization.",
                    this);
                SetUnavailable();
                return false;
            }

            settings = context.CombatDefinition.MovementSettings;
            if (!settings.IsValid)
            {
                Debug.LogError("BossMovementSettings are invalid.", this);
                SetUnavailable();
                return false;
            }

            target = null;
            movementLocks.Clear();
            facingLocks.Clear();
            retreatCooldownRemaining = 0f;
            retreatRequestedDistance = 0f;
            hasApproachDestination = false;
            isFacingRequestActive = false;
            ResetNavigationWatchdog(true);

            originalAgentSpeed = agent.speed;
            agent.updateRotation = false;
            agent.stoppingDistance = settings.StoppingDistance;
            agent.isStopped = true;
            agent.ResetPath();

            state = BossMovementState.Idle;
            isInitialized = true;
            return true;
        }

        public void SetTarget(Transform newTarget)
        {
            if (!isInitialized)
                return;

            target = newTarget;
            hasApproachDestination = false;
        }

        public void ClearTarget()
        {
            target = null;
            hasApproachDestination = false;
            StopNavigation();
            ResetNavigationWatchdog(true);
            if (isInitialized && state != BossMovementState.Unavailable)
                state = HasMovementLocks ? BossMovementState.MovementLocked : BossMovementState.Idle;
        }

        /// <summary>
        /// Requests movement toward the preferred-distance band.
        /// Movement is not issued while any movement lock is active.
        /// </summary>
        public bool RequestApproach()
        {
            if (!CanMove() || target == null)
                return false;

            float distance = HorizontalDistance(transform.position, target.position);
            if (distance <= settings.PreferredDistance)
            {
                RequestHoldPosition();
                return true;
            }

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= DirectionEpsilon)
            {
                RequestHoldPosition();
                return true;
            }

            direction.Normalize();
            Vector3 destination = target.position - direction * settings.PreferredDistance;
            destination.y = transform.position.y;

            if (!TrySetDestination(destination))
            {
                RequestHoldPosition();
                return false;
            }

            bool startingApproach = state != BossMovementState.Approach;
            lastApproachDestination = destination;
            hasApproachDestination = true;
            state = BossMovementState.Approach;
            if (startingApproach)
                ResetNavigationWatchdog();
            return true;
        }

        /// <summary>
        /// Stops at the current position. Hold does not continuously correct position.
        /// </summary>
        public bool RequestHoldPosition()
        {
            if (!isInitialized || HasMovementLocks || !AgentReady())
                return false;

            StopNavigation();
            state = BossMovementState.HoldPosition;
            hasApproachDestination = false;
            ResetNavigationWatchdog(true);
            return true;
        }

        /// <summary>
        /// Starts a bounded retreat. The caller decides when retreat is required.
        /// Requested distance is measured as actual horizontal displacement.
        /// </summary>
        public bool TryBeginRetreat(float requestedDistance)
        {
            if (!CanMove() || target == null || requestedDistance <= 0f ||
                retreatCooldownRemaining > 0f)
                return false;

            Vector3 away = transform.position - target.position;
            away.y = 0f;

            if (away.sqrMagnitude <= DirectionEpsilon)
                away = -transform.forward;

            away.y = 0f;
            if (away.sqrMagnitude <= DirectionEpsilon)
                return false;

            away.Normalize();

            retreatRequestedDistance = Mathf.Min(
                requestedDistance,
                settings.MaximumRetreatDistance);

            retreatStartPosition = transform.position;
            Vector3 desiredDestination =
                retreatStartPosition + away * retreatRequestedDistance;

            NavMeshHit hit;
            if (!NavMesh.SamplePosition(
                desiredDestination,
                out hit,
                Mathf.Max(1f, agent.radius * 2f),
                agent.areaMask))
            {
                return false;
            }

            if (!TrySetDestination(hit.position))
                return false;

            retreatDestination = hit.position;
            ResetNavigationWatchdog();
            agent.speed = settings.RetreatSpeed;
            retreatCooldownRemaining = settings.RetreatCooldown;
            state = BossMovementState.Retreat;
            hasApproachDestination = false;
            return true;
        }

        public int LockMovement(BossMovementLockOwner owner)
        {
            int token = nextLockId++;
            movementLocks.Add(token, owner);

            if (isInitialized)
            {
                StopNavigation();
                state = BossMovementState.MovementLocked;
            }

            return token;
        }

        public bool UnlockMovement(int token)
        {
            if (!movementLocks.Remove(token))
                return false;

            if (isInitialized && movementLocks.Count == 0 &&
                state == BossMovementState.MovementLocked)
            {
                state = BossMovementState.Idle;
            }

            return true;
        }

        public int LockFacing(BossMovementLockOwner owner)
        {
            int token = nextLockId++;
            facingLocks.Add(token, owner);
            return token;
        }

        public bool UnlockFacing(int token)
        {
            return facingLocks.Remove(token);
        }

        public void RequestFacing(
            FacingPolicy policy,
            Transform facingTarget,
            Vector3 fixedDirection)
        {
            if (!isInitialized)
                return;

            facingPolicy = policy;
            this.facingTarget = facingTarget;
            fixedFacingDirection = Flatten(fixedDirection);
            // Explicit NoRotation must override the default combat facing.
            isFacingRequestActive = true;

            if (policy == FacingPolicy.FaceTargetAtAttackStart &&
                facingTarget != null)
            {
                capturedFacingDirection =
                    Flatten(facingTarget.position - transform.position).normalized;
            }
            else if (policy == FacingPolicy.FixedDirection)
            {
                capturedFacingDirection = fixedFacingDirection.normalized;
            }
            else
            {
                capturedFacingDirection = Vector3.zero;
            }
        }

        public void ClearFacingRequest()
        {
            isFacingRequestActive = false;
            facingTarget = null;
            capturedFacingDirection = Vector3.zero;
        }

        private void Update()
        {
            if (!isInitialized)
                return;

            float deltaTime = Time.deltaTime;
            if (retreatCooldownRemaining > 0f)
                retreatCooldownRemaining = Mathf.Max(
                    0f, retreatCooldownRemaining - deltaTime);

            if (!AgentReady())
            {
                SetUnavailable();
                return;
            }

            if (HasMovementLocks)
            {
                state = BossMovementState.MovementLocked;
                StopNavigation();
                // Movement is locked, but a facing policy may still rotate the boss
                // (for example, FaceTargetDuringAnticipation).
                TickFacing(deltaTime);
                return;
            }

            if (state == BossMovementState.Approach)
            {
                TickApproach();
                if (state == BossMovementState.Approach)
                    TickNavigationWatchdog(deltaTime, false);
            }
            else if (state == BossMovementState.Retreat)
            {
                TickRetreat();
                if (state == BossMovementState.Retreat)
                    TickNavigationWatchdog(deltaTime, true);
            }

            TickFacing(deltaTime);
        }

        private void TickApproach()
        {
            if (target == null)
            {
                RequestHoldPosition();
                return;
            }

            float distance = HorizontalDistance(transform.position, target.position);
            if (distance <= settings.PreferredDistance)
            {
                RequestHoldPosition();
                return;
            }

            Vector3 direction = Flatten(target.position - transform.position);
            if (direction.sqrMagnitude <= DirectionEpsilon)
            {
                RequestHoldPosition();
                return;
            }

            direction.Normalize();
            Vector3 desired = target.position - direction * settings.PreferredDistance;
            desired.y = transform.position.y;

            if (!hasApproachDestination ||
                HorizontalDistance(desired, lastApproachDestination) >=
                DestinationRefreshDistance)
            {
                if (!TrySetDestination(desired))
                {
                    RequestHoldPosition();
                    return;
                }

                lastApproachDestination = desired;
                hasApproachDestination = true;
            }

            if (agent.pathPending)
                return;

        }

        private void TickRetreat()
        {
            float travelled = HorizontalDistance(transform.position, retreatStartPosition);
            if (travelled >= retreatRequestedDistance ||
                (!agent.pathPending && !agent.hasPath && agent.remainingDistance <=
                 agent.stoppingDistance + 0.05f))
            {
                FinishRetreat();
                return;
            }

        }

        private void FinishRetreat()
        {
            StopNavigation();
            agent.speed = originalAgentSpeed;
            state = BossMovementState.HoldPosition;
            ResetNavigationWatchdog(true);
        }

        private void TickNavigationWatchdog(float deltaTime, bool retreat)
        {
            if (agent.pathPending)
                return;

            bool reachedCompleteDestination = agent.hasPath &&
                agent.pathStatus == NavMeshPathStatus.PathComplete &&
                agent.remainingDistance <= agent.stoppingDistance + 0.05f;
            bool reachedPartialEndpoint = agent.hasPath &&
                agent.pathStatus == NavMeshPathStatus.PathPartial &&
                agent.remainingDistance <= agent.stoppingDistance + 0.05f;
            bool destinationReachedWithoutPath = !agent.hasPath &&
                hasActiveDestination &&
                HorizontalDistance(transform.position, activeDestination) <=
                agent.stoppingDistance + 0.1f;

            if (reachedCompleteDestination || reachedPartialEndpoint ||
                destinationReachedWithoutPath)
            {
                if (retreat)
                    FinishRetreat();
                else
                    RequestHoldPosition();
                return;
            }

            if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                TryRecoverNavigation(retreat);
                return;
            }

            float progress = HorizontalDistance(transform.position, lastProgressPosition);
            if (progress >= settings.NavigationProgressThreshold)
            {
                lastProgressPosition = transform.position;
                navigationNoProgressTimer = 0f;
                navigationRecoveryTimer = 0f;
                return;
            }

            navigationNoProgressTimer += deltaTime;
            if (navigationNoProgressTimer < settings.NavigationNoProgressTimeout)
                return;

            navigationRecoveryTimer += deltaTime;
            if (navigationRecoveryAttempts < settings.NavigationRecoveryAttempts)
            {
                TryRecoverNavigation(retreat);
                return;
            }

            if (navigationRecoveryTimer >= settings.NavigationRecoveryTimeout)
                AbortNavigationGracefully(retreat);
        }

        private void TryRecoverNavigation(bool retreat)
        {
            if (!AgentReady() || !hasActiveDestination)
            {
                AbortNavigationGracefully(retreat);
                return;
            }

            if (navigationRecoveryAttempts >= settings.NavigationRecoveryAttempts)
            {
                AbortNavigationGracefully(retreat);
                return;
            }

            navigationRecoveryAttempts++;
            navigationNoProgressTimer = 0f;
            navigationRecoveryTimer = 0f;
            lastProgressPosition = transform.position;

            agent.isStopped = false;
            if (!agent.SetDestination(activeDestination))
                AbortNavigationGracefully(retreat);
        }

        private void AbortNavigationGracefully(bool retreat)
        {
            if (retreat)
            {
                FinishRetreat();
                return;
            }

            RequestHoldPosition();
            ResetNavigationWatchdog(true);
        }

        private void ResetNavigationWatchdog(bool clearDestination = false)
        {
            if (clearDestination)
            {
                activeDestination = Vector3.zero;
                hasActiveDestination = false;
            }

            lastProgressPosition = transform.position;
            navigationNoProgressTimer = 0f;
            navigationRecoveryTimer = 0f;
            navigationRecoveryAttempts = 0;
        }

        private void TickFacing(float deltaTime)
        {
            if (HasFacingLocks)
                return;

            Vector3 direction = Vector3.zero;

            if (isFacingRequestActive)
            {
                switch (facingPolicy)
                {
                    case FacingPolicy.FaceTargetDuringAnticipation:
                        if (facingTarget != null)
                            direction = Flatten(facingTarget.position - transform.position);
                        break;

                    case FacingPolicy.FaceTargetAtAttackStart:
                    case FacingPolicy.FixedDirection:
                        direction = capturedFacingDirection;
                        break;

                    case FacingPolicy.NoRotation:
                        return;
                }
            }
            else if (target != null)
            {
                // Default combat behavior: continuously face the player while
                // idle, holding position, approaching, or retreating.
                direction = Flatten(target.position - transform.position);
            }

            if (direction.sqrMagnitude <= DirectionEpsilon)
                return;

            Quaternion desiredRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                desiredRotation,
                settings.RotationSpeed * deltaTime);
        }

        private bool CanMove()
        {
            return isInitialized && !HasMovementLocks && AgentReady();
        }

        private bool AgentReady()
        {
            return agent != null && agent.enabled && agent.isOnNavMesh;
        }

        private bool TrySetDestination(Vector3 destination)
        {
            if (!AgentReady())
                return false;

            agent.isStopped = false;
            if (!agent.SetDestination(destination))
                return false;

            activeDestination = destination;
            hasActiveDestination = true;
            return true;
        }

        private void StopNavigation()
        {
            if (agent == null || !agent.enabled || !agent.isOnNavMesh)
                return;

            agent.isStopped = true;
            agent.ResetPath();
            agent.speed = originalAgentSpeed;
        }

        private void SetUnavailable()
        {
            StopNavigation();
            state = BossMovementState.Unavailable;
        }

        private static Vector3 Flatten(Vector3 value)
        {
            value.y = 0f;
            return value;
        }

        private static float HorizontalDistance(Vector3 a, Vector3 b)
        {
            a.y = 0f;
            b.y = 0f;
            return Vector3.Distance(a, b);
        }

        public void ResetRuntime()
        {
            StopNavigation();

            movementLocks.Clear();
            facingLocks.Clear();
            target = null;
            facingTarget = null;
            isFacingRequestActive = false;
            hasApproachDestination = false;
            retreatCooldownRemaining = 0f;
            retreatRequestedDistance = 0f;
            ResetNavigationWatchdog(true);
            isInitialized = false;
            state = BossMovementState.Idle;

            if (agent != null)
                agent.updateRotation = false;
        }

        private void OnDisable()
        {
            ResetRuntime();
        }
    }
}
