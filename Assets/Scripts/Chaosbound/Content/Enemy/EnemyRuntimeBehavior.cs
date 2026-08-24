using System;
using UnityEngine;

[RequireComponent(typeof(EnemyRuntimeContext))]
[RequireComponent(typeof(EnemyRuntimeTargeting))]
[RequireComponent(typeof(EnemyRuntimeNavigation))]
[RequireComponent(typeof(EnemyCombat))]
public sealed class EnemyRuntimeBehavior :
    MonoBehaviour
{
    private EnemyRuntimeContext runtimeContext;
    private EnemyRuntimeTargeting targeting;
    private EnemyRuntimeNavigation navigation;
    private IEnemyMovementPolicy movementPolicy;

    private EnemyMovementIntent currentIntent;

    private EnemyCombat combat;

    public bool IsInitialized
    {
        get;
        private set;
    }

    public EnemyMovementIntent CurrentIntent
    {
        get
        {
            return currentIntent;
        }
    }

    private void Awake()
    {
        runtimeContext =
            GetComponent<EnemyRuntimeContext>();

        targeting =
            GetComponent<EnemyRuntimeTargeting>();

        navigation =
            GetComponent<EnemyRuntimeNavigation>();

        combat =
            GetComponent<EnemyCombat>();
    }

    /// <summary>
    /// Initializes the enemy behavior with the
    /// movement policy assigned to the enemy variant.
    /// </summary>
    public void Initialize(
        IEnemyMovementPolicy policy)
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

        if (targeting == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeTargeting is not available.");
        }

        if (!targeting.IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeTargeting has not been initialized.");
        }

        if (navigation == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeNavigation is not available.");
        }

        if (!navigation.IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeNavigation has not been initialized.");
        }

        if (combat == null)
        {
            throw new InvalidOperationException(
                "EnemyCombat is not available.");
        }

        movementPolicy =
            policy
            ?? throw new ArgumentNullException(
                nameof(policy));

        currentIntent =
            EnemyMovementIntent.None();

        IsInitialized = true;
    }

    /// <summary>
    /// Evaluates the current enemy behavior and
    /// produces a movement intent.
    /// </summary>
    public void Tick()
    {
        ValidateInitialized();

        if (!targeting.HasTarget)
        {
            currentIntent =
                EnemyMovementIntent.None();

            return;
        }

        Transform target =
            targeting.CurrentTarget;

        if (target == null)
        {
            currentIntent =
                EnemyMovementIntent.None();

            return;
        }

        if (combat.IsAttacking)
        {
            currentIntent =
                EnemyMovementIntent.None();

            return;
        }

        if (combat.CanAttack)
        {
            if (combat.RequestAttack())
            {
                currentIntent =
                    EnemyMovementIntent.None();

                return;
            }
        }

        EnemyMovementPolicyContext context =
            new EnemyMovementPolicyContext(
                transform.position,
                target.position,
                runtimeContext.Variant.PreferredDistance,
                runtimeContext.Variant.DistanceTolerance);

        currentIntent =
            movementPolicy.Evaluate(
                context);
    }

    /// <summary>
    /// Gets the current movement intent and clears it.
    /// </summary>
    public EnemyMovementIntent ConsumeIntent()
    {
        ValidateInitialized();

        EnemyMovementIntent intent =
            currentIntent;

        currentIntent =
            EnemyMovementIntent.None();

        return intent;
    }

    /// <summary>
    /// Shuts down active behavior without resetting
    /// the pooled runtime state.
    /// </summary>
    public void Shutdown()
    {
        if (!IsInitialized)
            return;

        currentIntent =
            EnemyMovementIntent.None();
    }

    /// <summary>
    /// Resets the behavior state when the
    /// pooled enemy is disabled.
    /// </summary>
    public void Reset()
    {
        currentIntent =
            EnemyMovementIntent.None();

        movementPolicy = null;

        IsInitialized = false;
    }

    private void ValidateInitialized()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeBehavior has not been initialized.");
        }

        if (runtimeContext == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeContext is not available.");
        }

        if (targeting == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeTargeting is not available.");
        }

        if (navigation == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeNavigation is not available.");
        }

        if (movementPolicy == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeBehavior has no movement policy.");
        }
    }

    private void OnDisable()
    {
        Reset();
    }
}