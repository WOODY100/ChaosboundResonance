using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Chaosbound.Systems.Dialogue.Integration.DGE.Nodes;
using Chaosbound.Systems.Dialogue.Integration.DGE.Editor;

namespace NovaVertexCode.DialogGraphEngine
{
    public class DGEGraphView : GraphView
    {
        public readonly Dictionary<DGEBaseNode, DGEGraphNode> _nodeCache = new Dictionary<DGEBaseNode, DGEGraphNode>();
        public DGEDialogManager dialogManager { get; private set; }

        public DGEGraphView()
        {
            dialogManager = new DGEDialogManager();

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            new DGEGraphToolbar(this, dialogManager);

            dialogManager.SwitchDialog(dialogManager.CurrentDialogFolder, this);

            graphViewChanged = changes =>
            {
                if (changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        if (edge.output?.node is DGEGraphNode outputNode && edge.input?.node is DGEGraphNode inputNode)
                        {
                            if (outputNode.node is DGEBaseNode baseNode && inputNode.node is DGEBaseNode targetNode)
                            {
                                string portName = edge.output.portName;
                                if (!portName.StartsWith("output_"))
                                {
                                    Debug.LogWarning($"Unexpected port name format: {portName} on node {outputNode.title}. Expected format: 'output_<number>'.");
                                    continue;
                                }

                                string indexStr = portName.Replace("output_", "");
                                if (!int.TryParse(indexStr, out int portIndex))
                                {
                                    Debug.LogWarning($"Failed to parse port index from port name: {portName} on node {outputNode.title}.");
                                    continue;
                                }

                                List<DGEBaseNode> outputsList = baseNode.outputs != null ? new List<DGEBaseNode>(baseNode.outputs) : new List<DGEBaseNode>();

                                while (outputsList.Count <= portIndex)
                                {
                                    outputsList.Add(null);
                                }

                                outputsList[portIndex] = targetNode;
                                baseNode.outputs = outputsList;

                                if (baseNode is DGERandomNode randomNode)
                                {
                                    List<float> probabilitiesList = randomNode.probabilities != null ? new List<float>(randomNode.probabilities) : new List<float>();
                                    while (probabilitiesList.Count <= portIndex)
                                    {
                                        probabilitiesList.Add(0);
                                    }
                                    randomNode.probabilities = probabilitiesList.ToArray();

                                    var portContainer = outputNode.outputContainer.Children()
                                        .FirstOrDefault(c => c.Q<Port>()?.portName == edge.output.portName);
                                    if (portContainer != null)
                                    {
                                        var probabilityField = portContainer.Q<IntegerField>();
                                        if (probabilityField != null)
                                        {
                                            randomNode.probabilities[portIndex] = probabilityField.value;
                                        }
                                    }
                                }

                                EditorUtility.SetDirty(baseNode);
                            }
                        }
                    }
                }

                if (changes.elementsToRemove != null && !dialogManager.IsClearingGraph)
                {
                    foreach (var element in changes.elementsToRemove)
                    {
                        if (element is Edge edge && edge.output?.node is DGEGraphNode outputNode && edge.input?.node is DGEGraphNode inputNode)
                        {
                            if (outputNode.node is DGEBaseNode baseNode && inputNode.node is DGEBaseNode targetNode)
                            {
                                string portName = edge.output.portName;
                                if (!portName.StartsWith("output_"))
                                {
                                    Debug.LogWarning($"Unexpected port name format: {portName} on node {outputNode.title}. Expected format: 'output_<number>'.");
                                    continue;
                                }

                                string indexStr = portName.Replace("output_", "");
                                if (!int.TryParse(indexStr, out int portIndex))
                                {
                                    Debug.LogWarning($"Failed to parse port index from port name: {portName} on node {outputNode.title}.");
                                    continue;
                                }

                                List<DGEBaseNode> outputsList = baseNode.outputs != null ? new List<DGEBaseNode>(baseNode.outputs) : new List<DGEBaseNode>();
                                if (portIndex < outputsList.Count)
                                {
                                    outputsList[portIndex] = null;
                                    if (baseNode is CBCommandNode)
                                    {
                                        while (outputsList.Count < CBCommandNode.OutputCount)
                                        {
                                            outputsList.Add(null);
                                        }
                                    }
                                    else
                                    {
                                        while (outputsList.Count > 0 && outputsList.Last() == null)
                                        {
                                            outputsList.RemoveAt(outputsList.Count - 1);
                                        }
                                    }

                                    baseNode.outputs = outputsList;

                                    if (baseNode is DGERandomNode randomNode)
                                    {
                                        List<float> probabilitiesList = randomNode.probabilities != null ? new List<float>(randomNode.probabilities) : new List<float>();
                                        if (portIndex < probabilitiesList.Count)
                                        {
                                            probabilitiesList[portIndex] = 0;
                                            while (probabilitiesList.Count > 0 && probabilitiesList.Last() == 0 && outputsList.Count <= probabilitiesList.Count - 1)
                                            {
                                                probabilitiesList.RemoveAt(probabilitiesList.Count - 1);
                                            }
                                            randomNode.probabilities = probabilitiesList.ToArray();
                                        }
                                    }

                                    EditorUtility.SetDirty(baseNode);

                                    inputNode.RefreshPorts();
                                    inputNode.RefreshExpandedState();
                                }
                            }
                        }
                    }
                }

                if (changes.elementsToRemove != null && !dialogManager.IsClearingGraph)
                {
                    var elementsToRemoveCopy = changes.elementsToRemove.ToList();
                    foreach (var element in elementsToRemoveCopy)
                    {
                        if (element is DGEGraphNode graphNode && graphNode.node is DGEBaseNode removedNode)
                        {
                            if (removedNode is DGEStartNode)
                            {
                                Debug.LogWarning("Cannot delete StartNode.");
                                changes.elementsToRemove.Remove(element);
                                continue;
                            }

                            foreach (var cachedNode in _nodeCache)
                            {
                                var baseNode = cachedNode.Key;
                                if (baseNode.outputs != null && baseNode.outputs.Contains(removedNode))
                                {
                                    List<DGEBaseNode> outputsList = baseNode.outputs.ToList();
                                    int index;
                                    while ((index = outputsList.IndexOf(removedNode)) != -1)
                                    {
                                        outputsList[index] = null;
                                    }

                                    if (baseNode is CBCommandNode)
                                    {
                                        while (outputsList.Count < CBCommandNode.OutputCount)
                                        {
                                            outputsList.Add(null);
                                        }
                                    }
                                    else
                                    {
                                        while (outputsList.Count > 0 && outputsList.Last() == null)
                                        {
                                            outputsList.RemoveAt(outputsList.Count - 1);
                                        }
                                    }

                                    baseNode.outputs = outputsList;

                                    if (baseNode is DGERandomNode randomNode)
                                    {
                                        List<float> probabilitiesList = randomNode.probabilities != null ? new List<float>(randomNode.probabilities) : new List<float>();
                                        while (probabilitiesList.Count > outputsList.Count)
                                        {
                                            probabilitiesList.RemoveAt(probabilitiesList.Count - 1);
                                        }
                                        randomNode.probabilities = probabilitiesList.ToArray();
                                    }

                                    EditorUtility.SetDirty(baseNode);
                                }
                            }
                            _nodeCache.Remove(removedNode);

                            string assetPath = AssetDatabase.GetAssetPath(removedNode);
                            if (!string.IsNullOrEmpty(assetPath))
                            {
                                AssetDatabase.DeleteAsset(assetPath);
                                AssetDatabase.SaveAssets();
                            }
                        }
                    }
                }
                return changes;
            };

