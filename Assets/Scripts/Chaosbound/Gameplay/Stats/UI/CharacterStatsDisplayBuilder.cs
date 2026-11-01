using System.Collections.Generic;

namespace Chaosbound.Gameplay.Stats.UI
{
    public sealed class CharacterStatsDisplayBuilder
    {
        private readonly PlayerModifierSystem modifierSystem;
        private readonly PlayerSkillLoadout skillLoadout;

        public CharacterStatsDisplayBuilder(
            PlayerModifierSystem modifierSystem,
            PlayerSkillLoadout skillLoadout)
        {
            this.modifierSystem = modifierSystem;
            this.skillLoadout = skillLoadout;
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
            BuildGlobalSkillModifiers(globalSkillModifiers);

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


        private void BuildGlobalSkillModifiers(
            List<GlobalSkillModifierDisplayEntry> result)
        {
            if (skillLoadout == null || skillLoadout.GlobalModifiers == null)
                return;

            IReadOnlyList<ModifierEntry> modifiers =
                skillLoadout.GlobalModifiers;

            for (int i = 0; i < modifiers.Count; i++)
            {
                ModifierEntry modifier = modifiers[i];
                string displayName;
                StatDisplayFormat format;
                float displayValue = modifier.Value;

                GetGlobalModifierPresentation(
                    modifier.Type,
                    out displayName,
                    out format);

                if (IsBooleanModifier(modifier.Type))
                    displayValue = 1f;

                result.Add(new GlobalSkillModifierDisplayEntry(
                    modifier.Type.ToString(),
                    displayName,
                    displayValue,
                    format));
            }
        }

        private static void GetGlobalModifierPresentation(
            SkillModifierType type,
            out string displayName,
            out StatDisplayFormat format)
        {
            displayName = type.ToString();
            format = StatDisplayFormat.Decimal;

            switch (type)
            {
                case SkillModifierType.FlatDamage:
                    displayName = "Skill Damage";
                    format = StatDisplayFormat.Integer;
                    break;
                case SkillModifierType.PercentDamage:
                    displayName = "Skill Damage";
                    format = StatDisplayFormat.Percent;
                    break;
                case SkillModifierType.CriticalChance:
                    displayName = "Skill Critical Chance";
                    format = StatDisplayFormat.Percent;
                    break;
                case SkillModifierType.CriticalDamageBonus:
                    displayName = "Skill Critical Damage";
                    format = StatDisplayFormat.Percent;
                    break;
                case SkillModifierType.CooldownPercent:
                    displayName = "Skill Cooldown Reduction";
                    format = StatDisplayFormat.Percent;
                    break;
                case SkillModifierType.TickRatePercent:
                    displayName = "Skill Tick Rate";
                    format = StatDisplayFormat.Percent;
                    break;
                case SkillModifierType.SpawnRadiusPercent:
                    displayName = "Skill Spawn Radius";
                    format = StatDisplayFormat.Percent;
                    break;
                case SkillModifierType.ImpactRadiusPercent:
                    displayName = "Skill Impact Radius";
                    format = StatDisplayFormat.Percent;
                    break;
                case SkillModifierType.RangePercent:
                    displayName = "Skill Range";
                    format = StatDisplayFormat.Percent;
                    break;
                case SkillModifierType.DurationPercent:
                    displayName = "Skill Duration";
                    format = StatDisplayFormat.Percent;
                    break;
                case SkillModifierType.ExtraCount:
                    displayName = "Extra Skill Count";
                    format = StatDisplayFormat.Integer;
                    break;
                case SkillModifierType.Penetration:
                    displayName = "Skill Penetration";
                    format = StatDisplayFormat.Integer;
                    break;
                case SkillModifierType.ChainCount:
                    displayName = "Skill Chain Count";
                    format = StatDisplayFormat.Integer;
                    break;
                case SkillModifierType.SpawnZoneOnHit:
                    displayName = "Spawn Zone Chance";
                    format = StatDisplayFormat.Percent;
                    break;
                case SkillModifierType.ExplodeOnKill:
                    displayName = "Explode on Kill";
                    format = StatDisplayFormat.Integer;
                    break;
                case SkillModifierType.SplitOnImpact:
                    displayName = "Split on Impact";
                    format = StatDisplayFormat.Integer;
                    break;
                case SkillModifierType.ApplyBurn:
                    displayName = "Apply Burn";
                    format = StatDisplayFormat.Integer;
                    break;
                case SkillModifierType.ApplyShock:
                    displayName = "Apply Shock";
                    format = StatDisplayFormat.Integer;
                    break;
                case SkillModifierType.ApplyPoison:
                    displayName = "Apply Poison";
                    format = StatDisplayFormat.Integer;
                    break;
            }
        }

        private static bool IsBooleanModifier(SkillModifierType type)
        {
            return type == SkillModifierType.ExplodeOnKill ||
                   type == SkillModifierType.SplitOnImpact ||
                   type == SkillModifierType.ApplyBurn ||
                   type == SkillModifierType.ApplyShock ||
                   type == SkillModifierType.ApplyPoison;
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