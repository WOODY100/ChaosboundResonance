using System;
using System.Collections.Generic;
using Chaosbound.Content.Items;
using Chaosbound.Content.Materials;
using Chaosbound.Gameplay.Items.Runtime;
using UnityEngine;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class TierUpPreviewDataBuilder
    {
        private readonly EquipmentStatDatabase statDatabase;
        private readonly EquipmentStatResolver statResolver;
        private readonly EquipmentUpgradeService upgradeService;

        public TierUpPreviewDataBuilder(
            EquipmentStatDatabase statDatabase,
            EquipmentUpgradeService upgradeService)
        {
            this.statDatabase =
                statDatabase ??
                throw new ArgumentNullException(nameof(statDatabase));

            this.upgradeService =
                upgradeService ??
                throw new ArgumentNullException(nameof(upgradeService));

            statResolver =
                new EquipmentStatResolver(statDatabase);
        }

        public bool TryBuild(
            ItemInstance itemInstance,
            ItemBaseData baseData,
            EquipmentStatOption selectedOption,
            out TierUpPreviewData previewData)
        {
            previewData = null;

            if (itemInstance == null || baseData == null)
            {
                Debug.LogError(
                    "[PreviewBuilder] ItemInstance or BaseData is null.");

                return false;
            }

            if (itemInstance.CurrentTier == ItemTier.Legendary)
            {
                Debug.LogError(
                    "[PreviewBuilder] Item is Legendary.");

                return false;
            }

            if (!string.Equals(
                    itemInstance.BaseDataId,
                    baseData.ContentId,
                    StringComparison.Ordinal))
            {
                Debug.LogError(
                    "[PreviewBuilder] BaseDataId mismatch. " +
                    $"Item: {itemInstance.BaseDataId} | " +
                    $"BaseData: {baseData.ContentId}");

                return false;
            }

            if (!itemInstance.HasValidTierUpSession())
            {
                Debug.LogError(
                    "[PreviewBuilder] Invalid item Tier Up session.");

                return false;
            }

            if (itemInstance.HasUnlockedStat(
                    selectedOption.StatType))
            {
                Debug.LogError(
                    "[PreviewBuilder] Selected stat already unlocked: " +
                    selectedOption.StatType);

                return false;
            }

            if (!IsSessionOption(itemInstance, selectedOption))
            {
                Debug.LogError(
                    "[PreviewBuilder] Selected option not found in session.");

                return false;
            }

            if (!statDatabase.TryGetDefinition(
                    selectedOption.StatType,
                    out EquipmentStatDefinition newDefinition))
            {
                Debug.LogError(
                    "[PreviewBuilder] New stat definition not found: " +
                    selectedOption.StatType);

                return false;
            }

            if (newDefinition.ModifierType !=
                selectedOption.ModifierType)
            {
                Debug.LogError(
                    "[PreviewBuilder] New stat ModifierType mismatch. " +
                    $"Definition: {newDefinition.ModifierType} | " +
                    $"Option: {selectedOption.ModifierType}");

                return false;
            }

            if (!upgradeService.TryGetTierUpCost(
                    itemInstance,
                    out ItemBaseData resolvedBaseData,
                    out IReadOnlyList<MaterialCost> costs))
            {
                Debug.LogError(
                    "[PreviewBuilder] Tier Up cost resolution failed.");

                return false;
            }

            if (resolvedBaseData != baseData)
            {
                Debug.LogError(
                    "[PreviewBuilder] Resolved BaseData mismatch.");

                return false;
            }

            List<EquipmentStatPreviewEntry> currentStats =
                BuildCurrentStats(baseData, itemInstance);

            if (currentStats == null)
            {
                Debug.LogError(
                    "[PreviewBuilder] BuildCurrentStats failed.");

                return false;
            }

            List<EquipmentStatPreviewEntry> projectedStats =
                BuildProjectedStats(
                    baseData,
                    itemInstance,
                    selectedOption);

            if (projectedStats == null)
            {
                Debug.LogError(
                    "[PreviewBuilder] BuildProjectedStats failed.");

                return false;
            }

            List<MaterialCostPreviewEntry> materialCosts =
                BuildMaterialCosts(costs);

            if (materialCosts == null)
            {
                Debug.LogError(
                    "[PreviewBuilder] BuildMaterialCosts failed.");

                return false;
            }

            previewData = new TierUpPreviewData(
                baseData.DisplayName,
                baseData.Icon,
                itemInstance.CurrentTier,
                (ItemTier)((int)itemInstance.CurrentTier + 1),
                itemInstance.UpgradeLevel,
                0,
                currentStats,
                projectedStats,
                newDefinition.DisplayName,
                FormatStatValue(
                    selectedOption.ModifierType,
                    selectedOption.RolledValue),
                materialCosts);

            Debug.Log(
                "[PreviewBuilder] Preview data generated successfully.");

            return true;
        }

        private static bool IsSessionOption(
            ItemInstance itemInstance,
            EquipmentStatOption selectedOption)
        {
            IReadOnlyList<EquipmentStatOption> options =
                itemInstance.TierUpOptions;

            if (options == null)
            {
                Debug.LogError(
                    "[PreviewBuilder] TierUpOptions is null.");

                return false;
            }

            for (int i = 0; i < options.Count; i++)
            {
                EquipmentStatOption candidate = options[i];

                if (candidate.StatType != selectedOption.StatType)
                    continue;

                if (candidate.ModifierType != selectedOption.ModifierType)
                    continue;

                if (!Mathf.Approximately(
                        candidate.RolledValue,
                        selectedOption.RolledValue))
                {
                    continue;
                }

                return true;
            }

            Debug.LogError(
                "[PreviewBuilder] Selected option does not match " +
                "any generated session option.");

            return false;
        }

        private List<EquipmentStatPreviewEntry> BuildCurrentStats(
            ItemBaseData baseData,
            ItemInstance itemInstance)
        {
            List<EquipmentStatPreviewEntry> result =
                new List<EquipmentStatPreviewEntry>();

            HashSet<StatType> registeredStats =
                new HashSet<StatType>();

            IReadOnlyList<EquipmentBaseStat> baseStats =
                baseData.BaseStats;

            if (baseStats != null)
            {
                for (int i = 0; i < baseStats.Count; i++)
                {
                    StatType statType = baseStats[i].StatType;

                    if (!registeredStats.Add(statType))
                        continue;

                    if (!TryAddCurrentStat(
                            baseData,
                            itemInstance,
                            statType,
                            result))
                    {
                        Debug.LogError(
                            "[PreviewBuilder] Could not add base stat: " +
                            statType);

                        return null;
                    }
                }
            }

            IReadOnlyList<EquipmentRolledStat> unlockedStats =
                itemInstance.UnlockedStats;

            if (unlockedStats != null)
            {
                for (int i = 0; i < unlockedStats.Count; i++)
                {
                    StatType statType =
                        unlockedStats[i].StatType;

                    if (!registeredStats.Add(statType))
                        continue;

                    if (!TryAddCurrentStat(
                            baseData,
                            itemInstance,
                            statType,
                            result))
                    {
                        Debug.LogError(
                            "[PreviewBuilder] Could not add unlocked stat: " +
                            statType);

                        return null;
                    }
                }
            }

            return result;
        }

        private bool TryAddCurrentStat(
            ItemBaseData baseData,
            ItemInstance itemInstance,
            StatType statType,
            List<EquipmentStatPreviewEntry> result)
        {
            if (!statDatabase.TryGetDefinition(
                    statType,
                    out EquipmentStatDefinition definition))
            {
                Debug.LogError(
                    "[PreviewBuilder] Current stat definition not found: " +
                    statType);

                return false;
            }

            if (!statResolver.TryResolve(
                    baseData,
                    itemInstance,
                    statType,
                    out ModifierType modifierType,
                    out float value))
            {
                Debug.LogError(
                    "[PreviewBuilder] Could not resolve current stat: " +
                    statType);

                return false;
            }

            result.Add(
                new EquipmentStatPreviewEntry(
                    definition.DisplayName,
                    FormatStatValue(modifierType, value),
                    false));

            return true;
        }

        private List<EquipmentStatPreviewEntry> BuildProjectedStats(
            ItemBaseData baseData,
            ItemInstance itemInstance,
            EquipmentStatOption selectedOption)
        {
            List<EquipmentStatPreviewEntry> result =
                new List<EquipmentStatPreviewEntry>();

            HashSet<StatType> registeredStats =
                new HashSet<StatType>();

            IReadOnlyList<EquipmentBaseStat> baseStats =
                baseData.BaseStats;

            if (baseStats != null)
            {
                for (int i = 0; i < baseStats.Count; i++)
                {
                    EquipmentBaseStat stat = baseStats[i];

                    if (!registeredStats.Add(stat.StatType))
                        continue;

                    if (!TryAddProjectedStat(
                            stat.StatType,
                            stat.ModifierType,
                            stat.Value,
                            false,
                            result))
                    {
                        Debug.LogError(
                            "[PreviewBuilder] Could not add projected " +
                            "base stat: " + stat.StatType);

                        return null;
                    }
                }
            }

            IReadOnlyList<EquipmentRolledStat> unlockedStats =
                itemInstance.UnlockedStats;

            if (unlockedStats != null)
            {
                for (int i = 0; i < unlockedStats.Count; i++)
                {
                    EquipmentRolledStat stat =
                        unlockedStats[i];

                    if (!registeredStats.Add(stat.StatType))
                        continue;

                    if (!TryAddProjectedStat(
                            stat.StatType,
                            stat.ModifierType,
                            stat.RolledValue,
                            false,
                            result))
                    {
                        Debug.LogError(
                            "[PreviewBuilder] Could not add projected " +
                            "unlocked stat: " + stat.StatType);

                        return null;
                    }
                }
            }

            if (!registeredStats.Add(selectedOption.StatType))
            {
                Debug.LogError(
                    "[PreviewBuilder] Selected stat already exists " +
                    "in projected stats: " + selectedOption.StatType);

                return null;
            }

            if (!TryAddProjectedStat(
                    selectedOption.StatType,
                    selectedOption.ModifierType,
                    selectedOption.RolledValue,
                    true,
                    result))
            {
                Debug.LogError(
                    "[PreviewBuilder] Could not add selected projected stat: " +
                    selectedOption.StatType);

                return null;
            }

            return result;
        }

        private bool TryAddProjectedStat(
            StatType statType,
            ModifierType modifierType,
            float value,
            bool isNew,
            List<EquipmentStatPreviewEntry> result)
        {
            if (!statDatabase.TryGetDefinition(
                    statType,
                    out EquipmentStatDefinition definition))
            {
                Debug.LogError(
                    "[PreviewBuilder] Projected stat definition not found: " +
                    statType);

                return false;
            }

            if (definition.ModifierType != modifierType)
            {
                Debug.LogError(
                    "[PreviewBuilder] Projected stat ModifierType mismatch. " +
                    $"Stat: {statType} | " +
                    $"Definition: {definition.ModifierType} | " +
                    $"Value: {modifierType}");

                return false;
            }

            result.Add(
                new EquipmentStatPreviewEntry(
                    definition.DisplayName,
                    FormatStatValue(modifierType, value),
                    isNew));

            return true;
        }

        private List<MaterialCostPreviewEntry> BuildMaterialCosts(
            IReadOnlyList<MaterialCost> costs)
        {
            if (costs == null || costs.Count == 0)
            {
                Debug.LogError(
                    "[PreviewBuilder] Material costs are null or empty.");

                return null;
            }

            List<MaterialCostPreviewEntry> result =
                new List<MaterialCostPreviewEntry>();

            for (int i = 0; i < costs.Count; i++)
            {
                MaterialCost cost = costs[i];

                if (!upgradeService.TryGetMaterialDisplayData(
                        cost.ContentId,
                        out MaterialDefinition definition,
                        out int ownedAmount))
                {
                    Debug.LogError(
                        "[PreviewBuilder] Could not resolve material: " +
                        cost.ContentId);

                    return null;
                }

                result.Add(
                    new MaterialCostPreviewEntry(
                        definition,
                        ownedAmount,
                        cost.Amount));
            }

            return result;
        }

        private static string FormatStatValue(
            ModifierType modifierType,
            float value)
        {
            switch (modifierType)
            {
                case ModifierType.Flat:
                    return $"+{value:0.##}";

                case ModifierType.Percent:
                    return $"+{value:0.##}%";

                case ModifierType.FinalMultiplier:
                    return $"×{value:0.##}";

                default:
                    return value.ToString("0.##");
            }
        }
    }
}