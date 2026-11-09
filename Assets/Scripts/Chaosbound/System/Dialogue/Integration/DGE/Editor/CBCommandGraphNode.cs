using System.Collections.Generic;

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

        private readonly Dictionary<string, DialogueActionDefinition>
            availableDefinitions =
                new Dictionary<string, DialogueActionDefinition>();

        private CBCommandNode commandNode;

        public CBCommandGraphNode(
            CBCommandNode commandNode,
            DGEGraphView graphView)
            : base(commandNode, graphView)
        {
            title = "Chaosbound Command";
            AddToClassList("cb-command-node");

            commandNode.EnsureOutputSlots();

            this.commandNode = commandNode;

            BuildDefinitionSelector(commandNode);
            BuildCommandOutputs();

            RefreshExpandedState();
            RefreshPorts();
        }

        private void BuildDefinitionSelector(
            CBCommandNode commandNode)
        {
            List<string> choices =
                LoadAvailableDefinitions(commandNode);

            string selectedPath =
                GetDefinitionPath(commandNode.ActionDefinition);

            int selectedIndex = choices.IndexOf(selectedPath);

            if (selectedIndex < 0)
                selectedIndex = 0;

            definitionDropdown = new DropdownField(
                "Command",
                choices,
                selectedIndex);

            definitionDropdown.RegisterValueChangedCallback(evt =>
            {
                DialogueActionDefinition selectedDefinition = null;

                if (!string.IsNullOrEmpty(evt.newValue))
                {
                    if (!availableDefinitions.TryGetValue(
                        evt.newValue,
                        out selectedDefinition))
                    {
                        Debug.LogError(
                            "[CBCommandGraphNode] Could not resolve " +
                            "selected definition at path: " + evt.newValue,
                            commandNode);

                        return;
                    }
                }

                if (commandNode.ActionDefinition == selectedDefinition)
                    return;

                Undo.RecordObject(
                    commandNode,
                    "Change Dialogue Command");

                commandNode.SetActionDefinition(selectedDefinition);

                EditorUtility.SetDirty(commandNode);
            });

            definitionDropdown.RegisterCallback<AttachToPanelEvent>(
                OnDropdownAttached);

            definitionDropdown.RegisterCallback<DetachFromPanelEvent>(
                OnDropdownDetached);

            mainContainer.Add(definitionDropdown);
        }

        private List<string> LoadAvailableDefinitions(
            CBCommandNode commandNode)
        {
            availableDefinitions.Clear();

            List<string> choices = new List<string>
            {
                string.Empty
            };

            string[] assetGuids =
                AssetDatabase.FindAssets("t:ScriptableObject");

            foreach (string guid in assetGuids)
            {
                string path =
                    AssetDatabase.GUIDToAssetPath(guid);

                if (string.IsNullOrEmpty(path))
                    continue;

                DialogueActionDefinition definition =
                    AssetDatabase.LoadAssetAtPath<
                        DialogueActionDefinition>(path);

                if (definition == null)
                    continue;

                availableDefinitions[path] = definition;
                choices.Add(path);
            }

            // Include the current definition if it was not
            // found by the general asset search.
            if (commandNode.ActionDefinition != null)
            {
                string selectedPath =
                    GetDefinitionPath(commandNode.ActionDefinition);

                if (!string.IsNullOrEmpty(selectedPath) &&
                    !availableDefinitions.ContainsKey(selectedPath))
                {
                    availableDefinitions[selectedPath] =
                        commandNode.ActionDefinition;

                    choices.Add(selectedPath);
                }
            }

            return choices;
        }

        private string GetDefinitionPath(
            DialogueActionDefinition definition)
        {
            if (definition == null)
                return string.Empty;

            return AssetDatabase.GetAssetPath(definition);
        }

        private void BuildCommandOutputs()
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

        private void OnDropdownAttached(AttachToPanelEvent evt)
        {
            Undo.undoRedoPerformed -= RefreshDefinitionDropdown;
            Undo.undoRedoPerformed += RefreshDefinitionDropdown;
        }

        private void OnDropdownDetached(DetachFromPanelEvent evt)
        {
            Undo.undoRedoPerformed -= RefreshDefinitionDropdown;
        }

        private void RefreshDefinitionDropdown()
        {
            if (definitionDropdown == null || commandNode == null)
                return;

            string selectedPath =
                GetDefinitionPath(commandNode.ActionDefinition);

            if (!definitionDropdown.choices.Contains(selectedPath))
            {
                Debug.LogWarning(
                    "[CBCommandGraphNode] Definition path not found " +
                    "in dropdown choices: " + selectedPath,
                    commandNode);

                return;
            }

            definitionDropdown.SetValueWithoutNotify(selectedPath);
        }
    }
}