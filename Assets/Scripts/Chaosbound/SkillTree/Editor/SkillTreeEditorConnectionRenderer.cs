#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class SkillTreeEditorConnectionRenderer
{
    private const float TangentDistance = 80f;

    public void Draw(
        IReadOnlyList<SkillTreeConnectionDescriptor> connections,
        IReadOnlyDictionary<string, SkillTreeEditorNode> nodes,
        Rect canvasRect,
        SkillTreeEditorGraphView graphView,
        string selectedNodeId)
    {
        if (connections == null)
            return;

        if (nodes == null)
            return;

        if (graphView == null)
            return;

        for (int i = 0; i < connections.Count; i++)
        {
            SkillTreeConnectionDescriptor connection =
                connections[i];

            if (!nodes.TryGetValue(
                connection.SourceNodeId,
                out SkillTreeEditorNode sourceNode))
            {
                continue;
            }

            if (!nodes.TryGetValue(
                connection.TargetNodeId,
                out SkillTreeEditorNode targetNode))
            {
                continue;
            }

            Vector2 sourceCenter =
                graphView.GraphToGUI(
                    sourceNode.Position,
                    canvasRect);

            Vector2 targetCenter =
                graphView.GraphToGUI(
                    targetNode.Position,
                    canvasRect);

            Vector2 direction =
                targetCenter - sourceCenter;

            if (direction.sqrMagnitude <= 0.000001f)
                continue;

            Vector2 sourcePosition =
                sourceNode.GetConnectionPoint(
                    sourceCenter,
                    direction);

            Vector2 targetPosition =
                targetNode.GetConnectionPoint(
                    targetCenter,
                    -direction);

            bool isRelatedToSelectedNode =
                !string.IsNullOrEmpty(selectedNodeId) &&
                connection.TargetNodeId == selectedNodeId;

            DrawConnection(
                sourcePosition,
                targetPosition,
                connection.Kind,
                isRelatedToSelectedNode);
        }
    }

    private void DrawConnection(
    Vector2 sourcePosition,
    Vector2 targetPosition,
    SkillTreeConnectionKind kind,
    bool isHighlighted)
    {
        Vector2 direction =
            targetPosition - sourcePosition;

        float distance =
            direction.magnitude;

        if (distance <= 0.000001f)
            return;

        Vector2 normalized =
            direction / distance;

        float tangentDistance =
            Mathf.Min(
                100f,
                distance * 0.4f);

        Vector2 sourceTangent;
        Vector2 targetTangent;

        if (Mathf.Abs(normalized.x) >=
            Mathf.Abs(normalized.y))
        {
            Vector2 horizontalDirection =
                new Vector2(
                    Mathf.Sign(normalized.x),
                    0f);

            sourceTangent =
                sourcePosition +
                horizontalDirection *
                tangentDistance;

            targetTangent =
                targetPosition -
                horizontalDirection *
                tangentDistance;
        }
        else
        {
            Vector2 verticalDirection =
                new Vector2(
                    0f,
                    Mathf.Sign(normalized.y));

            sourceTangent =
                sourcePosition +
                verticalDirection *
                tangentDistance;

            targetTangent =
                targetPosition -
                verticalDirection *
                tangentDistance;
        }

        Color previousColor =
            Handles.color;

        Color connectionColor;

        if (isHighlighted)
        {
            connectionColor =
                new Color(1f, 0.75f, 0.2f, 1f);
        }
        else if (kind ==
                 SkillTreeConnectionKind.StructuralBranchEntry)
        {
            connectionColor =
                new Color(0.8f, 0.8f, 0.8f, 1f);
        }
        else
        {
            connectionColor =
                new Color(0.65f, 0.65f, 0.65f, 1f);
        }

        Handles.color =
            connectionColor;

        Handles.DrawBezier(
            sourcePosition,
            targetPosition,
            sourceTangent,
            targetTangent,
            Handles.color,
            null,
            isHighlighted ? 4f : 3f);

        Handles.color =
            previousColor;
    }
}

#endif