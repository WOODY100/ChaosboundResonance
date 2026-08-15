using Chaosbound.Shared.Contracts;
using Chaosbound.Shared.Identifiers;
using UnityEngine;

namespace Chaosbound.Content.Enemy.MiniBosses
{
    [CreateAssetMenu(
        menuName = "Chaosbound/Enemies/Mini Boss")]
    public sealed class MiniBossData :
        ScriptableObject,
        IMaterializableReference
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