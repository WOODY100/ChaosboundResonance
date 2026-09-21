namespace Chaosbound.Gameplay.Stats.UI
{
    public static class StatDisplayNameFormatter
    {
        public static string Format(StatType statType)
        {
            switch (statType)
            {
                case StatType.Damage:
                    return "DAMAGE";

                case StatType.AttackSpeed:
                    return "ATTACK SPEED";

                case StatType.MovementSpeed:
                    return "MOVEMENT SPEED";

                case StatType.CritChance:
                    return "CRIT CHANCE";

                case StatType.CritDamage:
                    return "CRIT DAMAGE";

                case StatType.MaxHP:
                    return "MAX HP";

                case StatType.HPRegen:
                    return "HP REGEN";

                case StatType.DamageReduction:
                    return "DAMAGE REDUCTION";

                case StatType.Shield:
                    return "SHIELD";

                case StatType.PickupRadius:
                    return "PICKUP RADIUS";

                case StatType.Luck:
                    return "LUCK";

                case StatType.XPGain:
                    return "XP GAIN";

                default:
                    return statType.ToString().ToUpperInvariant();
            }
        }
    }
}