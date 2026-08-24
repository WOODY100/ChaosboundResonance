using System;
using UnityEngine;

[RequireComponent(typeof(PlayerModifierSystem))]
public sealed class PlayerHealth : MonoBehaviour
{
    private PlayerModifierSystem modifierSystem;

    private float currentHealth;
    private bool isDead;

    public float CurrentHealth =>
        currentHealth;

    public float MaxHealth =>
        modifierSystem.GetStat(
            StatType.MaxHP);

    public bool IsDead =>
        isDead;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    private void Awake()
    {
        modifierSystem =
            GetComponent<PlayerModifierSystem>();

        modifierSystem.OnStatChanged +=
            HandleStatChanged;
    }

    private void Start()
    {
        ResetHealth();
    }

    private void Update()
    {
        if (isDead)
            return;

        ApplyRegen();
    }

    private void OnDestroy()
    {
        if (modifierSystem != null)
        {
            modifierSystem.OnStatChanged -=
                HandleStatChanged;
        }
    }

    private void HandleStatChanged(
        StatType statType,
        float newValue)
    {
        if (statType != StatType.MaxHP)
            return;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0f,
                newValue);

        OnHealthChanged?.Invoke(
            currentHealth,
            newValue);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void ApplyRegen()
    {
        float regen =
            modifierSystem.GetStat(
                StatType.HPRegen);

        if (regen <= 0f)
            return;

        Heal(
            regen *
            Time.deltaTime);
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        if (amount <= 0f)
            return;

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
            $"Health: {previousHealth} -> " +
            $"{currentHealth} / {maxHealth}");

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead)
            return;

        if (amount <= 0f)
            return;

        float maxHealth =
            MaxHealth;

        currentHealth += amount;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0f,
                maxHealth);

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth);
    }

    public void ResetHealth()
    {
        isDead = false;

        currentHealth =
            MaxHealth;

        OnHealthChanged?.Invoke(
            currentHealth,
            MaxHealth);
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        OnDeath?.Invoke();
    }
}