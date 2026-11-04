#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace Chaosbound.Development.Analytics
{
    [Serializable]
    public sealed class ExpeditionTelemetryBreakdownRecord
    {
        public string key;
        public string displayName;
        public double value;
        public int hitCount;
    }

    [Serializable]
    public sealed class ExpeditionTelemetrySkillTierRecord
    {
        public string skillDefinitionId;
        public string skillId;
        public string skillDisplayName;
        public string enemyTier;
        public double value;
        public int hitCount;
    }

    [Serializable]
    public sealed class ExpeditionTelemetryBucket
    {
        public string sessionId;
        public int bucketIndex;
        public double expeditionStartSeconds;
        public double expeditionEndSeconds;
        public double sessionStartSeconds;
        public double sessionEndSeconds;

        public double damageDealtTotal;
        public double damageReceivedTotal;
        public double healingReceivedTotal;

        [NonSerialized] private bool hasSessionTime;

        internal void RegisterSessionTime(double sessionTimeSeconds)
        {
            if (double.IsNaN(sessionTimeSeconds) || double.IsInfinity(sessionTimeSeconds))
                return;

            sessionTimeSeconds = Math.Max(0d, sessionTimeSeconds);
            if (!hasSessionTime)
            {
                sessionStartSeconds = sessionTimeSeconds;
                hasSessionTime = true;
            }

            sessionEndSeconds = Math.Max(sessionEndSeconds, sessionTimeSeconds);
        }

        public List<ExpeditionTelemetryBreakdownRecord> damageDealtBySkill =
            new List<ExpeditionTelemetryBreakdownRecord>();
        public List<ExpeditionTelemetryBreakdownRecord> damageDealtBySource =
            new List<ExpeditionTelemetryBreakdownRecord>();
        public List<ExpeditionTelemetryBreakdownRecord> damageDealtByEnemyTier =
            new List<ExpeditionTelemetryBreakdownRecord>();
        public List<ExpeditionTelemetrySkillTierRecord> damageDealtBySkillAndTier =
            new List<ExpeditionTelemetrySkillTierRecord>();
        public List<ExpeditionTelemetryBreakdownRecord> damageReceivedBySource =
            new List<ExpeditionTelemetryBreakdownRecord>();
        public List<ExpeditionTelemetryBreakdownRecord> healingBySource =
            new List<ExpeditionTelemetryBreakdownRecord>();
    }

    /// <summary>
    /// Accumulates continuous combat metrics in fixed Expedition Time windows.
    /// Discrete events remain the responsibility of ExpeditionTelemetryCollector.
    /// </summary>
    internal sealed class ExpeditionTelemetryBucketAccumulator
    {
        public const double BucketDurationSeconds = 5d;
        private const string Unknown = "Unknown";
        private const string Unattributed = "Unknown/Unattributed";

        private readonly string sessionId;
        private readonly Dictionary<int, ExpeditionTelemetryBucket> bucketsByIndex =
            new Dictionary<int, ExpeditionTelemetryBucket>();
        private readonly List<ExpeditionTelemetryBucket> buckets =
            new List<ExpeditionTelemetryBucket>();

        public List<ExpeditionTelemetryBucket> Buckets => buckets;

        public ExpeditionTelemetryBucketAccumulator(string sessionId)
        {
            this.sessionId = sessionId ?? string.Empty;
        }

        public void AddDamageDealt(
            double amount,
            double expeditionTimeSeconds,
            double sessionTimeSeconds,
            string skillDefinitionId,
            string enemyTier,
            string sourceCategory,
            string skillId,
            string skillDisplayName)
        {
            if (!CanAccumulate(amount, expeditionTimeSeconds))
                return;

            ExpeditionTelemetryBucket bucket = GetOrCreateBucket(expeditionTimeSeconds);
            bucket.RegisterSessionTime(sessionTimeSeconds);
            string skillKey = Normalize(
                !string.IsNullOrEmpty(skillId) ? skillId :
                (!string.IsNullOrEmpty(skillDefinitionId) ? skillDefinitionId : sourceCategory),
                Unattributed);
            string tierKey = Normalize(enemyTier, Unknown);

            bucket.damageDealtTotal += amount;
            AddBreakdown(bucket.damageDealtBySkill, skillKey, amount, true,
                skillDisplayName);
            string sourceKey = Normalize(
                sourceCategory,
                string.IsNullOrEmpty(skillDefinitionId) ? Unattributed : "Skill");
            AddBreakdown(bucket.damageDealtBySource, sourceKey, amount, true);
            AddBreakdown(bucket.damageDealtByEnemyTier, tierKey, amount, true);
            AddSkillTierBreakdown(bucket.damageDealtBySkillAndTier,
                skillDefinitionId, skillKey, skillDisplayName, tierKey, amount);
        }

        public void AddDamageReceived(
            double amount,
            double expeditionTimeSeconds,
            double sessionTimeSeconds,
            string sourceCategory)
        {
            if (!CanAccumulate(amount, expeditionTimeSeconds))
                return;

            ExpeditionTelemetryBucket bucket = GetOrCreateBucket(expeditionTimeSeconds);
            bucket.RegisterSessionTime(sessionTimeSeconds);
            bucket.damageReceivedTotal += amount;
            AddBreakdown(bucket.damageReceivedBySource,
                Normalize(sourceCategory, Unknown), amount);
        }

        public void AddHealing(
            double amount,
            double expeditionTimeSeconds,
            double sessionTimeSeconds,
            string sourceType)
        {
            if (!CanAccumulate(amount, expeditionTimeSeconds))
                return;

            ExpeditionTelemetryBucket bucket = GetOrCreateBucket(expeditionTimeSeconds);
            bucket.RegisterSessionTime(sessionTimeSeconds);
            bucket.healingReceivedTotal += amount;
            AddBreakdown(bucket.healingBySource,
                Normalize(sourceType, Unknown), amount);
        }

        public void CloseAt(double expeditionTimeSeconds, double sessionTimeSeconds)
        {
            if (double.IsNaN(expeditionTimeSeconds) ||
                double.IsInfinity(expeditionTimeSeconds) ||
                expeditionTimeSeconds < 0d)
                return;

            int bucketIndex = GetBucketIndex(expeditionTimeSeconds);
            ExpeditionTelemetryBucket bucket;
            if (!bucketsByIndex.TryGetValue(bucketIndex, out bucket))
                return;

            double nominalEnd = (bucketIndex + 1) * BucketDurationSeconds;
            bucket.expeditionEndSeconds = Math.Min(
                nominalEnd,
                Math.Max(bucket.expeditionStartSeconds, expeditionTimeSeconds));
            bucket.RegisterSessionTime(sessionTimeSeconds);
        }

        private ExpeditionTelemetryBucket GetOrCreateBucket(double expeditionTimeSeconds)
        {
            int index = GetBucketIndex(expeditionTimeSeconds);
            ExpeditionTelemetryBucket bucket;
            if (bucketsByIndex.TryGetValue(index, out bucket))
                return bucket;

            double start = index * BucketDurationSeconds;
            bucket = new ExpeditionTelemetryBucket
            {
                sessionId = sessionId,
                bucketIndex = index,
                expeditionStartSeconds = start,
                expeditionEndSeconds = start + BucketDurationSeconds
            };

            bucketsByIndex.Add(index, bucket);
            buckets.Add(bucket);
            return bucket;
        }

        private static int GetBucketIndex(double expeditionTimeSeconds)
        {
            double index = Math.Floor(expeditionTimeSeconds / BucketDurationSeconds);
            if (index > int.MaxValue)
                return int.MaxValue;
            return index < 0d ? 0 : (int)index;
        }

        private static bool CanAccumulate(double amount, double expeditionTimeSeconds)
        {
            return amount > 0d &&
                   !double.IsNaN(amount) && !double.IsInfinity(amount) &&
                   expeditionTimeSeconds >= 0d &&
                   !double.IsNaN(expeditionTimeSeconds) &&
                   !double.IsInfinity(expeditionTimeSeconds);
        }

        private static string Normalize(string value, string fallback)
        {
            return string.IsNullOrEmpty(value) ? fallback : value;
        }

        private static void AddBreakdown(
            List<ExpeditionTelemetryBreakdownRecord> records,
            string key,
            double amount,
            bool countHit = false,
            string displayName = null)
        {
            for (int i = 0; i < records.Count; i++)
            {
                if (records[i].key == key)
                {
                    records[i].value += amount;
                    if (countHit) records[i].hitCount++;
                    if (!string.IsNullOrEmpty(displayName))
                        records[i].displayName = displayName;
                    return;
                }
            }

            records.Add(new ExpeditionTelemetryBreakdownRecord
            {
                key = key,
                displayName = displayName ?? string.Empty,
                value = amount,
                hitCount = countHit ? 1 : 0
            });
        }

        private static void AddSkillTierBreakdown(
            List<ExpeditionTelemetrySkillTierRecord> records,
            string skillDefinitionId,
            string skillId,
            string skillDisplayName,
            string enemyTier,
            double amount)
        {
            for (int i = 0; i < records.Count; i++)
            {
                ExpeditionTelemetrySkillTierRecord record = records[i];
                if (record.skillDefinitionId == skillDefinitionId && record.enemyTier == enemyTier)
                {
                    record.value += amount;
                    record.hitCount++;
                    return;
                }
            }

            records.Add(new ExpeditionTelemetrySkillTierRecord
            {
                skillDefinitionId = skillDefinitionId,
                skillId = skillId,
                skillDisplayName = skillDisplayName,
                enemyTier = enemyTier,
                value = amount,
                hitCount = 1
            });
        }
    }
}
#endif
