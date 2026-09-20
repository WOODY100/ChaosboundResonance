using System.Collections.Generic;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentComparisonDisplayData
    {
        private readonly List<EquipmentComparisonDisplayEntry> entries =
            new List<EquipmentComparisonDisplayEntry>();

        public IReadOnlyList<EquipmentComparisonDisplayEntry> Entries =>
            entries;

        public int Count =>
            entries.Count;

        public void Add(
            EquipmentComparisonDisplayEntry entry)
        {
            entries.Add(entry);
        }
    }
}