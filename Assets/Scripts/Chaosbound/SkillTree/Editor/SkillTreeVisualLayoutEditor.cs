#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillTreeVisualLayout))]
public sealed class SkillTreeVisualLayoutEditor : Editor
{
    private SkillTreeVisualLayout layout;

    private SkillTreeDatabase database;

    private void OnEnable()
    {
        layout = (SkillTreeVisualLayout)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Chaosbound Skill Tree Layout",
            EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(
            "Visual-only data. This asset stores NodeId positions and does not contain gameplay or progression state.",
            MessageType.Info);

        EditorGUILayout.Space();

        database = (SkillTreeDatabase)EditorGUILayout.ObjectField(
            "Database",
            database,
            typeof(SkillTreeDatabase),
            false);

        EditorGUILayout.Space();

        if (database == null)
        {
            EditorGUILayout.HelpBox(
                "Assign a SkillTreeDatabase to synchronize its nodes.",
                MessageType.Warning);

            return;
        }

        if (GUILayout.Button("Sync From Database"))
        {
            SyncFromDatabase();
        }
    }

    private void SyncFromDatabase()
    {
        if (database == null)
            return;

        Undo.RecordObject(
            layout,
            "Sync Skill Tree Visual Layout");

        IReadOnlyList<SkillTreeNodeDefinition> nodes =
            database.Nodes;

        if (nodes == null)
            return;

        for (int i = 0; i < nodes.Count; i++)
        {
            SkillTreeNodeDefinition node =
                nodes[i];

            if (node == null)
                continue;

            if (string.IsNullOrWhiteSpace(node.NodeId))
                continue;

            if (layout.TryGetPosition(
                node.NodeId,
                out _))
            {
                continue;
            }

            layout.SetPosition(
                node.NodeId,
                Vector2.zero);
        }

        EditorUtility.SetDirty(layout);

        Debug.Log(
            $"[{nameof(SkillTreeVisualLayoutEditor)}] " +
            $"Layout synchronized from database '{database.name}'.",
            layout);
    }
}

#endif