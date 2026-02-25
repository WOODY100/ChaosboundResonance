using System.Collections.Generic;
using UnityEngine;

public class RuntimeSkill
{
    public SkillDefinition Definition { get; private set; }

    public float CurrentCooldown { get; private set; }
    public float CooldownDuration { get; private set; }

    public bool IsOnCooldown => CurrentCooldown > 0f;

    public int ModifierCount => appliedModifiers.Count;

    private List<SkillModifierDefinition> appliedModifiers;
    private List<SkillEvolutionDefinition> appliedEvolutions;

    private SkillStats stats;

    public SkillStats Stats => stats;

    public event System.Action<RuntimeSkill> OnCooldownFinished;
    public event System.Action<RuntimeSkill> OnEvolutionApplied;

    public IReadOnlyList<SkillModifierDefinition> Modifiers => appliedModifiers;
    public IReadOnlyList<SkillEvolutionDefinition> Evolutions => appliedEvolutions;

    // ===============================
    // CONSTRUCTOR
    // ===============================

    public RuntimeSkill(SkillDefinition definition)
    {
        Definition = definition;

        appliedModifiers = new List<SkillModifierDefinition>();
        appliedEvolutions = new List<SkillEvolutionDefinition>();

        stats = new SkillStats
        {
            BaseDamage = definition.BaseDamage,
            BaseCooldown = definition.BaseCooldown,
            BaseSpawnRadius = definition.BaseSpawnRadius,
            BaseImpactRadius = definition.BaseImpactRadius,
            BaseRange = definition.BaseRange,
            BaseDuration = definition.BaseDuration,
            BaseCount = definition.BaseCount
        };

        RecalculateStats();
    }

    // ===============================
    // APPLY MODIFIER
    // ===============================

    public void ApplyModifier(SkillModifierDefinition modifier)
    {
        appliedModifiers.Add(modifier);

        RecalculateStats();
    }

    // ===============================
    // APPLY EVOLUTION
    // ===============================

    public void ApplyEvolution(SkillEvolutionDefinition evolution)
    {
        if (appliedEvolutions.Contains(evolution))
            return;

        appliedEvolutions.Add(evolution);

        RecalculateStats();

        OnEvolutionApplied?.Invoke(this);
    }

    // ===============================
    // RECALCULATION
    // ===============================

    private void RecalculateStats()
    {
        // =========================
        // RESET BASE VALUES
        // =========================
        stats.BaseDamage = Definition.BaseDamage;
        stats.BaseCooldown = Definition.BaseCooldown;
        stats.BaseSpawnRadius = Definition.BaseSpawnRadius;
        stats.BaseImpactRadius = Definition.BaseImpactRadius;
        stats.BaseRange = Definition.BaseRange;
        stats.BaseDuration = Definition.BaseDuration;
        stats.BaseCount = Definition.BaseCount;

        // =========================
        // RESET DAMAGE LAYERS
        // =========================
        stats.FlatDamage = 0f;
        stats.PercentDamage = 0f;
        stats.FinalDamageMultiplier = 1f;
        stats.CriticalChance = 0f;
        stats.CriticalMultiplier = 0f;
        stats.PercentTickRate = 0f;

        // =========================
        // RESET COOLDOWN
        // =========================
        stats.FlatCooldownReduction = 0f;
        stats.PercentCooldownReduction = 0f;

        // =========================
        // RESET AREA / COVERAGE
        // =========================
        stats.PercentSpawnRadius = 0f;
        stats.PercentImpactRadius = 0f;
        stats.PercentRange = 0f;
        stats.PercentDuration = 0f;

        stats.ExtraCount = 0;
        stats.PenetrationCount = 0;
        stats.BounceCount = 0;
        stats.ChainCount = 0;

        // =========================
        // RESET FLAGS
        // =========================
        stats.GrantsExplosion = false;
        stats.GrantsChaining = false;
        stats.GrantsSplit = false;

        // =========================
        // APPLY MODIFIERS
        // =========================
        foreach (var mod in appliedModifiers)
        {
            ApplyModifierToStats(mod);
        }

        // =========================
        // APPLY EVOLUTIONS
        // =========================
        foreach (var evo in appliedEvolutions)
        {
            ApplyEvolutionToStats(evo);
        }

        // =========================
        // FINAL CALCULATION
        // =========================
        stats.Calculate();
    }

