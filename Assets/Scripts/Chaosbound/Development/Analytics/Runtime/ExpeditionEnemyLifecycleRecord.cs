#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace Chaosbound.Development.Analytics
{
    /// <summary>
    /// One record per successfully materialized normal-enemy life.
    /// Timestamps use the official Expedition Runtime clock.
    /// </summary>
    [Serializable]
    public sealed class ExpeditionEnemyLifecycleRecord
    {
        public string enemyLifeId;
        public string enemyVariantId;
        public string enemyDisplayName;
        public string enemyTier;
        public List<string> enemyRoles = new List<string>();
        public float spawnedAtExpeditionSeconds;
        public float diedAtExpeditionSeconds;
        public float ttkSeconds;
        public float sessionEndExpeditionSeconds;
        public string outcome;
    }
}
#endif
