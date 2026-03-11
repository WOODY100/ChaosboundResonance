using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModifierSystem : MonoBehaviour
{
    public event Action<StatType, float> OnStatChanged;

    private Dictionary<StatType, PlayerStat> stats = new();

    private Dictionary<string, ModifierSource> metaSources = new();
    private Dictionary<string, ModifierSource> runSources = new();

    private void Awake()
    {
        InitializeBaseStats();
        RecalculateAll();
    }

    private void InitializeBaseStats()
    {
        CreateStat(StatType.Damage, 10f);
        CreateStat(StatType.AttackSpeed, 1f);
        CreateStat(StatType.MovementSpeed, 6f);
        CreateStat(StatType.MaxHP, 100f);
        CreateStat(StatType.HPRegen, 0f);
        CreateStat(StatType.ExpAttractionRadius, 1.1f);
    }

    private void CreateStat(StatType type, float baseValue)
    {
        stats[type] = new PlayerStat { BaseValue = baseValue };
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
            _ => null
        };
    }

    // =========================
    // RECALCULATION
    // =========================

    private void RecalculateAll()
    {

        foreach (var statType in stats.Keys)
        {
            List<StatModifier> collected = new();

            CollectModifiers(metaSources, statType, collected);
            CollectModifiers(runSources, statType, collected);

            stats[statType].Recalculate(collected);

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
        return stats[type].CurrentValue;
    }
}