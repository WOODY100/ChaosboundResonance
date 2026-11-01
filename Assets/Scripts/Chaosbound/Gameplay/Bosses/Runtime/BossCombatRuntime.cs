using Chaosbound.Content.Enemy.Bosses;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Owns phase progression, attack selection, sequence execution and Wait.
    /// Combat starts only after an explicit StartCombat call.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BossRuntimeContext))]
    [RequireComponent(typeof(BossHealth))]
    [RequireComponent(typeof(BossMovementRuntime))]
    public sealed class BossCombatRuntime : MonoBehaviour
    {
        [SerializeField, Min(0f)]
        private float failureRetryDelay = 1f;

        private readonly Dictionary<string, float> cooldowns =
            new Dictionary<string, float>(StringComparer.Ordinal);

        private readonly List<BossAttackDefinition> candidateBuffer =
            new List<BossAttackDefinition>();

        private readonly HashSet<string> attemptedThisCycle =
            new HashSet<string>(StringComparer.Ordinal);

        private BossRuntimeContext runtimeContext;
        private BossHealth health;
        private BossMovementRuntime movement;
        private BossAttackExecutorSystem executorSystem;
        private IBossAttackExecution activeExecution;
        private BossAttackExecutionContext activeExecutionContext;
        private BossPhaseDefinition activePhase;
        private BossPhaseDefinition pendingPhase;
        private Transform playerTarget;
        private BossCombatLifecycleState lifecycleState =
            BossCombatLifecycleState.Uninitialized;

        private float waitRemaining;
        private bool cooldownConsumedThisSequence;
        private string lastStartedAttackId;
        private int combatMovementLock = -1;
        private Vector3 targetPositionAtExecutionStart;
        private bool hasTargetPositionAtExecutionStart;
        private bool subscribedToDeath;

        public BossCombatLifecycleState LifecycleState => lifecycleState;
        public BossPhaseDefinition ActivePhase => activePhase;
        public bool HasActiveExecution => activeExecution != null;
        public Transform PlayerTarget => playerTarget;

        /// <summary>
        /// Presentation-only notification. Subscribers may play visuals/animations,
        /// but must never use this event to control attack timing or damage.
        /// </summary>
        public event Action<BossAttackDefinition, BossAttackExecutionState>
            PresentationStateChanged;

        /// <summary>
        /// Executors are registered per Boss instance before Initialize.
        /// Executor instances must not be shared between Bosses.
        /// </summary>
        public bool RegisterExecutor(IBossAttackExecutor executor)
        {
            if (lifecycleState != BossCombatLifecycleState.Uninitialized)
                throw new InvalidOperationException(
                    "Executors must be registered before BossCombatRuntime.Initialize.");

            if (executorSystem == null)
                executorSystem = new BossAttackExecutorSystem(failureRetryDelay);

            return executorSystem.Register(executor);
        }

        public bool Initialize()
        {
            if (lifecycleState != BossCombatLifecycleState.Uninitialized)
                return false;

            runtimeContext = GetComponent<BossRuntimeContext>();
            health = GetComponent<BossHealth>();
            movement = GetComponent<BossMovementRuntime>();

            if (runtimeContext == null || !runtimeContext.IsInitialized ||
                health == null || movement == null)
            {
                return MarkUnavailable("Boss combat dependencies are not initialized.");
            }

            playerTarget = runtimeContext.PlayerTarget;
            if (playerTarget == null)
                return MarkUnavailable("The expedition Player reference is unavailable.");

            BossCombatDefinition definition = runtimeContext.CombatDefinition;

            if (definition == null)
            {
                return MarkUnavailable(
                    "BossCombatDefinition is missing.");
            }

            if (!definition.Validate(out string error))
            {
                return MarkUnavailable(
                    "BossCombatDefinition is invalid: " + error);
            }

            if (!movement.IsInitialized && !movement.Initialize(runtimeContext))
                return MarkUnavailable("BossMovementRuntime could not initialize.");

            movement.SetTarget(playerTarget);

            if (executorSystem == null)
                executorSystem = new BossAttackExecutorSystem(failureRetryDelay);

            activePhase = definition.Phases[0];
            pendingPhase = null;
            cooldowns.Clear();
            attemptedThisCycle.Clear();
            lastStartedAttackId = null;
            waitRemaining = 0f;
            cooldownConsumedThisSequence = false;

            health.OnDeath += HandleDeath;
            subscribedToDeath = true;

            lifecycleState = health.IsDead
                ? BossCombatLifecycleState.Dead
                : BossCombatLifecycleState.Ready;

            if (lifecycleState == BossCombatLifecycleState.Dead)
                HandleDeath(health);

            return lifecycleState == BossCombatLifecycleState.Ready;
        }

        public bool StartCombat()
        {
            if (lifecycleState != BossCombatLifecycleState.Ready)
                return false;

            lifecycleState = BossCombatLifecycleState.Active;
            waitRemaining = 0f;
            return true;
        }

        public void StopCombat()
        {
            if (lifecycleState != BossCombatLifecycleState.Active)
                return;

            lifecycleState = BossCombatLifecycleState.Ready;
            CancelActiveExecution();
            ReleaseCombatLocks();
            movement?.RequestHoldPosition();
        }

        private void Update()
        {
            if (lifecycleState != BossCombatLifecycleState.Active)
                return;

            float deltaTime = Time.deltaTime;
            TickCooldowns(deltaTime);
            executorSystem?.Tick(deltaTime);

            if (health == null || health.IsDead)
            {
                HandleDeath(health);
                return;
            }

            if (playerTarget == null)
            {
                EnterWait();
                return;
            }

            EvaluatePendingPhase();

            if (activeExecution != null)
            {
                TickActiveExecution(deltaTime);
                return;
            }

            // Keep the preferred-distance band updated during combat waits.
            // An active retreat finishes before another attack can begin.
            RepositionOrHold();
            if (movement != null &&
                movement.State == BossMovementState.Retreat)
            {
                return;
            }

            if (waitRemaining > 0f)
            {
                waitRemaining -= deltaTime;
                if (waitRemaining > 0f)
                    return;
            }

            ApplyPendingPhaseIfPossible();

            if (TryStartAttack())
                return;

            RepositionOrHold();
            EnterWait();
        }

        private bool TryStartAttack()
        {
            candidateBuffer.Clear();
            attemptedThisCycle.Clear();

            IReadOnlyList<BossAttackDefinition> attacks = activePhase.AvailableAttacks;
            float distance = HorizontalDistance(transform.position, playerTarget.position);

            for (int i = 0; i < attacks.Count; i++)
            {
                BossAttackDefinition attack = attacks[i];
                if (attack == null || !attack.Validate(out _))
                    continue;
                if (attemptedThisCycle.Contains(attack.ContentId))
                    continue;
                if (cooldowns.TryGetValue(attack.ContentId, out float remaining) &&
                    remaining > 0f)
                    continue;
                if (executorSystem == null || !executorSystem.IsAttackAvailable(attack))
                    continue;
                if (distance < attack.MinimumRange || distance > attack.MaximumRange)
                    continue;

                candidateBuffer.Add(attack);
            }

            if (candidateBuffer.Count > 1 && !string.IsNullOrEmpty(lastStartedAttackId))
            {
                candidateBuffer.RemoveAll(
                    attack => attack.ContentId == lastStartedAttackId);
            }

            while (candidateBuffer.Count > 0)
            {
                BossAttackDefinition selected = SelectWeightedCandidate();
                candidateBuffer.Remove(selected);

                BossAttackExecutionContext context =
                    new BossAttackExecutionContext(
                        this,
                        playerTarget,
                        selected,
                        activePhase.PhaseId,
                        runtimeContext.CombatDefinition.BaseStats.BaseDamage,
                        playerTarget.position,
                        transform.position);

                if (!executorSystem.TryCreateExecution(context, out IBossAttackExecution execution))
                {
                    attemptedThisCycle.Add(selected.ContentId);
                    continue;
                }

                activeExecution = execution;
                activeExecutionContext = context;
                cooldownConsumedThisSequence = false;
                lastStartedAttackId = selected.ContentId;
                activeExecution.ReleaseRequested += HandleReleaseRequested;
                activeExecution.StateChanged += HandleExecutionStateChanged;
                NotifyPresentationState(
                    selected,
                    BossAttackExecutionState.Anticipation);
                targetPositionAtExecutionStart = Vector3.zero;
                hasTargetPositionAtExecutionStart = false;

                // The combat lock prevents translation, not facing. The selected
                // facing policy remains authoritative throughout its valid stage.
                combatMovementLock = movement.LockMovement(BossMovementLockOwner.Combat);
                movement.RequestFacing(
                    selected.FacingPolicy,
                    playerTarget,
                    transform.forward);

                waitRemaining = 0f;
                return true;
            }

            return false;
        }

        private BossAttackDefinition SelectWeightedCandidate()
        {
            float totalWeight = 0f;
            for (int i = 0; i < candidateBuffer.Count; i++)
                totalWeight += candidateBuffer[i].SelectionWeight;

            float roll = UnityEngine.Random.value * totalWeight;
            for (int i = 0; i < candidateBuffer.Count; i++)
            {
                roll -= candidateBuffer[i].SelectionWeight;
                if (roll <= 0f)
                    return candidateBuffer[i];
            }

            return candidateBuffer[candidateBuffer.Count - 1];
        }

        private void TickActiveExecution(float deltaTime)
        {
            activeExecution.Tick(deltaTime);

            if (activeExecution == null)
                return;

            if (activeExecution.State == BossAttackExecutionState.Completed ||
                activeExecution.State == BossAttackExecutionState.Cancelled ||
                activeExecution.State == BossAttackExecutionState.Failed)
            {
                FinishActiveExecution();
            }
        }

        private void HandleExecutionStateChanged(BossAttackExecutionState state)
        {
            if (activeExecutionContext == null)
                return;

            NotifyPresentationState(
                activeExecutionContext.AttackDefinition,
                state);

            if (playerTarget == null)
                return;

            if (state == BossAttackExecutionState.Execution &&
                !hasTargetPositionAtExecutionStart)
            {
                targetPositionAtExecutionStart = playerTarget.position;
                hasTargetPositionAtExecutionStart = true;

                // FaceTargetDuringAnticipation ends when the execution stage begins.
                if (activeExecutionContext.AttackDefinition.FacingPolicy ==
                    FacingPolicy.FaceTargetDuringAnticipation)
                {
                    movement.ClearFacingRequest();
                }
            }
        }

        private void NotifyPresentationState(
            BossAttackDefinition attack,
            BossAttackExecutionState state)
        {
            PresentationStateChanged?.Invoke(attack, state);
        }

        private void HandleReleaseRequested(int releaseIndex)
        {
            if (activeExecution == null ||
                activeExecutionContext == null ||
                lifecycleState != BossCombatLifecycleState.Active)
                return;

            BossAttackExecutionContext sequence = activeExecutionContext;
            BossAttackDefinition attack = sequence.AttackDefinition;

            Vector3 capturedPosition = Vector3.zero;
            bool hasCapturedPosition = false;
            Vector3 releaseOrigin = transform.position;

            switch (attack.TargetCapturePolicy)
            {
                case TargetCapturePolicy.AtAttackStart:
                    capturedPosition = sequence.TargetPositionAtAttackStart;
                    hasCapturedPosition = true;
                    break;

                case TargetCapturePolicy.AtExecutionStart:
                    if (!hasTargetPositionAtExecutionStart)
                    {
                        // Defensive fallback for executions that release immediately
                        // on entering Execution before the state notification is observed.
                        targetPositionAtExecutionStart = playerTarget.position;
                        hasTargetPositionAtExecutionStart = true;
                    }
                    capturedPosition = targetPositionAtExecutionStart;
                    hasCapturedPosition = true;
                    break;

                case TargetCapturePolicy.AtRelease:
                    capturedPosition = playerTarget.position;
                    hasCapturedPosition = true;
                    break;

                case TargetCapturePolicy.FixedOrigin:
                    // FixedOrigin deliberately does not capture a moving target.
                    // The effect origin remains anchored to the boss position at attack start.
                    releaseOrigin = sequence.OriginAtAttackStart;
                    break;
            }

            Vector3 direction = transform.forward;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.0001f)
                direction.Normalize();

            BossAttackReleaseContext releaseContext =
                new BossAttackReleaseContext(
                    releaseOrigin,
                    capturedPosition,
                    hasCapturedPosition,
                    direction,
                    sequence.EffectiveDamage,
                    BossEffectOwnership.BossBound);

            BossAttackMaterializationResult result =
                executorSystem.TryRelease(sequence, releaseContext, releaseIndex);

            if (result == BossAttackMaterializationResult.Success &&
                !cooldownConsumedThisSequence)
            {
                cooldowns[attack.ContentId] = attack.Cooldown;
                cooldownConsumedThisSequence = true;
            }
        }

        private void FinishActiveExecution()
        {
            if (activeExecution != null)
            {
                activeExecution.ReleaseRequested -= HandleReleaseRequested;
                activeExecution.StateChanged -= HandleExecutionStateChanged;
                activeExecution.Dispose();
            }

            activeExecution = null;
            activeExecutionContext = null;

            ReleaseCombatLocks();
            movement.ClearFacingRequest();
            hasTargetPositionAtExecutionStart = false;

            ApplyPendingPhaseIfPossible();
            waitRemaining = runtimeContext.CombatDefinition.CombatSettings
                .WaitReevaluationInterval;
        }

        private void ApplyPendingPhaseIfPossible()
        {
            if (activeExecution != null || pendingPhase == null)
                return;

            activePhase = pendingPhase;
            pendingPhase = null;
        }

        private void EvaluatePendingPhase()
        {
            IReadOnlyList<BossPhaseDefinition> phases =
                runtimeContext.CombatDefinition.Phases;

            float healthPercent = health.GetHealthPercent() * 100f;
            BossPhaseDefinition candidate = activePhase;

            for (int i = 1; i < phases.Count; i++)
            {
                if (healthPercent <= phases[i].HealthThreshold)
                    candidate = phases[i];
            }

            if (candidate != activePhase)
                pendingPhase = candidate;
        }

        private void RepositionOrHold()
        {
            if (movement == null || !movement.IsAvailable)
                return;

            BossMovementSettings movementSettings =
                runtimeContext.CombatDefinition.MovementSettings;

            float distance = HorizontalDistance(transform.position, playerTarget.position);

            // Do not replace the destination while a bounded retreat is active.
            if (movement.State == BossMovementState.Retreat)
                return;

            if (distance < movementSettings.MinimumDistance)
            {
                // Recover only the distance needed to reach PreferredDistance.
                // TryBeginRetreat enforces MaximumRetreatDistance and cooldown.
                float retreatDistance =
                    movementSettings.PreferredDistance - distance;

                if (!movement.TryBeginRetreat(retreatDistance) &&
                    movement.State != BossMovementState.HoldPosition)
                {
                    movement.RequestHoldPosition();
                }

                return;
            }

            if (distance > movementSettings.PreferredDistance)
            {
                if (movement.State != BossMovementState.Approach)
                    movement.RequestApproach();
            }
            else if (movement.State != BossMovementState.HoldPosition)
            {
                movement.RequestHoldPosition();
            }
        }

        private void EnterWait()
        {
            waitRemaining = runtimeContext != null
                ? runtimeContext.CombatDefinition.CombatSettings.WaitReevaluationInterval
                : 0.25f;
        }

        private void TickCooldowns(float deltaTime)
        {
            if (cooldowns.Count == 0)
                return;

            List<string> keys = new List<string>(cooldowns.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                float remaining = cooldowns[key] - deltaTime;
                if (remaining <= 0f)
                    cooldowns.Remove(key);
                else
                    cooldowns[key] = remaining;
            }
        }

        private void HandleDeath(BossHealth source)
        {
            if (lifecycleState == BossCombatLifecycleState.Dead)
                return;

            lifecycleState = BossCombatLifecycleState.Dead;
            CancelActiveExecution();
            ReleaseCombatLocks();

            if (movement != null)
            {
                movement.ClearFacingRequest();
                movement.LockMovement(BossMovementLockOwner.Death);
            }
        }

        private void CancelActiveExecution()
        {
            if (activeExecution == null)
                return;

            // Presentation must leave the attack state even when cancellation
            // originates from StopCombat, death, or encounter cleanup.
            if (activeExecutionContext != null)
            {
                NotifyPresentationState(
                    activeExecutionContext.AttackDefinition,
                    BossAttackExecutionState.Cancelled);
            }

            activeExecution.ReleaseRequested -= HandleReleaseRequested;
            activeExecution.StateChanged -= HandleExecutionStateChanged;
            activeExecution.Cancel();
            activeExecution.Dispose();
            activeExecution = null;
            activeExecutionContext = null;
        }

        private void ReleaseCombatLocks()
        {
            if (movement == null)
                return;

            if (combatMovementLock >= 0)
            {
                movement.UnlockMovement(combatMovementLock);
                combatMovementLock = -1;
            }

        }

        private bool MarkUnavailable(string reason)
        {
            lifecycleState = BossCombatLifecycleState.Unavailable;
            Debug.LogError(
                $"BossCombatRuntime on '{name}' is unavailable: {reason}",
                this);
            return false;
        }

        private static float HorizontalDistance(Vector3 a, Vector3 b)
        {
            a.y = 0f;
            b.y = 0f;
            return Vector3.Distance(a, b);
        }

        private void OnDisable()
        {
            if (subscribedToDeath && health != null)
                health.OnDeath -= HandleDeath;

            subscribedToDeath = false;
            CancelActiveExecution();
            ReleaseCombatLocks();

            cooldowns.Clear();
            candidateBuffer.Clear();
            attemptedThisCycle.Clear();
            executorSystem?.ResetEncounter();

            playerTarget = null;
            activePhase = null;
            pendingPhase = null;
            runtimeContext = null;
            health = null;
            movement = null;
            activeExecutionContext = null;
            hasTargetPositionAtExecutionStart = false;
            executorSystem = null;
            waitRemaining = 0f;
            lastStartedAttackId = null;
            cooldownConsumedThisSequence = false;
            lifecycleState = BossCombatLifecycleState.Uninitialized;
        }

        private void OnEnable()
        {
            // Intentionally does not initialize or start combat.
        }
    }
}
