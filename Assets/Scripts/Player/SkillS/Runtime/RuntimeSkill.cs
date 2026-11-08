using Chaosbound.Content.Expeditions.Runtime.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using Chaosbound.Development.Analytics;
#endif

public class RuntimeSkill
{
    public SkillDefinition Definition { get; private set; }

    public int Level { get; private set; }

    public float CurrentCooldown { get; private set; }

    public float CooldownDuration { get; private set; }

    public bool IsOnCooldown => CurrentCooldown > 0f;

    public int ModifierCount => appliedModifiers.Count;

    public bool HasModifiers => appliedModifiers.Count > 0;

    public bool IsAtMaxLevel =>
        Level >= progressionConfig.MaxSkillLevel;

    public float CooldownNormalized =>
        CooldownDuration <= 0f
            ? 0f
            : CurrentCooldown / CooldownDuration;

    private readonly List<SkillModifierDefinition> appliedModifiers = new();

    // Meta-progression modifiers are kept separate from run modifiers.
    private readonly List<ModifierEntry> globalModifiers = new();

    private readonly HashSet<SkillModifierDefinition> modifierHistory = new();

    private readonly RuntimeSkillProgressionConfig progressionConfig;

    private SkillStats stats;

    private bool evolutionPending;

    public bool CanEvolve =>
        Level >= progressionConfig.EvolutionRequiredLevel &&
        Definition.HasEvolutions;

    public bool IsEvolutionPending => evolutionPending;

    public SkillStats Stats => stats;

    public IReadOnlyList<SkillModifierDefinition> Modifiers =>
        appliedModifiers;

    public IReadOnlyList<ModifierEntry> GlobalModifiers =>
        globalModifiers;

    /// <summary>
    /// Replaces the complete global modifier layer and recalculates this skill.
    /// Replacing instead of appending prevents duplicate meta effects.
    /// </summary>
    public void SetGlobalModifiers(IReadOnlyList<ModifierEntry> modifiers)
    {
#if UNITY_EDITOR
        ExpeditionSkillStatsSnapshot previousStats =
            ExpeditionTelemetryCollector.CaptureSkillStats(stats);
        int previousLevel = Level;
#endif
        globalModifiers.Clear();

        if (modifiers != null)
        {
            for (int i = 0; i < modifiers.Count; i++)
                globalModifiers.Add(modifiers[i]);
        }

        RecalculateStats();
        CooldownDuration = stats.FinalCooldown;
        CurrentCooldown = Mathf.Min(CurrentCooldown, CooldownDuration);
#if UNITY_EDITOR
        ExpeditionTelemetryCollector.RegisterSkillStatsChanged(
            GetAnalyticsSkillId(), Definition.DisplayName,
            "GlobalModifiersReplaced", string.Empty, "GlobalSkillModifierLayer", 0f,
            previousLevel, Level, previousStats, stats);
#endif
    }

    public event Action<RuntimeSkill> OnCooldownFinished;

    public event Action<RuntimeSkill> OnLevelChanged;

    public RuntimeSkill(
        SkillDefinition definition,
        RuntimeSkillProgressionConfig progressionConfig)
    {
        Definition =
            definition ?? throw new ArgumentNullException(nameof(definition));

        this.progressionConfig =
            progressionConfig ?? throw new ArgumentNullException(nameof(progressionConfig));

        Level = 1;

        stats = new SkillStats
        {
            BaseDamage = definition.BaseDamage,
            BaseCooldown = definition.BaseCooldown,
            BaseSpawnRadius = definition.BaseSpawnRadius,
            BaseImpactRadius = definition.BaseImpactRadius,
            BaseRange = definition.BaseRange,
            BaseDuration = definition.BaseDuration,
            BaseTickRate = definition.BaseTickRate,
            BaseCount = definition.BaseCount
        };

        RecalculateStats();

        CooldownDuration = stats.FinalCooldown;
        CurrentCooldown = 0f;
#if UNITY_EDITOR
        ExpeditionTelemetryCollector.RegisterSkillStatsChanged(
            GetAnalyticsSkillId(), Definition.DisplayName,
            "SkillCreated", string.Empty, "InitialStats", 0f,
            0, Level, null, stats);
#endif
    }

    // =========================================================
    // MODIFIERS
    // =========================================================