            this.AddManipulator(CreateNodeContextualMenu("Add text node", "", "", DialogType.Text));
            this.AddManipulator(CreateNodeContextualMenu("Add action node", "", "", DialogType.Action));
            this.AddManipulator(CreateNodeContextualMenu("Add random node", "", "", DialogType.Random));
            this.AddManipulator(new ContextualMenuManipulator(menuEvent =>
            {
                menuEvent.menu.AppendAction("Add Chaosbound command node", actionEvent =>
                {
                    var position = GetLocalMousePosition(
                        actionEvent.eventInfo.localMousePosition);

                    var newNode = CreateCommandNode(position);

                    if (newNode != null)
                    {
                        AddElement(newNode);
                    }
                });
            }));
        }

        private DGEGraphNode CreateCommandNode(Vector2 position)
        {
            if (string.IsNullOrEmpty(dialogManager.CurrentDialogFolder))
            {
                Debug.LogError(
                    "[DGEGraphView] No dialog selected. " +
                    "Cannot create CBCommandNode.");

                return null;
            }

            CBCommandNode commandNode =
                ScriptableObject.CreateInstance<CBCommandNode>();

            commandNode.name = "CBCommandNode";
            commandNode.id = System.Guid.NewGuid().ToString();
            commandNode.position = position;
            commandNode.outputs = new List<DGEBaseNode>();

            commandNode.EnsureOutputSlots();

            dialogManager.SaveNode(
                commandNode,
                dialogManager.CurrentDialogFolder);

            AssetDatabase.SaveAssets();

            return GenerateGraphNode(commandNode);
        }

