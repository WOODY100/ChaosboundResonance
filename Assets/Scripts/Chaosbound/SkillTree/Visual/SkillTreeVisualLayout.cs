using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SkillTreeVisualLayout",
    menuName = "Chaosbound/Skill Tree/Visual Layout")]
public sealed class SkillTreeVisualLayout : ScriptableObject
{
    [Serializable]
    public struct NodePosition
    {
        public string NodeId;
        public Vector2 Position;

        public NodePosition(
            string nodeId,
            Vector2 position)
        {
            NodeId = nodeId;
            Position = position;
        }
    }

    [SerializeField]
    private List<NodePosition> nodePositions =
        new List<NodePosition>();

    public IReadOnlyList<NodePosition> NodePositions
    {
        get { return nodePositions; }
    }

    public bool TryGetPosition(
        string nodeId,
        out Vector2 position)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
        {
            position = Vector2.zero;
            return false;
        }

        for (int i = 0; i < nodePositions.Count; i++)
        {
            NodePosition entry = nodePositions[i];

            if (entry.NodeId == nodeId)
            {
                position = entry.Position;
                return true;
            }
        }

        position = Vector2.zero;
        return false;
    }

    public void SetPosition(
        string nodeId,
        Vector2 position)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            return;

        for (int i = 0; i < nodePositions.Count; i++)
        {
            if (nodePositions[i].NodeId != nodeId)
                continue;

            NodePosition updated =
                nodePositions[i];

            updated.Position = position;

            nodePositions[i] = updated;
            return;
        }

        nodePositions.Add(
            new NodePosition(
                nodeId,
                position));
    }

    public bool RemovePosition(
        string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            return false;

        for (int i = 0; i < nodePositions.Count; i++)
        {
            if (nodePositions[i].NodeId != nodeId)
                continue;

            nodePositions.RemoveAt(i);
            return true;
        }

        return false;
    }

    public void Clear()
    {
        nodePositions.Clear();
    }
}