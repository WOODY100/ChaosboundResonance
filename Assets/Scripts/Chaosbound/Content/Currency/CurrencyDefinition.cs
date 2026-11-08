using UnityEngine;

namespace Chaosbound.Content.Currency
{
    [CreateAssetMenu(
        fileName = "CurrencyDefinition",
        menuName = "Chaosbound/Currency/Currency Definition")]
    public sealed class CurrencyDefinition : ScriptableObject
    {
        [SerializeField]
        private string contentId;

        [SerializeField]
        private string displayName;

        [SerializeField]
        private Sprite icon;

        public string ContentId =>
            contentId;

        public string DisplayName =>
            displayName;

        public Sprite Icon =>
            icon;

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (contentId != null)
                contentId = contentId.Trim();

            if (displayName != null)
                displayName = displayName.Trim();
        }

#endif
    }
}