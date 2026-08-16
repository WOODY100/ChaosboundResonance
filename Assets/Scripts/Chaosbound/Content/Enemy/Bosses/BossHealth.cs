using System;
using UnityEngine;

public class BossHealth :
    MonoBehaviour,
    IDamageable
{
    [Header("Stats")]
    [SerializeField]
    private float maxHealth = 1000f;

    public float CurrentHealth { get; private set; }

    public bool IsDead { get; private set; }

    public event Action<BossHealth> OnDeath;

    public event Action<float> OnDamageTaken;

    private BossControllerBase controller;

    private void Awake()
    {
        controller =
            GetComponent<BossControllerBase>();

        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        CurrentHealth =
            maxHealth;

        IsDead = false;
    }

    public void TakeDamage(
        DamageData damageData)
    {
        if (IsDead)
            return;

        float finalDamage =
            damageData.amount;

        if (finalDamage <= 0f)
            return;

        CurrentHealth -=
            finalDamage;

        OnDamageTaken?.Invoke(
            finalDamage);

        FloatingDamageManager.Instance?.ShowDamage(
            transform.position,
            finalDamage,
            damageData.isCrit);

        controller?.OnHealthChanged(
            CurrentHealth / maxHealth);

        if (CurrentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        controller?.OnDeath();

        OnDeath?.Invoke(this);
    }

    public float GetHealthPercent()
    {
        return CurrentHealth / maxHealth;
    }
}