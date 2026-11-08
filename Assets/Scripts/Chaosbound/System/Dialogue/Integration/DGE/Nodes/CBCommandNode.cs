using System.Collections.Generic;
using UnityEngine;

using NovaVertexCode.DialogGraphEngine;
using Chaosbound.Systems.Dialogue.Runtime.Actions.Definitions;

namespace Chaosbound.Systems.Dialogue.Integration.DGE.Nodes
{
    [CreateAssetMenu(
        fileName = "CBCommandNode",
        menuName = "Chaosbound/Dialogue/Command Node")]
    public sealed class CBCommandNode : DGEBaseNode
    {
        public const int SuccessOutputIndex = 0;
        public const int FailureOutputIndex = 1;
        public const int CancelledOutputIndex = 2;

        public const int OutputCount = 3;

        [SerializeField]
        private DialogueActionDefinition actionDefinition;

        public DialogueActionDefinition ActionDefinition =>
            actionDefinition;

        public void SetActionDefinition(
            DialogueActionDefinition definition)
        {
            actionDefinition = definition;
        }

        public void EnsureOutputSlots()
        {
            if (outputs == null)
            {
                outputs = new List<DGEBaseNode>();
            }

            while (outputs.Count < OutputCount)
            {
                outputs.Add(null);
            }

            if (outputs.Count > OutputCount)
            {
                Debug.LogError(
                    "[CBCommandNode] Invalid output count. " +
                    "Command nodes must have exactly three output slots.",
                    this);
            }
        }

        public DGEBaseNode GetOutput(int index)
        {
            if (index < 0 || index >= OutputCount)
                return null;

            if (outputs == null || outputs.Count <= index)
                return null;

            return outputs[index];
        }
    }
}