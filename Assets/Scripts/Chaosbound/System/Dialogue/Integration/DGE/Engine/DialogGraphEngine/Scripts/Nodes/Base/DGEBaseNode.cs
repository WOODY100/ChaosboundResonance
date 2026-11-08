using UnityEngine;
using System.Collections.Generic;

namespace NovaVertexCode.DialogGraphEngine
{
    public class DGEBaseNode : ScriptableObject
    {
        [HideInInspector]
        public string id;

        [HideInInspector]
        public string actorName;

        [HideInInspector]
        public string text;

        [HideInInspector]
        public Vector2 position;

        [HideInInspector]
        public List<DGEBaseNode> outputs;
    }
}