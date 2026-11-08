using System.Linq;
using UnityEngine;

namespace NovaVertexCode.DialogGraphEngine
{
    [CreateAssetMenu(fileName = "ActionNode", menuName = "Nodes/Action Node")]
    public class DGEActionNode : DGEBaseSelectableNode
    {
        public override DGEBaseNode NextNode()
        {
            return outputs != null && outputs.Count > 0 ? outputs.First() : null;
        }
    }
}
