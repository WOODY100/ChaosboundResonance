using System.Collections.Generic;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class ItemComparisonResult
    {
        private readonly List<EquipmentComparisonStat> stats =
            new List<EquipmentComparisonStat>();

        public ItemInstance EquippedItem { get; }

        public ItemInstance CandidateItem { get; }

        public ItemBaseData EquippedBaseData { get; }

        public ItemBaseData CandidateBaseData { get; }

        public EquipmentType EquipmentType { get; }

        public IReadOnlyList<EquipmentComparisonStat> Stats =>
            stats;

        public ItemComparisonResult(
            ItemInstance equippedItem,
            ItemInstance candidateItem,
            ItemBaseData equippedBaseData,
            ItemBaseData candidateBaseData,
            EquipmentType equipmentType)
        {
            EquippedItem =
                equippedItem;

            CandidateItem =
                candidateItem;

            EquippedBaseData =
                equippedBaseData;

            CandidateBaseData =
                candidateBaseData;

            EquipmentType =
                equipmentType;
        }

        public void AddStat(
            EquipmentComparisonStat comparisonStat)
        {
            stats.Add(comparisonStat);
        }
    }
}