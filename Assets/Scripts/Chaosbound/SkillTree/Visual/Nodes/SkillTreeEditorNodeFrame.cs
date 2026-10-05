#if UNITY_EDITOR

using UnityEngine;

public sealed class SkillTreeEditorNodeFrame
{
    public void Draw(
        Rect rect,
        Color color,
        float thickness,
        SkillTreeEditorNodeFrameKind frameKind,
        float innerInset,
        float innerThickness)
    {
        Color previousColor =
            GUI.color;

        GUI.color =
            color;

        if (frameKind ==
            SkillTreeEditorNodeFrameKind.Special)
        {
            DrawSpecialFrame(
                rect,
                thickness,
                innerInset,
                innerThickness);
        }
        else
        {
            DrawStandardFrame(
                rect,
                thickness);
        }

        GUI.color =
            previousColor;
    }

    private void DrawStandardFrame(
        Rect rect,
        float thickness)
    {
        GUI.DrawTexture(
            new Rect(
                rect.x,
                rect.y,
                rect.width,
                thickness),
            Texture2D.whiteTexture);

        GUI.DrawTexture(
            new Rect(
                rect.x,
                rect.y + rect.height - thickness,
                rect.width,
                thickness),
            Texture2D.whiteTexture);

        GUI.DrawTexture(
            new Rect(
                rect.x,
                rect.y,
                thickness,
                rect.height),
            Texture2D.whiteTexture);

        GUI.DrawTexture(
            new Rect(
                rect.x + rect.width - thickness,
                rect.y,
                thickness,
                rect.height),
            Texture2D.whiteTexture);
    }

    private void DrawSpecialFrame(
        Rect rect,
        float thickness,
        float innerInset,
        float innerThickness)
    {
        // Outer frame
        DrawStandardFrame(
            rect,
            thickness);

        // Inner frame
        Rect innerRect =
            new Rect(
                rect.x + innerInset,
                rect.y + innerInset,
                rect.width - innerInset * 2f,
                rect.height - innerInset * 2f);

        DrawStandardFrame(
            innerRect,
            innerThickness);
    }
}

#endif