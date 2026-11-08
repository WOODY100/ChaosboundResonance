using System;
using System.Collections.Generic;
using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Content.Materials;

namespace Chaosbound.Gameplay.Equipment
{
    [Serializable]
    public struct EquipmentTierConfiguration
    {
        [SerializeField]
        private ItemTier tier;

        [SerializeField]
        private int rerollCost;

        public ItemTier Tier => tier;
        public int RerollCost => rerollCost;
    }

    [Serializable]
    public struct EquipmentUpgradeCostConfiguration
    {
        [SerializeField]
        private string itemContentId;

        [SerializeField]
        private EquipmentType equipmentType;

        [SerializeField]
        private ItemTier tier;

        [SerializeField]
        private int upgradeLevel;

        [SerializeField]
        private List<MaterialCost> materialCosts;

        public string ItemContentId => itemContentId;
        public EquipmentType EquipmentType => equipmentType;
        public ItemTier Tier => tier;
        public int UpgradeLevel => upgradeLevel;

        public IReadOnlyList<MaterialCost> MaterialCosts =>
            materialCosts;
    }

    [Serializable]
    public struct EquipmentTierUpCostConfiguration
    {
        [SerializeField]
        private string itemContentId;

        [SerializeField]
        private EquipmentType equipmentType;

        [SerializeField]
        private ItemTier currentTier;

        [SerializeField]
        private List<MaterialCost> materialCosts;

        public string ItemContentId => itemContentId;
        public EquipmentType EquipmentType => equipmentType;
        public ItemTier CurrentTier => currentTier;

        public IReadOnlyList<MaterialCost> MaterialCosts =>
            materialCosts;
    }

    [CreateAssetMenu(
        menuName = "Chaosbound/Equipment/Equipment Progression Config",
        fileName = "EquipmentProgressionConfig")]
    public sealed class EquipmentProgressionConfig : ScriptableObject
    {
        [Header("Tier Progression")]

        [SerializeField]
        private int upgradesPerTier = 5;

        [SerializeField]
        private int optionsPerTier = 3;

        [Header("Tier Configuration")]

        [SerializeField]
        private List<EquipmentTierConfiguration> tierConfigurations =
            new List<EquipmentTierConfiguration>();

        [Header("Normal Upgrade Costs")]

        [SerializeField]
        private List<EquipmentUpgradeCostConfiguration> upgradeCosts =
            new List<EquipmentUpgradeCostConfiguration>();

        [Header("Tier Up Costs")]

        [SerializeField]
        private List<EquipmentTierUpCostConfiguration> tierUpCosts =
            new List<EquipmentTierUpCostConfiguration>();

        public int UpgradesPerTier => upgradesPerTier;
        public int OptionsPerTier => optionsPerTier;

        public IReadOnlyList<EquipmentTierConfiguration>
            TierConfigurations => tierConfigurations;

        public IReadOnlyList<EquipmentUpgradeCostConfiguration>
            UpgradeCosts => upgradeCosts;

        public IReadOnlyList<EquipmentTierUpCostConfiguration>
            TierUpCosts => tierUpCosts;

        //==========================================================
        // Tier Configuration
        //==========================================================

        public bool TryGetTierConfiguration(
            ItemTier tier,
            out EquipmentTierConfiguration configuration)
        {
            for (int i = 0; i < tierConfigurations.Count; i++)
            {
                if (tierConfigurations[i].Tier != tier)
                    continue;

                configuration = tierConfigurations[i];
                return true;
            }

            configuration = default;
            return false;
        }

        //==========================================================
        // Upgrade Cost Resolution
        //==========================================================

        public bool TryGetUpgradeCost(
            string itemContentId,
            EquipmentType equipmentType,
            ItemTier tier,
            int currentUpgradeLevel,
            out IReadOnlyList<MaterialCost> costs)
        {
            costs = null;

            if (string.IsNullOrWhiteSpace(itemContentId))
                return false;

            itemContentId = itemContentId.Trim();

            // Priority 1: Specific item.
            if (TryFindUpgradeCost(
                    itemContentId,
                    EquipmentType.None,
                    tier,
                    currentUpgradeLevel,
                    out costs))
            {
                return costs != null && costs.Count > 0;
            }

            if (HasUpgradeCostEntry(
                    itemContentId,
                    EquipmentType.None,
                    tier,
                    currentUpgradeLevel))
            {
                return false;
            }

            // Priority 2: Equipment type.
            if (equipmentType != EquipmentType.None)
            {
                if (TryFindUpgradeCost(
                        string.Empty,
                        equipmentType,
                        tier,
                        currentUpgradeLevel,
                        out costs))
                {
                    return costs != null && costs.Count > 0;
                }

                if (HasUpgradeCostEntry(
                        string.Empty,
                        equipmentType,
                        tier,
                        currentUpgradeLevel))
                {
                    return false;
                }
            }

            // Priority 3: Generic fallback.
            if (TryFindUpgradeCost(
                    string.Empty,
                    EquipmentType.None,
                    tier,
                    currentUpgradeLevel,
                    out costs))
            {
                return costs != null && costs.Count > 0;
            }

            return false;
        }

