using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NovaVertexCode.DialogGraphEngine
{
    public enum DialogType
    {
        Start,
        Text,
        Action,        
        Random
    }

    public enum PlaceholderType
    { 
        ActorName,
        Text
    }

    public class DGEGraphNode : Node
    {
        public DGEBaseNode node;
        private int outputPortCounter = 0;
        private DGEGraphView graphView;

        public DGEGraphNode() { }

        public DGEGraphNode(DGEBaseNode node, DGEGraphView graphView = null)
        {
            this.node = node;
            this.graphView = graphView;
            Initialize(node?.text ?? "Node", node?.position ?? Vector2.zero);
        }

        public virtual void Initialize(string nodeName, Vector2 position)
        {
            if (node == null)
            {
                node = ScriptableObject.CreateInstance<DGEBaseNode>();
                node.text = nodeName;
                node.position = position;
                node.id = System.Guid.NewGuid().ToString();
            }

            if (node is DGEStartNode)
            {
                title = "Start";
                AddToClassList("start-node");
            }
            else if (node is DGETextNode)
            {                
                title = "Text";
                AddToClassList("text-node");
            }
            else if (node is DGEActionNode)
            {
                title = "Action";
                AddToClassList("action-node");
            }
            else if (node is DGERandomNode)
            {
                title = "Random";
                AddToClassList("random-node");
            }
            else
            {
                title = "Node";
            }

            SetPosition(new Rect(position, new Vector2(200, 150)));

            if (node is DGETextNode textNode)
            {
                var actorNameTextField = CreateTextField(textNode.actorName, "Actor name...");
                actorNameTextField.textField.RegisterValueChangedCallback(evt =>
                {
                    actorNameTextField.placeholder.style.display = string.IsNullOrEmpty(evt.newValue) ? DisplayStyle.Flex : DisplayStyle.None;
                    if (node != null)
                    {
                        textNode.actorName = evt.newValue;
                        EditorUtility.SetDirty(node);
                    }
                });
                actorNameTextField.placeholder.style.display = string.IsNullOrEmpty(textNode.actorName) ? DisplayStyle.Flex : DisplayStyle.None;
                mainContainer.Add(actorNameTextField.textField);

                var dialogTextField = CreateTextField(textNode.text, "Text...");
                dialogTextField.textField.RegisterValueChangedCallback(evt =>
                {
                    dialogTextField.placeholder.style.display = string.IsNullOrEmpty(evt.newValue) ? DisplayStyle.Flex : DisplayStyle.None;
                    if (node != null)
                    {
                        textNode.text = evt.newValue;
                        EditorUtility.SetDirty(node);
                    }
                });
                dialogTextField.placeholder.style.display = string.IsNullOrEmpty(textNode.text) ? DisplayStyle.Flex : DisplayStyle.None;
                mainContainer.Add(dialogTextField.textField);
            }
            if (node is DGEActionNode actionNode)
            {
                var actorNameTextField = CreateTextField(actionNode.text, "Action...");
                actorNameTextField.textField.RegisterValueChangedCallback(evt =>
                {
                    actorNameTextField.placeholder.style.display = string.IsNullOrEmpty(evt.newValue) ? DisplayStyle.Flex : DisplayStyle.None;
                    if (node != null)
                    {
                        node.text = evt.newValue;
                        EditorUtility.SetDirty(node);
                    }
                });
                actorNameTextField.placeholder.style.display = string.IsNullOrEmpty(actionNode.text) ? DisplayStyle.Flex : DisplayStyle.None;
                mainContainer.Add(actorNameTextField.textField);
            }

            if (node is DGEActionNode || node is DGETextNode || node is DGERandomNode)
            {
                var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(DGEBaseNode));
                inputPort.portName = "input";
                inputContainer.Add(inputPort);
            }
            else if (!(node is DGEStartNode))
            {
                var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(DGEBaseNode));
                inputPort.portName = "input";
                inputContainer.Add(inputPort);
            }

            var selectAssetButton = new Button(() =>
            {
                string assetPath = AssetDatabase.GetAssetPath(node);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                    EditorGUIUtility.PingObject(asset);
                    Selection.activeObject = asset;
                }
            })
            {
                text = "📌"
            };
            selectAssetButton.style.marginLeft = 5;
            titleContainer.Add(selectAssetButton);

            if (node is DGETextNode)
            {
                var addOutputButton = new Button(() => AddOutputPort())
                {
                    text = "Add Output"
                };
                addOutputButton.style.marginTop = 5;
                extensionContainer.Add(addOutputButton);

                outputPortCounter = 0;
                if (node.outputs != null && node.outputs.Count > 0)
                {
                    foreach (var _ in node.outputs)
                    {
                        AddOutputPort();
                    }
                }
                else
                {
                    AddOutputPort();
                }
            }
            else if (node is DGERandomNode randomNode)
            {
                var addOutputButton = new Button(() => AddOutputPortWithProbability())
                {
                    text = "Add Output"
                };
                addOutputButton.style.marginTop = 5;
                extensionContainer.Add(addOutputButton);

                outputPortCounter = 0;
                if (node.outputs != null && node.outputs.Count > 0)
                {
                    for (int i = 0; i < node.outputs.Count; i++)
                    {
                        AddOutputPortWithProbability();
                        var portContainer = outputContainer.Children().Last() as VisualElement;
                        var probabilityField = portContainer.Q<IntegerField>();
                        if (probabilityField != null && i < randomNode.probabilities.Length)
                        {
                            probabilityField.value = (int)randomNode.probabilities[i];
                        }
                    }
                }
                else
                {
                    AddOutputPortWithProbability();
                }
            }
            else if (node is DGEActionNode)
            {
                var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(DGEBaseNode));
                outputPort.portName = "output_0";
                outputContainer.Add(outputPort);
            }
            else
            {
                var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(DGEBaseNode));
                outputPort.portName = "output_0";
                outputContainer.Add(outputPort);
            }

            RefreshExpandedState();
            RefreshPorts();
        }

        private (TextField textField, Label placeholder) CreateTextField(string initialText, string placeholderText)
        {
            var textField = new TextField
            {
                multiline = true,
                value = initialText
            };
            textField.style.minWidth = 200;
            textField.style.maxWidth = 200;
            textField.style.whiteSpace = WhiteSpace.Normal;
            textField.style.flexWrap = Wrap.Wrap;

            var placeholder = textField.Q<Label>("unity-placeholder");
            if (placeholder == null)
            {
                placeholder = new Label();
                placeholder.AddToClassList("unity-placeholder");
                placeholder.style.opacity = 0.5f;
                placeholder.style.position = Position.Absolute;
                placeholder.style.left = 5;
                placeholder.style.top = 2;
                textField.Add(placeholder);
            }
            placeholder.text = placeholderText;

            placeholder.style.display = string.IsNullOrEmpty(initialText) ? DisplayStyle.Flex : DisplayStyle.None;

            return (textField, placeholder);
        }

        private void AddOutputPortWithProbability()
        {
            var newPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(DGEBaseNode));
            int currentIndex = outputPortCounter;

            if (node is DGERandomNode randomNode)
            {
                var portContainer = new VisualElement();
                portContainer.style.flexDirection = FlexDirection.Row;
                portContainer.style.alignItems = Align.Center;
                portContainer.style.justifyContent = Justify.FlexStart;

                var removeButton = new Button(() =>
                {
                    string portName = newPort.portName;
                    if (portName.StartsWith("output_"))
                    {
                        string indexStr = portName.Replace("output_", "");
                        if (int.TryParse(indexStr, out int index))
                        {
                            RemoveOutputPort(index);
                        }
                    }
                })
                {
                    text = "X"
                };
                removeButton.style.marginLeft = 5;
                removeButton.style.marginRight = 5;

                var probabilityField = new IntegerField
                {
                    value = 0
                };
                probabilityField.style.marginRight = 10;
                probabilityField.style.height = 20;
                probabilityField.style.paddingLeft = 5;
                probabilityField.style.paddingRight = 5;
                probabilityField.style.width = 50;
                probabilityField.RegisterValueChangedCallback(evt =>
                {
                    int newValue = Mathf.Clamp(evt.newValue, 0, 100);
                    if (newValue != evt.newValue)
                    {
                        probabilityField.value = newValue;
                    }
                    if (currentIndex < randomNode.outputs.Count)
                    {
                        if (randomNode.probabilities == null || randomNode.probabilities.Length < randomNode.outputs.Count)
                        {
                            Array.Resize(ref randomNode.probabilities, randomNode.outputs.Count);
                        }
                        randomNode.probabilities[currentIndex] = newValue;
                        EditorUtility.SetDirty(randomNode);
                    }
                });

                portContainer.Add(removeButton);
                portContainer.Add(probabilityField);
                portContainer.Add(newPort);
                outputContainer.Add(portContainer);
            }

            newPort.portName = $"output_{currentIndex}";

            outputPortCounter++;
            RefreshPorts();
            RefreshExpandedState();
        }

        private void AddOutputPort()
        {
            var newPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(DGEBaseNode));
            int currentIndex = outputPortCounter;

            var portContainer = new VisualElement();
            portContainer.style.flexDirection = FlexDirection.Row;
            portContainer.style.alignItems = Align.Center;
            portContainer.style.justifyContent = Justify.FlexStart;

            var removeButton = new Button(() =>
            {
                string portName = newPort.portName;
                if (portName.StartsWith("output_"))
                {
                    string indexStr = portName.Replace("output_", "");
                    if (int.TryParse(indexStr, out int index))
                    {
                        RemoveOutputPort(index);
                    }
                }
            })
            {
                text = "X"
            };
            removeButton.style.marginLeft = 5;
            removeButton.style.marginRight = 5;

            var spacer = new VisualElement();
            spacer.style.flexGrow = 1.0f;
            portContainer.Add(removeButton);
            portContainer.Add(spacer);
            portContainer.Add(newPort);
            outputContainer.Add(portContainer);

            newPort.portName = $"output_{currentIndex}";

            outputPortCounter++;
            RefreshPorts();
            RefreshExpandedState();
        }

        private void RemoveOutputPort(int indexToRemove)
        {
            if (node == null || indexToRemove < 0 || indexToRemove >= outputPortCounter)
                return;

            if (outputPortCounter == 1)
            {
                Debug.LogWarning("Cannot remove the last output port. At least one output port is required.");
                return;
            }

            Port portToRemove = null;
            VisualElement portContainerToRemove = null;
            foreach (var child in outputContainer.Children().ToList())
            {
                var port = child.Q<Port>();
                if (port != null && port.portName == $"output_{indexToRemove}")
                {
                    portToRemove = port;
                    portContainerToRemove = child;
                    break;
                }
            }

            if (portToRemove != null && portContainerToRemove != null)
            {
                if (graphView != null)
                {
                    var edgesToRemove = graphView.edges.ToList().Where(e => e.output == portToRemove).ToList();
                    foreach (var edge in edgesToRemove)
                    {
                        if (edge.input != null && edge.input.node is DGEGraphNode inputNode)
                        {
                            foreach (var cachedNode in graphView._nodeCache)
                            {
                                var baseNode = cachedNode.Key;
                                if (baseNode == node) continue;
                                if (baseNode.outputs != null)
                                {
                                    List<DGEBaseNode> outputsList = baseNode.outputs.ToList();
                                    int index;
                                    while ((index = outputsList.IndexOf(inputNode.node)) != -1)
                                    {
                                        outputsList[index] = null;
                                        Debug.Log($"Cleared reference to {inputNode.node.text} from {baseNode.text} at index {index}");
                                    }
                                    while (outputsList.Count > 0 && outputsList.Last() == null)
                                    {
                                        outputsList.RemoveAt(outputsList.Count - 1);
                                    }
                                    baseNode.outputs = outputsList;
                                    EditorUtility.SetDirty(baseNode);
                                }
                            }

                            inputNode.inputContainer.Clear();
                            var newInputPort = inputNode.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(DGEBaseNode));
                            newInputPort.portName = "input";
                            inputNode.inputContainer.Add(newInputPort);
                            edge.input.Disconnect(edge);
                            inputNode.RefreshPorts();
                        }
                        graphView.RemoveElement(edge);
                    }
                }

                outputContainer.Remove(portContainerToRemove);
            }

            if (node is DGETextNode textNode)
            {
                var outputsList = textNode.outputs != null ? new List<DGEBaseNode>(textNode.outputs) : new List<DGEBaseNode>();
                if (indexToRemove < outputsList.Count)
                {
                    outputsList.RemoveAt(indexToRemove);
                    textNode.outputs = outputsList;
                    EditorUtility.SetDirty(textNode);
                }
            }
            else if (node is DGERandomNode randomNode)
            {
                var outputsList = randomNode.outputs != null ? new List<DGEBaseNode>(randomNode.outputs) : new List<DGEBaseNode>();
                var probabilitiesList = randomNode.probabilities != null ? new List<float>(randomNode.probabilities) : new List<float>();
                if (indexToRemove < outputsList.Count)
                {
                    outputsList.RemoveAt(indexToRemove);
                    probabilitiesList.RemoveAt(indexToRemove);
                    randomNode.outputs = outputsList;
                    randomNode.probabilities = probabilitiesList.ToArray();
                    EditorUtility.SetDirty(randomNode);
                }
            }

            outputPortCounter--;
            for (int i = indexToRemove; i < outputPortCounter; i++)
            {
                foreach (var child in outputContainer.Children())
                {
                    var port = child.Q<Port>();
                    if (port != null && port.portName == $"output_{i + 1}")
                    {
                        port.portName = $"output_{i}";
                        break;
                    }
                }
            }

            RefreshPorts();
            RefreshExpandedState();
        }

        public virtual void Draw()
        {
            RefreshExpandedState();
            RefreshPorts();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            if (node != null)
            {
                node.position = newPos.position;
                EditorUtility.SetDirty(node);
            }
        }
    }
}