    public bool CanApplyModifier(SkillModifierDefinition modifier)
    {
        if (modifier == null)
            return false;

        if (IsAtMaxLevel)
            return false;

        if (!modifier.IsStackable &&
            modifierHistory.Contains(modifier))
        {
            return false;
        }

        return true;
    }

    public bool ApplyModifier(SkillModifierDefinition modifier)
    {
        if (!CanApplyModifier(modifier))
            return false;

#if UNITY_EDITOR
        ExpeditionSkillStatsSnapshot previousStats =
            ExpeditionTelemetryCollector.CaptureSkillStats(stats);
        int previousLevel = Level;
#endif
        appliedModifiers.Add(modifier);

        modifierHistory.Add(modifier);

        RecalculateStats();

        Level++;
#if UNITY_EDITOR
        ExpeditionTelemetryCollector.RegisterSkillStatsChanged(
            GetAnalyticsSkillId(), Definition.DisplayName,
            "SkillModifierApplied", modifier.DisplayName,
            modifier.UsesMultiModifier ? "SkillModifierDefinition.MultiEntry" : "SkillModifierDefinition.Legacy",
            modifier.UsesMultiModifier ? 0f : modifier.Value,
            previousLevel, Level, previousStats, stats);
#endif
        OnLevelChanged?.Invoke(this);

        return true;
    }

    // =========================================================
    // EVOLUTION TRANSFER
    // =========================================================

    public SkillEvolutionTransferPreview BuildEvolutionTransferPreview(
        SkillDefinition resultingDefinition)
    {
        if (resultingDefinition == null)
            throw new ArgumentNullException(nameof(resultingDefinition));

        List<SkillModifierDefinition> retained = new();
        List<SkillModifierDefinition> dropped = new();

        foreach (SkillModifierDefinition modifier in appliedModifiers)
        {
            if (resultingDefinition.PossibleModifiers.Contains(modifier))
                retained.Add(modifier);
            else
                dropped.Add(modifier);
        }

        return new SkillEvolutionTransferPreview(
            retained,
            dropped);
    }

    public RuntimeSkill CreateEvolvedSkill(
        SkillDefinition resultingDefinition)
    {
        if (resultingDefinition == null)
            throw new ArgumentNullException(nameof(resultingDefinition));

        SkillEvolutionTransferPreview preview =
            BuildEvolutionTransferPreview(resultingDefinition);

        RuntimeSkill evolvedSkill =
            new RuntimeSkill(
                resultingDefinition,
                progressionConfig);

        evolvedSkill.appliedModifiers.Clear();

        foreach (SkillModifierDefinition modifier in preview.RetainedModifiers)
        {
            evolvedSkill.appliedModifiers.Add(modifier);
        }

        foreach (SkillModifierDefinition modifier in modifierHistory)
        {
            evolvedSkill.modifierHistory.Add(modifier);
        }

#if UNITY_EDITOR
        ExpeditionSkillStatsSnapshot previousEvolvedStats =
            ExpeditionTelemetryCollector.CaptureSkillStats(evolvedSkill.stats);
#endif
        evolvedSkill.RecalculateStats();
#if UNITY_EDITOR
        ExpeditionTelemetryCollector.RegisterSkillStatsChanged(
            evolvedSkill.GetAnalyticsSkillId(), resultingDefinition.DisplayName,
            "EvolutionModifiersTransferred", string.Empty, "RetainedSkillModifiers",
            preview.RetainedModifiers.Count, 1, evolvedSkill.Level,
            previousEvolvedStats, evolvedSkill.stats);
#endif

        evolvedSkill.CooldownDuration =
            evolvedSkill.stats.FinalCooldown;

        // Evolution explicitly starts ready.
        evolvedSkill.CurrentCooldown = 0f;

        return evolvedSkill;
    }

    // =========================================================
    // EVOLUTION PENDING
    // =========================================================

    public void MarkEvolutionPending()
    {
        if (!CanEvolve)
            return;

        evolutionPending = true;
    }

    public void ClearEvolutionPending()
    {
        evolutionPending = false;
    }

    // =========================================================
    // COOLDOWN
    // =========================================================

    public void StartCooldown(float duration)
    {
        duration = Mathf.Max(0f, duration);

        CooldownDuration = duration;
        CurrentCooldown = duration;
    }

