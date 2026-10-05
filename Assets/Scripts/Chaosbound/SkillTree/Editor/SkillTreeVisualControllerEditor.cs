#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillTreeVisualController))]
public sealed class SkillTreeVisualControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        SkillTreeVisualController controller =
            (SkillTreeVisualController)target;

        if (GUILayout.Button("Build Skill Tree"))
        {
            controller.Build();

            EditorUtility.SetDirty(controller);
        }

        if (GUILayout.Button("Apply Layout"))
        {
            controller.ApplyLayout();
            EditorUtility.SetDirty(controller);
        }

        if (GUILayout.Button("Clear Skill Tree"))
        {
            controller.Clear();

            EditorUtility.SetDirty(controller);
        }
    }
}

#endif