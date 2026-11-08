using System.Collections.Generic;
using Chaosbound.Content.Items;
using Chaosbound.Content.Materials;
using UnityEngine;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class TierUpPreviewData
    {
        public string ItemName { get; }
        public Sprite ItemIcon { get; }

        public ItemTier CurrentTier { get; }
        public ItemTier ProjectedTier { get; }

        public int CurrentUpgradeLevel { get; }
        public int ProjectedUpgradeLevel { get; }

        public IReadOnlyList<EquipmentStatPreviewEntry>
            CurrentStats
        { get; }

        public IReadOnlyList<EquipmentStatPreviewEntry>
            ProjectedStats
        { get; }

        public string NewStatName { get; }
        public string NewStatValue { get; }

        public IReadOnlyList<MaterialCostPreviewEntry>
            MaterialCosts
        { get; }

        public TierUpPreviewData(
            string itemName,
            Sprite itemIcon,
            ItemTier currentTier,
            ItemTier projectedTier,
            int currentUpgradeLevel,
            int projectedUpgradeLevel,
            IReadOnlyList<EquipmentStatPreviewEntry> currentStats,
            IReadOnlyList<EquipmentStatPreviewEntry> projectedStats,
            string newStatName,
            string newStatValue,
            IReadOnlyList<MaterialCostPreviewEntry> materialCosts)
        {
            ItemName = itemName;
            ItemIcon = itemIcon;

            CurrentTier = currentTier;
            ProjectedTier = projectedTier;

            CurrentUpgradeLevel = currentUpgradeLevel;
            ProjectedUpgradeLevel = projectedUpgradeLevel;

            CurrentStats = currentStats;
            ProjectedStats = projectedStats;

            NewStatName = newStatName;
            NewStatValue = newStatValue;

            MaterialCosts = materialCosts;
        }
    }

    public sealed class EquipmentStatPreviewEntry
    {
        public string DisplayName { get; }
        public string DisplayValue { get; }
        public bool IsNew { get; }

        public EquipmentStatPreviewEntry(
            string displayName,
            string displayValue,
            bool isNew)
        {
            DisplayName = displayName;
            DisplayValue = displayValue;
            IsNew = isNew;
        }
    }

    public sealed class MaterialCostPreviewEntry
    {
        public MaterialDefinition Definition { get; }
        public int OwnedAmount { get; }
        public int RequiredAmount { get; }

        public MaterialCostPreviewEntry(
            MaterialDefinition definition,
            int ownedAmount,
            int requiredAmount)
        {
            Definition = definition;
            OwnedAmount = ownedAmount;
            RequiredAmount = requiredAmount;
        }
    }
}