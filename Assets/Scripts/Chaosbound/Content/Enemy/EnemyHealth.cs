using System;
using UnityEngine;

[RequireComponent(typeof(EnemyRuntimeContext))]
[RequireComponent(typeof(EnemyRuntimeStats))]
public sealed class EnemyHealth :
    MonoBehaviour,
    IDamageable
{
    public float MaxHealth
    {
        get;
        private set;
    }

    public float CurrentHealth
    {
        get;
        private set;
    }

    public bool IsDead
    {
        get;
        private set;
    }

    public event Action<EnemyHealth> OnDeath;

    public event Action<float> OnDamageTaken;

    private EnemyRuntimeStats runtimeStats;
    private EnemyRuntimeContext runtimeContext;

    public string LastHitSkillDefinitionId { get; private set; }
    public string LastHitExecutionId { get; private set; }
    public string LastHitSourceCategory { get; private set; }
    public string LastHitSkillId { get; private set; }
    public string LastHitSkillDisplayName { get; private set; }
    public string EnemyTierId { get; private set; }
    public string EnemyLifeId { get; private set; }

    private void Awake()
    {
        runtimeStats =
            GetComponent<EnemyRuntimeStats>();
        runtimeContext = GetComponent<EnemyRuntimeContext>();
    }

    private void OnEnable()
    {
        IsDead = false;
    }

    /// <summary>
    /// Initializes health from the effective runtime stats.
    ///
    /// This method is called explicitly by the Spawn Runtime
    /// after EnemyRuntimeStats has been initialized.
    /// </summary>
    public void Initialize()
    {
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

        MaxHealth =
            runtimeStats.MaxHealth;

        CurrentHealth =
            MaxHealth;

        IsDead = false;
        LastHitSkillDefinitionId = null;
        LastHitExecutionId = null;
        LastHitSourceCategory = null;
        LastHitSkillId = null;
        LastHitSkillDisplayName = null;
        EnemyTierId = runtimeContext != null && runtimeContext.Variant != null
            ? runtimeContext.Variant.Tier.ToString() : null;
#if UNITY_EDITOR
        EnemyLifeId = Guid.NewGuid().ToString("N");
#else
        EnemyLifeId = null;
#endif
    }

    /// <summary>
    /// Updates maximum health while preserving the enemy's current health ratio.
    /// </summary>
    public void RefreshMaxHealth(float newMaxHealth)
    {
        if (newMaxHealth < 0f)
            throw new ArgumentOutOfRangeException(nameof(newMaxHealth));

        float previousMaxHealth = MaxHealth;
        float healthRatio = previousMaxHealth > 0f
            ? CurrentHealth / previousMaxHealth
            : 1f;

        MaxHealth = newMaxHealth;

        if (IsDead)
        {
            CurrentHealth = 0f;
            return;
        }

        CurrentHealth = Mathf.Clamp(MaxHealth * healthRatio, 0f, MaxHealth);
    }

    public void TakeDamage(
        DamageData damageData)
    {
        if (IsDead)
            return;

        float finalDamage = damageData.amount;

        if (finalDamage <= 0f)
            return;

        float healthBeforeDamage =
            CurrentHealth;

        CurrentHealth =
            Mathf.Max(
                0f,
                CurrentHealth - finalDamage);

#if UNITY_EDITOR
        float effectiveDamage = healthBeforeDamage - CurrentHealth;
        if (effectiveDamage > 0f)
        {
            string skillId = damageData.skillDefinitionId;
            string tierId = EnemyTierId;
            // Every effective hit replaces the previous last-hit source,
            // including unknown damage, so attribution cannot become stale.
            LastHitSkillDefinitionId = skillId;
            LastHitExecutionId = damageData.executionId;
            LastHitSourceCategory = damageData.analyticsSourceCategory;
            LastHitSkillId = damageData.skillId;
            LastHitSkillDisplayName = damageData.skillDisplayName;
            Chaosbound.Development.Analytics.ExpeditionTelemetryCollector
                .RegisterDamageDealtToEnemy(
                    effectiveDamage, skillId, tierId,
                    damageData.analyticsSourceCategory,
                    damageData.skillId,
                    damageData.skillDisplayName);

            Chaosbound.Development.Analytics.ExpeditionTelemetryCollector
                .RegisterEnemyLifeDamage(
                    EnemyLifeId,
                    tierId,
                    skillId,
                    damageData.skillId,
                    damageData.skillDisplayName,
                    damageData.analyticsSourceCategory,
                    damageData.executionId,
                    effectiveDamage);
        }
#endif

        OnDamageTaken?.Invoke(
            finalDamage);

        FloatingDamageManager.Instance?.ShowDamage(
            transform.position,
            finalDamage,
            damageData.isCrit);

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        OnDeath?.Invoke(this);
    }

    private void OnDisable()
    {
        MaxHealth = 0f;
        CurrentHealth = 0f;
        IsDead = false;
        LastHitSkillDefinitionId = null;
        LastHitExecutionId = null;
        LastHitSourceCategory = null;
        LastHitSkillId = null;
        LastHitSkillDisplayName = null;
        EnemyTierId = null;
        EnemyLifeId = null;
    }
}