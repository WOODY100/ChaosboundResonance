using UnityEngine;

using Chaosbound.Content.Expeditions.Authoring;

namespace Chaosbound.Content.Expeditions.Assets
{
    [CreateAssetMenu(
        fileName = "New Expedition",
        menuName = "Chaosbound/Content/Expedition")]
    public sealed class ExpeditionAsset : ScriptableObject
    {
        [SerializeField]
        private ExpeditionAuthoring m_expedition = new();

        public ExpeditionAuthoring Expedition => m_expedition;
    }
}