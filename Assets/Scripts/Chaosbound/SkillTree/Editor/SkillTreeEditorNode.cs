using Chaosbound.SkillTree;
using UnityEngine;

public sealed class SkillTreeEditorNode
{
    private const float Width = 130f;
    private const float Height = 76f;

    public Vector2 Size
    {
        get
        {
            return new Vector2(
                Width,
                Height);
        }
    }

    private readonly SkillTreeNodeDefinition definition;

    private readonly SkillTreeEditorNodeStyle style;

    private readonly SkillTreeEditorNodeFrame frame;

    public bool IsSelected { get; set; }

    public SkillTreeEditorNode(
    SkillTreeNodeDefinition nodeDefinition)
    {
        definition = nodeDefinition;
        style = new SkillTreeEditorNodeStyle();
        frame = new SkillTreeEditorNodeFrame();

        VisualState =
            SkillTreeEditorNodeState.Available;
    }

    public SkillTreeNodeDefinition Definition
    {
        get { return definition; }
    }

    public string NodeId
    {
        get
        {
            return definition != null
                ? definition.NodeId
                : string.Empty;
        }
    }

    public Vector2 Position { get; set; }

    public Rect GetRect(Vector2 guiPosition)
    {
        Vector2 size =
            Size;

        return new Rect(
            guiPosition.x - size.x * 0.5f,
            guiPosition.y - size.y * 0.5f,
            size.x,
            size.y);
    }

    public Vector2 GetConnectionPoint(
        Vector2 guiPosition,
        Vector2 direction)
    {
        Rect rect =
            GetRect(guiPosition);

        if (direction.sqrMagnitude <= 0.000001f)
            return rect.center;

        Vector2 normalizedDirection =
            direction.normalized;

        Vector2 halfSize =
            rect.size * 0.5f;

        float x =
            Mathf.Abs(normalizedDirection.x) > 0.000001f
                ? halfSize.x / Mathf.Abs(normalizedDirection.x)
                : float.PositiveInfinity;

        float y =
            Mathf.Abs(normalizedDirection.y) > 0.000001f
                ? halfSize.y / Mathf.Abs(normalizedDirection.y)
                : float.PositiveInfinity;

        float distance =
            Mathf.Min(x, y);

        return rect.center +
               normalizedDirection * distance;
    }

