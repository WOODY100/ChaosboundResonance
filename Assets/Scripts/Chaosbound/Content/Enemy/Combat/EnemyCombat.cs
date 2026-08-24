using System;
using UnityEngine;

[RequireComponent(typeof(EnemyRuntimeContext))]
[RequireComponent(typeof(EnemyRuntimeTargeting))]
[RequireComponent(typeof(EnemyRuntimeNavigation))]
[RequireComponent(typeof(EnemyRuntimePresentation))]
[RequireComponent(typeof(EnemyRuntimeStats))]
public sealed class EnemyCombat :
    MonoBehaviour
{
    [SerializeField]
    private Transform abilityOrigin;

    private EnemyRuntimeContext runtimeContext;
    private EnemyRuntimeTargeting targeting;
    private EnemyRuntimeNavigation navigation;

    private EnemyAttackDefinition attackDefinition;

    private EnemyRuntimeStats runtimeStats;

    private EnemyHealth health;

    private float attackElapsedTime;
    private float cooldownRemaining;

    private bool impactTriggered;

    private Vector3 attackFacing;

    private EnemyRuntimePresentation presentation;

    private EnemyAbilityExecutionSystem abilityExecutionSystem;
    private EnemyRuntimeAbility runtimeAbility;

    public float AttackElapsedTime =>
        attackElapsedTime;

    public float CooldownRemaining =>
        cooldownRemaining;

    public Vector3 AttackFacing =>
        attackFacing;

    public bool IsInitialized
    {
        get;
        private set;
    }

    public EnemyCombatState State
    {
        get;
        private set;
    }

    public bool IsAttacking =>
        IsInitialized &&
        State == EnemyCombatState.Attacking;

    public bool IsOnCooldown =>
        IsInitialized &&
        State == EnemyCombatState.Cooldown;

    public bool CanAttack
    {
        get
        {
            if (!IsInitialized)
                return false;

            if (State != EnemyCombatState.Ready)
                return false;

            if (targeting == null ||
                !targeting.HasTarget)
            {
                return false;
            }

            Transform target =
                targeting.CurrentTarget;

            if (target == null)
                return false;

            return IsTargetInAttackRange(target);
        }
    }

    public EnemyAttackDefinition AttackDefinition =>
        attackDefinition;

    private void Update()
    {
        if (health.IsDead)
            return;

        if (!IsInitialized)
            return;

        switch (State)
        {
            case EnemyCombatState.Attacking:
                UpdateAttack();
                break;

            case EnemyCombatState.Cooldown:
                UpdateCooldown();
                break;
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

        State =
            EnemyCombatState.Ready;

        presentation =
            GetComponent<EnemyRuntimePresentation>();

        health =
            GetComponent<EnemyHealth>();

        runtimeStats =
            GetComponent<EnemyRuntimeStats>();

        abilityExecutionSystem =
            new EnemyAbilityExecutionSystem();

        runtimeAbility =
            new EnemyRuntimeAbility();
    }

    /// <summary>
    /// Initializes enemy combat using the attack
    /// configuration provided by the enemy variant.
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

        if (presentation == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimePresentation is not available.");
        }

        if (!presentation.IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyRuntimePresentation has not been initialized.");
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

        EnemyVariantData variant =
            runtimeContext.Variant;

        if (variant == null)
        {
            throw new InvalidOperationException(
                "EnemyRuntimeContext does not contain an EnemyVariantData.");
        }

        attackDefinition =
    variant.Attack;

        if (attackDefinition == null)
        {
            throw new InvalidOperationException(
                $"Enemy '{name}' does not contain an attack definition.");
        }

        if (attackDefinition.Ability != null)
        {
            runtimeAbility.Initialize(
                attackDefinition.Ability,
                transform);
        }
        else
        {
            runtimeAbility.Reset();
        }

        if (attackDefinition.AnimationClip == null)
        {
            throw new InvalidOperationException(
                $"Enemy '{name}' attack definition does not contain an AnimationClip.");
        }

        if (health == null)
        {
            throw new InvalidOperationException(
                "EnemyHealth is not available.");
        }

        State =
            EnemyCombatState.Ready;

        IsInitialized = true;
    }

    private bool IsTargetInAttackRange(
    Transform target)
    {
        Vector3 offset =
            target.position -
            transform.position;

        offset.y = 0f;

        float distance =
            offset.magnitude;

        if (distance >
            attackDefinition.Range)
        {
            return false;
        }

        if (distance <= 0.0001f)
            return true;

        Vector3 direction =
            offset.normalized;

        float angle =
            Vector3.Angle(
                transform.forward,
                direction);

        return angle <=
            attackDefinition.Angle * 0.5f;
    }

    /// <summary>
    /// Requests the enemy to begin its configured attack.
    /// </summary>
    public bool RequestAttack()
    {
        ValidateInitialized();

        if (!CanAttack)
            return false;

        Transform target =
            targeting.CurrentTarget;

        if (target == null)
            return false;

        CaptureAttackFacing(
            target);

        attackElapsedTime = 0f;
        impactTriggered = false;

        State =
            EnemyCombatState.Attacking;

        presentation.PlayAttack(
            attackDefinition.AnimationClip);

        return true;
    }

    /// <summary>
    /// Shuts down combat for a dead enemy while
    /// preserving the runtime state for the death phase.
    /// </summary>
    public void Shutdown()
    {
        if (!IsInitialized)
            return;

        if (State ==
            EnemyCombatState.Attacking)
        {
            presentation.StopAttack();
        }

        attackElapsedTime = 0f;
        cooldownRemaining = 0f;

        impactTriggered = true;

        State =
            EnemyCombatState.Ready;
    }

    /// <summary>
    /// Resets combat state for pooled reuse.
    /// </summary>
    public void Reset()
    {
        abilityExecutionSystem.Reset();
        runtimeAbility.Reset();

        attackDefinition = null;

        attackElapsedTime = 0f;
        cooldownRemaining = 0f;
        impactTriggered = false;

        attackFacing = Vector3.zero;

        State =
            EnemyCombatState.Ready;

        IsInitialized = false;
    }

    private void ValidateInitialized()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException(
                "EnemyCombat has not been initialized.");
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

        if (attackDefinition == null)
        {
            throw new InvalidOperationException(
                "EnemyCombat has no attack definition.");
        }
    }

    private void CaptureAttackFacing(
        Transform target)
    {
        Vector3 direction =
            target.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <=
            0.0001f)
        {
            attackFacing =
                transform.forward;

            return;
        }

        attackFacing =
            direction.normalized;

        transform.rotation =
            Quaternion.LookRotation(
                attackFacing,
                Vector3.up);
    }

    private void UpdateAttack()
    {
        if (attackDefinition == null)
            return;

        if (attackDefinition.AnimationClip == null)
            return;

        float duration =
            attackDefinition.AnimationClip.length;

        if (duration <= 0f)
        {
            CompleteAttack();
            return;
        }

        attackElapsedTime +=
            Time.deltaTime;

        float normalizedTime =
            attackElapsedTime /
            duration;

        if (!impactTriggered &&
            normalizedTime >=
            attackDefinition.ImpactNormalizedTime)
        {
            impactTriggered = true;

            ExecuteImpact();
        }

        if (attackElapsedTime >= duration)
        {
            CompleteAttack();
        }
    }

    private void ExecuteImpact()
    {
        if (!IsInitialized)
            return;

        if (runtimeAbility != null &&
            runtimeAbility.IsInitialized)
        {
            ExecuteAbilityImpact();
            return;
        }

        Transform target =
            targeting.CurrentTarget;

        if (target == null)
            return;

        if (!IsTargetValidAtImpact(target))
            return;

        ExecuteMeleeImpact(target);
    }

    private void ExecuteMeleeImpact(
    Transform target)
    {
        PlayerDamageReceiver damageReceiver =
            target.GetComponentInParent<PlayerDamageReceiver>();

        if (damageReceiver == null)
            return;

        DamageData damageData =
            new DamageData(
                runtimeStats.Damage,
                attackDefinition.DamageType);

        damageData.source = gameObject;

        damageReceiver.ReceiveDamage(
            damageData);
    }

    private void ExecuteAbilityImpact()
    {
        EnemyAbilityExecutionContext context =
            new EnemyAbilityExecutionContext(
                transform,
                GetAbilityOrigin(),
                attackFacing,
                runtimeStats.Damage,
                attackDefinition.DamageType,
                attackDefinition.Range);

        abilityExecutionSystem.Execute(
            runtimeAbility,
            context);
    }

    private Vector3 GetAbilityOrigin()
    {
        if (abilityOrigin != null)
            return abilityOrigin.position;

        return transform.position;
    }

    private bool IsTargetValidAtImpact(
    Transform target)
    {
        if (target == null)
            return false;

        Vector3 offset =
            target.position -
            transform.position;

        offset.y = 0f;

        float distance =
            offset.magnitude;

        if (distance >
            attackDefinition.Range)
        {
            return false;
        }

        if (distance <= 0.0001f)
            return true;

        Vector3 direction =
            offset.normalized;

        float angle =
            Vector3.Angle(
                attackFacing,
                direction);

        return angle <=
            attackDefinition.Angle * 0.5f;
    }

    private void CompleteAttack()
    {
        presentation.StopAttack();

        attackElapsedTime = 0f;
        impactTriggered = false;

        cooldownRemaining =
            Mathf.Max(
                0f,
                attackDefinition.Cooldown);

        State =
            EnemyCombatState.Cooldown;
    }

    private void UpdateCooldown()
    {
        cooldownRemaining -=
            Time.deltaTime;

        if (cooldownRemaining > 0f)
            return;

        cooldownRemaining = 0f;

        State =
            EnemyCombatState.Ready;
    }

    private void OnDisable()
    {
        Reset();
    }
}