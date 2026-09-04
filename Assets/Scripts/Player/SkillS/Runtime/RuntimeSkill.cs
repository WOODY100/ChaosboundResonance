using Chaosbound.Content.Expeditions.Runtime.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        appliedModifiers.Add(modifier);

        modifierHistory.Add(modifier);

        RecalculateStats();

        Level++;

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

        evolvedSkill.RecalculateStats();

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
        stats.CriticalMultiplier = 1f;

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
                stats.PercentDamage += value;
                break;

            case SkillModifierType.CriticalChance:
                stats.CriticalChance += value;
                break;

            case SkillModifierType.CriticalMultiplier:
                stats.CriticalMultiplier += value;
                break;

            case SkillModifierType.CooldownPercent:
                stats.PercentCooldownReduction += value;
                break;

            case SkillModifierType.TickRatePercent:
                stats.PercentTickRate += value;
                break;

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
}