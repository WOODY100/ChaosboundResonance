using System.Collections.Generic;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentStatSnapshot
    {
        private readonly Dictionary<StatType, EquipmentStatValue> stats =
            new Dictionary<StatType, EquipmentStatValue>();

        public IReadOnlyDictionary<StatType, EquipmentStatValue> Stats =>
            stats;

        public int Count =>
            stats.Count;

        public void Set(
            StatType statType,
            ModifierType modifierType,
            float value)
        {
            stats[statType] =
                new EquipmentStatValue(
                    modifierType,
                    value);
        }

        public bool TryGet(
            StatType statType,
            out EquipmentStatValue statValue)
        {
            return stats.TryGetValue(
                statType,
                out statValue);
        }
    }
}