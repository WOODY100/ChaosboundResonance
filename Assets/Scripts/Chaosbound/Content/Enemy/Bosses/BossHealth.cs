using System;
using UnityEngine;

public class BossHealth :
    MonoBehaviour,
    IDamageable
{
    [SerializeField]
    private float maxHealth = 1000f;

    [SerializeField]
    private float currentHealth;

    public float CurrentHealth =>
        currentHealth;

    public bool IsDead =>
        currentHealth <= 0f;

    public event Action<BossHealth> OnDeath;

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
        currentHealth =
            maxHealth;
    }

    public void TakeDamage(
        DamageData damage)
    {
        if (IsDead)
            return;

        currentHealth -=
            damage.amount;

        if (FloatingDamageManager.Instance != null)
        {
            FloatingDamageManager.Instance.ShowDamage(
                transform.position,
                damage.amount,
                damage.isCrit);
        }

        float healthPercent =
            maxHealth > 0f
                ? currentHealth / maxHealth
                : 0f;

        controller?.OnHealthChanged(
            healthPercent);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (IsDead == false)
            return;

        controller?.OnDeath();

        OnDeath?.Invoke(this);
    }

    public float GetHealthPercent()
    {
        if (maxHealth <= 0f)
            return 0f;

        return currentHealth / maxHealth;
    }
}