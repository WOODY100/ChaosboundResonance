using System;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentComparisonTooltipBuilder
    {
        public EquipmentComparisonTooltipData Build(
            EquipmentComparisonDisplayData displayData,
            ItemComparisonResult comparisonResult)
        {
            if (displayData == null)
                throw new ArgumentNullException(
                    nameof(displayData));

            if (comparisonResult == null)
                throw new ArgumentNullException(
                    nameof(comparisonResult));

            EquipmentComparisonTooltipData tooltipData =
                new EquipmentComparisonTooltipData(
                    comparisonResult.EquippedItem,
                    comparisonResult.CandidateItem,
                    comparisonResult.EquippedBaseData,
                    comparisonResult.CandidateBaseData,
                    comparisonResult.EquipmentType);

            for (int i = 0;
                 i < displayData.Entries.Count;
                 i++)
            {
                EquipmentComparisonDisplayEntry displayEntry =
                    displayData.Entries[i];

                EquipmentComparisonTooltipRowData row =
                    new EquipmentComparisonTooltipRowData(
                        displayEntry.StatType,
                        displayEntry.HasEquippedValue,
                        displayEntry.HasCandidateValue,
                        displayEntry.EquippedModifierType,
                        displayEntry.CandidateModifierType,
                        displayEntry.EquippedValue,
                        displayEntry.CandidateValue,
                        displayEntry.HasDifference,
                        displayEntry.Difference,
                        displayEntry.State);

                tooltipData.AddRow(row);
            }

            return tooltipData;
        }
    }
}