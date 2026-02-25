using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour
{
    [Header("=== BASE STATS ===")]
    [SerializeField] private float baseDamage = 5f;

    [Header("=== ATTACK SPEED ===")]
    [SerializeField] private float baseAttackSpeed = 1f;

    [Header("=== DAMAGE TYPE ===")]
    [SerializeField] private DamageType currentDamageType = DamageType.Physical;

    // ===============================
    // Runtime Modifiers (Global)
    // ===============================

    private float flatDamageBonus;
    private float percentDamageBonus;
    private float finalDamageMultiplier = 1f;
    private float attackSpeedMultiplier = 1f;

    // ===============================
    // Cached Final Values
    // ===============================

    public float FinalDamage { get; private set; }

    public float FinalAttackSpeed => baseAttackSpeed * attackSpeedMultiplier;

    public DamageType CurrentDamageType => currentDamageType;

    public event Action OnStatsRecalculated;

    // ===============================
    // PUBLIC API
    // ===============================

    public void AddFlatDamage(float amount)
    {
        flatDamageBonus += amount;
        Recalculate();
    }

    public void AddPercentDamage(float amount)
    {
        percentDamageBonus += amount;
        Recalculate();
    }

    public void MultiplyFinalDamage(float multiplier)
    {
        finalDamageMultiplier *= multiplier;
        Recalculate();
    }

    public void AddAttackSpeedPercent(float percent)
    {
        attackSpeedMultiplier += percent;
        OnStatsRecalculated?.Invoke();
    }

    public void MultiplyAttackSpeed(float multiplier)
    {
        attackSpeedMultiplier *= multiplier;
        OnStatsRecalculated?.Invoke();
    }

    public void ResetAttackSpeed()
    {
        attackSpeedMultiplier = 1f;
        OnStatsRecalculated?.Invoke();
    }

    public void SetDamageType(DamageType type)
    {
        currentDamageType = type;
    }

    public void ResetRuntimeModifiers()
    {
        flatDamageBonus = 0f;
        percentDamageBonus = 0f;
        finalDamageMultiplier = 1f;

        Recalculate();
    }

    // ===============================
    // CORE CALCULATION
    // ===============================

    private void Recalculate()
    {
        FinalDamage =
            (baseDamage + flatDamageBonus)
            * (1f + percentDamageBonus)
            * finalDamageMultiplier;

        OnStatsRecalculated?.Invoke();
    }

    private void Awake()
    {
        Recalculate();
    }
}