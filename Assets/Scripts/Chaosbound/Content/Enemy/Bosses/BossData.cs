using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Shared.Contracts;
using Chaosbound.Shared.Identifiers;
using UnityEngine;

namespace Chaosbound.Content.Enemy.Bosses
{
    [CreateAssetMenu(
        menuName = "Chaosbound/Enemies/Boss")]
    public sealed class BossData :
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