    // ===============================
    // MODIFIER LOGIC
    // ===============================

    private void ApplyModifierToStats(SkillModifierDefinition modifier)
    {
        switch (modifier.ModifierType)
        {
            // DAMAGE
            case SkillModifierType.FlatDamage:
                stats.FlatDamage += modifier.Value;
                break;

            case SkillModifierType.PercentDamage:
                stats.PercentDamage += modifier.Value;
                break;

            case SkillModifierType.CriticalChance:
                stats.CriticalChance += modifier.Value;
                break;

            case SkillModifierType.CriticalMultiplier:
                stats.CriticalMultiplier += modifier.Value;
                break;

            // TEMPO
            case SkillModifierType.CooldownPercent:
                stats.PercentCooldownReduction += modifier.Value;
                break;

            case SkillModifierType.TickRatePercent:
                stats.PercentTickRate += modifier.Value;
                break;

            // AREA
            case SkillModifierType.SpawnRadiusPercent:
                stats.PercentSpawnRadius += modifier.Value;
                break;

            case SkillModifierType.ImpactRadiusPercent:
                stats.PercentImpactRadius += modifier.Value;
                break;

            case SkillModifierType.RangePercent:
                stats.PercentRange += modifier.Value;
                break;

            case SkillModifierType.DurationPercent:
                stats.PercentDuration += modifier.Value;
                break;

            case SkillModifierType.ExtraCount:
                stats.ExtraCount += Mathf.RoundToInt(modifier.Value);
                break;

            case SkillModifierType.Penetration:
                stats.PenetrationCount += Mathf.RoundToInt(modifier.Value);
                break;

            case SkillModifierType.ChainCount:
                stats.ChainCount += Mathf.RoundToInt(modifier.Value);
                break;

            // SPECIAL
            case SkillModifierType.SplitOnImpact:
                stats.GrantsSplit = true;
                break;

            case SkillModifierType.ExplodeOnKill:
                stats.GrantsExplosion = true;
                break;

            default:
                Debug.LogWarning($"Unhandled modifier type: {modifier.ModifierType}");
                break;
        }
    }

    // ===============================
    // EVOLUTION LOGIC
    // ===============================

    private void ApplyEvolutionToStats(SkillEvolutionDefinition evolution)
    {
        stats.FlatDamage += evolution.BonusFlatDamage;
        stats.PercentDamage += evolution.BonusPercentDamage;
        stats.PercentSpawnRadius += evolution.BonusSpawnRadiusPercent;
        stats.PercentImpactRadius += evolution.BonusImpactRadiusPercent;
        stats.PercentCooldownReduction += evolution.BonusCooldownPercent;
        stats.ExtraCount += evolution.BonusExtraCount;

        if (evolution.GrantsChaining)
            stats.GrantsChaining = true;

        if (evolution.GrantsExplosionOnHit)
            stats.GrantsExplosion = true;
    }

    // ===============================
    // COOLDOWN MANAGEMENT
    // ===============================
    public void StartCooldown(float duration)
    {
        CooldownDuration = duration;
        CurrentCooldown = duration;
    }

    public void TickCooldown(float deltaTime)
    {
        if (CurrentCooldown <= 0f)
            return;

        CurrentCooldown -= deltaTime;

        if (CurrentCooldown <= 0f)
        {
            CurrentCooldown = 0f;
            OnCooldownFinished?.Invoke(this);
        }
    }
}