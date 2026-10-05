using Chaosbound.SkillTree;
using UnityEngine;

public sealed class SkillTreeEditorNodeStyle
{
    public readonly Color BackgroundColor =
        new Color(
            0.12f,
            0.12f,
            0.12f,
            1f);

    public readonly Color TypeTextColor =
        new Color(
            0.55f,
            0.55f,
            0.55f,
            1f);

    public readonly Color CostTextColor =
        new Color(
            0.75f,
            0.75f,
            0.75f,
            1f);

    public readonly Color NameTextColor =
        new Color(
            0.9f,
            0.9f,
            0.9f,
            1f);

    public readonly Color SelectionColor =
        new Color(
            0.3f,
            0.8f,
            1f,
            1f);

    public readonly Color RootAccentColor =
        new Color(
            0.85f,
            0.85f,
            0.85f,
            1f);

    public readonly Color GateAccentColor =
        new Color(
            0.55f,
            0.65f,
            0.75f,
            1f);

    public readonly Color StatAccentColor =
        new Color(
            0.65f,
            0.75f,
            0.85f,
            1f);

    public readonly Color FocusAccentColor =
        new Color(
            0.75f,
            0.65f,
            0.85f,
            1f);

    public readonly Color SpecializationAccentColor =
        new Color(
            0.85f,
            0.65f,
            0.45f,
            1f);

    public readonly Color ChoiceAccentColor =
        new Color(
            0.75f,
            0.75f,
            0.45f,
            1f);

    public readonly Color KeystoneAccentColor =
        new Color(
            0.9f,
            0.55f,
            0.3f,
            1f);

    public readonly Color LockedBackgroundColor =
        new Color(
            0.065f,
            0.065f,
            0.075f,
            1f);

    public readonly Color AvailableBackgroundColor =
        new Color(
            0.13f,
            0.10f,
            0.16f,
            1f);

    public readonly Color PurchasedBackgroundColor =
        new Color(
            0.22f,
            0.12f,
            0.26f,
            1f);

    public readonly Color LockedTextColor =
        new Color(
            0.45f,
            0.45f,
            0.45f,
            1f);

    public readonly Color PurchasedTextColor =
        new Color(
            1f,
            1f,
            1f,
            1f);

    public readonly Color LockedIconColor =
        new Color(
            0.45f,
            0.45f,
            0.45f,
            1f);

    public readonly Color AvailableIconColor =
        new Color(
            1f,
            1f,
            1f,
            1f);

    public readonly Color PurchasedIconColor =
        new Color(
            1.15f,
            0.9f,
            1.15f,
            1f);

    public readonly int TypeFontSize = 9;

    public readonly int CostFontSize = 10;

    public readonly int NameFontSize = 11;

    public readonly float HorizontalPadding = 7f;

    public readonly float NameHorizontalPadding = 8f;

    public readonly float HeaderHeight = 14f;

    public readonly float IconSize = 28f;

    public readonly float IconTop = 20f;

    public readonly float NameTop = 48f;

    public readonly float NameHeight = 20f;

    public readonly float SelectionThickness = 3f;

    public readonly FontStyle NameFontStyle =
        FontStyle.Bold;

    public readonly float AccentIndicatorSize = 6f;

    public readonly float AccentIndicatorMargin = 5f;

    public readonly float LockedAccentIntensity = 0.35f;

    public readonly float AvailableAccentIntensity = 1f;

    public readonly float PurchasedAccentIntensity = 1.35f;

    public readonly float BorderThickness = 1f;

    public readonly float LockedBorderIntensity = 0.45f;

    public readonly float AvailableBorderIntensity = 0.85f;

    public readonly float PurchasedBorderIntensity = 1.25f;

    public readonly float StandardBorderThickness = 1f;

    public readonly float SpecialBorderThickness = 2f;

    public readonly float SpecialInnerBorderInset = 4f;

    public readonly float SpecialInnerBorderThickness = 1f;

