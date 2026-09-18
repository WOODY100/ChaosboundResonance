using UnityEngine;

namespace Chaosbound.Content.Materials
{
    [CreateAssetMenu(
        fileName = "MaterialDefinition",
        menuName = "Chaosbound/Materials/Material Definition")]
    public sealed class MaterialDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string contentId;

        [Header("Presentation")]
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [SerializeField] private string description;

        [Header("World")]
        [SerializeField] private GameObject worldPrefab;

        public string ContentId => contentId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public string Description => description;
        public GameObject WorldPrefab => worldPrefab;
    }
}