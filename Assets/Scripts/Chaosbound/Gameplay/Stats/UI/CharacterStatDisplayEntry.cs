using Chaosbound.Gameplay.Equipment;

namespace Chaosbound.Gameplay.Stats.UI
{
    public sealed class CharacterStatDisplayEntry
    {
        public StatType StatType { get; }
        public float Value { get; }
        public StatDisplayFormat Format { get; }

        public CharacterStatDisplayEntry(
            StatType statType,
            float value,
            StatDisplayFormat format)
        {
            StatType = statType;
            Value = value;
            Format = format;
        }
    }
}