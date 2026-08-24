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

    private void Awake()
    {
        runtimeStats =
            GetComponent<EnemyRuntimeStats>();
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
    }

    public void TakeDamage(
        DamageData damageData)
    {
        if (IsDead)
            return;

        float finalDamage =
            DamageProcessor.CalculateDamage(
                this,
                damageData);

        if (finalDamage <= 0f)
            return;

        CurrentHealth =
            Mathf.Max(
                0f,
                CurrentHealth - finalDamage);

        OnDamageTaken?.Invoke(
            finalDamage);

        FloatingDamageManager.Instance?.ShowDamage(
            transform.position,
            finalDamage,
            false);

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
    }
}