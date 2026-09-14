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

        [Header("World")]
        [SerializeField] private GameObject worldPrefab;

        public string ContentId => contentId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public GameObject WorldPrefab => worldPrefab;
    }
}