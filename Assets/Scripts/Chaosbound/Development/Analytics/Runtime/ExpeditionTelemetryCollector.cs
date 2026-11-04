#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Development.Analytics
{
    [Serializable]
    public sealed class ExpeditionTelemetryEvent
    {
        public float elapsedSeconds;
        public float expeditionElapsedSeconds;
        public string eventType;
        public float value;
        public int integerValue;
        public string detail;
    }

    [Serializable]
    public sealed class ExpeditionLevelUpOptionRecord
    {
        public string offerId;
        public string optionId;
        public int level;
        public int optionIndex;
        public string optionType;
        public string displayName;
        public string rarity;
        public string skillDefinition;
        public string effectSummary;
    }

    [Serializable]
    public sealed class ExpeditionLevelUpEffectRecord
    {
        public string effectType;
        public string definitionName;
        public string targetSkill;
        public int targetSlot = -1;
        public string targetStat;
        public string modifierType;
        public float configuredValue;
        public float previousValue;
        public float newValue;
        public bool succeeded;
        public string status;
    }

    [Serializable]
    public sealed class ExpeditionLevelUpSelectionRecord
    {
        public string offerId;
        public string optionId;
        public int level;
        public string optionType;
        public string displayName;
        public string selectedAtUtc;
        public string status;
        public List<ExpeditionLevelUpEffectRecord> effects =
            new List<ExpeditionLevelUpEffectRecord>();
    }

    [Serializable]
    public sealed class ExpeditionPhaseRecord
    {
        public string phaseId;
        public string sourceId;
        public string sourceName;
        public string status;
        public bool hasStarted;
        public bool hasCompleted;
        public bool hasDuration;
        public double startedAtExpeditionSeconds;
        public double completedAtExpeditionSeconds;
        public double durationSeconds;
        public double startedAtSessionSeconds;
        public double completedAtSessionSeconds;
        public bool hasObservedDuration;
        public double observedDurationSeconds;
        public double sessionEndExpeditionSeconds;
        public double sessionEndSessionSeconds;
    }


    [Serializable]
    public sealed class ExpeditionSkillStatsSnapshot
    {
        public float finalDamage;
        public float finalCooldown;
        public float finalSpawnRadius;
        public float finalImpactRadius;
        public float finalRange;
        public float finalDuration;
        public float finalTickRate;
        public int finalCount;
        public float criticalChance;
        public float criticalDamageBonus;
        public int penetrationCount;
        public int bounceCount;
        public int chainCount;
        public bool grantsExplosion;
        public bool grantsChaining;
        public bool grantsSplit;
        public bool spawnZoneOnHit;
        public float spawnZoneChance;

        public ExpeditionSkillStatsSnapshot(SkillStats stats)
        {
            if (stats == null) return;
            finalDamage = stats.FinalDamage;
            finalCooldown = stats.FinalCooldown;
            finalSpawnRadius = stats.FinalSpawnRadius;
            finalImpactRadius = stats.FinalImpactRadius;
            finalRange = stats.FinalRange;
            finalDuration = stats.FinalDuration;
            finalTickRate = stats.FinalTickRate;
            finalCount = stats.FinalCount;
            criticalChance = stats.CriticalChance;
            criticalDamageBonus = stats.CriticalDamageBonus;
            penetrationCount = stats.PenetrationCount;
            bounceCount = stats.BounceCount;
            chainCount = stats.ChainCount;
            grantsExplosion = stats.GrantsExplosion;
            grantsChaining = stats.GrantsChaining;
            grantsSplit = stats.GrantsSplit;
            spawnZoneOnHit = stats.SpawnZoneOnHit;
            spawnZoneChance = stats.SpawnZoneChance;
        }
    }

    [Serializable]
    public sealed class ExpeditionSkillStatsChangeRecord
    {
        public string skillDefinitionId;
        public string skillDisplayName;
        public string changeSource;
        public string modifierName;
        public string modifierType;
        public float configuredValue;
        public int levelBefore;
        public int levelAfter;
        public double sessionSeconds;
        public double expeditionSeconds;
        public bool hasPreviousStats;
        public ExpeditionSkillStatsSnapshot previousStats;
        public ExpeditionSkillStatsSnapshot currentStats;
    }

    [Serializable]
    public sealed class ExpeditionSkillTreeEffectRecord
    {
        public string nodeId;
        public string nodeDisplayName;
        public string optionId;
        public string optionDisplayName;
        public int effectIndex;
        public string effectType;
        public string destinationDomain;
        public string target;
        public string modifierType;
        public float configuredValue;
        public double sessionSeconds;
        public double expeditionSeconds;
    }

    [Serializable]
    public sealed class ExpeditionTelemetryReport
    {
        public string sessionId;
        public string startedAtUtc;
        public string endedAtUtc;
        // Kept for compatibility with V1 exports; this is SessionTime.
        public float durationSeconds;
        public float sessionDurationSeconds;
        public float expeditionDurationSeconds;
        public bool hasExpeditionClock;
        public int initialLevel;
        public int finalLevel;
        public float experienceGained;
        public int levelUps;
        public float damageDealtToEnemies;
        public float damageReceived;
        public float healingReceived;
        public int enemiesDefeated;
        public int enemiesSpawned;
        public int enemiesSurvivedAtSessionEnd;
        public string startScene;
        public string endScene;
        public bool startedAutomatically;
        public bool stoppedAutomatically;
        public List<ExpeditionTelemetryEvent> events =
            new List<ExpeditionTelemetryEvent>();
        public List<ExpeditionTelemetryBucket> buckets =
            new List<ExpeditionTelemetryBucket>();
        public List<ExpeditionLevelUpOptionRecord> levelUpOptions =
            new List<ExpeditionLevelUpOptionRecord>();
        public List<ExpeditionLevelUpSelectionRecord> levelUpSelections =
            new List<ExpeditionLevelUpSelectionRecord>();
        public List<ExpeditionEnemySkillContributionRecord> enemySkillContributions =
            new List<ExpeditionEnemySkillContributionRecord>();
        public List<ExpeditionEnemyLifecycleRecord> enemyLifecycles =
            new List<ExpeditionEnemyLifecycleRecord>();
        public List<ExpeditionPhaseRecord> phaseRecords =
            new List<ExpeditionPhaseRecord>();
        public List<ExpeditionSkillStatsChangeRecord> skillStatsChanges =
            new List<ExpeditionSkillStatsChangeRecord>();
        public List<ExpeditionSkillTreeEffectRecord> skillTreeEffects =
            new List<ExpeditionSkillTreeEffectRecord>();
    }

    /// <summary>
    /// Editor-only observer for expedition balance telemetry.
    /// It never changes gameplay state or decisions.
    /// </summary>
    public static class ExpeditionTelemetryCollector
    {
        private static ExpeditionTelemetryReport currentReport;
        private static float recordingStartedAt;
        private static float latestExpeditionElapsedSeconds;
        private static bool hasExpeditionClock;
        private static bool isRecording;
        private static ExpeditionTelemetryBucketAccumulator bucketAccumulator;
        private static readonly Dictionary<string, Dictionary<string, ExpeditionEnemySkillContributionRecord>>
            enemyLifeContributions =
                new Dictionary<string, Dictionary<string, ExpeditionEnemySkillContributionRecord>>();
        private static readonly Dictionary<string, ExpeditionEnemyLifecycleRecord>
            enemyLifeRecords =
                new Dictionary<string, ExpeditionEnemyLifecycleRecord>();
        private static readonly Dictionary<string, ExpeditionPhaseRecord>
            activePhaseRecords =
                new Dictionary<string, ExpeditionPhaseRecord>();
        private static readonly HashSet<string> registeredSkillTreeEffectKeys =
            new HashSet<string>();

        public static bool IsRecording => isRecording;
        public static ExpeditionTelemetryReport CurrentReport => currentReport;

        public static void StartRecording(int initialLevel)
        {
            StartRecording(initialLevel, Time.realtimeSinceStartup, false, string.Empty);
        }

        public static void StartAutomaticRecording(int initialLevel, string sceneName)
        {
            StartRecording(initialLevel, Time.realtimeSinceStartup, true, sceneName);
        }

        private static void StartRecording(
            int initialLevel,
            float startRealtime,
            bool automatic,
            string sceneName)
        {
            if (isRecording)
                return;

            enemyLifeContributions.Clear();
            enemyLifeRecords.Clear();
            activePhaseRecords.Clear();
            registeredSkillTreeEffectKeys.Clear();
            currentReport = new ExpeditionTelemetryReport
            {
                sessionId = Guid.NewGuid().ToString("N"),
                startedAtUtc = DateTime.UtcNow.ToString("o"),
                initialLevel = Mathf.Max(1, initialLevel),
                finalLevel = Mathf.Max(1, initialLevel),
                startScene = sceneName,
                startedAutomatically = automatic
            };

            recordingStartedAt = startRealtime;
            latestExpeditionElapsedSeconds = 0f;
            hasExpeditionClock = false;
            bucketAccumulator = new ExpeditionTelemetryBucketAccumulator(
                currentReport.sessionId);
            currentReport.buckets = bucketAccumulator.Buckets;
            isRecording = true;
            AddEvent("SessionStarted", 0f, initialLevel);
        }

        public static void StopRecording(int finalLevel)
        {
            StopRecording(finalLevel, string.Empty, false);
        }

        public static void StopAutomaticRecording(int finalLevel, string sceneName)
        {
            StopRecording(finalLevel, sceneName, true);
        }

        private static void StopRecording(
            int finalLevel,
            string sceneName,
            bool automatic)
        {
            if (!isRecording || currentReport == null)
                return;

            UpdateDuration();
            FinalizeLivingEnemiesAtSessionEnd();
            FinalizeActivePhasesAtSessionEnd();
            if (bucketAccumulator != null)
                bucketAccumulator.CloseAt(latestExpeditionElapsedSeconds, GetSessionElapsedSeconds());
            currentReport.finalLevel = Mathf.Max(currentReport.finalLevel, Mathf.Max(1, finalLevel));
            currentReport.endedAtUtc = DateTime.UtcNow.ToString("o");
            currentReport.endScene = sceneName;
            currentReport.stoppedAutomatically = automatic;
            AddEvent("SessionStopped", 0f, finalLevel);
            UpdateDuration();
            isRecording = false;
        }

        /// <summary>
        /// Supplies the official Expedition Runtime clock. The value is sampled
        /// by the Editor observer and is never used to drive gameplay.
        /// </summary>
        public static void UpdateExpeditionClock(float elapsedSeconds, int currentLevel)
        {
            if (!isRecording || currentReport == null)
                return;

            if (!float.IsNaN(elapsedSeconds) && !float.IsInfinity(elapsedSeconds))
            {
                latestExpeditionElapsedSeconds = Mathf.Max(0f, elapsedSeconds);
                hasExpeditionClock = true;
                currentReport.expeditionDurationSeconds = latestExpeditionElapsedSeconds;
                currentReport.hasExpeditionClock = true;
            }

            currentReport.finalLevel = Mathf.Max(currentReport.finalLevel, Mathf.Max(1, currentLevel));
            UpdateDuration();
        }

        public static float GetElapsedSeconds()
        {
            if (currentReport == null)
                return 0f;

            if (isRecording)
                UpdateDuration();

            return currentReport.sessionDurationSeconds;
        }

        public static ExpeditionSkillStatsSnapshot CaptureSkillStats(SkillStats stats)
        {
            return stats == null ? null : new ExpeditionSkillStatsSnapshot(stats);
        }

        public static void RegisterSkillStatsChanged(
            string skillDefinitionId,
            string skillDisplayName,
            string changeSource,
            string modifierName,
            string modifierType,
            float configuredValue,
            int levelBefore,
            int levelAfter,
            ExpeditionSkillStatsSnapshot previousStats,
            SkillStats currentStats)
        {
            if (!isRecording || currentReport == null || currentStats == null)
                return;

            ExpeditionSkillStatsSnapshot currentSnapshot =
                new ExpeditionSkillStatsSnapshot(currentStats);
            if (changeSource == "GlobalModifiersReplaced" &&
                previousStats != null && SkillStatsSnapshotsEqual(previousStats, currentSnapshot))
                return;

            double expeditionTime = latestExpeditionElapsedSeconds;
            ExpeditionSkillStatsChangeRecord record = new ExpeditionSkillStatsChangeRecord
            {
                skillDefinitionId = skillDefinitionId ?? string.Empty,
                skillDisplayName = skillDisplayName ?? string.Empty,
                changeSource = changeSource ?? "Unknown",
                modifierName = modifierName ?? string.Empty,
                modifierType = modifierType ?? string.Empty,
                configuredValue = configuredValue,
                levelBefore = levelBefore,
                levelAfter = levelAfter,
                sessionSeconds = GetSessionElapsedSeconds(),
                expeditionSeconds = expeditionTime,
                hasPreviousStats = previousStats != null,
                previousStats = previousStats,
                currentStats = currentSnapshot
            };

            currentReport.skillStatsChanges.Add(record);
            AddEvent(
                "SkillStatsChanged",
                configuredValue,
                levelAfter,
                record.skillDefinitionId + " | " + record.changeSource + " | " + record.modifierName);
        }

        public static void RegisterSkillTreeEffect(
            string nodeId,
            string nodeDisplayName,
            string optionId,
            string optionDisplayName,
            int effectIndex,
            string effectType,
            string destinationDomain,
            string target,
            string modifierType,
            float configuredValue)
        {
            if (!isRecording || currentReport == null ||
                string.IsNullOrWhiteSpace(nodeId) ||
                string.IsNullOrWhiteSpace(effectType))
                return;

            string key = nodeId + "|" + (optionId ?? string.Empty) + "|" +
                effectIndex.ToString() + "|" + effectType + "|" +
                (destinationDomain ?? string.Empty);
            if (!registeredSkillTreeEffectKeys.Add(key))
                return;

            ExpeditionSkillTreeEffectRecord record =
                new ExpeditionSkillTreeEffectRecord
                {
                    nodeId = nodeId,
                    nodeDisplayName = nodeDisplayName ?? string.Empty,
                    optionId = optionId ?? string.Empty,
                    optionDisplayName = optionDisplayName ?? string.Empty,
                    effectIndex = effectIndex,
                    effectType = effectType,
                    destinationDomain = destinationDomain ?? string.Empty,
                    target = target ?? string.Empty,
                    modifierType = modifierType ?? string.Empty,
                    configuredValue = configuredValue,
                    sessionSeconds = GetSessionElapsedSeconds(),
                    expeditionSeconds = latestExpeditionElapsedSeconds
                };

            currentReport.skillTreeEffects.Add(record);
            AddEvent(
                "SkillTreeEffectObserved",
                configuredValue,
                effectIndex,
                nodeId + "|" + (optionId ?? string.Empty) + "|" +
                    effectType + "|" + record.destinationDomain);
        }

        private static bool SkillStatsSnapshotsEqual(
            ExpeditionSkillStatsSnapshot a, ExpeditionSkillStatsSnapshot b)
        {
            return Mathf.Approximately(a.finalDamage, b.finalDamage) &&
                Mathf.Approximately(a.finalCooldown, b.finalCooldown) &&
                Mathf.Approximately(a.finalSpawnRadius, b.finalSpawnRadius) &&
                Mathf.Approximately(a.finalImpactRadius, b.finalImpactRadius) &&
                Mathf.Approximately(a.finalRange, b.finalRange) &&
                Mathf.Approximately(a.finalDuration, b.finalDuration) &&
                Mathf.Approximately(a.finalTickRate, b.finalTickRate) &&
                a.finalCount == b.finalCount &&
                Mathf.Approximately(a.criticalChance, b.criticalChance) &&
                Mathf.Approximately(a.criticalDamageBonus, b.criticalDamageBonus) &&
                a.penetrationCount == b.penetrationCount &&
                a.bounceCount == b.bounceCount &&
                a.chainCount == b.chainCount &&
                a.grantsExplosion == b.grantsExplosion &&
                a.grantsChaining == b.grantsChaining &&
                a.grantsSplit == b.grantsSplit &&
                a.spawnZoneOnHit == b.spawnZoneOnHit &&
                Mathf.Approximately(a.spawnZoneChance, b.spawnZoneChance);
        }

        public static void RegisterPhaseStarted(
            string phaseId,
            string sourceId,
            string sourceName,
            double expeditionElapsedSeconds)
        {
            if (!isRecording || currentReport == null ||
                string.IsNullOrEmpty(phaseId) ||
                double.IsNaN(expeditionElapsedSeconds) ||
                double.IsInfinity(expeditionElapsedSeconds))
                return;

            string key = BuildPhaseKey(phaseId, sourceId);
            if (activePhaseRecords.ContainsKey(key))
                return;

            double expeditionTime = Math.Max(0d, expeditionElapsedSeconds);
            double sessionTime = GetSessionElapsedSeconds();

            ExpeditionPhaseRecord record = new ExpeditionPhaseRecord
            {
                phaseId = phaseId,
                sourceId = sourceId ?? string.Empty,
                sourceName = sourceName ?? string.Empty,
                status = "Active",
                hasStarted = true,
                hasCompleted = false,
                hasDuration = false,
                startedAtExpeditionSeconds = expeditionTime,
                startedAtSessionSeconds = sessionTime
            };

            activePhaseRecords.Add(key, record);
            currentReport.phaseRecords.Add(record);
            AddPhaseEvent("PhaseStarted", record, expeditionTime, sessionTime);
        }

        public static void RegisterPhaseCompleted(
            string phaseId,
            string sourceId,
            string sourceName,
            double expeditionElapsedSeconds)
        {
            CompletePhase(
                phaseId,
                sourceId,
                sourceName,
                expeditionElapsedSeconds,
                false);
        }

        public static void RegisterPhaseMilestone(
            string phaseId,
            string sourceId,
            string sourceName,
            double expeditionElapsedSeconds)
        {
            CompletePhase(
                phaseId,
                sourceId,
                sourceName,
                expeditionElapsedSeconds,
                true);
        }

        private static void CompletePhase(
            string phaseId,
            string sourceId,
            string sourceName,
            double expeditionElapsedSeconds,
            bool createMilestoneIfMissing)
        {
            if (!isRecording || currentReport == null ||
                string.IsNullOrEmpty(phaseId) ||
                double.IsNaN(expeditionElapsedSeconds) ||
                double.IsInfinity(expeditionElapsedSeconds))
                return;

            string key = BuildPhaseKey(phaseId, sourceId);
            ExpeditionPhaseRecord record;

            if (!activePhaseRecords.TryGetValue(key, out record))
            {
                if (!createMilestoneIfMissing)
                    return;

                double milestoneTime = Math.Max(0d, expeditionElapsedSeconds);
                double milestoneSessionTime = GetSessionElapsedSeconds();

                record = new ExpeditionPhaseRecord
                {
                    phaseId = phaseId,
                    sourceId = sourceId ?? string.Empty,
                    sourceName = sourceName ?? string.Empty,
                    status = "Completed",
                    hasStarted = false,
                    hasCompleted = true,
                    hasDuration = false,
                    completedAtExpeditionSeconds = milestoneTime,
                    completedAtSessionSeconds = milestoneSessionTime
                };

                currentReport.phaseRecords.Add(record);
                AddPhaseEvent(
                    "PhaseCompleted",
                    record,
                    milestoneTime,
                    milestoneSessionTime);
                return;
            }

            double completedTime = Math.Max(
                record.startedAtExpeditionSeconds,
                expeditionElapsedSeconds);
            double completedSessionTime = GetSessionElapsedSeconds();

            record.completedAtExpeditionSeconds = completedTime;
            record.completedAtSessionSeconds = completedSessionTime;
            record.durationSeconds =
                completedTime - record.startedAtExpeditionSeconds;
            record.hasCompleted = true;
            record.hasDuration = true;
            record.status = "Completed";

            activePhaseRecords.Remove(key);
            AddPhaseEvent(
                "PhaseCompleted",
                record,
                completedTime,
                completedSessionTime);
        }

        private static string BuildPhaseKey(
            string phaseId,
            string sourceId)
        {
            return phaseId + "|" + (sourceId ?? string.Empty);
        }

        private static void AddPhaseEvent(
            string eventType,
            ExpeditionPhaseRecord record,
            double expeditionElapsedSeconds,
            double sessionElapsedSeconds)
        {
            UpdateDuration();
            currentReport.events.Add(new ExpeditionTelemetryEvent
            {
                elapsedSeconds = (float)sessionElapsedSeconds,
                expeditionElapsedSeconds = (float)Math.Max(0d, expeditionElapsedSeconds),
                eventType = eventType,
                value = record.hasDuration ? (float)record.durationSeconds : 0f,
                integerValue = 0,
                detail = record.phaseId + "|" +
                    (record.sourceId ?? string.Empty) + "|" +
                    (record.sourceName ?? string.Empty) + "|" +
                    record.status
            });
        }

        public static void RegisterExperienceGained(float amount)
        {
            if (!CanRecord(amount)) return;
            currentReport.experienceGained += amount;
            AddEvent("ExperienceGained", amount, 0);
        }

        public static void RegisterLevelUp(int level)
        {
            if (!isRecording || currentReport == null) return;
            currentReport.levelUps++;
            currentReport.finalLevel = Mathf.Max(currentReport.finalLevel, Mathf.Max(1, level));
            AddEvent("LevelUp", 0f, level);
        }

        public static string RegisterLevelUpOptionsGenerated(
            List<UpgradeOption> options,
            int level)
        {
            if (!isRecording || currentReport == null || options == null)
                return string.Empty;

            string offerId = Guid.NewGuid().ToString("N");

            for (int i = 0; i < options.Count; i++)
            {
                UpgradeOption option = options[i];
                if (option == null)
                    continue;

                option.AnalyticsOptionId = Guid.NewGuid().ToString("N");

                ExpeditionLevelUpOptionRecord record =
                    new ExpeditionLevelUpOptionRecord
                    {
                        offerId = offerId,
                        optionId = option.AnalyticsOptionId,
                        level = level,
                        optionIndex = i,
                        optionType = GetOptionType(option),
                        displayName = GetOptionDisplayName(option),
                        rarity = GetOptionRarity(option),
                        skillDefinition = option.SkillDefinition != null
                            ? option.SkillDefinition.name : string.Empty,
                        effectSummary = BuildEffectSummary(option)
                    };

                currentReport.levelUpOptions.Add(record);

                AddEvent("LevelUpOptionOffered", 0f, i,
                    record.displayName + " | " + record.optionType);
            }

            AddEvent("LevelUpOptionsGenerated", options.Count, level, offerId);
            return offerId;
        }

        public static void RegisterLevelUpOptionSelected(
            UpgradeOption option,
            int level)
        {
            if (!isRecording || currentReport == null || option == null)
                return;

            ExpeditionLevelUpOptionRecord offered =
                currentReport.levelUpOptions.Find(
                    item => item.optionId == option.AnalyticsOptionId);

            ExpeditionLevelUpSelectionRecord selection =
                new ExpeditionLevelUpSelectionRecord
                {
                    offerId = offered != null ? offered.offerId : string.Empty,
                    optionId = option.AnalyticsOptionId,
                    level = level,
                    optionType = GetOptionType(option),
                    displayName = GetOptionDisplayName(option),
                    selectedAtUtc = DateTime.UtcNow.ToString("o"),
                    status = "Selected"
                };

            currentReport.levelUpSelections.Add(selection);
            AddEvent("LevelUpOptionSelected", 0f, level,
                selection.displayName + " | " + selection.optionType);
        }

        public static void RegisterLevelUpEffect(
            string optionId,
            string effectType,
            string definitionName,
            string targetSkill,
            int targetSlot,
            string targetStat,
            string modifierType,
            float configuredValue,
            float previousValue,
            float newValue,
            bool succeeded,
            string status)
        {
            if (!isRecording || currentReport == null)
                return;

            ExpeditionLevelUpSelectionRecord selection =
                currentReport.levelUpSelections.Find(
                    item => item.optionId == optionId);

            if (selection == null)
                return;

            selection.effects.Add(new ExpeditionLevelUpEffectRecord
            {
                effectType = effectType,
                definitionName = definitionName,
                targetSkill = targetSkill,
                targetSlot = targetSlot,
                targetStat = targetStat,
                modifierType = modifierType,
                configuredValue = configuredValue,
                previousValue = previousValue,
                newValue = newValue,
                succeeded = succeeded,
                status = status
            });

            AddEvent("LevelUpEffect" + status, newValue - previousValue,
                targetSlot, effectType + " | " + definitionName);
        }

        public static void CompleteLevelUpSelection(string optionId)
        {
            if (!isRecording || currentReport == null)
                return;

            ExpeditionLevelUpSelectionRecord selection =
                currentReport.levelUpSelections.Find(
                    item => item.optionId == optionId);

            if (selection == null || selection.status != "Selected")
                return;

            bool hasFailure = false;
            if (selection.effects != null)
            {
                for (int i = 0; i < selection.effects.Count; i++)
                {
                    if (!selection.effects[i].succeeded)
                    {
                        hasFailure = true;
                        break;
                    }
                }
            }

            selection.status = hasFailure ? "AppliedWithFailures" : "Applied";
        }

        public static void UpdateLevelUpSelectionStatus(
            string optionId,
            string status)
        {
            if (!isRecording || currentReport == null)
                return;

            ExpeditionLevelUpSelectionRecord selection =
                currentReport.levelUpSelections.Find(
                    item => item.optionId == optionId);

            if (selection != null)
                selection.status = status;
        }

        private static string GetOptionType(UpgradeOption option)
        {
            if (option.SkillDefinition != null)
                return "AddNewSkill";

            if (option.Effects != null && option.Effects.Count > 0)
                return option.Effects[0].EffectType.ToString();

            return "Unknown";
        }

        private static string GetOptionDisplayName(UpgradeOption option)
        {
            if (option.SkillDefinition != null)
                return option.SkillDefinition.DisplayName;

            if (option.Effects != null)
            {
                foreach (UpgradeEffect effect in option.Effects)
                {
                    if (effect.SkillModifier != null)
                        return effect.SkillModifier.DisplayName;
                    if (effect.GlobalDefinition != null)
                        return effect.GlobalDefinition.DisplayName;
                }
            }

            return "Fallback Upgrade";
        }

        private static string GetOptionRarity(UpgradeOption option)
        {
            if (option.SkillDefinition != null)
                return option.SkillDefinition.Rarity.ToString();

            if (option.Effects != null)
            {
                foreach (UpgradeEffect effect in option.Effects)
                {
                    if (effect.SkillModifier != null)
                        return effect.SkillModifier.Rarity.ToString();
                    if (effect.GlobalDefinition != null)
                        return effect.GlobalDefinition.Rarity.ToString();
                }
            }

            return "Common";
        }

        private static string BuildEffectSummary(UpgradeOption option)
        {
            if (option.Effects == null || option.Effects.Count == 0)
                return string.Empty;

            List<string> parts = new List<string>();
            foreach (UpgradeEffect effect in option.Effects)
            {
                if (effect == null)
                    continue;

                string name = effect.EffectType.ToString();
                if (effect.SkillModifier != null)
                    name += ":" + effect.SkillModifier.name;
                else if (effect.GlobalDefinition != null)
                    name += ":" + effect.GlobalDefinition.name;
                else if (effect.EffectType == UpgradeEffectType.GlobalModifier)
                    name += ":" + effect.TargetStat + " " +
                        effect.ModifierType + " " + effect.Value;

                parts.Add(name);
            }

            return string.Join(";", parts.ToArray());
        }

        public static void RegisterDamageDealtToEnemy(float amount)
        {
            RegisterDamageDealtToEnemy(amount, null, null);
        }

        public static void RegisterDamageDealtToEnemy(
            float amount,
            string skillDefinitionId,
            string enemyTier)
        {
            RegisterDamageDealtToEnemy(amount, skillDefinitionId, enemyTier, null);
        }

        public static void RegisterDamageDealtToEnemy(
            float amount,
            string skillDefinitionId,
            string enemyTier,
            string sourceCategory)
        {
            RegisterDamageDealtToEnemy(amount, skillDefinitionId, enemyTier,
                sourceCategory, null, null);
        }

        public static void RegisterDamageDealtToEnemy(
            float amount,
            string skillDefinitionId,
            string enemyTier,
            string sourceCategory,
            string skillId,
            string skillDisplayName)
        {
            if (!CanRecord(amount)) return;
            currentReport.damageDealtToEnemies += amount;

            if (bucketAccumulator != null)
            {
                bucketAccumulator.AddDamageDealt(
                    amount,
                    latestExpeditionElapsedSeconds,
                    GetSessionElapsedSeconds(),
                    skillDefinitionId,
                    enemyTier,
                    sourceCategory,
                    skillId,
                    skillDisplayName);
            }
        }

        public static void RegisterPlayerDamageReceived(float amount)
        {
            RegisterPlayerDamageReceived(amount, null);
        }

        public static void RegisterPlayerDamageReceived(
            float amount,
            string sourceCategory)
        {
            if (!CanRecord(amount)) return;
            currentReport.damageReceived += amount;

            if (bucketAccumulator != null)
            {
                bucketAccumulator.AddDamageReceived(
                    amount,
                    latestExpeditionElapsedSeconds,
                    GetSessionElapsedSeconds(),
                    sourceCategory);
            }
        }

        public static void RegisterPlayerHealing(float amount)
        {
            RegisterPlayerHealing(amount, null);
        }

        public static void RegisterPlayerHealing(
            float amount,
            string sourceType)
        {
            if (!CanRecord(amount)) return;
            currentReport.healingReceived += amount;

            if (bucketAccumulator != null)
            {
                bucketAccumulator.AddHealing(
                    amount,
                    latestExpeditionElapsedSeconds,
                    GetSessionElapsedSeconds(),
                    sourceType);
            }
        }

        public static void RegisterEnemyLifeDamage(
            string enemyLifeId,
            string enemyTier,
            string skillDefinitionId,
            string skillId,
            string skillDisplayName,
            string sourceCategory,
            string executionId,
            float effectiveDamage)
        {
            if (!CanRecord(effectiveDamage) || string.IsNullOrEmpty(enemyLifeId))
                return;

            Dictionary<string, ExpeditionEnemySkillContributionRecord> contributions;
            if (!enemyLifeContributions.TryGetValue(enemyLifeId, out contributions))
            {
                contributions =
                    new Dictionary<string, ExpeditionEnemySkillContributionRecord>();
                enemyLifeContributions.Add(enemyLifeId, contributions);
            }

            string contributionId = !string.IsNullOrEmpty(skillDefinitionId)
                ? skillDefinitionId
                : (!string.IsNullOrEmpty(skillId)
                    ? skillId
                    : (!string.IsNullOrEmpty(sourceCategory)
                        ? sourceCategory
                        : "Unknown/Unattributed"));

            ExpeditionEnemySkillContributionRecord record;
            if (!contributions.TryGetValue(contributionId, out record))
            {
                record = new ExpeditionEnemySkillContributionRecord
                {
                    enemyLifeId = enemyLifeId,
                    enemyTier = string.IsNullOrEmpty(enemyTier) ? "Unknown" : enemyTier,
                    skillDefinitionId = skillDefinitionId ?? string.Empty,
                    skillId = string.IsNullOrEmpty(skillId)
                        ? (string.IsNullOrEmpty(sourceCategory) ? "Unknown/Unattributed" : sourceCategory)
                        : skillId,
                    skillDisplayName = skillDisplayName ?? string.Empty,
                    sourceCategory = sourceCategory ?? string.Empty
                };

                contributions.Add(contributionId, record);
                currentReport.enemySkillContributions.Add(record);
            }

            record.damage += effectiveDamage;
            record.hitCount++;
            record.lastExecutionId = executionId ?? string.Empty;
        }

        public static void RegisterEnemySpawned(
            string enemyLifeId,
            string enemyVariantId,
            string enemyDisplayName,
            string enemyTier,
            string[] enemyRoles,
            float spawnedAtExpeditionSeconds)
        {
            if (!isRecording || currentReport == null ||
                string.IsNullOrEmpty(enemyLifeId) ||
                enemyLifeRecords.ContainsKey(enemyLifeId))
                return;

            ExpeditionEnemyLifecycleRecord record =
                new ExpeditionEnemyLifecycleRecord
                {
                    enemyLifeId = enemyLifeId,
                    enemyVariantId = enemyVariantId ?? string.Empty,
                    enemyDisplayName = enemyDisplayName ?? string.Empty,
                    enemyTier = string.IsNullOrEmpty(enemyTier) ? "Unknown" : enemyTier,
                    enemyRoles = enemyRoles != null
                        ? new List<string>(enemyRoles)
                        : new List<string>(),
                    spawnedAtExpeditionSeconds = Mathf.Max(0f, spawnedAtExpeditionSeconds),
                    outcome = "Active"
                };

            enemyLifeRecords.Add(enemyLifeId, record);
            currentReport.enemyLifecycles.Add(record);
            currentReport.enemiesSpawned++;
            AddEvent("EnemySpawned", 1f, currentReport.enemiesSpawned,
                record.enemyVariantId + "|" + record.enemyTier + "|" + record.enemyLifeId);
        }

        public static void RegisterEnemyLifeDefeated(string enemyLifeId)
        {
            RegisterEnemyLifeDefeated(enemyLifeId, latestExpeditionElapsedSeconds);
        }

        public static void RegisterEnemyLifeDefeated(
            string enemyLifeId,
            float diedAtExpeditionSeconds)
        {
            if (!isRecording || currentReport == null ||
                string.IsNullOrEmpty(enemyLifeId))
                return;

            ExpeditionEnemyLifecycleRecord lifecycleRecord;
            if (enemyLifeRecords.TryGetValue(enemyLifeId, out lifecycleRecord) &&
                lifecycleRecord.outcome == "Active")
            {
                lifecycleRecord.diedAtExpeditionSeconds =
                    Mathf.Max(lifecycleRecord.spawnedAtExpeditionSeconds, diedAtExpeditionSeconds);
                lifecycleRecord.ttkSeconds =
                    lifecycleRecord.diedAtExpeditionSeconds - lifecycleRecord.spawnedAtExpeditionSeconds;
                lifecycleRecord.outcome = "Defeated";
            }

            Dictionary<string, ExpeditionEnemySkillContributionRecord> contributions;
            if (!enemyLifeContributions.TryGetValue(enemyLifeId, out contributions))
                return;

            foreach (KeyValuePair<string, ExpeditionEnemySkillContributionRecord> pair in contributions)
                pair.Value.enemyDefeated = true;
        }

        private static void FinalizeActivePhasesAtSessionEnd()
        {
            if (currentReport == null || activePhaseRecords.Count == 0)
                return;

            double sessionEnd = GetSessionElapsedSeconds();
            double expeditionEnd = Math.Max(0d, latestExpeditionElapsedSeconds);

            foreach (KeyValuePair<string, ExpeditionPhaseRecord> pair in activePhaseRecords)
            {
                ExpeditionPhaseRecord record = pair.Value;
                if (record == null || record.hasCompleted)
                    continue;

                record.status = "ActiveAtSessionEnd";
                record.hasObservedDuration = true;
                record.observedDurationSeconds = Math.Max(
                    0d,
                    expeditionEnd - record.startedAtExpeditionSeconds);
                record.sessionEndExpeditionSeconds = expeditionEnd;
                record.sessionEndSessionSeconds = sessionEnd;
            }

            activePhaseRecords.Clear();
        }

        private static void FinalizeLivingEnemiesAtSessionEnd()
        {
            if (currentReport == null || enemyLifeRecords.Count == 0)
                return;

            foreach (KeyValuePair<string, ExpeditionEnemyLifecycleRecord> pair in enemyLifeRecords)
            {
                ExpeditionEnemyLifecycleRecord record = pair.Value;
                if (record == null || record.outcome != "Active")
                    continue;

                record.outcome = "SurvivedAtSessionEnd";
                record.sessionEndExpeditionSeconds = Mathf.Max(
                    record.spawnedAtExpeditionSeconds,
                    latestExpeditionElapsedSeconds);
                currentReport.enemiesSurvivedAtSessionEnd++;
            }
        }

        public static void RegisterSkillLastHit(
            string skillDefinitionId, string executionId, string enemyTier)
        {
            RegisterSkillLastHit(skillDefinitionId, executionId, enemyTier, null);
        }

        public static void RegisterSkillLastHit(
            string skillDefinitionId, string executionId, string enemyTier,
            string sourceCategory)
        {
            RegisterSkillLastHit(skillDefinitionId, executionId, enemyTier,
                sourceCategory, null, null);
        }

        public static void RegisterSkillLastHit(
            string skillDefinitionId,
            string executionId,
            string enemyTier,
            string sourceCategory,
            string skillId,
            string skillDisplayName)
        {
            if (!isRecording || currentReport == null)
                return;

            string sourceId = !string.IsNullOrEmpty(skillDefinitionId)
                ? skillDefinitionId
                : (!string.IsNullOrEmpty(sourceCategory)
                    ? sourceCategory
                    : "Unknown/Unattributed");
            string detail = sourceId + "|" +
                (skillId ?? string.Empty) + "|" +
                (skillDisplayName ?? string.Empty) + "|" +
                (executionId ?? string.Empty) + "|" +
                (enemyTier ?? "Unknown") + "|" +
                (sourceCategory ?? string.Empty);
            AddEvent("EnemyLastHit", 1f, 0, detail);
        }

        public static void RegisterEnemyDefeated()
        {
            if (!isRecording || currentReport == null) return;
            currentReport.enemiesDefeated++;
            AddEvent("EnemyDefeated", 1f, currentReport.enemiesDefeated);
        }

        private static bool CanRecord(float amount)
        {
            return isRecording && currentReport != null && amount > 0f &&
                   !float.IsNaN(amount) && !float.IsInfinity(amount);
        }

        private static void AddEvent(
            string eventType,
            float value,
            int integerValue,
            string detail = "")
        {
            if (currentReport == null) return;
            UpdateDuration();
            currentReport.events.Add(new ExpeditionTelemetryEvent
            {
                elapsedSeconds = currentReport.sessionDurationSeconds,
                expeditionElapsedSeconds = hasExpeditionClock ? latestExpeditionElapsedSeconds : 0f,
                eventType = eventType,
                value = value,
                integerValue = integerValue,
                detail = detail
            });
        }

        private static double GetSessionElapsedSeconds()
        {
            if (currentReport == null)
                return 0d;

            return Math.Max(0d, Time.realtimeSinceStartup - recordingStartedAt);
        }

        private static void UpdateDuration()
        {
            if (currentReport == null) return;
            float elapsed = Mathf.Max(0f, Time.realtimeSinceStartup - recordingStartedAt);
            currentReport.durationSeconds = elapsed;
            currentReport.sessionDurationSeconds = elapsed;
            if (hasExpeditionClock)
                currentReport.expeditionDurationSeconds = latestExpeditionElapsedSeconds;
        }
    }
}
#endif
