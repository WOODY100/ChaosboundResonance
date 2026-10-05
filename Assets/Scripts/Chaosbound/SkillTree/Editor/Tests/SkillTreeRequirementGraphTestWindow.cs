#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public sealed class SkillTreeRequirementGraphTestWindow : EditorWindow
{
    private SkillTreeDatabase database;

    private Vector2 scrollPosition;

    [MenuItem("Chaosbound/Skill Tree/Test Requirement Graph")]
    private static void Open()
    {
        GetWindow<SkillTreeRequirementGraphTestWindow>(
            "Skill Tree Graph Test");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Skill Tree Requirement Graph Test",
            EditorStyles.boldLabel);

        EditorGUILayout.Space();

        database = (SkillTreeDatabase)EditorGUILayout.ObjectField(
            "Skill Tree Database",
            database,
            typeof(SkillTreeDatabase),
            false);

        EditorGUILayout.Space();

        if (database == null)
        {
            EditorGUILayout.HelpBox(
                "Assign a SkillTreeDatabase to begin the test.",
                MessageType.Info);

            return;
        }

        EditorGUILayout.LabelField(
            "Tree ID",
            database.TreeId);

        EditorGUILayout.LabelField(
            "Nodes",
            database.Nodes != null
                ? database.Nodes.Count.ToString()
                : "0");

        EditorGUILayout.Space();

        if (GUILayout.Button("Validate Database"))
        {
            bool isValid = database.Validate();

            Debug.Log(
                $"[SkillTree Graph Test] Database validation result: {isValid}",
                database);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Build Requirement Graph"))
        {
            BuildGraph();
        }

        EditorGUILayout.Space();

        DrawNodeSummary();
    }

    private void BuildGraph()
    {
        IReadOnlyList<SkillTreeConnectionDescriptor> prerequisiteConnections =
            SkillTreeRequirementGraphBuilder.Build(database);

        IReadOnlyList<SkillTreeConnectionDescriptor> structuralConnections =
            SkillTreeStructuralGraphBuilder.Build(database);

        Debug.Log(
            $"[SkillTree Graph Test] " +
            $"Requirement connections: {prerequisiteConnections.Count}",
            database);

        for (int i = 0; i < prerequisiteConnections.Count; i++)
        {
            SkillTreeConnectionDescriptor connection =
                prerequisiteConnections[i];

            Debug.Log(
                $"[SkillTree Graph Test] " +
                $"[{connection.Kind}] " +
                $"{connection.SourceNodeId} -> " +
                $"{connection.TargetNodeId}",
                database);
        }

        Debug.Log(
            $"[SkillTree Graph Test] " +
            $"Structural connections: {structuralConnections.Count}",
            database);

        for (int i = 0; i < structuralConnections.Count; i++)
        {
            SkillTreeConnectionDescriptor connection =
                structuralConnections[i];

            Debug.Log(
                $"[SkillTree Graph Test] " +
                $"[{connection.Kind}] " +
                $"{connection.SourceNodeId} -> " +
                $"{connection.TargetNodeId}",
                database);
        }

        Debug.Log(
            $"[SkillTree Graph Test] " +
            $"Total connections: " +
            (prerequisiteConnections.Count + structuralConnections.Count),
            database);
    }

    private void DrawNodeSummary()
    {
        if (database.Nodes == null)
            return;

        EditorGUILayout.LabelField(
            "Current Nodes",
            EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(
            scrollPosition,
            GUILayout.MinHeight(150f));

        for (int i = 0; i < database.Nodes.Count; i++)
        {
            SkillTreeNodeDefinition node =
                database.Nodes[i];

            if (node == null)
            {
                EditorGUILayout.LabelField(
                    $"[{i}] NULL",
                    EditorStyles.miniLabel);

                continue;
            }

            EditorGUILayout.LabelField(
                $"[{i}] {node.NodeId}");
        }

        EditorGUILayout.EndScrollView();
    }
}

#endif