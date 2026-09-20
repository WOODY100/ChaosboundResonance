namespace Chaosbound.Gameplay.Equipment
{
    public readonly struct EquipmentStatValue
    {
        public ModifierType ModifierType { get; }

        public float Value { get; }

        public EquipmentStatValue(
            ModifierType modifierType,
            float value)
        {
            ModifierType = modifierType;
            Value = value;
        }
    }
}