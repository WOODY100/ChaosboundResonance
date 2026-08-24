using System;
using UnityEngine;

[RequireComponent(typeof(BossControllerBase))]
public class BossHealth :
    MonoBehaviour,
    IDamageable
{
    [Header("Health")]
    [SerializeField]
    private float maxHealth = 1000f;

    private float currentHealth;

    public float CurrentHealth =>
        currentHealth;

    public float MaxHealth =>
        maxHealth;

    public bool IsDead =>
        currentHealth <= 0f;

    public event Action<BossHealth> OnDeath;

    private BossControllerBase controller;

    private void Awake()
    {
        controller =
            GetComponent<BossControllerBase>();
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(
        DamageData damage)
    {
        if (IsDead)
            return;

        if (damage.amount <= 0f)
            return;

        currentHealth -= damage.amount;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0f,
                maxHealth);

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
        if (!IsDead)
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

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
    }
}