using System.Collections.Generic;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentComparisonTooltipData
    {
        private readonly List<EquipmentComparisonTooltipRowData> rows =
            new List<EquipmentComparisonTooltipRowData>();

        public ItemInstance EquippedItem { get; }

        public ItemInstance CandidateItem { get; }

        public ItemBaseData EquippedBaseData { get; }

        public ItemBaseData CandidateBaseData { get; }

        public EquipmentType EquipmentType { get; }

        public IReadOnlyList<EquipmentComparisonTooltipRowData> Rows =>
            rows;

        public int RowCount =>
            rows.Count;

        public EquipmentComparisonTooltipData(
            ItemInstance equippedItem,
            ItemInstance candidateItem,
            ItemBaseData equippedBaseData,
            ItemBaseData candidateBaseData,
            EquipmentType equipmentType)
        {
            EquippedItem = equippedItem;
            CandidateItem = candidateItem;

            EquippedBaseData = equippedBaseData;
            CandidateBaseData = candidateBaseData;

            EquipmentType = equipmentType;
        }

        public void AddRow(
            EquipmentComparisonTooltipRowData row)
        {
            rows.Add(row);
        }
    }
}