    public Color GetAccentColor(
        SkillTreeNodeType nodeType)
    {
        switch (nodeType)
        {
            case SkillTreeNodeType.Root:
                return RootAccentColor;

            case SkillTreeNodeType.Gate:
                return GateAccentColor;

            case SkillTreeNodeType.Stat:
                return StatAccentColor;

            case SkillTreeNodeType.Focus:
                return FocusAccentColor;

            case SkillTreeNodeType.Specialization:
                return SpecializationAccentColor;

            case SkillTreeNodeType.Choice:
                return ChoiceAccentColor;

            case SkillTreeNodeType.Keystone:
                return KeystoneAccentColor;

            default:
                return TypeTextColor;
        }
    }

    public Color GetAccentColor(
        SkillTreeNodeType nodeType,
        SkillTreeEditorNodeState state)
    {
        Color accentColor =
            GetAccentColor(nodeType);

        float intensity;

        switch (state)
        {
            case SkillTreeEditorNodeState.Locked:
                intensity =
                    LockedAccentIntensity;
                break;

            case SkillTreeEditorNodeState.Purchased:
                intensity =
                    PurchasedAccentIntensity;
                break;

            case SkillTreeEditorNodeState.Available:
            default:
                intensity =
                    AvailableAccentIntensity;
                break;
        }

        return new Color(
            Mathf.Clamp01(
                accentColor.r * intensity),

            Mathf.Clamp01(
                accentColor.g * intensity),

            Mathf.Clamp01(
                accentColor.b * intensity),

            accentColor.a);
    }

    public Color GetBackgroundColor(
        SkillTreeEditorNodeState state)
    {
        switch (state)
        {
            case SkillTreeEditorNodeState.Locked:
                return LockedBackgroundColor;

            case SkillTreeEditorNodeState.Purchased:
                return PurchasedBackgroundColor;

            case SkillTreeEditorNodeState.Available:
            default:
                return AvailableBackgroundColor;
        }
    }

    public Color GetIconColor(
        SkillTreeEditorNodeState state)
    {
        switch (state)
        {
            case SkillTreeEditorNodeState.Locked:
                return LockedIconColor;

            case SkillTreeEditorNodeState.Purchased:
                return PurchasedIconColor;

            case SkillTreeEditorNodeState.Available:
            default:
                return AvailableIconColor;
        }
    }

    public Color GetBorderColor(
        SkillTreeNodeType nodeType,
        SkillTreeEditorNodeState state)
    {
        Color accentColor =
            GetAccentColor(nodeType);

        float intensity;

        switch (state)
        {
            case SkillTreeEditorNodeState.Locked:
                intensity =
                    LockedBorderIntensity;
                break;

            case SkillTreeEditorNodeState.Purchased:
                intensity =
                    PurchasedBorderIntensity;
                break;

            case SkillTreeEditorNodeState.Available:
            default:
                intensity =
                    AvailableBorderIntensity;
                break;
        }

        return new Color(
            Mathf.Clamp01(
                accentColor.r * intensity),

            Mathf.Clamp01(
                accentColor.g * intensity),

            Mathf.Clamp01(
                accentColor.b * intensity),

            0.85f);
    }

    public bool UsesSpecialFrame(
    SkillTreeNodeType nodeType)
    {
        switch (nodeType)
        {
            case SkillTreeNodeType.Root:
            case SkillTreeNodeType.Gate:
            case SkillTreeNodeType.Choice:
            case SkillTreeNodeType.Keystone:
                return true;

            default:
                return false;
        }
    }

    public float GetBorderThickness(
    SkillTreeNodeType nodeType)
    {
        if (UsesSpecialFrame(nodeType))
            return SpecialBorderThickness;

        return StandardBorderThickness;
    }

    public SkillTreeEditorNodeFrameKind GetFrameKind(
    SkillTreeNodeType nodeType)
    {
        if (UsesSpecialFrame(nodeType))
            return SkillTreeEditorNodeFrameKind.Special;

        return SkillTreeEditorNodeFrameKind.Standard;
    }
}