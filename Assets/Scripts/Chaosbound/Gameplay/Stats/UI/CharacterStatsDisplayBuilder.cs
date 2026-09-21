using System.Collections.Generic;

namespace Chaosbound.Gameplay.Stats.UI
{
    public sealed class CharacterStatsDisplayBuilder
    {
        private readonly PlayerModifierSystem modifierSystem;

        public CharacterStatsDisplayBuilder(
            PlayerModifierSystem modifierSystem)
        {
            this.modifierSystem = modifierSystem;
        }

        public CharacterStatsDisplayData Build()
        {
            List<CharacterStatDisplayEntry> offense =
                new List<CharacterStatDisplayEntry>();

            List<CharacterStatDisplayEntry> defense =
                new List<CharacterStatDisplayEntry>();

            List<CharacterStatDisplayEntry> utility =
                new List<CharacterStatDisplayEntry>();

            List<GlobalSkillModifierDisplayEntry> globalSkillModifiers =
                new List<GlobalSkillModifierDisplayEntry>();

            BuildOffense(offense);
            BuildDefense(defense);
            BuildUtility(utility);

            return new CharacterStatsDisplayData(
                offense,
                defense,
                utility,
                globalSkillModifiers);
        }

        private void BuildOffense(
            List<CharacterStatDisplayEntry> result)
        {
            result.Add(CreateEntry(
                StatType.Damage,
                StatDisplayFormat.Integer));

            result.Add(CreateEntry(
                StatType.AttackSpeed,
                StatDisplayFormat.Decimal));

            result.Add(CreateEntry(
                StatType.CritChance,
                StatDisplayFormat.Percent));

            result.Add(CreateEntry(
                StatType.CritDamage,
                StatDisplayFormat.Percent));
        }

        private void BuildDefense(
            List<CharacterStatDisplayEntry> result)
        {
            result.Add(CreateEntry(
                StatType.MaxHP,
                StatDisplayFormat.Integer));

            result.Add(CreateEntry(
                StatType.HPRegen,
                StatDisplayFormat.PerSecond));

            result.Add(CreateEntry(
                StatType.DamageReduction,
                StatDisplayFormat.Percent));

            result.Add(CreateEntry(
                StatType.Shield,
                StatDisplayFormat.Integer));
        }

        private void BuildUtility(
            List<CharacterStatDisplayEntry> result)
        {
            result.Add(CreateEntry(
                StatType.MovementSpeed,
                StatDisplayFormat.Decimal));

            result.Add(CreateEntry(
                StatType.PickupRadius,
                StatDisplayFormat.Decimal));

            result.Add(CreateEntry(
                StatType.Luck,
                StatDisplayFormat.Percent));

            result.Add(CreateEntry(
                StatType.XPGain,
                StatDisplayFormat.Percent));
        }

        private CharacterStatDisplayEntry CreateEntry(
            StatType statType,
            StatDisplayFormat format)
        {
            float value =
                modifierSystem.GetStat(statType);

            return new CharacterStatDisplayEntry(
                statType,
                value,
                format);
        }
    }
}