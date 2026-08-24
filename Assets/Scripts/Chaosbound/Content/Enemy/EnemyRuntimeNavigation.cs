using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyRuntimeContext))]
[RequireComponent(typeof(EnemyRuntimeStats))]
public sealed class EnemyRuntimeNavigation :
    MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemyRuntimeContext runtimeContext;
    private EnemyRuntimeStats runtimeStats;

    public bool IsInitialized
    {
        get;
        private set;
    }

    public bool IsOnNavMesh =>
        agent != null &&
        agent.enabled &&
        agent.isOnNavMesh;

    public bool IsStopped =>
        agent != null &&
        agent.enabled &&
        agent.isStopped;

    public Vector3 Velocity =>
        agent != null && agent.enabled
            ? agent.velocity
            : Vector3.zero;

    public Vector3 DesiredVelocity =>
        agent != null && agent.enabled
            ? agent.desiredVelocity
            : Vector3.zero;

    private void Awake()
    {
        agent =
            GetComponent<NavMeshAgent>();

        runtimeContext =
            GetComponent<EnemyRuntimeContext>();

        runtimeStats =
            GetComponent<EnemyRuntimeStats>();

        agent.updatePosition = true;
        agent.updateRotation = false;

        agent.enabled = false;
    }

    /// <summary>
    /// Initializes navigation after the enemy runtime
    /// context and runtime stats are valid.
    /// </summary>
    public void Initialize()
    {
        if (runtimeContext == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeContext is not available.");
        }

        if (!runtimeContext.IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeContext has not been initialized.");
        }

        if (runtimeStats == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeStats is not available.");
        }

        if (!runtimeStats.IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeStats has not been initialized.");
        }

        if (runtimeStats.MoveSpeed <= 0f)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeStats.MoveSpeed must be greater than zero.");
        }

        agent.speed =
            runtimeStats.MoveSpeed;

        agent.updatePosition = true;
        agent.updateRotation = false;

        agent.enabled = true;

        if (!agent.isOnNavMesh)
        {
            agent.enabled = false;

            throw new InvalidOperationException(
                $"Enemy '{name}' could not bind to the NavMesh.");
        }

        agent.isStopped = true;

        IsInitialized = true;
    }

    /// <summary>
    /// Executes the supplied movement intent.
    /// Navigation owns the interpretation of movement intents.
    /// </summary>
    public void ExecuteIntent(
        EnemyMovementIntent intent)
    {
        ValidateInitialized();

        if (intent == null)
        {
            Stop();
            return;
        }

        switch (intent.Type)
        {
            case EnemyMovementIntentType.None:
                Stop();
                break;

            case EnemyMovementIntentType.MoveToTarget:
                MoveToPosition(
                    intent.Position);
                break;

            case EnemyMovementIntentType.MoveToPosition:
                MoveToPosition(
                    intent.Position);
                break;

            case EnemyMovementIntentType.MaintainDistance:
                ExecuteMaintainDistance(
                    intent);
                break;

            case EnemyMovementIntentType.Retreat:
                ExecuteRetreat(
                    intent);
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported movement intent type: {intent.Type}.");
        }
    }

    private void MoveToPosition(
        Vector3 position)
    {
        agent.isStopped = false;

        agent.SetDestination(
            position);
    }

    private void ExecuteMaintainDistance(
        EnemyMovementIntent intent)
    {
        Vector3 targetPosition =
            intent.Position;

        float preferredDistance =
            intent.Distance;

        Vector3 offset =
            transform.position -
            targetPosition;

        offset.y = 0f;

        if (offset.sqrMagnitude <= 0.0001f)
        {
            Stop();
            return;
        }

        float currentDistance =
            offset.magnitude;

        if (currentDistance <= preferredDistance)
        {
            Stop();
            return;
        }

        MoveToPosition(
            targetPosition);
    }

    private void ExecuteRetreat(
        EnemyMovementIntent intent)
    {
        Vector3 targetPosition =
            intent.Position;

        Vector3 awayDirection =
            transform.position -
            targetPosition;

        awayDirection.y = 0f;

        if (awayDirection.sqrMagnitude <= 0.0001f)
        {
            Stop();
            return;
        }

        Vector3 retreatPosition =
            transform.position +
            awayDirection.normalized *
            intent.Distance;

        MoveToPosition(
            retreatPosition);
    }

    /// <summary>
    /// Stops navigation while preserving the current
    /// navigation state.
    /// </summary>
    public void Stop()
    {
        ValidateInitialized();

        agent.isStopped = true;
    }

    /// <summary>
    /// Resumes navigation along the current path.
    /// </summary>
    public void Resume()
    {
        ValidateInitialized();

        agent.isStopped = false;
    }

    /// <summary>
    /// Shuts down navigation for a dead enemy while
    /// preserving its initialized runtime state.
    /// </summary>
    public void Shutdown()
    {
        if (agent == null)
            return;

        if (agent.enabled &&
            agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    /// <summary>
    /// Clears the current navigation path and stops movement.
    /// </summary>
    public void ResetNavigation()
    {
        if (agent == null)
            return;

        if (agent.enabled &&
            agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        IsInitialized = false;
    }

    private void ValidateInitialized()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeNavigation has not been initialized.");
        }

        if (agent == null ||
            !agent.enabled)
        {
            throw new InvalidOperationException(
                "NavMeshAgent is not available.");
        }

        if (!agent.isOnNavMesh)
        {
            throw new InvalidOperationException(
                $"Enemy '{name}' is not currently bound to a NavMesh.");
        }
    }

    private void OnDisable()
    {
        ResetNavigation();

        if (agent != null &&
            agent.enabled)
        {
            agent.enabled = false;
        }
    }
}