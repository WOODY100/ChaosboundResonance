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

    private void ApplyRegen()
    {
        float regen = modifierSystem.GetStat(StatType.HPRegen);

        if (regen <= 0f)
            return;

        Heal(regen * Time.deltaTime);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);

        OnHealthChanged?.Invoke(currentHealth, MaxHealth);

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);

        OnHealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
}