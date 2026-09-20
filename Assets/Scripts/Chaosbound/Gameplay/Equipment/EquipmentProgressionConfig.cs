using System;
using System.Collections.Generic;
using UnityEngine;
using Chaosbound.Content.Items;

namespace Chaosbound.Gameplay.Equipment
{
    [Serializable]
    public struct EquipmentTierConfiguration
    {
        [SerializeField]
        private ItemTier tier;

        [SerializeField]
        private int rerollCost;

        public ItemTier Tier =>
            tier;

        public int RerollCost =>
            rerollCost;
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

        public int UpgradesPerTier =>
            upgradesPerTier;

        public int OptionsPerTier =>
            optionsPerTier;

        public IReadOnlyList<EquipmentTierConfiguration>
            TierConfigurations =>
            tierConfigurations;

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

        private void OnValidate()
        {
            upgradesPerTier = Mathf.Max(
                1,
                upgradesPerTier);

            optionsPerTier = Mathf.Max(
                1,
                optionsPerTier);

            tierConfigurations ??=
                new List<EquipmentTierConfiguration>();

            HashSet<ItemTier> registeredTiers =
                new HashSet<ItemTier>();

            for (int i = 0; i < tierConfigurations.Count; i++)
            {
                EquipmentTierConfiguration configuration =
                    tierConfigurations[i];

                if (!registeredTiers.Add(
                    configuration.Tier))
                {
                    Debug.LogError(
                        $"{name}: Duplicate Equipment Tier Configuration " +
                        $"for Tier '{configuration.Tier}'.",
                        this);
                }
            }
        }
    }
}