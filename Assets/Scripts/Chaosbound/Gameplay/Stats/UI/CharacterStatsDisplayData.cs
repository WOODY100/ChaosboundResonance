using System.Collections.Generic;

namespace Chaosbound.Gameplay.Stats.UI
{
    public sealed class CharacterStatsDisplayData
    {
        public IReadOnlyList<CharacterStatDisplayEntry> Offense { get; }
        public IReadOnlyList<CharacterStatDisplayEntry> Defense { get; }
        public IReadOnlyList<CharacterStatDisplayEntry> Utility { get; }

        public IReadOnlyList<GlobalSkillModifierDisplayEntry>
            GlobalSkillModifiers
        { get; }

        public CharacterStatsDisplayData(
            IReadOnlyList<CharacterStatDisplayEntry> offense,
            IReadOnlyList<CharacterStatDisplayEntry> defense,
            IReadOnlyList<CharacterStatDisplayEntry> utility,
            IReadOnlyList<GlobalSkillModifierDisplayEntry> globalSkillModifiers)
        {
            Offense = offense;
            Defense = defense;
            Utility = utility;
            GlobalSkillModifiers = globalSkillModifiers;
        }
    }
}