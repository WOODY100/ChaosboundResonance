using System.Linq;
using UnityEngine;

namespace NovaVertexCode.DialogGraphEngine
{
    [CreateAssetMenu(fileName = "StartNode", menuName = "Nodes/Start Node")]
    public class DGEStartNode : DGEBaseSelectableNode
    {
        public override DGEBaseNode NextNode()
        {
            return outputs != null && outputs.Count > 0 ? outputs.First() : null;
        }
    }
}
