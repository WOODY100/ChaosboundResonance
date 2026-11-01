using Chaosbound.Content.Enemy.Contracts;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Shared.Contracts;
using Chaosbound.Shared.Identifiers;
using UnityEngine;

namespace Chaosbound.Content.Enemy.Bosses
{
    [CreateAssetMenu(menuName = "Chaosbound/Enemies/Boss")]
    public sealed class BossData :
        ScriptableObject,
        IMaterializableReference,
        ISpawnPrefabReference,
        IBossCombatContent
    {
        [Header("Identity")]
        [SerializeField] private string m_ContentId;
        [SerializeField] private string m_DisplayName;

        public ContentId Id => new ContentId(m_ContentId);
        public string DisplayName => m_DisplayName;

        [Header("Spawn")]
        [SerializeField] private GameObject m_SpawnPrefab;
        public GameObject SpawnPrefab => m_SpawnPrefab;

        [Header("Combat")]
        [SerializeField] private BossCombatDefinition m_CombatDefinition;
        public BossCombatDefinition CombatDefinition => m_CombatDefinition;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(m_ContentId))
                Debug.LogError($"BossData '{name}' requires a ContentId.", this);
            if (string.IsNullOrWhiteSpace(m_DisplayName))
                Debug.LogWarning($"BossData '{name}' has no DisplayName.", this);
            if (m_SpawnPrefab == null)
                Debug.LogError($"BossData '{name}' requires a SpawnPrefab.", this);
            if (m_CombatDefinition == null)
            {
                Debug.LogError(
                    $"BossData '{name}' requires a CombatDefinition.",
                    this);
            }
            else if (!m_CombatDefinition.Validate(out string error))
            {
                Debug.LogError(
                    $"BossData '{name}' has an invalid CombatDefinition: {error}",
                    this);
            }
        }
    }
}
