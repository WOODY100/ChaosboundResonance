using System;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentComparisonDisplayBuilder
    {
        public EquipmentComparisonDisplayData Build(
            ItemComparisonResult comparisonResult)
        {
            if (comparisonResult == null)
                throw new ArgumentNullException(
                    nameof(comparisonResult));

            EquipmentComparisonDisplayData displayData =
                new EquipmentComparisonDisplayData();

            for (int i = 0;
                 i < comparisonResult.Stats.Count;
                 i++)
            {
                EquipmentComparisonStat comparisonStat =
                    comparisonResult.Stats[i];

                EquipmentComparisonDisplayEntry entry =
                    BuildEntry(comparisonStat);

                displayData.Add(entry);
            }

            return displayData;
        }

        private EquipmentComparisonDisplayEntry BuildEntry(
            EquipmentComparisonStat comparisonStat)
        {
            EquipmentComparisonDisplayState state =
                ResolveState(comparisonStat);

            bool hasDifference =
                comparisonStat.IsDirectlyComparable;

            float difference =
                hasDifference
                    ? comparisonStat.Difference
                    : 0f;

            return new EquipmentComparisonDisplayEntry(
                comparisonStat.StatType,
                comparisonStat.HasEquippedValue,
                comparisonStat.HasCandidateValue,
                comparisonStat.EquippedModifierType,
                comparisonStat.CandidateModifierType,
                comparisonStat.EquippedValue,
                comparisonStat.CandidateValue,
                hasDifference,
                difference,
                state);
        }

        private EquipmentComparisonDisplayState ResolveState(
            EquipmentComparisonStat comparisonStat)
        {
            if (!comparisonStat.HasEquippedValue &&
                !comparisonStat.HasCandidateValue)
            {
                return EquipmentComparisonDisplayState.None;
            }

            if (!comparisonStat.HasEquippedValue)
            {
                return EquipmentComparisonDisplayState.CandidateOnly;
            }

            if (!comparisonStat.HasCandidateValue)
            {
                return EquipmentComparisonDisplayState.EquippedOnly;
            }

            if (!comparisonStat.IsDirectlyComparable)
            {
                return EquipmentComparisonDisplayState
                    .DifferentModifierType;
            }

            if (comparisonStat.Difference > 0f)
            {
                return EquipmentComparisonDisplayState.Improved;
            }

            if (comparisonStat.Difference < 0f)
            {
                return EquipmentComparisonDisplayState.Reduced;
            }

            return EquipmentComparisonDisplayState.Unchanged;
        }
    }
}