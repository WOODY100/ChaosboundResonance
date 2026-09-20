using Chaosbound.Gameplay.Equipment;

namespace Chaosbound.Gameplay.Items.UI.Tooltip
{
    public readonly struct EquipmentTooltipStatData
    {
        public StatType StatType { get; }
        public ModifierType ModifierType { get; }
        public float Value { get; }

        public EquipmentTooltipStatData(
            StatType statType,
            ModifierType modifierType,
            float value)
        {
            StatType = statType;
            ModifierType = modifierType;
            Value = value;
        }
    }
}