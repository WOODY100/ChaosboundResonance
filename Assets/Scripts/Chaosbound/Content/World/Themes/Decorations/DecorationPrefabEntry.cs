using System;
using UnityEngine;

namespace Chaosbound.Content.World.Themes.Decorations
{
    [Serializable]
    public class DecorationPrefabEntry
    {
        [Header("Decoration")]
        [SerializeField]
        private GameObject prefab;

        [Header("Selection")]
        [Min(1)]
        [SerializeField]
        private int weight = 1;

        [Header("Placement")]
        [SerializeField]
        private bool randomYRotation = true;

        [Header("Scale")]
        [SerializeField]
        private Vector2 scaleRange = Vector2.one;

        public GameObject Prefab => prefab;

        public int Weight => weight;

        public bool RandomYRotation => randomYRotation;

        public Vector2 ScaleRange => scaleRange;
    }
}