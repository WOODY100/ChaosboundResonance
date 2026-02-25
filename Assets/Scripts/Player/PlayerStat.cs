using System.Collections.Generic;

public class PlayerStat
{
    public float BaseValue;
    public float CurrentValue { get; private set; }

    public void Recalculate(List<StatModifier> modifiers)
    {
        float flat = 0f;
        float percent = 0f;
        float finalMultiplier = 1f;

        foreach (var mod in modifiers)
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

        CurrentValue = (BaseValue + flat) * (1f + percent) * finalMultiplier;
    }
}