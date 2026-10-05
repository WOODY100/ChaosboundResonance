#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class SkillTreeEditorWindow : EditorWindow
{
    private SkillTreeDatabase database;
    private SkillTreeVisualLayout visualLayout;
    private SkillTreeDatabase lastDatabase;
    private SkillTreeVisualLayout lastVisualLayout;
    private SkillTreeEditorGraphView graphView;
    private SkillTreeEditorNode draggedNode;
    private SkillTreeEditorNode selectedNode;

    private readonly List<SkillTreeEditorNode> selectedNodes =
    new List<SkillTreeEditorNode>();

    private readonly Dictionary<SkillTreeEditorNode, Vector2>
        dragStartNodePositions =
            new Dictionary<SkillTreeEditorNode, Vector2>();
    private bool isPanning;
    private Vector2 panStartMousePosition;
    private Vector2 dragStartMousePosition;
    private Vector2 dragStartNodePosition;

    // GRID SETTINGS
    private const float DefaultGridSize = 100f;
    private float gridSize = DefaultGridSize;
    private readonly Color gridMinorColor =
        new Color(1f, 1f, 1f, 0.06f);
    private readonly Color gridMajorColor =
        new Color(1f, 1f, 1f, 0.12f);
    private readonly Color gridOriginColor =
        new Color(0.65f, 0.25f, 1f, 0.55f);

    private const int MajorGridInterval = 5;

    private bool showInfoPanel = true;

    private readonly List<SkillTreeEditorNode> editorNodes =
    new List<SkillTreeEditorNode>();

    private readonly Dictionary<string, SkillTreeEditorNode>
    editorNodeLookup =
        new Dictionary<string, SkillTreeEditorNode>();

    private readonly SkillTreeEditorGraphBuilder graphBuilder =
        new SkillTreeEditorGraphBuilder();

    private readonly SkillTreeEditorConnectionRenderer
        connectionRenderer =
            new SkillTreeEditorConnectionRenderer();

    private List<SkillTreeConnectionDescriptor>
        connections =
            new List<SkillTreeConnectionDescriptor>();

    [MenuItem("Chaosbound/Skill Tree/Visual Editor")]
    public static void Open()
    {
        SkillTreeEditorWindow window =
            GetWindow<SkillTreeEditorWindow>();

        window.titleContent =
            new GUIContent("Chaosbound Skill Tree");

        window.minSize =
            new Vector2(800f, 600f);

        window.Show();
    }

    private void OnEnable()
    {
        graphView =
            new SkillTreeEditorGraphView();
    }

    private void OnGUI()
    {
        DrawToolbar();

        EditorGUILayout.Space();

        EnsureEditorNodesCurrent();

        DrawInfoPanel();

        EditorGUILayout.Space(2f);

        DrawCanvasArea();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(
            EditorStyles.toolbar);

        database =
            (SkillTreeDatabase)EditorGUILayout.ObjectField(
                database,
                typeof(SkillTreeDatabase),
                false,
                GUILayout.Width(280f));

        visualLayout =
            (SkillTreeVisualLayout)EditorGUILayout.ObjectField(
                visualLayout,
                typeof(SkillTreeVisualLayout),
                false,
                GUILayout.Width(280f));

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawCanvasArea()
    {
        Rect canvasRect =
            GUILayoutUtility.GetRect(
                GUIContent.none,
                GUIStyle.none,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));

        GUI.Box(
            canvasRect,
            GUIContent.none);

        if (database == null)
            return;

        if (visualLayout == null)
            return;

        Rect localCanvasRect =
            new Rect(
                0f,
                0f,
                canvasRect.width,
                canvasRect.height);

        GUI.BeginClip(canvasRect);

        ProcessZoomInput(
            localCanvasRect);

        ProcessPanInput(
            localCanvasRect);

        ProcessCanvasInput(
            localCanvasRect);

        DrawGrid(localCanvasRect);
        DrawGridOrigin(localCanvasRect);

        connectionRenderer.Draw(
            connections,
            editorNodeLookup,
            localCanvasRect,
            graphView,
            selectedNode != null
                ? selectedNode.NodeId
                : string.Empty);

        for (int i = 0; i < editorNodes.Count; i++)
        {
            SkillTreeEditorNode editorNode =
                editorNodes[i];

            Vector2 guiPosition =
                graphView.GraphToGUI(
                    editorNode.Position,
                    localCanvasRect);

            Rect nodeRect =
                editorNode.GetRect(
                    guiPosition);

            editorNode.Draw(
                nodeRect);
        }

        GUI.EndClip();
    }

    private void DrawInfoPanel()
    {
        EditorGUILayout.BeginVertical(
            EditorStyles.helpBox);

        showInfoPanel =
            EditorGUILayout.Foldout(
                showInfoPanel,
                "Skill Tree Information",
                true);

        if (showInfoPanel)
        {
            DrawCompactStatus();

            EditorGUILayout.Space(3f);

            DrawCompactSelectedNodeInspector();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawCompactStatus()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            "Database",
            database != null
                ? database.name
                : "None",
            GUILayout.Width(420f));

        EditorGUILayout.LabelField(
            "Nodes",
            database != null &&
            database.Nodes != null
                ? database.Nodes.Count.ToString()
                : "0",
            GUILayout.Width(100f));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            "Visual Layout",
            visualLayout != null
                ? visualLayout.name
                : "None",
            GUILayout.Width(420f));

        EditorGUILayout.LabelField(
            "Positions",
            visualLayout != null &&
            visualLayout.NodePositions != null
                ? visualLayout.NodePositions.Count.ToString()
                : "0",
            GUILayout.Width(100f));

        EditorGUILayout.LabelField(
            "Connections",
            connections.Count.ToString(),
            GUILayout.Width(120f));

        EditorGUILayout.EndHorizontal();
    }

    private void DrawCompactSelectedNodeInspector()
    {
        if (selectedNode == null ||
            selectedNode.Definition == null)
        {
            EditorGUILayout.LabelField(
                "Selected Node",
                "None");

            return;
        }

        SkillTreeNodeDefinition definition =
            selectedNode.Definition;

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            "Selected",
            definition.NodeId,
            GUILayout.Width(220f));

        EditorGUILayout.LabelField(
            definition.DisplayName,
            EditorStyles.boldLabel,
            GUILayout.Width(300f));

        EditorGUILayout.LabelField(
            definition.NodeType.ToString(),
            GUILayout.Width(180f));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            "Branch",
            definition.Branch.ToString(),
            GUILayout.Width(220f));

        EditorGUILayout.LabelField(
            "Cost",
            definition.Cost.ToString(),
            GUILayout.Width(300f));

        EditorGUILayout.LabelField(
            "Requirement",
            GetRequirementTypeName(
                definition.Requirement),
            GUILayout.Width(180f));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            "Description",
            string.IsNullOrEmpty(
                definition.Description)
                ? "No description."
                : definition.Description);

        if (definition.Icon != null)
        {
            Rect iconRect =
                GUILayoutUtility.GetRect(
                    40f,
                    40f,
                    GUILayout.Width(40f));

            EditorGUI.DrawPreviewTexture(
                iconRect,
                AssetPreview.GetAssetPreview(
                    definition.Icon));
        }

        EditorGUILayout.EndHorizontal();

        if (definition.Requirement != null)
        {
            EditorGUILayout.Space(2f);

            EditorGUILayout.LabelField(
                "Requirement Tree",
                EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            DrawRequirementTree(
                definition.Requirement,
                0);

            EditorGUI.indentLevel--;
        }
    }

    private void DrawStatus()
    {
        EditorGUILayout.BeginVertical(
            EditorStyles.helpBox);

        EditorGUILayout.LabelField(
            "Database",
            database != null
                ? database.name
                : "None");

        if (database != null)
        {
            EditorGUILayout.LabelField(
                "Nodes",
                database.Nodes != null
                    ? database.Nodes.Count.ToString()
                    : "0");
        }

        EditorGUILayout.Space(2f);

        EditorGUILayout.LabelField(
            "Visual Layout",
            visualLayout != null
                ? visualLayout.name
                : "None");

        if (visualLayout != null)
        {
            EditorGUILayout.LabelField(
                "Positions",
                visualLayout.NodePositions != null
                    ? visualLayout.NodePositions.Count.ToString()
                    : "0");

            EditorGUILayout.LabelField(
                "Connections",
                connections.Count.ToString());
        }

        EditorGUILayout.EndVertical();
    }

    private void EnsureEditorNodesCurrent()
    {
        if (database == lastDatabase &&
            visualLayout == lastVisualLayout)
        {
            return;
        }

        lastDatabase = database;
        lastVisualLayout = visualLayout;

        RebuildEditorNodes();
    }

    private void RebuildEditorNodes()
    {
        string selectedNodeId =
            selectedNode != null
                ? selectedNode.NodeId
                : string.Empty;

        editorNodes.Clear();
        editorNodeLookup.Clear();
        connections.Clear();

        if (database == null)
        {
            selectedNode = null;
            return;
        }

        IReadOnlyList<SkillTreeNodeDefinition> nodes =
            database.Nodes;

        if (nodes == null)
        {
            selectedNode = null;
            return;
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            SkillTreeNodeDefinition definition =
                nodes[i];

            if (definition == null)
                continue;

            if (string.IsNullOrWhiteSpace(
                definition.NodeId))
            {
                continue;
            }

            SkillTreeEditorNode editorNode =
                new SkillTreeEditorNode(
                    definition);

            if (visualLayout != null &&
                visualLayout.TryGetPosition(
                    definition.NodeId,
                    out Vector2 position))
            {
                editorNode.Position =
                    position;
            }

            if (definition.NodeId ==
                selectedNodeId)
            {
                editorNode.IsSelected = true;
                selectedNode = editorNode;
            }

            editorNodes.Add(
                editorNode);

            editorNodeLookup.Add(
                editorNode.NodeId,
                editorNode);
        }

        if (!string.IsNullOrEmpty(selectedNodeId) &&
            selectedNode == null)
        {
            selectedNode = null;
        }

        connections =
            graphBuilder.Build(
                database);
    }

    private void ProcessCanvasInput(
        Rect canvasRect)
    {
        Event currentEvent =
            Event.current;

        if (currentEvent == null)
            return;

        Vector2 mousePosition =
            currentEvent.mousePosition;

        if (currentEvent.type == EventType.MouseDown &&
                currentEvent.button == 0 &&
                canvasRect.Contains(mousePosition))
        {
            SkillTreeEditorNode clickedNode =
                FindNodeAtPosition(
                    mousePosition,
                    canvasRect);

            bool additiveSelection =
                currentEvent.shift;

            if (clickedNode == null)
            {
                ClearNodeSelection();

                draggedNode = null;
                dragStartNodePositions.Clear();

                currentEvent.Use();

                GUI.FocusControl(null);
                Repaint();

                return;
            }

            if (!additiveSelection)
            {
                ClearNodeSelection();
            }

            ToggleNodeSelection(clickedNode);

            draggedNode = clickedNode;

            dragStartMousePosition =
                mousePosition;

            dragStartNodePositions.Clear();

            for (int i = 0; i < selectedNodes.Count; i++)
            {
                SkillTreeEditorNode node =
                    selectedNodes[i];

                if (node == null)
                    continue;

                dragStartNodePositions[node] =
                    node.Position;
            }

            currentEvent.Use();

            GUI.FocusControl(null);

            Repaint();

            return;
        }

        if (currentEvent.type == EventType.MouseDrag &&
                currentEvent.button == 0 &&
                draggedNode != null &&
                selectedNodes.Count > 0)
        {
            Vector2 mouseDelta =
                mousePosition -
                dragStartMousePosition;

            Vector2 graphDelta =
                new Vector2(
                    mouseDelta.x,
                    -mouseDelta.y) /
                graphView.Zoom;

            bool freeMove =
                currentEvent.control;

            if (!freeMove)
            {
                Vector2 snappedDelta =
                    graphView.SnapToGrid(
                        graphDelta,
                        gridSize);

                graphDelta =
                    snappedDelta;
            }

            for (int i = 0; i < selectedNodes.Count; i++)
            {
                SkillTreeEditorNode node =
                    selectedNodes[i];

                if (node == null)
                    continue;

                if (!dragStartNodePositions.TryGetValue(
                        node,
                        out Vector2 startPosition))
                {
                    continue;
                }

                node.Position =
                    startPosition + graphDelta;
            }

            currentEvent.Use();

            Repaint();
        }

        if (currentEvent.type == EventType.MouseUp &&
            currentEvent.button == 0 &&
            draggedNode != null)
        {
            SaveDraggedNodePositions();

            draggedNode = null;
            dragStartNodePositions.Clear();

            currentEvent.Use();
        }
    }

    private void SaveDraggedNodePositions()
    {
        if (visualLayout == null)
            return;

        for (int i = 0; i < selectedNodes.Count; i++)
        {
            SkillTreeEditorNode node =
                selectedNodes[i];

            if (node == null)
                continue;

            SkillTreeVisualLayoutEditorUtility.SetNodePosition(
                visualLayout,
                node.NodeId,
                node.Position);
        }
    }

    private void ProcessZoomInput(
        Rect canvasRect)
    {
        Event currentEvent =
            Event.current;

        if (currentEvent == null)
            return;

        if (currentEvent.type != EventType.ScrollWheel)
            return;

        if (!canvasRect.Contains(
            currentEvent.mousePosition))
        {
            return;
        }

        float zoomFactor =
            currentEvent.delta.y > 0f
                ? 1f / 1.1f
                : 1.1f;

        graphView.ZoomAt(
            zoomFactor,
            currentEvent.mousePosition,
            canvasRect);

        currentEvent.Use();

        Repaint();
    }

    private void ProcessPanInput(
        Rect canvasRect)
    {
        Event currentEvent =
            Event.current;

        if (currentEvent == null)
            return;

        Vector2 mousePosition =
            currentEvent.mousePosition;

        if (currentEvent.type == EventType.MouseDown &&
            currentEvent.button == 2 &&
            canvasRect.Contains(mousePosition))
        {
            isPanning = true;

            panStartMousePosition =
                mousePosition;

            currentEvent.Use();

            return;
        }

        if (currentEvent.type == EventType.MouseDrag &&
            currentEvent.button == 2 &&
            isPanning)
        {
            Vector2 delta =
                mousePosition -
                panStartMousePosition;

            graphView.PanBy(delta);

            panStartMousePosition =
                mousePosition;

            currentEvent.Use();

            Repaint();

            return;
        }

        if (currentEvent.type == EventType.MouseUp &&
            currentEvent.button == 2 &&
            isPanning)
        {
            isPanning = false;

            currentEvent.Use();

            Repaint();
        }
    }

    private SkillTreeEditorNode FindNodeAtPosition(
        Vector2 mousePosition,
        Rect canvasRect)
    {
        for (int i = editorNodes.Count - 1;
             i >= 0;
             i--)
        {
            SkillTreeEditorNode node =
                editorNodes[i];

            Vector2 guiPosition =
                graphView.GraphToGUI(
                    node.Position,
                    canvasRect);

            if (node.ContainsPoint(
                guiPosition,
                mousePosition))
            {
                return node;
            }
        }

        return null;
    }

    private void DrawSelectedNodeInspector()
    {
        EditorGUILayout.BeginVertical(
            "box");

        EditorGUILayout.LabelField(
            "Selected Node",
            EditorStyles.boldLabel);

        if (selectedNode == null ||
            selectedNode.Definition == null)
        {
            EditorGUILayout.LabelField(
                "Node ID",
                "None");

            EditorGUILayout.EndVertical();
            return;
        }

        SkillTreeNodeDefinition definition =
            selectedNode.Definition;

        EditorGUILayout.LabelField(
            "Node ID",
            definition.NodeId);

        EditorGUILayout.LabelField(
            "Display Name",
            definition.DisplayName);

        EditorGUILayout.LabelField(
            "Branch",
            definition.Branch.ToString());

        EditorGUILayout.LabelField(
            "Node Type",
            definition.NodeType.ToString());

        EditorGUILayout.LabelField(
            "Cost",
            definition.Cost.ToString());

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Requirement",
            GetRequirementTypeName(
                definition.Requirement));
        
        if (definition.Requirement != null)
        {
            EditorGUILayout.Space();

            DrawRequirementTree(
                definition.Requirement,
                0);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Description",
            EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(
            string.IsNullOrEmpty(definition.Description)
                ? "No description."
                : definition.Description,
            MessageType.None);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Icon",
            EditorStyles.boldLabel);

        if (definition.Icon != null)
        {
            Rect iconRect =
                GUILayoutUtility.GetRect(
                    64f,
                    64f,
                    GUILayout.ExpandWidth(false));

            EditorGUI.DrawPreviewTexture(
                iconRect,
                AssetPreview.GetAssetPreview(
                    definition.Icon));
        }
        else
        {
            EditorGUILayout.LabelField(
                "None");
        }

        EditorGUILayout.EndVertical();
    }

    private string GetRequirementTypeName(
        NodeRequirement requirement)
    {
        if (requirement == null)
            return "None";

        if (requirement is NodePurchasedRequirement)
            return "Node Purchased";

        if (requirement is AllRequirement)
            return "All";

        if (requirement is AnyRequirement)
            return "Any";

        return requirement.GetType().Name;
    }

    private void DrawRequirementTree(
        NodeRequirement requirement,
        int indent)
    {
        if (requirement == null)
        {
            EditorGUILayout.LabelField(
                "None");
            return;
        }

        NodePurchasedRequirement purchased =
            requirement as NodePurchasedRequirement;

        if (purchased != null)
        {
            EditorGUI.indentLevel = indent;

            EditorGUILayout.LabelField(
                "Node Purchased",
                purchased.NodeId);

            EditorGUI.indentLevel = 0;
            return;
        }

        AllRequirement all =
            requirement as AllRequirement;

        if (all != null)
        {
            EditorGUI.indentLevel = indent;

            EditorGUILayout.LabelField(
                "ALL",
                EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            DrawRequirementChildren(
                all.Requirements,
                indent + 1);

            EditorGUI.indentLevel = 0;
            return;
        }

        AnyRequirement any =
            requirement as AnyRequirement;

        if (any != null)
        {
            EditorGUI.indentLevel = indent;

            EditorGUILayout.LabelField(
                "ANY",
                EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            DrawRequirementChildren(
                any.Requirements,
                indent + 1);

            EditorGUI.indentLevel = 0;
            return;
        }

        EditorGUILayout.LabelField(
            requirement.GetType().Name);
    }

    private void DrawRequirementChildren(
        NodeRequirement[] requirements,
        int indent)
    {
        if (requirements == null ||
            requirements.Length == 0)
        {
            EditorGUILayout.LabelField(
                "No requirements.");
            return;
        }

        for (int i = 0; i < requirements.Length; i++)
        {
            NodeRequirement child =
                requirements[i];

            DrawRequirementTree(
                child,
                indent);
        }
    }

    private bool IsConnectionRelatedToSelectedNode(
    SkillTreeConnectionDescriptor connection)
    {
        if (selectedNode == null)
            return false;

        return connection.TargetNodeId ==
               selectedNode.NodeId;
    }

    private void DrawGrid(Rect canvasRect)
    {
        if (gridSize <= 0f)
            return;

        Vector2 topLeft =
            graphView.GUIToGraph(
                new Vector2(canvasRect.xMin, canvasRect.yMin),
                canvasRect);

        Vector2 bottomRight =
            graphView.GUIToGraph(
                new Vector2(canvasRect.xMax, canvasRect.yMax),
                canvasRect);

        float minX = Mathf.Min(topLeft.x, bottomRight.x);
        float maxX = Mathf.Max(topLeft.x, bottomRight.x);
        float minY = Mathf.Min(topLeft.y, bottomRight.y);
        float maxY = Mathf.Max(topLeft.y, bottomRight.y);

        int startX =
            Mathf.FloorToInt(minX / gridSize);

        int endX =
            Mathf.CeilToInt(maxX / gridSize);

        int startY =
            Mathf.FloorToInt(minY / gridSize);

        int endY =
            Mathf.CeilToInt(maxY / gridSize);

        Handles.BeginGUI();

        for (int x = startX; x <= endX; x++)
        {
            float graphX = x * gridSize;

            Vector2 top =
                graphView.GraphToGUI(
                    new Vector2(graphX, minY),
                    canvasRect);

            Vector2 bottom =
                graphView.GraphToGUI(
                    new Vector2(graphX, maxY),
                    canvasRect);

            bool isMajor =
                x % MajorGridInterval == 0;

            Handles.color =
                isMajor
                    ? gridMajorColor
                    : gridMinorColor;

            Handles.DrawLine(top, bottom);
        }

        for (int y = startY; y <= endY; y++)
        {
            float graphY = y * gridSize;

            Vector2 left =
                graphView.GraphToGUI(
                    new Vector2(minX, graphY),
                    canvasRect);

            Vector2 right =
                graphView.GraphToGUI(
                    new Vector2(maxX, graphY),
                    canvasRect);

            bool isMajor =
                y % MajorGridInterval == 0;

            Handles.color =
                isMajor
                    ? gridMajorColor
                    : gridMinorColor;

            Handles.DrawLine(left, right);
        }

        Handles.EndGUI();
    }

    private void DrawGridOrigin(Rect canvasRect)
    {
        Vector2 origin =
            graphView.GraphToGUI(
                Vector2.zero,
                canvasRect);

        if (!canvasRect.Contains(origin))
            return;

        const float markerSize = 8f;
        const float crossSize = 14f;

        Handles.BeginGUI();

        Handles.color = gridOriginColor;

        Handles.DrawWireDisc(
            origin,
            Vector3.forward,
            markerSize);

        Handles.DrawLine(
            origin + Vector2.left * crossSize,
            origin + Vector2.right * crossSize);

        Handles.DrawLine(
            origin + Vector2.up * crossSize,
            origin + Vector2.down * crossSize);

        Handles.EndGUI();
    }

    private void ClearNodeSelection()
    {
        for (int i = 0; i < selectedNodes.Count; i++)
        {
            SkillTreeEditorNode node = selectedNodes[i];

            if (node != null)
                node.IsSelected = false;
        }

        selectedNodes.Clear();
        selectedNode = null;
    }

    private void ToggleNodeSelection(
    SkillTreeEditorNode node)
    {
        if (node == null)
            return;

        if (selectedNodes.Contains(node))
        {
            node.IsSelected = false;
            selectedNodes.Remove(node);

            if (selectedNode == node)
            {
                selectedNode =
                    selectedNodes.Count > 0
                        ? selectedNodes[selectedNodes.Count - 1]
                        : null;
            }

            return;
        }

        node.IsSelected = true;
        selectedNodes.Add(node);
        selectedNode = node;
    }
}

#endif