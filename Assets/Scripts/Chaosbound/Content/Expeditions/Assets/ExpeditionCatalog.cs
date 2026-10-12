
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Assets
{
    [CreateAssetMenu(
        fileName = "ExpeditionCatalog",
        menuName = "Chaosbound/Content/Expedition Catalog")]
    public sealed class ExpeditionCatalog : ScriptableObject
    {
        [SerializeField]
        private List<ExpeditionAsset> m_expeditions = new();

        public IReadOnlyList<ExpeditionAsset> Expeditions => m_expeditions;

        public int Count => m_expeditions != null
            ? m_expeditions.Count
            : 0;

        public ExpeditionAsset GetExpedition(int index)
        {
            if (m_expeditions == null ||
                index < 0 ||
                index >= m_expeditions.Count)
            {
                return null;
            }

            return m_expeditions[index];
        }

        public bool Validate()
        {
            bool isValid = true;

            if (m_expeditions == null ||
                m_expeditions.Count == 0)
            {
                Debug.LogError(
                    $"[{nameof(ExpeditionCatalog)}] Catalog contains no expeditions.",
                    this);

                return false;
            }

            HashSet<string> expeditionIds =
                new HashSet<string>(System.StringComparer.Ordinal);

            for (int i = 0; i < m_expeditions.Count; i++)
            {
                ExpeditionAsset asset = m_expeditions[i];

                if (asset == null)
                {
                    Debug.LogError(
                        $"[{nameof(ExpeditionCatalog)}] Expedition at index {i} is null.",
                        this);

                    isValid = false;
                    continue;
                }

                if (asset.Expedition == null ||
                    asset.Expedition.Identity == null)
                {
                    Debug.LogError(
                        $"[{nameof(ExpeditionCatalog)}] Expedition at index {i} has no valid identity configuration.",
                        asset);

                    isValid = false;
                    continue;
                }

                string expeditionId = asset.Expedition.Identity.Id;

                if (string.IsNullOrWhiteSpace(expeditionId))
                {
                    Debug.LogError(
                        $"[{nameof(ExpeditionCatalog)}] Expedition at index {i} has an empty ID.",
                        asset);

                    isValid = false;
                    continue;
                }

                if (!expeditionIds.Add(expeditionId))
                {
                    Debug.LogError(
                        $"[{nameof(ExpeditionCatalog)}] Duplicate expedition ID detected: '{expeditionId}'.",
                        asset);

                    isValid = false;
                }
            }

            return isValid;
        }

        private void OnValidate()
        {
            if (m_expeditions == null)
                m_expeditions = new List<ExpeditionAsset>();

            m_expeditions.RemoveAll(expedition => expedition == null);
        }
    }
}
