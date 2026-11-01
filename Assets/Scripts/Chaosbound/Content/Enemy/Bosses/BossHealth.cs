using System;
using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField, Min(1f)] private float maxHealth = 1000f;
    private float currentHealth;
    private BossControllerBase legacyController;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0f;
    public event Action<BossHealth> OnDeath;

    private void Awake()
    {
        legacyController = GetComponent<BossControllerBase>();
    }

    private void OnEnable()
    {
        // Legacy/default initialization remains unchanged. The new Boss Runtime
        // explicitly overrides this value from BossCombatDefinition afterwards.
        InitializeLegacy();
    }

    private void InitializeLegacy()
    {
        currentHealth = maxHealth;
    }

    public void InitializeForRuntime(float initialMaxHealth)
    {
        if (initialMaxHealth <= 0f)
            throw new ArgumentOutOfRangeException(nameof(initialMaxHealth));

        maxHealth = initialMaxHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(DamageData damage)
    {
        if (IsDead || damage.amount <= 0f)
            return;

        currentHealth = Mathf.Clamp(currentHealth - damage.amount, 0f, maxHealth);

        if (FloatingDamageManager.Instance != null)
        {
            FloatingDamageManager.Instance.ShowDamage(
                transform.position, damage.amount, damage.isCrit);
        }

        float healthPercent = maxHealth > 0f ? currentHealth / maxHealth : 0f;
        legacyController?.OnHealthChanged(healthPercent);

        if (IsDead)
            Die();
    }

    private void Die()
    {
        if (!IsDead)
            return;

        legacyController?.OnDeath();
        OnDeath?.Invoke(this);
    }

    public float GetHealthPercent()
    {
        return maxHealth <= 0f ? 0f : currentHealth / maxHealth;
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
    }
}