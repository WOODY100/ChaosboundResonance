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
        stats.CriticalMultiplier = 1f;
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
            ApplyModifierToStatsFlexible(mod);
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

    private void ApplySingleModifier(SkillModifierType type, float value)
    {
        switch (type)
        {
            // DAMAGE
            case SkillModifierType.FlatDamage:
                stats.FlatDamage += value;
                break;

            case SkillModifierType.PercentDamage:
                stats.PercentDamage += value;
                break;

            case SkillModifierType.CriticalChance:
                stats.CriticalChance += value;
                break;

            case SkillModifierType.CriticalMultiplier:
                stats.CriticalMultiplier += value;
                break;

            // TEMPO
            case SkillModifierType.CooldownPercent:
                stats.PercentCooldownReduction += value;
                break;

            case SkillModifierType.TickRatePercent:
                stats.PercentTickRate += value;
                break;

            // AREA
            case SkillModifierType.SpawnRadiusPercent:
                stats.PercentSpawnRadius += value;
                break;

            case SkillModifierType.ImpactRadiusPercent:
                stats.PercentImpactRadius += value;
                break;

            case SkillModifierType.RangePercent:
                stats.PercentRange += value;
                break;

            case SkillModifierType.DurationPercent:
                stats.PercentDuration += value;
                break;

            case SkillModifierType.ExtraCount:
                stats.ExtraCount += Mathf.RoundToInt(value);
                break;

            case SkillModifierType.Penetration:
                stats.PenetrationCount += Mathf.RoundToInt(value);
                break;

            case SkillModifierType.ChainCount:
                stats.ChainCount += Mathf.RoundToInt(value);
                break;

            // SPECIAL
            case SkillModifierType.SplitOnImpact:
                stats.GrantsSplit = true;
                break;

            case SkillModifierType.ExplodeOnKill:
                stats.GrantsExplosion = true;
                break;

            case SkillModifierType.SpawnZoneOnHit:
                stats.SpawnZoneOnHit = true;
                stats.SpawnZoneChance += value;
                break;

            default:
                case SkillModifierType.ApplyPoison:
                case SkillModifierType.ApplyBurn:
                case SkillModifierType.ApplyShock:
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

    private void ApplyModifierToStatsFlexible(SkillModifierDefinition modifier)
    {
        // 🔥 NUEVO SISTEMA (multi-modifier)
        if (modifier.Modifiers != null && modifier.Modifiers.Length > 0)
        {
            foreach (var entry in modifier.Modifiers)
            {
                ApplySingleModifier(entry.Type, entry.Value);
            }
        }
        else
        {
            // 🔹 LEGACY
            ApplySingleModifier(modifier.ModifierType, modifier.Value);
        }
    }
}