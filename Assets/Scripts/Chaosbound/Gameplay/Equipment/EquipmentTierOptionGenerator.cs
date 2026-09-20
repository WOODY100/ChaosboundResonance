using System;
using System.Collections.Generic;
using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentTierOptionGenerator
    {
        private readonly EquipmentStatDatabase statDatabase;
        private readonly int optionsPerTier;

        public EquipmentTierOptionGenerator(
            EquipmentStatDatabase statDatabase,
            int optionsPerTier)
        {
            if (statDatabase == null)
                throw new ArgumentNullException(nameof(statDatabase));

            if (optionsPerTier <= 0)
                throw new ArgumentOutOfRangeException(nameof(optionsPerTier));

            this.statDatabase = statDatabase;
            this.optionsPerTier = optionsPerTier;
        }

        public bool TryGenerate(
            ItemBaseData baseData,
            ItemInstance itemInstance,
            out List<EquipmentStatOption> options)
        {
            options = new List<EquipmentStatOption>();

            if (baseData == null)
                return false;

            if (itemInstance == null)
                return false;

            List<EquipmentStatDefinition> candidates =
                BuildCandidates(baseData, itemInstance);

            if (candidates.Count < optionsPerTier)
                return false;

            for (int i = 0; i < optionsPerTier; i++)
            {
                EquipmentStatDefinition selected =
                    SelectWeighted(candidates);

                if (selected == null)
                    return false;

                float rolledValue = UnityEngine.Random.Range(
                    selected.MinimumValue,
                    selected.MaximumValue);

                options.Add(
                    new EquipmentStatOption(
                        selected.StatType,
                        selected.ModifierType,
                        rolledValue));

                candidates.Remove(selected);
            }

            return options.Count == optionsPerTier;
        }

        private List<EquipmentStatDefinition> BuildCandidates(
            ItemBaseData baseData,
            ItemInstance itemInstance)
        {
            List<EquipmentStatDefinition> candidates =
                new List<EquipmentStatDefinition>();

            IReadOnlyList<EquipmentStatDefinition> definitions =
                statDatabase.Definitions;

            for (int i = 0; i < definitions.Count; i++)
            {
                EquipmentStatDefinition definition = definitions[i];

                if (definition == null)
                    continue;

                if (definition.Weight <= 0f)
                    continue;

                if (HasBaseStat(baseData, definition.StatType))
                    continue;

                if (itemInstance.HasUnlockedStat(definition.StatType))
                    continue;

                candidates.Add(definition);
            }

            return candidates;
        }

        private static bool HasBaseStat(
            ItemBaseData baseData,
            StatType statType)
        {
            IReadOnlyList<EquipmentBaseStat> baseStats =
                baseData.BaseStats;

            if (baseStats == null)
                return false;

            for (int i = 0; i < baseStats.Count; i++)
            {
                if (baseStats[i].StatType == statType)
                    return true;
            }

            return false;
        }

        private static EquipmentStatDefinition SelectWeighted(
            List<EquipmentStatDefinition> candidates)
        {
            float totalWeight = 0f;

            for (int i = 0; i < candidates.Count; i++)
            {
                totalWeight += Mathf.Max(
                    0f,
                    candidates[i].Weight);
            }

            if (totalWeight <= 0f)
                return null;

            float roll = UnityEngine.Random.value * totalWeight;

            for (int i = 0; i < candidates.Count; i++)
            {
                float weight = Mathf.Max(
                    0f,
                    candidates[i].Weight);

                if (roll < weight)
                    return candidates[i];

                roll -= weight;
            }

            return candidates[candidates.Count - 1];
        }
    }
}