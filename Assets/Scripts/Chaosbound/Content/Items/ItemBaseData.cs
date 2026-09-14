using UnityEngine;

namespace Chaosbound.Content.Items
{
    [CreateAssetMenu(
        menuName = "Chaosbound/Items/Item Base Data",
        fileName = "ItemBaseData")]
    public sealed class ItemBaseData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField]
        private string contentId;

        [SerializeField]
        private string displayName;

        [TextArea]
        [SerializeField]
        private string description;

        [SerializeField]
        private Sprite icon;

        [Header("Classification")]
        [SerializeField]
        private ItemTier baseTier = ItemTier.Common;

        [Header("World Representation")]
        [SerializeField]
        private GameObject worldPrefab;

        public string ContentId =>
            contentId;

        public string DisplayName =>
            displayName;

        public string Description =>
            description;

        public Sprite Icon =>
            icon;

        public ItemTier BaseTier =>
            baseTier;

        public GameObject WorldPrefab =>
            worldPrefab;

        private void OnValidate()
        {
            contentId =
                contentId != null
                    ? contentId.Trim()
                    : string.Empty;

            displayName =
                displayName != null
                    ? displayName.Trim()
                    : string.Empty;
        }
    }
}