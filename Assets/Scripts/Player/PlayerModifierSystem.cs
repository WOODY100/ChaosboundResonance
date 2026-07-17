using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModifierSystem : MonoBehaviour
{
    public event Action<StatType, float> OnStatChanged;

    private readonly Dictionary<StatType, PlayerStat> stats = new();

    private readonly Dictionary<string, ModifierSource> metaSources = new();

    private readonly Dictionary<string, ModifierSource> runSources = new();

    private readonly List<StatModifier> modifierBuffer = new(16);

    private void Awake()
    {
        InitializeBaseStats();
        RecalculateAll();
    }

    private void InitializeBaseStats()
    {
        CreateStat(StatType.Damage, 10f);
        CreateStat(StatType.AttackSpeed, 1f);
        CreateStat(StatType.MovementSpeed, 4f);
        CreateStat(StatType.MaxHP, 100f);
        CreateStat(StatType.HPRegen, 0f);
        CreateStat(StatType.ExpAttractionRadius, 1.1f);
    }

    private void CreateStat(StatType type, float baseValue)
    {
        PlayerStat stat = new();
        stat.SetBaseValue(baseValue);

        stats[type] = stat;
    }

    // =========================
    // ADD / REMOVE SOURCES
    // =========================

    public void AddSource(ModifierLayer layer, ModifierSource source)
    {
        var target = GetLayer(layer);
        target[source.SourceID] = source;
        RecalculateAll();
    }

    public void RemoveSource(ModifierLayer layer, string sourceID)
    {
        var target = GetLayer(layer);

        if (target.Remove(sourceID))
            RecalculateAll();
    }

    public void ClearLayer(ModifierLayer layer)
    {
        GetLayer(layer).Clear();
        RecalculateAll();
    }

    private Dictionary<string, ModifierSource> GetLayer(ModifierLayer layer)
    {
        return layer switch
        {
            ModifierLayer.Meta => metaSources,
            ModifierLayer.Run => runSources,
            _ => throw new ArgumentOutOfRangeException(
                nameof(layer),
                layer,
                $"Unknown modifier layer '{layer}'.")
        };
    }

    // =========================
    // RECALCULATION
    // =========================

    private void RecalculateAll()
    {
        foreach (var statType in stats.Keys)
        {
            modifierBuffer.Clear();

            CollectModifiers(metaSources, statType, modifierBuffer);
            CollectModifiers(runSources, statType, modifierBuffer);

            stats[statType].Recalculate(modifierBuffer);

            OnStatChanged?.Invoke(statType, stats[statType].CurrentValue);
        }
    }

    private void CollectModifiers(
        Dictionary<string, ModifierSource> layer,
        StatType statType,
        List<StatModifier> result)
    {
        foreach (var source in layer.Values)
        {
            foreach (var mod in source.Modifiers)
            {
                if (mod.StatType == statType)
                    result.Add(mod);
            }
        }
    }

    // =========================
    // ACCESS
    // =========================

    public float GetStat(StatType type)
    {
        if (!stats.TryGetValue(type, out var stat))
        {
            throw new InvalidOperationException(
                $"Stat '{type}' is not registered in PlayerModifierSystem.");
        }

        return stat.CurrentValue;
    }
}