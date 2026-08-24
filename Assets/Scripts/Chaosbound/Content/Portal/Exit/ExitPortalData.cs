using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Shared.Contracts;
using Chaosbound.Shared.Identifiers;
using UnityEngine;

namespace Chaosbound.Content.Portal.Exit
{
    [CreateAssetMenu(
        menuName = "Chaosbound/Portals/Exit Portal")]
    public sealed class ExitPortalData :
        ScriptableObject,
        IMaterializableReference,
        ISpawnPrefabReference
    {
        [Header("Identity")]

        [SerializeField]
        private string m_ContentId;

        [SerializeField]
        private string m_DisplayName;

        public ContentId Id =>
            new(m_ContentId);

        public string DisplayName =>
            m_DisplayName;

        [Header("Spawn")]

        [SerializeField]
        private GameObject m_SpawnPrefab;

        public GameObject SpawnPrefab =>
            m_SpawnPrefab;
    }
}