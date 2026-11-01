using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Shared.Identifiers;
using UnityEngine;

namespace Chaosbound.Content.Enemy.Contracts
{
    /// <summary>
    /// Common authored content contract consumed by the shared Boss Runtime.
    /// Spawn identity and domain ownership remain with BossData/MiniBossData.
    /// </summary>
    public interface IBossCombatContent
    {
        ContentId Id { get; }
        string DisplayName { get; }
        GameObject SpawnPrefab { get; }
        BossCombatDefinition CombatDefinition { get; }
    }
}
