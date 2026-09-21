using System;
using Player.Enums;

public static class StatTypeRules
{
    public static PercentBehavior GetPercentBehavior(
        StatType statType)
    {
        switch (statType)
        {
            // -------------------------------------------------
            // Percent Additive
            // -------------------------------------------------

            case StatType.CritChance:
            case StatType.CritDamage:
            case StatType.DamageReduction:
            case StatType.Luck:
            case StatType.XPGain:
                return PercentBehavior.Additive;

            // -------------------------------------------------
            // Percent Multiplicative
            // -------------------------------------------------

            case StatType.Damage:
            case StatType.AttackSpeed:
            case StatType.MovementSpeed:
            case StatType.MaxHP:
            case StatType.HPRegen:
            case StatType.Shield:
            case StatType.PickupRadius:
                return PercentBehavior.Multiplicative;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(statType),
                    statType,
                    $"Unknown StatType '{statType}'.");
        }
    }
}