#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillTreeNodeView))]
public sealed class SkillTreeNodeViewEditor : Editor
{
    private SkillTreeNodeView nodeView;

    private void OnEnable()
    {
        nodeView = (SkillTreeNodeView)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Chaosbound Skill Tree Node",
            EditorStyles.boldLabel);

        EditorGUILayout.LabelField(
            "Node ID",
            nodeView.NodeId);

        if (nodeView.RectTransform != null)
        {
            EditorGUILayout.Vector2Field(
                "Visual Position",
                nodeView.RectTransform.anchoredPosition);
        }
    }

    private void OnSceneGUI()
    {
        if (nodeView == null)
            return;

        RectTransform rectTransform =
            nodeView.RectTransform;

        if (rectTransform == null)
            return;

        SkillTreeVisualController controller =
            nodeView.GetComponentInParent<SkillTreeVisualController>();

        if (controller == null)
            return;

        Vector3 currentWorldPosition =
            rectTransform.position;

        EditorGUI.BeginChangeCheck();

        Vector3 newWorldPosition =
            Handles.FreeMoveHandle(
                currentWorldPosition,
                HandleUtility.GetHandleSize(
                    currentWorldPosition) * 0.15f,
                Vector3.zero,
                Handles.RectangleHandleCap);

        if (!EditorGUI.EndChangeCheck())
            return;

        SkillTreeVisualLayout layout =
            controller.VisualLayout;

        Undo.RecordObject(
            rectTransform,
            "Move Skill Tree Node");

        if (layout != null)
        {
            Undo.RecordObject(
                layout,
                "Move Skill Tree Node");
        }

        rectTransform.position =
            newWorldPosition;

        if (layout != null)
        {
            layout.SetPosition(
                nodeView.NodeId,
                rectTransform.anchoredPosition);

            EditorUtility.SetDirty(
                layout);
        }

        EditorUtility.SetDirty(
            rectTransform);
    }
}

#endif