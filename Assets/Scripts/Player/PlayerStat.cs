using System.Collections.Generic;
using Player.Enums;

public class PlayerStat
{
    public float BaseValue { get; private set; }
    public float CurrentValue { get; private set; }

    internal void SetBaseValue(float value)
    {
        BaseValue = value;
    }

    public void Recalculate(
        List<StatModifier> modifiers,
        PercentBehavior percentBehavior)
    {
        float flat = 0f;
        float percent = 0f;
        float finalMultiplier = 1f;

        foreach (StatModifier mod in modifiers)
        {
            switch (mod.ModifierType)
            {
                case ModifierType.Flat:
                    flat += mod.Value;
                    break;

                case ModifierType.Percent:
                    percent += mod.Value;
                    break;

                case ModifierType.FinalMultiplier:
                    finalMultiplier *= mod.Value;
                    break;
            }
        }

        float value =
            BaseValue + flat;

        if (percentBehavior ==
            PercentBehavior.Additive)
        {
            value += percent;
        }
        else
        {
            value *= 1f + percent;
        }

        value *= finalMultiplier;

        CurrentValue = value;
    }
}