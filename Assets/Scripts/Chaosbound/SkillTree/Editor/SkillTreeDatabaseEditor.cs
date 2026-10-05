using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillTreeDatabase))]
public sealed class SkillTreeDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(12);

        EditorGUILayout.LabelField(
            "Validation",
            EditorStyles.boldLabel);

        SkillTreeDatabase database =
            (SkillTreeDatabase)target;

        if (GUILayout.Button("Validate Database"))
        {
            bool isValid = database.Validate();

            if (isValid)
            {
                Debug.Log(
                    $"[{nameof(SkillTreeDatabase)}] Database '{database.TreeId}' is valid.",
                    database);
            }
        }
    }
}