    public void Draw(Rect rect)
    {
        if (definition == null)
            return;

        Color previousColor =
            GUI.color;

        // -------------------------------------------------
        // Base
        // -------------------------------------------------

        GUI.color =
            style.GetBackgroundColor(
                VisualState);

        GUI.Box(
            rect,
            GUIContent.none);

        GUI.color =
            previousColor;

        // -------------------------------------------------
        // Border
        // -------------------------------------------------

        Color borderColor =
            style.GetBorderColor(
                definition.NodeType,
                VisualState);

        float borderThickness =
            style.GetBorderThickness(
                definition.NodeType);

        SkillTreeEditorNodeFrameKind frameKind =
            style.GetFrameKind(
                definition.NodeType);

        frame.Draw(
            rect,
            borderColor,
            borderThickness,
            frameKind,
            style.SpecialInnerBorderInset,
            style.SpecialInnerBorderThickness);

        Color previousBorderColor =
            GUI.color;

        GUI.color =
            borderColor;

        // Top
        GUI.DrawTexture(
            new Rect(
                rect.x,
                rect.y,
                rect.width,
                borderThickness),
            Texture2D.whiteTexture);

        // Bottom
        GUI.DrawTexture(
            new Rect(
                rect.x,
                rect.y + rect.height - borderThickness,
                rect.width,
                borderThickness),
            Texture2D.whiteTexture);

        // Left
        GUI.DrawTexture(
            new Rect(
                rect.x,
                rect.y,
                borderThickness,
                rect.height),
            Texture2D.whiteTexture);

        // Right
        GUI.DrawTexture(
            new Rect(
                rect.x + rect.width - borderThickness,
                rect.y,
                borderThickness,
                rect.height),
            Texture2D.whiteTexture);

        GUI.color =
            previousBorderColor;

        // -------------------------------------------------
        // Header - Node Type
        // -------------------------------------------------

        Color accentColor =
            style.GetAccentColor(
                definition.NodeType,
                VisualState);

        GUIStyle typeStyle =
            new GUIStyle(GUI.skin.label);

        typeStyle.alignment =
            TextAnchor.UpperLeft;

        typeStyle.fontSize =
            style.TypeFontSize;

        typeStyle.normal.textColor =
            style.TypeTextColor;

        Rect accentRect =
            new Rect(
                rect.x +
                style.HorizontalPadding,

                rect.y +
                style.AccentIndicatorMargin,

                style.AccentIndicatorSize,
                style.AccentIndicatorSize);

        Color previousAccentColor =
            GUI.color;

        GUI.color =
            accentColor;

        GUI.DrawTexture(
            accentRect,
            Texture2D.whiteTexture);

        GUI.color =
            previousAccentColor;

        GUI.Label(
            new Rect(
                rect.x +
                style.HorizontalPadding +
                style.AccentIndicatorSize +
                4f,

                rect.y + 5f,

                rect.width - 40f,
                style.HeaderHeight),

            definition.NodeType.ToString(),
            typeStyle);

        // -------------------------------------------------
        // Cost
        // -------------------------------------------------

        GUIStyle costStyle =
            new GUIStyle(GUI.skin.label);

        costStyle.alignment =
            TextAnchor.UpperRight;

        costStyle.fontSize =
            style.CostFontSize;

        costStyle.normal.textColor =
            style.CostTextColor;

        GUI.Label(
            new Rect(
                rect.x + style.HorizontalPadding,
                rect.y + 5f,
                rect.width - 14f,
                style.HeaderHeight),
            definition.Cost.ToString(),
            costStyle);

        // -------------------------------------------------
        // Icon
        // -------------------------------------------------

        if (definition.Icon != null)
        {
            Rect iconRect =
                new Rect(
                    rect.center.x -
                    style.IconSize * 0.5f,

                    rect.y +
                    style.IconTop,

                    style.IconSize,
                    style.IconSize);

            Color previousIconColor =
                GUI.color;

            GUI.color =
                style.GetIconColor(
                    VisualState);

            GUI.DrawTexture(
                iconRect,
                definition.Icon.texture,
                ScaleMode.ScaleToFit,
                true);

            GUI.color =
                previousIconColor;
        }

        // -------------------------------------------------
        // Display Name
        // -------------------------------------------------

        string displayName =
            string.IsNullOrEmpty(
                definition.DisplayName)
                ? definition.NodeId
                : definition.DisplayName;

        GUIStyle nameStyle =
            new GUIStyle(GUI.skin.label);

        nameStyle.alignment =
            TextAnchor.MiddleCenter;

        nameStyle.fontSize =
            style.NameFontSize;

        nameStyle.fontStyle =
            style.NameFontStyle;

        nameStyle.clipping =
            TextClipping.Clip;

        nameStyle.normal.textColor =
            style.NameTextColor;

        GUI.Label(
            new Rect(
                rect.x +
                style.NameHorizontalPadding,

                rect.y +
                style.NameTop,

                rect.width -
                style.NameHorizontalPadding * 2f,

                style.NameHeight),
            displayName,
            nameStyle);

        // -------------------------------------------------
        // Selection
        // -------------------------------------------------

        if (!IsSelected)
            return;

        Color selectionPreviousColor =
            GUI.color;

        GUI.color =
            style.SelectionColor;

        GUI.Box(
            new Rect(
                rect.x -
                style.SelectionThickness,

                rect.y -
                style.SelectionThickness,

                rect.width +
                style.SelectionThickness * 2f,

                rect.height +
                style.SelectionThickness * 2f),
            GUIContent.none);

        GUI.color =
            selectionPreviousColor;
    }

    public bool ContainsPoint(
        Vector2 guiPosition,
        Vector2 mousePosition)
    {
        Rect rect =
            GetRect(guiPosition);

        return rect.Contains(
            mousePosition);
    }

    public SkillTreeEditorNodeState VisualState
    {
        get;
        set;
    }
}