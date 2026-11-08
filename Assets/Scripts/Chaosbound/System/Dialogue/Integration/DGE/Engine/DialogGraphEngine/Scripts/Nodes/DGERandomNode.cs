using UnityEngine;

namespace NovaVertexCode.DialogGraphEngine
{
    [CreateAssetMenu(fileName = "RandomNode", menuName = "Nodes/Random Node")]
    public class DGERandomNode : DGEBaseSelectableNode
    {
        [HideInInspector]
        public float[] probabilities;

        public override DGEBaseNode NextNode()
        {
            if (outputs == null || outputs.Count == 0 || probabilities == null || probabilities.Length != outputs.Count)
            {
                return null;
            }

            float total = 0;
            foreach (float prob in probabilities)
            {
                total += prob;
            }

            if (total <= 0)
            {
                return null;
            }

            float randomValue = Random.Range(0f, total);
            float cumulative = 0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                cumulative += probabilities[i];
                if (randomValue <= cumulative)
                {
                    return outputs[i];
                }
            }

            return outputs[outputs.Count - 1];
        }
    }
}