    public void TickCooldown(float deltaTime)
    {
        if (CurrentCooldown <= 0f)
            return;

        CurrentCooldown =
            Mathf.Max(
                0f,
                CurrentCooldown - deltaTime);

        if (CurrentCooldown == 0f)
            OnCooldownFinished?.Invoke(this);
    }

    // =========================================================
    // STATS
    // =========================================================

    private void RecalculateStats()
    {
        ResetStats();

        foreach (SkillModifierDefinition modifier in appliedModifiers)
        {
            ApplyModifierToStatsFlexible(modifier);
        }

        // Global meta modifiers are applied after run-specific modifiers,
        // while remaining stored in their own collection.
        for (int i = 0; i < globalModifiers.Count; i++)
        {
            ModifierEntry modifier = globalModifiers[i];
            ApplySingleModifier(modifier.Type, modifier.Value);
        }

        stats.Calculate();
    }

    private void ResetStats()
    {
        stats.BaseDamage = Definition.BaseDamage;
        stats.BaseCooldown = Definition.BaseCooldown;
        stats.BaseSpawnRadius = Definition.BaseSpawnRadius;
        stats.BaseImpactRadius = Definition.BaseImpactRadius;
        stats.BaseRange = Definition.BaseRange;
        stats.BaseDuration = Definition.BaseDuration;
        stats.BaseTickRate = Definition.BaseTickRate;
        stats.BaseCount = Definition.BaseCount;

        stats.FlatDamage = 0f;
        stats.PercentDamage = 0f;
        stats.FinalDamageMultiplier = 1f;

        stats.CriticalChance = 0f;
        stats.CriticalDamageBonus = 0f;

        stats.PercentTickRate = 0f;

        stats.FlatCooldownReduction = 0f;
        stats.PercentCooldownReduction = 0f;

        stats.PercentSpawnRadius = 0f;
        stats.PercentImpactRadius = 0f;
        stats.PercentRange = 0f;
        stats.PercentDuration = 0f;

        stats.ExtraCount = 0;
        stats.PenetrationCount = 0;
        stats.BounceCount = 0;
        stats.ChainCount = 0;

        stats.GrantsExplosion = false;
        stats.GrantsChaining = false;
        stats.GrantsSplit = false;

        stats.SpawnZoneOnHit = false;
        stats.SpawnZoneChance = 0f;
    }

    private void ApplyModifierToStatsFlexible(
        SkillModifierDefinition modifier)
    {
        if (modifier.Modifiers != null &&
            modifier.Modifiers.Length > 0)
        {
            foreach (ModifierEntry entry in modifier.Modifiers)
            {
                ApplySingleModifier(
                    entry.Type,
                    entry.Value);
            }
        }
        else
        {
            ApplySingleModifier(
                modifier.ModifierType,
                modifier.Value);
        }
    }

    private void ApplySingleModifier(
        SkillModifierType type,
        float value)
    {
        switch (type)
        {
            case SkillModifierType.FlatDamage:
                stats.FlatDamage += value;
                break;

            case SkillModifierType.PercentDamage:
                stats.PercentDamage += value / 100f;
                break;

            case SkillModifierType.CriticalChance:
                stats.CriticalChance += value / 100f;
                break;

            case SkillModifierType.CriticalDamageBonus:
                stats.CriticalDamageBonus += value / 100f;
                break;

            case SkillModifierType.CooldownPercent:
                stats.PercentCooldownReduction += value / 100f;
                break;

            case SkillModifierType.TickRatePercent:
                stats.PercentTickRate += value / 100f;
                break;

            case SkillModifierType.SpawnRadiusPercent:
                stats.PercentSpawnRadius += value / 100f;
                break;

            case SkillModifierType.ImpactRadiusPercent:
                stats.PercentImpactRadius += value / 100f;
                break;

            case SkillModifierType.RangePercent:
                stats.PercentRange += value / 100f;
                break;

            case SkillModifierType.DurationPercent:
                stats.PercentDuration += value / 100f;
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
#if UNITY_EDITOR
    private string GetAnalyticsSkillId()
    {
        if (Definition == null) return string.Empty;
        return string.IsNullOrEmpty(Definition.AnalyticsId)
            ? Definition.name
            : Definition.AnalyticsId;
    }
#endif

}