namespace Chaosbound.Gameplay.Stats.UI
{
    public sealed class GlobalSkillModifierDisplayEntry
    {
        public string ModifierId { get; }
        public string DisplayName { get; }
        public float Value { get; }
        public StatDisplayFormat Format { get; }

        public GlobalSkillModifierDisplayEntry(
            string modifierId,
            string displayName,
            float value,
            StatDisplayFormat format)
        {
            ModifierId = modifierId;
            DisplayName = displayName;
            Value = value;
            Format = format;
        }
    }
}