        public override List<Port> GetCompatiblePorts(
            Port startPort,
            NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port ||
                    startPort.node == port.node ||
                    startPort.direction == port.direction)
                {
                    return;
                }

                var startNode =
                    (startPort.node as DGEGraphNode)?.node;

                var targetNode =
                    (port.node as DGEGraphNode)?.node;

                if (startNode == null || targetNode == null)
                    return;

                DGEBaseNode sourceNode;
                DGEBaseNode destinationNode;

                if (startPort.direction == Direction.Output)
                {
                    sourceNode = startNode;
                    destinationNode = targetNode;
                }
                else
                {
                    sourceNode = targetNode;
                    destinationNode = startNode;
                }

                bool isCompatible =
                    (sourceNode is DGEStartNode &&
                        destinationNode is DGETextNode) ||

                    (sourceNode is DGETextNode &&
                        (destinationNode is DGEActionNode ||
                         destinationNode is DGERandomNode)) ||

                    (sourceNode is DGEActionNode &&
                        (destinationNode is DGETextNode ||
                         destinationNode is DGERandomNode ||
                         destinationNode is CBCommandNode)) ||

                    (sourceNode is DGERandomNode &&
                        destinationNode is DGETextNode) ||

                    (sourceNode is CBCommandNode &&
                        (destinationNode is DGETextNode ||
                         destinationNode is CBCommandNode));

