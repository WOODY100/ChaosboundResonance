using System.Text;
using TMPro;
using UnityEngine;

public sealed class SkillTreeNodeEffectDetailsView : MonoBehaviour
{
    [Header("Content")]

    [SerializeField]
    private TMP_Text textEffects;

    private void Awake()
    {
        Hide();
    }

    public void Show(
        SkillTreeNodeDefinition definition)
    {
        if (definition == null)
        {
            Hide();
            return;
        }

        if (definition.Effects == null ||
            definition.Effects.Length == 0)
        {
            Hide();
            return;
        }

        StringBuilder builder =
            new StringBuilder();

        for (int i = 0;
             i < definition.Effects.Length;
             i++)
        {
            SkillTreeEffect effect =
                definition.Effects[i];

            if (effect == null)
                continue;

            BuildEffectText(
                effect,
                builder);
        }

        if (builder.Length == 0)
        {
            Hide();
            return;
        }

        if (textEffects != null)
        {
            textEffects.text =
                builder.ToString();
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (textEffects != null)
        {
            textEffects.text =
                string.Empty;
        }

        gameObject.SetActive(false);
    }

    private void BuildEffectText(
        SkillTreeEffect effect,
        StringBuilder builder)
    {
        PlayerStatEffect playerStatEffect =
            effect as PlayerStatEffect;

        if (playerStatEffect != null)
        {
            AppendLine(
                builder,
                FormatPlayerStatEffect(
                    playerStatEffect));

            return;
        }

        GlobalSkillModifierEffect
            globalSkillModifierEffect =
                effect as GlobalSkillModifierEffect;

        if (globalSkillModifierEffect != null)
        {
            AppendLine(
                builder,
                FormatGlobalSkillModifierEffect(
                    globalSkillModifierEffect));

            return;
        }

        RuleEffect ruleEffect =
            effect as RuleEffect;

        if (ruleEffect != null)
        {
            AppendLine(
                builder,
                FormatRuleEffect(
                    ruleEffect));

            return;
        }

        AppendLine(
            builder,
            effect.GetType().Name);
    }

    private void AppendLine(
        StringBuilder builder,
        string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        if (builder.Length > 0)
        {
            builder.AppendLine();
        }

        builder.Append("• ");
        builder.Append(text);
    }

    private string FormatPlayerStatEffect(
        PlayerStatEffect effect)
    {
        string statName =
            FormatEnumName(effect.StatType);

        switch (effect.ModifierType)
        {
            case ModifierType.Flat:
                return
                    $"+{effect.Value:0.##} " +
                    statName;

            case ModifierType.Percent:
                return
                    $"+{effect.Value * 100f:0.##}% " +
                    statName;

            case ModifierType.FinalMultiplier:
                return
                    $"+{effect.Value * 100f:0.##}% " +
                    $"Final {statName}";

            default:
                return
                    $"{statName} " +
                    $"{effect.Value:0.##}";
        }
    }

    private string FormatGlobalSkillModifierEffect(
        GlobalSkillModifierEffect effect)
    {
        string modifierName =
            FormatGlobalSkillModifierName(
                effect.ModifierType);

        return
            FormatGlobalSkillModifierValue(
                effect.ModifierType,
                modifierName,
                effect.Value);
    }

    private string FormatGlobalSkillModifierValue(
        SkillModifierType modifierType,
        string modifierName,
        float value)
    {
        switch (modifierType)
        {
            case SkillModifierType.FlatDamage:
                return
                    $"+{value:0.##} " +
                    modifierName;

            case SkillModifierType.PercentDamage:
                return
                    $"+{value * 100f:0.##}% " +
                    modifierName;

            case SkillModifierType.CooldownPercent:
            case SkillModifierType.TickRatePercent:
            case SkillModifierType.SpawnRadiusPercent:
            case SkillModifierType.ImpactRadiusPercent:
            case SkillModifierType.RangePercent:
            case SkillModifierType.DurationPercent:
                return
                    $"+{value * 100f:0.##}% " +
                    modifierName;

            case SkillModifierType.ExtraCount:
            case SkillModifierType.Penetration:
            case SkillModifierType.ChainCount:
                return
                    $"+{value:0.##} " +
                    modifierName;

            default:
                return
                    $"{modifierName} " +
                    $"{value:0.##}";
        }
    }

    private string FormatGlobalSkillModifierName(
        SkillModifierType modifierType)
    {
        switch (modifierType)
        {
            case SkillModifierType.FlatDamage:
                return "Skill Damage";

            case SkillModifierType.PercentDamage:
                return "Skill Damage";

            case SkillModifierType.CooldownPercent:
                return "Cooldown Reduction";

            case SkillModifierType.TickRatePercent:
                return "Tick Rate";

            case SkillModifierType.SpawnRadiusPercent:
                return "Spawn Radius";

            case SkillModifierType.ImpactRadiusPercent:
                return "Impact Radius";

            case SkillModifierType.RangePercent:
                return "Range";

            case SkillModifierType.DurationPercent:
                return "Duration";

            case SkillModifierType.ExtraCount:
                return "Extra Count";

            case SkillModifierType.Penetration:
                return "Penetration";

            case SkillModifierType.ChainCount:
                return "Chain Count";

            default:
                return FormatEnumName(
                    modifierType);
        }
    }

    private string FormatRuleEffect(
        RuleEffect effect)
    {
        if (string.IsNullOrWhiteSpace(
            effect.RuleId))
        {
            return "Rule";
        }

        return
            $"Rule: {effect.RuleId}";
    }

    private string FormatEnumName<T>(
        T value)
    {
        string raw =
            value.ToString();

        if (string.IsNullOrEmpty(raw))
            return string.Empty;

        StringBuilder builder =
            new StringBuilder();

        for (int i = 0;
             i < raw.Length;
             i++)
        {
            char character =
                raw[i];

            if (i > 0 &&
                char.IsUpper(character) &&
                !char.IsUpper(raw[i - 1]))
            {
                builder.Append(' ');
            }

            builder.Append(character);
        }

        return builder.ToString();
    }
}