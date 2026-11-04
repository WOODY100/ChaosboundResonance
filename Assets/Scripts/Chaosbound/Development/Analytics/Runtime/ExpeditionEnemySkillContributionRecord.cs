#if UNITY_EDITOR
using System;

namespace Chaosbound.Development.Analytics
{
    /// <summary>
    /// Damage contribution made by one source to one materialized enemy life.
    /// This is telemetry-only data and does not participate in gameplay.
    /// </summary>
    [Serializable]
    public sealed class ExpeditionEnemySkillContributionRecord
    {
        public string enemyLifeId;
        public string enemyTier;
        public string skillDefinitionId;
        public string skillId;
        public string skillDisplayName;
        public string sourceCategory;
        public string lastExecutionId;
        public float damage;
        public int hitCount;
        public bool enemyDefeated;
    }
}
#endif
