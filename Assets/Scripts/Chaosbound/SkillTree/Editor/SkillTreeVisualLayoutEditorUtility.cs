#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class SkillTreeVisualLayoutEditorUtility
{
    public static bool SetNodePosition(
        SkillTreeVisualLayout layout,
        string nodeId,
        Vector2 position)
    {
        if (layout == null)
            return false;

        if (string.IsNullOrWhiteSpace(nodeId))
            return false;

        layout.SetPosition(
            nodeId,
            position);

        EditorUtility.SetDirty(
            layout);

        return true;
    }
}

#endif