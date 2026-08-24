using UnityEngine;
using System;

[RequireComponent(typeof(PlayerModifierSystem))]
public class PlayerHealth : MonoBehaviour
{
    private PlayerModifierSystem modifierSystem;

    private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => modifierSystem.GetStat(StatType.MaxHP);

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    private void Awake()
    {
        modifierSystem = GetComponent<PlayerModifierSystem>();

        modifierSystem.OnStatChanged += HandleStatChanged;
    }

    private void Start()
    {
        currentHealth = MaxHealth;
        OnHealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    private void Update()
    {
        ApplyRegen();
    }

    private void OnDestroy()
    {
        if (modifierSystem != null)
            modifierSystem.OnStatChanged -= HandleStatChanged;
    }

    private void HandleStatChanged(StatType statType, float newValue)
    {
        if (statType != StatType.MaxHP)
            return;

        currentHealth = Mathf.Clamp(currentHealth, 0f, newValue);

        OnHealthChanged?.Invoke(currentHealth, newValue);
    }

    private void ApplyRegen()
    {
        float regen = modifierSystem.GetStat(StatType.HPRegen);

        if (regen <= 0f)
            return;

        Heal(regen * Time.deltaTime);
    }

    public void TakeDamage(float amount)
    {
        float previousHealth =
            currentHealth;

        currentHealth -= amount;

        float maxHealth =
            MaxHealth;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0f,
                maxHealth);

        Debug.Log(
            $"[PlayerHealth] Damage: {amount} | " +
            $"Health: {previousHealth} -> {currentHealth} / {maxHealth}");

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth);

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        float maxHealth = MaxHealth;

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
}