using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

using NovaVertexCode.DialogGraphEngine;
using Chaosbound.Systems.Dialogue.Integration.DGE.Nodes;
using Chaosbound.Systems.Dialogue.Runtime.Actions.Definitions;

namespace Chaosbound.Systems.Dialogue.Integration.DGE.Editor
{
    public sealed class CBCommandGraphNode : DGEGraphNode
    {
        private DropdownField definitionDropdown;
        private List<DialogueActionDefinition> availableDefinitions =
            new List<DialogueActionDefinition>();

        public CBCommandGraphNode(
            CBCommandNode commandNode,
            DGEGraphView graphView)
            : base(commandNode, graphView)
        {
            title = "Chaosbound Command";
            AddToClassList("cb-command-node");

            commandNode.EnsureOutputSlots();

            BuildDefinitionSelector(commandNode);
            BuildCommandOutputs(commandNode);

            RefreshExpandedState();
            RefreshPorts();
        }

        private void BuildDefinitionSelector(
            CBCommandNode commandNode)
        {
            availableDefinitions = LoadAvailableDefinitions();

            List<string> choices = availableDefinitions
                .Select(definition => definition.name)
                .ToList();

            choices.Insert(0, "Select command...");

            string selectedName =
                commandNode.ActionDefinition != null
                    ? commandNode.ActionDefinition.name
                    : "Select command...";

            definitionDropdown = new DropdownField(
                "Command",
                choices,
                selectedName);

            definitionDropdown.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == "Select command...")
                {
                    commandNode.SetActionDefinition(null);
                }
                else
                {
                    DialogueActionDefinition selected =
                        availableDefinitions.FirstOrDefault(
                            definition => definition.name == evt.newValue);

                    commandNode.SetActionDefinition(selected);
                }

                EditorUtility.SetDirty(commandNode);
            });

            mainContainer.Add(definitionDropdown);
        }

        private List<DialogueActionDefinition> LoadAvailableDefinitions()
        {
            List<DialogueActionDefinition> definitions =
                new List<DialogueActionDefinition>();

            string[] assetGuids =
                AssetDatabase.FindAssets("t:DialogueActionDefinition");

            foreach (string guid in assetGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                DialogueActionDefinition definition =
                    AssetDatabase.LoadAssetAtPath<DialogueActionDefinition>(
                        path);

                if (definition != null)
                {
                    definitions.Add(definition);
                }
            }

            return definitions;
        }

        private void BuildCommandOutputs(CBCommandNode commandNode)
        {
            outputContainer.Clear();

            AddCommandOutput(
                CBCommandNode.SuccessOutputIndex,
                "Success");

            AddCommandOutput(
                CBCommandNode.FailureOutputIndex,
                "Failure");

            AddCommandOutput(
                CBCommandNode.CancelledOutputIndex,
                "Cancelled");
        }

        private void AddCommandOutput(int index, string label)
        {
            Port port = InstantiatePort(
                Orientation.Horizontal,
                Direction.Output,
                Port.Capacity.Single,
                typeof(DGEBaseNode));

            port.portName = "output_" + index;
            port.name = label;
            port.AddToClassList("cb-command-output");

            outputContainer.Add(port);
        }
    }
}