        private bool TryFindUpgradeCost(
            string itemContentId,
            EquipmentType equipmentType,
            ItemTier tier,
            int upgradeLevel,
            out IReadOnlyList<MaterialCost> costs)
        {
            costs = null;

            EquipmentUpgradeCostConfiguration matchedConfiguration =
                default;

            int matchCount = 0;

            for (int i = 0; i < upgradeCosts.Count; i++)
            {
                EquipmentUpgradeCostConfiguration configuration =
                    upgradeCosts[i];

                if (configuration.Tier != tier ||
                    configuration.UpgradeLevel != upgradeLevel)
                {
                    continue;
                }

                if (!string.Equals(
                        NormalizeId(configuration.ItemContentId),
                        itemContentId,
                        StringComparison.Ordinal))
                {
                    continue;
                }

                if (configuration.EquipmentType != equipmentType)
                    continue;

                matchedConfiguration = configuration;
                matchCount++;

                if (matchCount > 1)
                    return false;
            }

            if (matchCount != 1)
                return false;

            costs = matchedConfiguration.MaterialCosts;
            return costs != null && costs.Count > 0;
        }

        private bool HasUpgradeCostEntry(
            string itemContentId,
            EquipmentType equipmentType,
            ItemTier tier,
            int upgradeLevel)
        {
            for (int i = 0; i < upgradeCosts.Count; i++)
            {
                EquipmentUpgradeCostConfiguration configuration =
                    upgradeCosts[i];

                if (configuration.Tier == tier &&
                    configuration.UpgradeLevel == upgradeLevel &&
                    configuration.EquipmentType == equipmentType &&
                    string.Equals(
                        NormalizeId(configuration.ItemContentId),
                        itemContentId,
                        StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        //==========================================================
        // Tier Up Cost Resolution
        //==========================================================

        public bool TryGetTierUpCost(
            string itemContentId,
            EquipmentType equipmentType,
            ItemTier currentTier,
            out IReadOnlyList<MaterialCost> costs)
        {
            costs = null;

            if (string.IsNullOrWhiteSpace(itemContentId))
                return false;

            itemContentId = itemContentId.Trim();

            // Priority 1: Specific item.
            if (TryFindTierUpCost(
                    itemContentId,
                    EquipmentType.None,
                    currentTier,
                    out costs))
            {
                return costs != null && costs.Count > 0;
            }

            if (HasTierUpCostEntry(
                    itemContentId,
                    EquipmentType.None,
                    currentTier))
            {
                return false;
            }

            // Priority 2: Equipment type.
            if (equipmentType != EquipmentType.None)
            {
                if (TryFindTierUpCost(
                        string.Empty,
                        equipmentType,
                        currentTier,
                        out costs))
                {
                    return costs != null && costs.Count > 0;
                }

                if (HasTierUpCostEntry(
                        string.Empty,
                        equipmentType,
                        currentTier))
                {
                    return false;
                }
            }

            // Priority 3: Generic fallback.
            if (TryFindTierUpCost(
                    string.Empty,
                    EquipmentType.None,
                    currentTier,
                    out costs))
            {
                return costs != null && costs.Count > 0;
            }

            return false;
        }

        private bool TryFindTierUpCost(
            string itemContentId,
            EquipmentType equipmentType,
            ItemTier currentTier,
            out IReadOnlyList<MaterialCost> costs)
        {
            costs = null;

            EquipmentTierUpCostConfiguration matchedConfiguration =
                default;

            int matchCount = 0;

            for (int i = 0; i < tierUpCosts.Count; i++)
            {
                EquipmentTierUpCostConfiguration configuration =
                    tierUpCosts[i];

                if (configuration.CurrentTier != currentTier)
                    continue;

                if (configuration.EquipmentType != equipmentType)
                    continue;

                if (!string.Equals(
                        NormalizeId(configuration.ItemContentId),
                        itemContentId,
                        StringComparison.Ordinal))
                {
                    continue;
                }

                matchedConfiguration = configuration;
                matchCount++;

                if (matchCount > 1)
                    return false;
            }

            if (matchCount != 1)
                return false;

            costs = matchedConfiguration.MaterialCosts;
            return costs != null && costs.Count > 0;
        }

        private bool HasTierUpCostEntry(
            string itemContentId,
            EquipmentType equipmentType,
            ItemTier currentTier)
        {
            for (int i = 0; i < tierUpCosts.Count; i++)
            {
                EquipmentTierUpCostConfiguration configuration =
                    tierUpCosts[i];

                if (configuration.CurrentTier == currentTier &&
                    configuration.EquipmentType == equipmentType &&
                    string.Equals(
                        NormalizeId(configuration.ItemContentId),
                        itemContentId,
                        StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        //==========================================================
        // Helpers
        //==========================================================

        private static string NormalizeId(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim();
        }

        //==========================================================
        // Validation
        //==========================================================

        private void OnValidate()
        {
            upgradesPerTier = Mathf.Max(1, upgradesPerTier);
            optionsPerTier = Mathf.Max(1, optionsPerTier);

            tierConfigurations ??=
                new List<EquipmentTierConfiguration>();

            upgradeCosts ??=
                new List<EquipmentUpgradeCostConfiguration>();

            tierUpCosts ??=
                new List<EquipmentTierUpCostConfiguration>();

            ValidateTierConfigurations();
            ValidateUpgradeCosts();
            ValidateTierUpCosts();
        }

        private void ValidateTierConfigurations()
        {
            HashSet<ItemTier> registeredTiers =
                new HashSet<ItemTier>();

            for (int i = 0; i < tierConfigurations.Count; i++)
            {
                EquipmentTierConfiguration configuration =
                    tierConfigurations[i];

                if (!registeredTiers.Add(configuration.Tier))
                {
                    Debug.LogError(
                        $"{name}: Duplicate Tier Configuration " +
                        $"for '{configuration.Tier}'.",
                        this);
                }
            }
        }

        private void ValidateUpgradeCosts()
        {
            HashSet<string> registeredKeys =
                new HashSet<string>(StringComparer.Ordinal);

            for (int i = 0; i < upgradeCosts.Count; i++)
            {
                EquipmentUpgradeCostConfiguration configuration =
                    upgradeCosts[i];

                string itemId =
                    NormalizeId(configuration.ItemContentId);

                if (!ValidateScope(
                        itemId,
                        configuration.EquipmentType,
                        $"Upgrade entry {i}"))
                {
                    continue;
                }

                string key =
                    $"{itemId}|{configuration.EquipmentType}|" +
                    $"{configuration.Tier}|{configuration.UpgradeLevel}";

                if (!registeredKeys.Add(key))
                {
                    Debug.LogError(
                        $"{name}: Duplicate Upgrade Cost key '{key}'.",
                        this);
                }

                if (configuration.UpgradeLevel < 0 ||
                    configuration.UpgradeLevel >= upgradesPerTier)
                {
                    Debug.LogError(
                        $"{name}: Invalid Upgrade Level " +
                        $"'{configuration.UpgradeLevel}'.",
                        this);
                }

                ValidateMaterialCosts(
                    configuration.MaterialCosts,
                    $"Upgrade {key}");
            }
        }

        private void ValidateTierUpCosts()
        {
            HashSet<string> registeredKeys =
                new HashSet<string>(StringComparer.Ordinal);

            for (int i = 0; i < tierUpCosts.Count; i++)
            {
                EquipmentTierUpCostConfiguration configuration =
                    tierUpCosts[i];

                string itemId =
                    NormalizeId(configuration.ItemContentId);

                if (!ValidateScope(
                        itemId,
                        configuration.EquipmentType,
                        $"Tier Up entry {i}"))
                {
                    continue;
                }

                string key =
                    $"{itemId}|{configuration.EquipmentType}|" +
                    $"{configuration.CurrentTier}";

                if (!registeredKeys.Add(key))
                {
                    Debug.LogError(
                        $"{name}: Duplicate Tier Up Cost key '{key}'.",
                        this);
                }

                ValidateMaterialCosts(
                    configuration.MaterialCosts,
                    $"Tier Up {key}");
            }
        }

        private bool ValidateScope(
            string itemContentId,
            EquipmentType equipmentType,
            string entryName)
        {
            if (!string.IsNullOrEmpty(itemContentId) &&
                equipmentType != EquipmentType.None)
            {
                Debug.LogError(
                    $"{name}: {entryName} cannot define both " +
                    "ItemContentId and EquipmentType. " +
                    "Choose one scope.",
                    this);

                return false;
            }

            if (string.IsNullOrEmpty(itemContentId) &&
                equipmentType == EquipmentType.None)
            {
                // Generic configuration is valid.
                return true;
            }

            return true;
        }

        private void ValidateMaterialCosts(
            IReadOnlyList<MaterialCost> costs,
            string configurationName)
        {
            if (costs == null || costs.Count == 0)
            {
                Debug.LogError(
                    $"{name}: {configurationName} has no material costs.",
                    this);

                return;
            }

            HashSet<string> registeredIds =
                new HashSet<string>(StringComparer.Ordinal);

            for (int i = 0; i < costs.Count; i++)
            {
                MaterialCost cost = costs[i];

                if (!cost.IsValid)
                {
                    Debug.LogError(
                        $"{name}: Invalid material cost in " +
                        $"{configurationName}, entry {i}.",
                        this);

                    continue;
                }

                string contentId = cost.ContentId.Trim();

                if (!registeredIds.Add(contentId))
                {
                    Debug.LogError(
                        $"{name}: Duplicate material '{contentId}' " +
                        $"in {configurationName}.",
                        this);
                }
            }
        }
    }
}