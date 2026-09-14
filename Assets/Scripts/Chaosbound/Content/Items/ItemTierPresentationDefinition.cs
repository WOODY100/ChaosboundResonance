using UnityEngine;

namespace Chaosbound.Content.Items
{
    [System.Serializable]
    public sealed class ItemTierPresentationDefinition
    {
        [SerializeField]
        private ItemTier tier;

        [Header("Name")]
        [SerializeField]
        private Color nameColor = Color.white;

        [Header("Presentation")]
        [SerializeField]
        private GameObject presentationPrefab;

        public ItemTier Tier =>
            tier;

        public Color NameColor =>
            nameColor;

        public GameObject PresentationPrefab =>
            presentationPrefab;
    }
}