                if (isCompatible)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        public DGEGraphNode GenerateGraphNode(DGEBaseNode node)
        {
            if (node == null)
            {
                return null;
            }

            if (_nodeCache.TryGetValue(node, out var existingNode))
            {
                return existingNode;
            }

            DGEGraphNode graphNode;

            if (node is CBCommandNode commandNode)
            {
                commandNode.EnsureOutputSlots();

                graphNode = new CBCommandGraphNode(commandNode, this);
            }
            else
            {
                graphNode = new DGEGraphNode(node, this);
            }
            _nodeCache[node] = graphNode;

            graphNode.expanded = true;
            var collapseButton = graphNode.Q("collapse-button");
            if (collapseButton != null)
            {
                collapseButton.style.display = DisplayStyle.None;
            }

            if (node is DGETextNode)
            {
                graphNode.titleContainer.style.backgroundColor = new StyleColor(new Color(0.2f, 0.5f, 0.3f, 0.8f));
            }
            else if (node is DGEActionNode)
            {
                graphNode.titleContainer.style.backgroundColor = new StyleColor(new Color(0.3f, 0.4f, 0.6f, 0.8f));
            }
            else if (node is DGERandomNode)
            {
                graphNode.titleContainer.style.backgroundColor = new StyleColor(new Color(0.6f, 0.4f, 0.2f, 0.8f));
            }
            else if (node is DGEStartNode)
            {
                graphNode.titleContainer.style.backgroundColor = new StyleColor(new Color(0.5f, 0.2f, 0.2f, 0.8f));
            }
            else if (node is CBCommandNode)
            {
                graphNode.titleContainer.style.backgroundColor =
                    new StyleColor(new Color(0.45f, 0.25f, 0.55f, 0.9f));
            }
            else
            {
                graphNode.style.backgroundColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f, 0.8f));
            }            

            graphNode.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                if (evt.target == graphNode && evt.newValue == false)
                {
                    graphNode.expanded = true;
                }
            });

            if (node.outputs != null && node.outputs.Count > 0)
            {
                var outputPorts = graphNode.outputContainer.Query<Port>().ToList();
                int portIndex = 0;
                foreach (var output in node.outputs)
                {
                    if (output == null)
                    {
                        portIndex++;
                        continue;
                    }

                    bool isValidConnection =
                        (node is DGETextNode &&
                            (output is DGEActionNode || output is DGERandomNode)) ||

                        (node is DGEActionNode &&
                            (output is DGERandomNode ||
                             output is DGETextNode ||
                             output is CBCommandNode)) ||

                        (node is DGERandomNode &&
                            output is DGETextNode) ||

                        (node is DGEStartNode &&
                            output is DGETextNode) ||

                        (node is CBCommandNode &&
                            (output is DGETextNode || output is CBCommandNode));

                    if (!isValidConnection)
                    {
                        Debug.LogWarning($"Invalid connection from {node.text} ({node.GetType().Name}) to {output.text} ({output.GetType().Name}). Skipping edge creation.");
                        portIndex++;
                        continue;
                    }

                    var connectedNode = GenerateGraphNode(output);
                    if (connectedNode == null)
                    {
                        portIndex++;
                        continue;
                    }

                    var connectedInputPort = connectedNode.inputContainer.Query<Port>().ToList().FirstOrDefault(p => p.direction == Direction.Input);
                    if (connectedInputPort != null)
                    {
                        Port outputPort;
                        if (node is CBCommandNode)
                        {
                            outputPort = portIndex < outputPorts.Count
                                ? outputPorts[portIndex]
                                : null;

                            portIndex++;
                        }
                        else if (node is DGETextNode ||
                                 node is DGERandomNode ||
                                 node is DGEStartNode)
                        {
                            outputPort = portIndex < outputPorts.Count
                                ? outputPorts[portIndex]
                                : null;

                            portIndex++;
                        }
                        else
                        {
                            outputPort = graphNode.outputContainer
                                .Query<Port>()
                                .ToList()
                                .FirstOrDefault(p => p.direction == Direction.Output);
                        }

                        if (outputPort != null && !edges.Any(e => e.output == outputPort && e.input == connectedInputPort))
                        {
                            var edge = new Edge
                            {
                                output = outputPort,
                                input = connectedInputPort
                            };
                            edge.input.Connect(edge);
                            edge.output.Connect(edge);
                            AddElement(edge);
                        }
                        else
                        {
                            Debug.LogWarning($"No suitable output port found for node {graphNode.title} or edge already exists. Skipping edge creation.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"No input port found for node {connectedNode.title}. Skipping edge creation.");
                    }
                }
            }

            graphNode.RefreshExpandedState();
            graphNode.RefreshPorts();
            return graphNode;
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, string actorName, string text, DialogType dialogType)
        {
            return new ContextualMenuManipulator(menuEvent =>
            {
                menuEvent.menu.AppendAction(actionTitle, actionEvent =>
                {
                    var position = GetLocalMousePosition(actionEvent.eventInfo.localMousePosition);
                    var newNode = CreateNode(actorName, text, dialogType, position);
                    if (newNode != null)
                    {
                        AddElement(newNode);
                    }
                });
            });
        }

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            var worldMousePosition = mousePosition;
            var localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            return localMousePosition;
        }

        public DGEGraphNode CreateNode(string actorName, string text, DialogType dialogType, Vector2 position)
        {
            if (dialogType == DialogType.Start)
            {
                EditorUtility.DisplayDialog("Error", "Manual creation of StartNode is not allowed.", "OK");
                return null;
            }

            string assetPath = dialogManager.CreateNodeAsset(dialogType, actorName, text, position);
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            DGEBaseNode newNode = AssetDatabase.LoadAssetAtPath<DGEBaseNode>(assetPath);
            return GenerateGraphNode(newNode);
        }
    }
}