using Chaosbound.Core.Composition;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public sealed class SkillTreeVisualController : MonoBehaviour
{
    [SerializeField]
    private GameObject skillTreeWindow;

    [SerializeField]
    private SkillTreeDatabase database;

    [SerializeField]
    private SkillTreeNodeView nodeViewPrefab;

    [SerializeField]
    private RectTransform nodeLayer;

    [SerializeField]
    private RectTransform connectionLayer;

    [SerializeField]
    private SkillTreeVisualLayout visualLayout;

    [SerializeField]
    private Material lockedConnectionMaterial;

    [SerializeField]
    private Material availableConnectionMaterial;

    [SerializeField]
    private Material purchasedConnectionMaterial;

    [SerializeField]
    private SkillTreeNodeDetailsView nodeDetailsView;

    [SerializeField]
    private TMP_Text skillPointsText;

    public event Action<SkillTreeNodeDefinition> NodeSelected;

    public SkillTreeVisualLayout VisualLayout
    {
        get { return visualLayout; }
    }

    private readonly Dictionary<string, SkillTreeNodeView> nodeViews =
        new Dictionary<string, SkillTreeNodeView>();

    private SkillTreeRuntime runtime;

    private SkillTreeNodeView selectedNodeView;

    public SkillTreeDatabase Database
    {
        get { return database; }
    }

    public SkillTreeRuntime Runtime
    {
        get { return runtime; }
    }

    public IReadOnlyDictionary<string, SkillTreeNodeView> NodeViews
    {
        get { return nodeViews; }
    }

    public SkillTreeNodeView SelectedNodeView
    {
        get { return selectedNodeView; }
    }

    public SkillTreeNodeDefinition SelectedNodeDefinition
    {
        get
        {
            if (selectedNodeView == null)
                return null;

            return selectedNodeView.Definition;
        }
    }

    private readonly Dictionary<string, SkillTreeConnectionView> connectionViews =
    new Dictionary<string, SkillTreeConnectionView>();

    public IReadOnlyDictionary<string, SkillTreeConnectionView> ConnectionViews
    {
        get { return connectionViews; }
    }

    private void Awake()
    {
        ClearSelection();

        if (nodeDetailsView != null)
        {
            nodeDetailsView.PurchaseRequested +=
                HandlePurchaseRequested;

            nodeDetailsView.ChoiceRequested +=
                HandleChoiceRequested;
        }
    }

    public void Build()
    {
        Clear();

        if (database == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeVisualController)}] " +
                "No SkillTreeDatabase assigned.",
                this);

            return;
        }

        if (nodeViewPrefab == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeVisualController)}] " +
                "No SkillTreeNodeView prefab assigned.",
                this);

            return;
        }

        if (nodeLayer == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeVisualController)}] " +
                "No node layer assigned.",
                this);

            return;
        }

        if (connectionLayer == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeVisualController)}] " +
                "No connection layer assigned.",
                this);

            return;
        }

        if (runtime == null ||
            runtime.Database != database)
        {
            BootstrapContext context =
                BootstrapContext.Current;

            if (context == null)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeVisualController)}] " +
                    "No BootstrapContext available.",
                    this);

                return;
            }

            if (context.PersistentMetaRuntime == null)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeVisualController)}] " +
                    "No PersistentMetaRuntime available.",
                    this);

                return;
            }

            runtime = new SkillTreeRuntime(
                database,
                context.PersistentMetaRuntime);
        }

        IReadOnlyList<SkillTreeNodeDefinition> nodes =
            database.Nodes;

        if (nodes == null)
            return;

        for (int i = 0; i < nodes.Count; i++)
        {
            SkillTreeNodeDefinition definition = nodes[i];

            if (definition == null)
                continue;

            if (string.IsNullOrWhiteSpace(definition.NodeId))
                continue;

            if (nodeViews.ContainsKey(definition.NodeId))
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeVisualController)}] " +
                    $"Duplicate NodeId '{definition.NodeId}'.",
                    this);

                continue;
            }

            SkillTreeNodeView view =
                Instantiate(nodeViewPrefab, nodeLayer);

            view.name =
                $"NodeView_{definition.NodeId}";

            view.Initialize(definition);
            view.Selected += HandleNodeSelected;

            if (visualLayout != null &&
                visualLayout.TryGetPosition(
                    definition.NodeId,
                    out Vector2 position))
            {
                view.RectTransform.anchoredPosition =
                    position;
            }

            nodeViews.Add(
                definition.NodeId,
                view);

        }

        RefreshVisualStatesFromRuntime();
        BuildConnections();
        RefreshSkillPoints();
    }

    public void ApplyLayout()
    {
        if (visualLayout == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeVisualController)}] " +
                "No SkillTreeVisualLayout assigned.",
                this);

            return;
        }

        foreach (KeyValuePair<string, SkillTreeNodeView> pair
            in nodeViews)
        {
            string nodeId = pair.Key;
            SkillTreeNodeView view = pair.Value;

            if (view == null || view.RectTransform == null)
                continue;

            if (!visualLayout.TryGetPosition(
                nodeId,
                out Vector2 position))
            {
                continue;
            }

            view.RectTransform.anchoredPosition =
                position;
        }
    }

    public bool TrySaveNodePosition(
        SkillTreeNodeView nodeView)
    {
        if (nodeView == null)
            return false;

        if (visualLayout == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeVisualController)}] " +
                "No SkillTreeVisualLayout assigned.",
                this);

            return false;
        }

        if (nodeView.RectTransform == null)
            return false;

        string nodeId = nodeView.NodeId;

        if (string.IsNullOrWhiteSpace(nodeId))
            return false;

        visualLayout.SetPosition(
            nodeId,
            nodeView.RectTransform.anchoredPosition);

        return true;
    }

    public bool TryGetNodeView(
        string nodeId,
        out SkillTreeNodeView nodeView)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
        {
            nodeView = null;
            return false;
        }

        return nodeViews.TryGetValue(
            nodeId,
            out nodeView);
    }

    public bool SetNodeVisualState(
        string nodeId,
        SkillTreeVisualState state)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            return false;

        if (!nodeViews.TryGetValue(
            nodeId,
            out SkillTreeNodeView nodeView))
        {
            return false;
        }

        if (nodeView == null)
            return false;

        nodeView.SetVisualState(state);

        return true;
    }

    public void SetNodeVisualStates(
        IReadOnlyDictionary<string, SkillTreeVisualState> states)
    {
        if (states == null)
            return;

        foreach (
            KeyValuePair<string, SkillTreeVisualState> pair
            in states)
        {
            SetNodeVisualState(
                pair.Key,
                pair.Value);
        }
    }

    public void Clear()
    {
        selectedNodeView = null;

        if (nodeDetailsView != null)
            nodeDetailsView.Hide();

        connectionViews.Clear();

        nodeViews.Clear();

        if (nodeLayer == null)
            return;

        if (connectionLayer != null)
        {
            for (int i = connectionLayer.childCount - 1;
                 i >= 0;
                 i--)
            {
                Transform child =
                    connectionLayer.GetChild(i);

                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }

        for (int i = nodeLayer.childCount - 1; i >= 0; i--)
        {
            Transform child = nodeLayer.GetChild(i);

            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
    }

    public void RefreshVisualStatesFromRuntime()
    {
        if (runtime == null)
            return;

        foreach (KeyValuePair<string, SkillTreeNodeView> pair
            in nodeViews)
        {
            string nodeId = pair.Key;
            SkillTreeNodeView view = pair.Value;

            if (view == null)
                continue;

            SkillTreeVisualState state =
                runtime.GetNodeState(nodeId);

            view.SetVisualState(state);
        }
    }

    private void RefreshSkillPoints()
    {
        if (skillPointsText == null)
            return;

        BootstrapContext context =
            BootstrapContext.Current;

        if (context == null)
            return;

        if (context.PersistentMetaRuntime == null)
            return;

        if (context.PersistentMetaRuntime.State == null)
            return;

        skillPointsText.text =
            context.PersistentMetaRuntime.State.SkillPoints.ToString();
    }

    private void BuildConnections()
    {
        ClearConnectionViews();

        if (connectionLayer == null)
            return;

        List<SkillTreeConnectionDescriptor> connections =
            SkillTreeRequirementGraphBuilder.Build(database);

        IReadOnlyList<SkillTreeConnectionDescriptor> structuralConnections =
            SkillTreeStructuralGraphBuilder.Build(database);

        if (structuralConnections != null)
        {
            for (int i = 0;
                 i < structuralConnections.Count;
                 i++)
            {
                connections.Add(
                    structuralConnections[i]);
            }
        }

        for (int i = 0;
             i < connections.Count;
             i++)
        {
            SkillTreeConnectionDescriptor connection =
                connections[i];

            if (!nodeViews.TryGetValue(
                connection.SourceNodeId,
                out SkillTreeNodeView sourceNode))
            {
                continue;
            }

            if (!nodeViews.TryGetValue(
                connection.TargetNodeId,
                out SkillTreeNodeView targetNode))
            {
                continue;
            }

            GameObject connectionObject =
                new GameObject(
                    $"ConnectionView_" +
                    connection.SourceNodeId +
                    "_" +
                    connection.TargetNodeId,
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(SimpleTalentTreeUi.LineRendererUI),
                    typeof(SkillTreeConnectionView));

            RectTransform connectionTransform =
                connectionObject.GetComponent<RectTransform>();

            connectionTransform.SetParent(
                connectionLayer,
                false);

            connectionTransform.anchorMin =
                Vector2.zero;

            connectionTransform.anchorMax =
                Vector2.one;

            connectionTransform.offsetMin =
                Vector2.zero;

            connectionTransform.offsetMax =
                Vector2.zero;

            SkillTreeConnectionView connectionView =
                connectionObject.GetComponent<SkillTreeConnectionView>();

            connectionView.Initialize(
                connection,
                sourceNode,
                targetNode);

            connectionView.ConfigureMaterials(
                lockedConnectionMaterial,
                availableConnectionMaterial,
                purchasedConnectionMaterial);

            connectionView.RefreshVisualStateFromTarget();

            string connectionId =
                connection.SourceNodeId +
                "->" +
                connection.TargetNodeId;

            connectionViews.Add(
                connectionId,
                connectionView);
        }
    }

    private void ClearConnectionViews()
    {
        connectionViews.Clear();

        if (connectionLayer == null)
            return;

        for (int i = connectionLayer.childCount - 1; i >= 0; i--)
        {
            Transform child =
                connectionLayer.GetChild(i);

            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
    }

    private void HandleNodeSelected(
    SkillTreeNodeView nodeView)
    {
        if (nodeView == null)
            return;

        if (nodeView.Definition == null)
            return;

        selectedNodeView = nodeView;

        if (nodeDetailsView != null)
        {
            nodeDetailsView.Show(
                nodeView.Definition);

            RefreshSelectedNodeDetails();
        }

        NodeSelected?.Invoke(
            nodeView.Definition);
    }

    public void ClearSelection()
    {
        selectedNodeView = null;

        if (nodeDetailsView != null)
            nodeDetailsView.Hide();
    }

    public void CloseSkillTree()
    {
        ClearSelection();

        if (skillTreeWindow != null)
            skillTreeWindow.SetActive(false);
    }

    private void RefreshSelectedNodeDetails()
    {
        if (runtime == null)
            return;

        if (selectedNodeView == null)
            return;

        if (selectedNodeView.Definition == null)
            return;

        if (nodeDetailsView == null)
            return;

        string nodeId =
            selectedNodeView.Definition.NodeId;

        SkillTreeVisualState state =
            runtime.GetNodeState(nodeId);

        string selectedChoiceOptionId = null;

        if (state ==
            SkillTreeVisualState.Purchased)
        {
            runtime.State.TryGetSelectedChoice(
                nodeId,
                out selectedChoiceOptionId);
        }

        nodeDetailsView.SetRuntimeState(
            state,
            selectedChoiceOptionId);
    }

    private void HandlePurchaseRequested()
    {
        if (runtime == null)
            return;

        if (selectedNodeView == null)
            return;

        if (selectedNodeView.Definition == null)
            return;

        string nodeId =
            selectedNodeView.Definition.NodeId;

        bool purchased =
            runtime.PurchaseNode(nodeId);

        if (!purchased)
            return;

        RefreshVisualStatesFromRuntime();

        RefreshConnectionVisualStates();

        RefreshSelectedNodeDetails();

        RefreshSkillPoints();
    }

    private void RefreshConnectionVisualStates()
    {
        foreach (
            KeyValuePair<string, SkillTreeConnectionView> pair
            in connectionViews)
        {
            SkillTreeConnectionView connectionView =
                pair.Value;

            if (connectionView == null)
                continue;

            connectionView.RefreshVisualStateFromTarget();
        }
    }

    private void HandleChoiceRequested(
    string optionId)
    {
        if (runtime == null)
            return;

        if (selectedNodeView == null)
            return;

        if (selectedNodeView.Definition == null)
            return;

        string nodeId =
            selectedNodeView.Definition.NodeId;

        bool purchased =
            runtime.PurchaseChoice(
                nodeId,
                optionId);

        if (!purchased)
            return;

        RefreshVisualStatesFromRuntime();

        RefreshConnectionVisualStates();

        RefreshSelectedNodeDetails();

        RefreshSkillPoints();
    }

    public void OpenSkillTree()
    {
        if (skillTreeWindow == null)
        {
            Debug.LogError(
                "[SkillTreeVisualController] " +
                "SkillTreeWindow is not assigned.",
                this);

            return;
        }

        skillTreeWindow.SetActive(true);
        Build();
    }

    private void OnDestroy()
    {
        if (nodeDetailsView != null)
        {
            nodeDetailsView.PurchaseRequested -=
                HandlePurchaseRequested;

            nodeDetailsView.ChoiceRequested -=
                HandleChoiceRequested;
        }
    }
}