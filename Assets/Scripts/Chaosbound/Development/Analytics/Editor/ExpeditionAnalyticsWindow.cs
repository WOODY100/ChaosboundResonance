#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Chaosbound.Development.Analytics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chaosbound.Development.Analytics.Editor
{
    public sealed class ExpeditionAnalyticsWindow : EditorWindow
    {
        private Vector2 mainScroll;
        private Vector2 eventScroll;
        private bool showPhaseRecords = true;
        private bool showLevelUpRecords = true;
        private bool showSkillPerformance = true;
        private bool showSkillTierBreakdown = true;
        private bool showEnemyTierBreakdown = true;
        private bool showEnemyRoleBreakdown = true;
        private bool showEnemyLifecycleRecords;
        private bool showSkillStatsHistory = true;
        private bool showSkillTreeEffectRecords = true;

        [MenuItem("Window/Chaosbound/Expedition Analytics")]
        public static void Open()
        {
            ExpeditionAnalyticsWindow window =
                GetWindow<ExpeditionAnalyticsWindow>(
                    "Expedition Analytics");

            window.minSize = new Vector2(390f, 430f);
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.update += Repaint;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Repaint;
        }

        private void OnGUI()
        {
            mainScroll = EditorGUILayout.BeginScrollView(
                mainScroll,
                false,
                true);

            ExpeditionTelemetryReport report =
                ExpeditionTelemetryCollector.CurrentReport;

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(
                "EXPEDITION ANALYTICS",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Herramienta de observación exclusiva del Unity Editor. " +
                "No modifica estadísticas ni decisiones del gameplay.",
                MessageType.Info);

            DrawRecordingControls();

            report = ExpeditionTelemetryCollector.CurrentReport;

            if (report == null)
            {
                EditorGUILayout.HelpBox(
                    "No hay una sesión registrada. La grabación comienza automáticamente al cargar la escena Expedition.",
                    MessageType.None);

                EditorGUILayout.EndScrollView();
                return;
            }

            DrawSummary(report);
            DrawLevelUpAnalytics(report);
            DrawPhaseSummary(report);
            DrawSkillPerformance(report);
            DrawSkillStatsHistory(report);
            DrawSkillTreeEffectProvenance(report);
            DrawEnemyAnalytics(report);
            DrawExportControls(report);
            DrawEventLog(report);

            EditorGUILayout.EndScrollView();
        }

        private void DrawRecordingControls()
        {
            EditorGUILayout.BeginHorizontal();

            GUI.enabled =
                EditorApplication.isPlaying &&
                SceneManager.GetActiveScene().name == "Expedition" &&
                !ExpeditionTelemetryCollector.IsRecording;

            if (GUILayout.Button("Start Recording", GUILayout.Height(28f)))
            {
                PlayerExperienceSystem experience =
                    FindObjectOfType<PlayerExperienceSystem>();

                int initialLevel =
                    experience != null ? experience.CurrentLevel : 1;

                ExpeditionTelemetryCollector.StartRecording(initialLevel);
            }

            GUI.enabled =
                ExpeditionTelemetryCollector.IsRecording;

            if (GUILayout.Button("Stop Recording", GUILayout.Height(28f)))
                StopRecording();

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            string status =
                ExpeditionTelemetryCollector.IsRecording
                    ? "RECORDING"
                    : "STOPPED";

            EditorGUILayout.LabelField(
                "Status",
                status,
                EditorStyles.boldLabel);
        }

        private void StopRecording()
        {
            PlayerExperienceSystem experience =
                FindObjectOfType<PlayerExperienceSystem>();

            ExpeditionTelemetryReport report =
                ExpeditionTelemetryCollector.CurrentReport;

            int finalLevel =
                experience != null
                    ? experience.CurrentLevel
                    : (report != null ? report.finalLevel : 1);

            ExpeditionTelemetryCollector.StopRecording(finalLevel);
        }

        private void DrawSummary(ExpeditionTelemetryReport report)
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("SESSION SUMMARY", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Session ID", report.sessionId);
            EditorGUILayout.LabelField(
                "Session time",
                FormatDuration(ExpeditionTelemetryCollector.GetElapsedSeconds()));
            EditorGUILayout.LabelField(
                "Expedition time",
                report.hasExpeditionClock
                    ? FormatDuration(report.expeditionDurationSeconds)
                    : "Waiting for Expedition Runtime...");
            EditorGUILayout.LabelField(
                "Session mode",
                report.startedAutomatically ? "Automatic (scene)" : "Manual");

            EditorGUILayout.Space(4f);

            DrawMetric("XP gained", report.experienceGained.ToString("N1"));
            DrawMetric("Level ups", report.levelUps.ToString());
            DrawMetric("Upgrade options offered",
                report.levelUpOptions != null ? report.levelUpOptions.Count.ToString() : "0");
            DrawMetric("Upgrade choices selected",
                report.levelUpSelections != null ? report.levelUpSelections.Count.ToString() : "0");
            DrawMetric("Initial / Final level",
                report.initialLevel + " / " + report.finalLevel);
            DrawMetric("Damage dealt to enemies",
                report.damageDealtToEnemies.ToString("N1"));
            DrawMetric("Damage received",
                report.damageReceived.ToString("N1"));
            DrawMetric("Healing received",
                report.healingReceived.ToString("N1"));
            DrawMetric("Enemies defeated",
                report.enemiesDefeated.ToString("N0"));
            DrawMetric("Captured events",
                report.events != null ? report.events.Count.ToString("N0") : "0");
        }

        private void DrawMetric(string label, string value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label);
            GUILayout.Label(value, EditorStyles.boldLabel, GUILayout.Width(115f));
            EditorGUILayout.EndHorizontal();
        }


        private void DrawLevelUpAnalytics(ExpeditionTelemetryReport report)
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("LEVEL UP & MODIFIER ANALYTICS", EditorStyles.boldLabel);

            int offered = report.levelUpOptions != null ? report.levelUpOptions.Count : 0;
            int selected = report.levelUpSelections != null ? report.levelUpSelections.Count : 0;
            int applied = 0;
            int appliedWithFailures = 0;
            int replacementCancelled = 0;
            int failed = 0;
            int effectCount = 0;
            int successfulEffects = 0;
            int failedEffects = 0;

            Dictionary<string, int> selectionsByType = new Dictionary<string, int>();
            Dictionary<string, int> effectsByType = new Dictionary<string, int>();
            Dictionary<string, float> observedStatDeltas = new Dictionary<string, float>();

            if (report.levelUpSelections != null)
            {
                foreach (ExpeditionLevelUpSelectionRecord selection in report.levelUpSelections)
                {
                    if (selection == null)
                        continue;

                    string status = string.IsNullOrEmpty(selection.status) ? "Unknown" : selection.status;
                    if (status == "Applied" || status == "AppliedWithReplacement")
                        applied++;
                    else if (status == "AppliedWithFailures")
                        appliedWithFailures++;
                    else if (status == "ReplacementCancelled")
                        replacementCancelled++;
                    else if (status == "ReplacementFailed")
                        failed++;

                    string optionType = string.IsNullOrEmpty(selection.optionType) ? "Unknown" : selection.optionType;
                    IncrementCount(selectionsByType, optionType);

                    if (selection.effects == null)
                        continue;

                    foreach (ExpeditionLevelUpEffectRecord effect in selection.effects)
                    {
                        if (effect == null)
                            continue;

                        effectCount++;
                        string effectType = string.IsNullOrEmpty(effect.effectType) ? "Unknown" : effect.effectType;
                        IncrementCount(effectsByType, effectType);

                        if (effect.succeeded)
                            successfulEffects++;
                        else
                            failedEffects++;

                        if (effect.succeeded &&
                            effect.effectType == "GlobalModifier" &&
                            !string.IsNullOrEmpty(effect.targetStat))
                        {
                            float delta = effect.newValue - effect.previousValue;
                            float current;
                            observedStatDeltas.TryGetValue(effect.targetStat, out current);
                            observedStatDeltas[effect.targetStat] = current + delta;
                        }
                    }
                }
            }

            DrawMetric("Options offered", offered.ToString("N0"));
            DrawMetric("Selections made", selected.ToString("N0"));
            DrawMetric("Applied selections", applied.ToString("N0"));
            DrawMetric("Applied with failures", appliedWithFailures.ToString("N0"));
            DrawMetric("Replacement cancelled / failed",
                (replacementCancelled + failed).ToString("N0"));
            DrawMetric("Recorded effects", effectCount.ToString("N0"));
            DrawMetric("Successful / failed effects",
                successfulEffects.ToString("N0") + " / " + failedEffects.ToString("N0"));

            showLevelUpRecords = EditorGUILayout.Foldout(
                showLevelUpRecords,
                "Selection and Modifier Details",
                true);

            if (!showLevelUpRecords)
                return;

            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("Selections by option type", EditorStyles.boldLabel);
            if (selectionsByType.Count == 0)
                EditorGUILayout.LabelField("No selections recorded.");
            else
                foreach (KeyValuePair<string, int> item in selectionsByType)
                    DrawMetric(item.Key, item.Value.ToString("N0"));

            EditorGUILayout.Space(3f);
            EditorGUILayout.LabelField("Effects by type", EditorStyles.boldLabel);
            if (effectsByType.Count == 0)
                EditorGUILayout.LabelField("No effects recorded.");
            else
                foreach (KeyValuePair<string, int> item in effectsByType)
                    DrawMetric(item.Key, item.Value.ToString("N0"));

            EditorGUILayout.Space(3f);
            EditorGUILayout.LabelField("Observed Global Modifier stat deltas", EditorStyles.boldLabel);
            if (observedStatDeltas.Count == 0)
            {
                EditorGUILayout.LabelField("No successful Global Modifier stat deltas recorded.");
            }
            else
            {
                foreach (KeyValuePair<string, float> item in observedStatDeltas)
                    DrawMetric(item.Key, item.Value.ToString("+0.###;-0.###;0"));
            }

            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField("Selection History", EditorStyles.boldLabel);

            if (report.levelUpSelections != null)
            {
                foreach (ExpeditionLevelUpSelectionRecord selection in report.levelUpSelections)
                {
                    if (selection == null)
                        continue;

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(
                        "Lv. " + selection.level + " — " +
                        (selection.displayName ?? "Unknown"),
                        EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Type / Status",
                        (selection.optionType ?? "Unknown") + " / " +
                        (selection.status ?? "Unknown"));
                    if (!string.IsNullOrEmpty(selection.offerId))
                        EditorGUILayout.LabelField("Offer ID", selection.offerId);

                    if (selection.effects != null)
                    {
                        foreach (ExpeditionLevelUpEffectRecord effect in selection.effects)
                        {
                            if (effect == null)
                                continue;

                            string detail = (effect.effectType ?? "Effect") + " — " +
                                (effect.definitionName ?? string.Empty);
                            if (!string.IsNullOrEmpty(effect.targetSkill))
                                detail += " | Skill: " + effect.targetSkill;
                            if (!string.IsNullOrEmpty(effect.targetStat))
                                detail += " | Stat: " + effect.targetStat;
                            if (!string.IsNullOrEmpty(effect.modifierType))
                                detail += " | Modifier: " + effect.modifierType;

                            EditorGUILayout.LabelField(detail);
                            if (effect.effectType == "GlobalModifier")
                            {
                                EditorGUILayout.LabelField(
                                    "Configured: " + effect.configuredValue.ToString("0.###") +
                                    " | Observed stat: " + effect.previousValue.ToString("0.###") +
                                    " → " + effect.newValue.ToString("0.###"));
                            }
                            else if (effect.effectType == "SkillModifier")
                            {
                                EditorGUILayout.LabelField(
                                    "Skill level: " + effect.previousValue.ToString("0.###") +
                                    " → " + effect.newValue.ToString("0.###"));
                            }

                            EditorGUILayout.LabelField(
                                "Effect result",
                                effect.succeeded ? "Succeeded" : (effect.status ?? "Failed"));
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.HelpBox(
                "Global Modifier deltas show the observed stat value before and after each applied run upgrade. " +
                "Skill Modifier records currently expose skill level changes, not the individual RuntimeSkill stat deltas. " +
                "Persistent Skill Tree effects are not attributed to individual nodes/effects by this report. " +
                "These records support observed associations; they do not prove that one upgrade caused later healing or damage.",
                MessageType.Info);
        }

        private static void IncrementCount(Dictionary<string, int> counts, string key)
        {
            int value;
            counts.TryGetValue(key, out value);
            counts[key] = value + 1;
        }

        private void DrawPhaseSummary(ExpeditionTelemetryReport report)
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("PHASE ANALYTICS", EditorStyles.boldLabel);

            if (report.phaseRecords == null || report.phaseRecords.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No hay registros de fases en esta sesión. " +
                    "Las fases se mostrarán cuando se produzcan sus transiciones instrumentadas.",
                    MessageType.None);
                return;
            }

            int startedCount = 0;
            int completedCount = 0;
            int activeAtEndCount = 0;
            int milestoneCount = 0;

            foreach (ExpeditionPhaseRecord phase in report.phaseRecords)
            {
                if (phase == null)
                    continue;

                if (phase.hasStarted)
                    startedCount++;

                if (phase.hasCompleted)
                    completedCount++;

                if (phase.status == "ActiveAtSessionEnd")
                    activeAtEndCount++;

                if (!phase.hasStarted && phase.hasCompleted)
                    milestoneCount++;
            }

            DrawMetric("Phase records", report.phaseRecords.Count.ToString());
            DrawMetric("Started phases", startedCount.ToString());
            DrawMetric("Completed phases", completedCount.ToString());
            DrawMetric("Milestones", milestoneCount.ToString());
            DrawMetric("Active at session end", activeAtEndCount.ToString());

            showPhaseRecords = EditorGUILayout.Foldout(
                showPhaseRecords,
                "Phase Records",
                true);

            if (!showPhaseRecords)
                return;

            EditorGUI.indentLevel++;

            foreach (ExpeditionPhaseRecord phase in report.phaseRecords)
            {
                if (phase == null)
                    continue;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                string title = string.IsNullOrEmpty(phase.sourceName)
                    ? phase.phaseId
                    : phase.phaseId + " — " + phase.sourceName;

                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Status", phase.status ?? "Unknown");

                if (!string.IsNullOrEmpty(phase.sourceId))
                    EditorGUILayout.LabelField("Source ID", phase.sourceId);

                if (phase.hasStarted)
                {
                    EditorGUILayout.LabelField(
                        "Started (Expedition)",
                        FormatDuration((float)phase.startedAtExpeditionSeconds));

                    EditorGUILayout.LabelField(
                        "Started (Session)",
                        FormatDuration((float)phase.startedAtSessionSeconds));
                }
                else
                {
                    EditorGUILayout.LabelField(
                        "Start",
                        "Not recorded (milestone)");
                }

                if (phase.hasCompleted)
                {
                    EditorGUILayout.LabelField(
                        "Completed (Expedition)",
                        FormatDuration((float)phase.completedAtExpeditionSeconds));

                    EditorGUILayout.LabelField(
                        "Completed (Session)",
                        FormatDuration((float)phase.completedAtSessionSeconds));
                }

                if (phase.hasDuration)
                {
                    EditorGUILayout.LabelField(
                        "Duration",
                        FormatDuration((float)phase.durationSeconds));
                }
                else if (phase.hasObservedDuration)
                {
                    EditorGUILayout.LabelField(
                        "Observed duration (not completed)",
                        FormatDuration((float)phase.observedDurationSeconds));
                }
                else if (!phase.hasStarted && phase.hasCompleted)
                {
                    EditorGUILayout.LabelField(
                        "Duration",
                        "Not applicable");
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUI.indentLevel--;
        }


        private sealed class SkillPerformanceAggregate
        {
            public string key;
            public string skillId;
            public string displayName;
            public string sourceCategory;
            public string tier;
            public float damage;
            public int hitCount;
        }

        private void DrawSkillPerformance(ExpeditionTelemetryReport report)
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("SKILL PERFORMANCE", EditorStyles.boldLabel);

            if (report.enemySkillContributions == null ||
                report.enemySkillContributions.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No hay contribuciones de daño registradas todavía. " +
                    "Los datos aparecerán cuando las habilidades instrumentadas dañen enemigos durante una sesión.",
                    MessageType.None);
                return;
            }

            Dictionary<string, SkillPerformanceAggregate> bySkill =
                new Dictionary<string, SkillPerformanceAggregate>();
            Dictionary<string, SkillPerformanceAggregate> bySkillTier =
                new Dictionary<string, SkillPerformanceAggregate>();

            float attributedDamage = 0f;
            int attributedHits = 0;

            foreach (ExpeditionEnemySkillContributionRecord contribution
                in report.enemySkillContributions)
            {
                if (contribution == null)
                    continue;

                string identity = !string.IsNullOrEmpty(contribution.skillDefinitionId)
                    ? contribution.skillDefinitionId
                    : (!string.IsNullOrEmpty(contribution.skillId)
                        ? contribution.skillId
                        : "Unknown/Unattributed");

                string displayName = !string.IsNullOrEmpty(contribution.skillDisplayName)
                    ? contribution.skillDisplayName
                    : identity;

                AddSkillAggregate(
                    bySkill,
                    identity,
                    identity,
                    displayName,
                    contribution.sourceCategory,
                    string.Empty,
                    contribution.damage,
                    contribution.hitCount);

                string tier = string.IsNullOrEmpty(contribution.enemyTier)
                    ? "Unknown"
                    : contribution.enemyTier;

                AddSkillAggregate(
                    bySkillTier,
                    identity + "|" + tier,
                    identity,
                    displayName,
                    contribution.sourceCategory,
                    tier,
                    contribution.damage,
                    contribution.hitCount);

                attributedDamage += contribution.damage;
                attributedHits += contribution.hitCount;
            }

            float expeditionMinutes = report.expeditionDurationSeconds / 60f;
            DrawMetric("Skill contribution records",
                report.enemySkillContributions.Count.ToString("N0"));
            DrawMetric("Attributed damage",
                attributedDamage.ToString("N1"));
            DrawMetric("Attributed hits",
                attributedHits.ToString("N0"));
            DrawMetric("Damage per expedition minute",
                expeditionMinutes > 0f
                    ? (attributedDamage / expeditionMinutes).ToString("N1")
                    : "N/A");

            showSkillPerformance = EditorGUILayout.Foldout(
                showSkillPerformance, "By Skill", true);

            if (showSkillPerformance)
            {
                EditorGUI.indentLevel++;
                foreach (SkillPerformanceAggregate aggregate in bySkill.Values)
                    DrawSkillAggregate(aggregate, expeditionMinutes, false);
                EditorGUI.indentLevel--;
            }

            showSkillTierBreakdown = EditorGUILayout.Foldout(
                showSkillTierBreakdown, "By Skill and Enemy Tier", true);

            if (showSkillTierBreakdown)
            {
                EditorGUI.indentLevel++;
                foreach (SkillPerformanceAggregate aggregate in bySkillTier.Values)
                    DrawSkillAggregate(aggregate, expeditionMinutes, true);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.HelpBox(
                "Damage and hit counts represent effective HP damage, aggregated per enemy life. " +
                "Last-hit attribution is recorded separately and is not treated as total skill kills. " +
                "Unknown/Unattributed remains separate when source identity is unavailable.",
                MessageType.Info);
        }

        private static void AddSkillAggregate(
            Dictionary<string, SkillPerformanceAggregate> aggregates,
            string key,
            string skillId,
            string displayName,
            string sourceCategory,
            string tier,
            float damage,
            int hitCount)
        {
            SkillPerformanceAggregate aggregate;
            if (!aggregates.TryGetValue(key, out aggregate))
            {
                aggregate = new SkillPerformanceAggregate
                {
                    key = key,
                    skillId = skillId,
                    displayName = displayName,
                    sourceCategory = sourceCategory,
                    tier = tier
                };
                aggregates.Add(key, aggregate);
            }

            aggregate.damage += damage;
            aggregate.hitCount += hitCount;
        }

        private void DrawSkillAggregate(
            SkillPerformanceAggregate aggregate,
            float expeditionMinutes,
            bool includeTier)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            string title = aggregate.displayName;
            if (includeTier)
                title += " — " + aggregate.tier;

            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Skill ID", aggregate.skillId);
            if (!string.IsNullOrEmpty(aggregate.sourceCategory))
                EditorGUILayout.LabelField("Source category", aggregate.sourceCategory);

            DrawMetric("Effective damage", aggregate.damage.ToString("N2"));
            DrawMetric("Effective hits", aggregate.hitCount.ToString("N0"));
            DrawMetric("Average damage / hit",
                aggregate.hitCount > 0
                    ? (aggregate.damage / aggregate.hitCount).ToString("N2")
                    : "N/A");
            DrawMetric("Damage / expedition minute",
                expeditionMinutes > 0f
                    ? (aggregate.damage / expeditionMinutes).ToString("N2")
                    : "N/A");

            EditorGUILayout.EndVertical();
        }


        private sealed class EnemyTierAggregate
        {
            public string tier;
            public int spawned;
            public int defeated;
            public int survived;
            public int active;
            public List<float> ttkValues = new List<float>();
        }

        private sealed class EnemyRoleAggregate
        {
            public string role;
            public int enemyCount;
        }

        private void DrawSkillStatsHistory(ExpeditionTelemetryReport report)
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("SKILL STAT EVOLUTION", EditorStyles.boldLabel);

            int count = report.skillStatsChanges != null
                ? report.skillStatsChanges.Count : 0;
            DrawMetric("Recorded skill stat changes", count.ToString("N0"));
            if (count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No skill stat changes have been recorded in this session.",
                    MessageType.None);
                return;
            }

            showSkillStatsHistory = EditorGUILayout.Foldout(
                showSkillStatsHistory, "Skill Stat Change Records", true);
            if (!showSkillStatsHistory) return;

            EditorGUI.indentLevel++;
            for (int i = 0; i < report.skillStatsChanges.Count; i++)
            {
                ExpeditionSkillStatsChangeRecord record = report.skillStatsChanges[i];
                if (record == null) continue;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    string.IsNullOrEmpty(record.skillDisplayName)
                        ? record.skillDefinitionId : record.skillDisplayName,
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Source", record.changeSource);
                if (!string.IsNullOrEmpty(record.modifierName))
                    EditorGUILayout.LabelField("Modifier", record.modifierName);
                EditorGUILayout.LabelField("Modifier type", record.modifierType);
                EditorGUILayout.LabelField("Level",
                    record.levelBefore + " → " + record.levelAfter);
                EditorGUILayout.LabelField("Session / Expedition",
                    FormatDuration((float)record.sessionSeconds) + " / " +
                    FormatDuration((float)record.expeditionSeconds));

                if (record.currentStats != null)
                {
                    DrawSkillStatDelta("Final damage", record,
                        x => x.finalDamage);
                    DrawSkillStatDelta("Final cooldown", record,
                        x => x.finalCooldown);
                    DrawSkillStatDelta("Spawn radius", record,
                        x => x.finalSpawnRadius);
                    DrawSkillStatDelta("Impact radius", record,
                        x => x.finalImpactRadius);
                    DrawSkillStatDelta("Range", record, x => x.finalRange);
                    DrawSkillStatDelta("Duration", record, x => x.finalDuration);
                    DrawSkillStatDelta("Tick rate", record, x => x.finalTickRate);
                    DrawSkillStatDelta("Count", record, x => x.finalCount);
                    DrawSkillStatDelta("Critical chance", record,
                        x => x.criticalChance);
                    DrawSkillStatDelta("Penetration / Chain / Bounce", record,
                        x => x.penetrationCount + "/" + x.chainCount + "/" + x.bounceCount);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.HelpBox(
                "Snapshots show the calculated RuntimeSkill values before and after each recorded change. " +
                "Global modifier replacements are attributed to the global layer, not to a specific Skill Tree node; " +
                "that provenance is reserved for the next instrumentation block.",
                MessageType.Info);
        }

        private static void DrawSkillStatDelta(
            string label, ExpeditionSkillStatsChangeRecord record,
            Func<ExpeditionSkillStatsSnapshot, float> selector)
        {
            float current = selector(record.currentStats);
            string previous = record.hasPreviousStats && record.previousStats != null
                ? selector(record.previousStats).ToString("N2")
                : "N/A";
            EditorGUILayout.LabelField(label, previous + " → " + current.ToString("N2"));
        }

        private static void DrawSkillStatDelta(
            string label, ExpeditionSkillStatsChangeRecord record,
            Func<ExpeditionSkillStatsSnapshot, string> selector)
        {
            string current = selector(record.currentStats);
            string previous = record.hasPreviousStats && record.previousStats != null
                ? selector(record.previousStats) : "N/A";
            EditorGUILayout.LabelField(label, previous + " → " + current);
        }

        private void DrawSkillTreeEffectProvenance(ExpeditionTelemetryReport report)
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("SKILL TREE EFFECT PROVENANCE", EditorStyles.boldLabel);

            int count = report.skillTreeEffects != null
                ? report.skillTreeEffects.Count : 0;
            DrawMetric("Observed Skill Tree effects", count.ToString("N0"));

            if (count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No Skill Tree effect provenance has been observed in this session. " +
                    "Effects are recorded when the active modifier sources are built or refreshed.",
                    MessageType.None);
                return;
            }

            showSkillTreeEffectRecords = EditorGUILayout.Foldout(
                showSkillTreeEffectRecords, "Effect Provenance Records", true);
            if (!showSkillTreeEffectRecords)
                return;

            EditorGUI.indentLevel++;
            for (int i = 0; i < report.skillTreeEffects.Count; i++)
            {
                ExpeditionSkillTreeEffectRecord record = report.skillTreeEffects[i];
                if (record == null)
                    continue;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    string.IsNullOrEmpty(record.nodeDisplayName)
                        ? record.nodeId : record.nodeDisplayName,
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Node ID", record.nodeId);
                if (!string.IsNullOrEmpty(record.optionId))
                {
                    EditorGUILayout.LabelField("Selected option",
                        record.optionDisplayName + " (" + record.optionId + ")");
                }
                EditorGUILayout.LabelField("Effect", record.effectType +
                    " [" + record.effectIndex + "]");
                EditorGUILayout.LabelField("Destination", record.destinationDomain);
                EditorGUILayout.LabelField("Target", record.target);
                EditorGUILayout.LabelField("Modifier type", record.modifierType);
                DrawMetric("Configured value",
                    record.configuredValue.ToString("N3"));
                EditorGUILayout.LabelField("Expedition time",
                    record.expeditionSeconds.ToString("N2") + " s");
                EditorGUILayout.EndVertical();
            }
            EditorGUI.indentLevel--;
        }

        private void DrawEnemyAnalytics(ExpeditionTelemetryReport report)
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("ENEMY ANALYTICS", EditorStyles.boldLabel);

            if (report.enemyLifecycles == null ||
                report.enemyLifecycles.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No hay ciclos de vida de enemigos registrados. " +
                    "Los registros se generan cuando EnemyMaterializer materializa enemigos normales durante una sesión.",
                    MessageType.None);
                return;
            }

            int spawned = report.enemyLifecycles.Count;
            int defeated = 0;
            int survived = 0;
            int active = 0;
            List<float> allTtk = new List<float>();
            Dictionary<string, EnemyTierAggregate> byTier =
                new Dictionary<string, EnemyTierAggregate>();
            Dictionary<string, EnemyRoleAggregate> byRole =
                new Dictionary<string, EnemyRoleAggregate>();

            foreach (ExpeditionEnemyLifecycleRecord enemy in report.enemyLifecycles)
            {
                if (enemy == null)
                    continue;

                string outcome = string.IsNullOrEmpty(enemy.outcome)
                    ? "Unknown"
                    : enemy.outcome;

                if (outcome == "Defeated")
                {
                    defeated++;
                    if (enemy.ttkSeconds >= 0f)
                        allTtk.Add(enemy.ttkSeconds);
                }
                else if (outcome == "SurvivedAtSessionEnd")
                {
                    survived++;
                }
                else if (outcome == "Active")
                {
                    active++;
                }

                string tier = string.IsNullOrEmpty(enemy.enemyTier)
                    ? "Unknown"
                    : enemy.enemyTier;

                EnemyTierAggregate tierAggregate;
                if (!byTier.TryGetValue(tier, out tierAggregate))
                {
                    tierAggregate = new EnemyTierAggregate { tier = tier };
                    byTier.Add(tier, tierAggregate);
                }

                tierAggregate.spawned++;
                if (outcome == "Defeated")
                {
                    tierAggregate.defeated++;
                    tierAggregate.ttkValues.Add(enemy.ttkSeconds);
                }
                else if (outcome == "SurvivedAtSessionEnd")
                {
                    tierAggregate.survived++;
                }
                else if (outcome == "Active")
                {
                    tierAggregate.active++;
                }

                if (enemy.enemyRoles != null)
                {
                    foreach (string role in enemy.enemyRoles)
                    {
                        string roleId = string.IsNullOrEmpty(role) ? "Unknown" : role;
                        EnemyRoleAggregate roleAggregate;
                        if (!byRole.TryGetValue(roleId, out roleAggregate))
                        {
                            roleAggregate = new EnemyRoleAggregate { role = roleId };
                            byRole.Add(roleId, roleAggregate);
                        }

                        // An enemy may have multiple roles. Role totals are memberships,
                        // not mutually exclusive enemy counts.
                        roleAggregate.enemyCount++;
                    }
                }
            }

            DrawMetric("Enemy instances materialized", spawned.ToString("N0"));
            DrawMetric("Defeated", defeated.ToString("N0"));
            DrawMetric("Survived at session end", survived.ToString("N0"));
            DrawMetric("Still active in current report", active.ToString("N0"));

            if (allTtk.Count > 0)
            {
                allTtk.Sort();
                DrawMetric("Average TTK (defeated only)",
                    Average(allTtk).ToString("N2") + " s");
                DrawMetric("Median TTK (defeated only)",
                    Median(allTtk).ToString("N2") + " s");
                DrawMetric("Fastest TTK",
                    allTtk[0].ToString("N2") + " s");
                DrawMetric("Longest TTK",
                    allTtk[allTtk.Count - 1].ToString("N2") + " s");
            }
            else
            {
                DrawMetric("TTK", "N/A — no defeated enemy records");
            }

            showEnemyTierBreakdown = EditorGUILayout.Foldout(
                showEnemyTierBreakdown, "By Enemy Tier", true);

            if (showEnemyTierBreakdown)
            {
                EditorGUI.indentLevel++;
                foreach (EnemyTierAggregate tier in byTier.Values)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(tier.tier, EditorStyles.boldLabel);
                    DrawMetric("Spawned", tier.spawned.ToString("N0"));
                    DrawMetric("Defeated", tier.defeated.ToString("N0"));
                    DrawMetric("Survived at session end", tier.survived.ToString("N0"));
                    DrawMetric("Active", tier.active.ToString("N0"));

                    if (tier.ttkValues.Count > 0)
                    {
                        tier.ttkValues.Sort();
                        DrawMetric("Average TTK",
                            Average(tier.ttkValues).ToString("N2") + " s");
                        DrawMetric("Median TTK",
                            Median(tier.ttkValues).ToString("N2") + " s");
                    }
                    else
                    {
                        DrawMetric("TTK", "N/A");
                    }

                    EditorGUILayout.EndVertical();
                }
                EditorGUI.indentLevel--;
            }

            showEnemyRoleBreakdown = EditorGUILayout.Foldout(
                showEnemyRoleBreakdown, "By Enemy Role", true);

            if (showEnemyRoleBreakdown)
            {
                EditorGUI.indentLevel++;
                foreach (EnemyRoleAggregate role in byRole.Values)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(role.role);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(role.enemyCount.ToString("N0"), GUILayout.Width(55f));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }

            showEnemyLifecycleRecords = EditorGUILayout.Foldout(
                showEnemyLifecycleRecords, "Individual Enemy Lifecycle Records", false);

            if (showEnemyLifecycleRecords)
            {
                EditorGUI.indentLevel++;
                foreach (ExpeditionEnemyLifecycleRecord enemy in report.enemyLifecycles)
                {
                    if (enemy == null)
                        continue;

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(
                        string.IsNullOrEmpty(enemy.enemyDisplayName)
                            ? enemy.enemyVariantId
                            : enemy.enemyDisplayName,
                        EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Life ID", enemy.enemyLifeId);
                    EditorGUILayout.LabelField("Variant ID", enemy.enemyVariantId);
                    EditorGUILayout.LabelField("Tier / Outcome",
                        (enemy.enemyTier ?? "Unknown") + " / " +
                        (enemy.outcome ?? "Unknown"));
                    EditorGUILayout.LabelField("Roles",
                        enemy.enemyRoles != null && enemy.enemyRoles.Count > 0
                            ? string.Join(", ", enemy.enemyRoles.ToArray())
                            : "None recorded");
                    EditorGUILayout.LabelField("Spawned at Expedition Time",
                        FormatDuration(enemy.spawnedAtExpeditionSeconds));

                    if (enemy.outcome == "Defeated")
                    {
                        EditorGUILayout.LabelField("Died at Expedition Time",
                            FormatDuration(enemy.diedAtExpeditionSeconds));
                        EditorGUILayout.LabelField("TTK",
                            enemy.ttkSeconds.ToString("N2") + " s");
                    }
                    else if (enemy.outcome == "SurvivedAtSessionEnd")
                    {
                        EditorGUILayout.LabelField("Session end",
                            FormatDuration(enemy.sessionEndExpeditionSeconds));
                        EditorGUILayout.LabelField("Observed lifetime",
                            Mathf.Max(0f, enemy.sessionEndExpeditionSeconds -
                                enemy.spawnedAtExpeditionSeconds).ToString("N2") + " s");
                    }

                    EditorGUILayout.EndVertical();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.HelpBox(
                "TTK se calcula únicamente para enemigos derrotados: tiempo de muerte menos tiempo de materialización, ambos en Expedition Time. " +
                "Los supervivientes al final de la sesión no se incluyen en promedios de TTK. " +
                "Los roles no son mutuamente excluyentes: un enemigo puede contribuir a más de una categoría.",
                MessageType.Info);
        }

        private static float Average(List<float> values)
        {
            if (values == null || values.Count == 0)
                return 0f;

            float total = 0f;
            for (int i = 0; i < values.Count; i++)
                total += values[i];

            return total / values.Count;
        }

        private static float Median(List<float> sortedValues)
        {
            if (sortedValues == null || sortedValues.Count == 0)
                return 0f;

            int middle = sortedValues.Count / 2;
            if (sortedValues.Count % 2 == 0)
                return (sortedValues[middle - 1] + sortedValues[middle]) * 0.5f;

            return sortedValues[middle];
        }

        private void DrawExportControls(ExpeditionTelemetryReport report)
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("EXPORT", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Export JSON"))
            {
                string path = EditorUtility.SaveFilePanel(
                    "Export Expedition Analytics JSON",
                    Application.dataPath,
                    "ExpeditionAnalytics_" + report.sessionId + ".json",
                    "json");

                if (!string.IsNullOrEmpty(path))
                {
                    File.WriteAllText(
                        path,
                        JsonUtility.ToJson(report, true),
                        Encoding.UTF8);
                    EditorUtility.RevealInFinder(path);
                }
            }

            if (GUILayout.Button("Export CSV"))
            {
                string path = EditorUtility.SaveFilePanel(
                    "Export Expedition Analytics CSV",
                    Application.dataPath,
                    "ExpeditionAnalytics_" + report.sessionId + ".csv",
                    "csv");

                if (!string.IsNullOrEmpty(path))
                {
                    File.WriteAllText(
                        path,
                        BuildCsv(report),
                        new UTF8Encoding(true));
                    EditorUtility.RevealInFinder(path);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawEventLog(ExpeditionTelemetryReport report)
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("EVENT LOG", EditorStyles.boldLabel);

            if (report.events == null || report.events.Count == 0)
            {
                EditorGUILayout.LabelField("No events captured.");
                return;
            }

            eventScroll = EditorGUILayout.BeginScrollView(
                eventScroll,
                GUILayout.MinHeight(120f));

            int start = Mathf.Max(0, report.events.Count - 100);

            for (int i = start; i < report.events.Count; i++)
            {
                ExpeditionTelemetryEvent telemetryEvent =
                    report.events[i];

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(
                    FormatDuration(telemetryEvent.elapsedSeconds),
                    GUILayout.Width(65f));

                GUILayout.Label(
                    telemetryEvent.expeditionElapsedSeconds > 0f
                        ? FormatDuration(telemetryEvent.expeditionElapsedSeconds)
                        : "--:--:--",
                    GUILayout.Width(65f));

                GUILayout.Label(
                    telemetryEvent.eventType,
                    GUILayout.MinWidth(145f));

                GUILayout.FlexibleSpace();

                GUILayout.Label(
                    telemetryEvent.value.ToString("N1"),
                    GUILayout.Width(65f));

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (report.events.Count > 100)
            {
                EditorGUILayout.LabelField(
                    "Mostrando los últimos 100 eventos.",
                    EditorStyles.miniLabel);
            }
        }

        private static string BuildCsv(ExpeditionTelemetryReport report)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("SessionId,StartedAtUtc,EndedAtUtc,SessionDurationSeconds,ExpeditionDurationSeconds,HasExpeditionClock,StartScene,EndScene,StartedAutomatically,StoppedAutomatically");
            builder.Append(EscapeCsv(report.sessionId)).Append(",");
            builder.Append(EscapeCsv(report.startedAtUtc)).Append(",");
            builder.Append(EscapeCsv(report.endedAtUtc)).Append(",");
            builder.Append(report.durationSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
            builder.Append(report.expeditionDurationSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
            builder.Append(report.hasExpeditionClock).Append(",");
            builder.Append(EscapeCsv(report.startScene)).Append(",");
            builder.Append(EscapeCsv(report.endScene)).Append(",");
            builder.Append(report.startedAutomatically).Append(",");
            builder.AppendLine(report.stoppedAutomatically.ToString());

            builder.AppendLine();
            builder.AppendLine("Metric,Value");
            AppendMetric(builder, "InitialLevel", report.initialLevel);
            AppendMetric(builder, "FinalLevel", report.finalLevel);
            AppendMetric(builder, "ExperienceGained", report.experienceGained);
            AppendMetric(builder, "LevelUps", report.levelUps);
            AppendMetric(builder, "DamageDealtToEnemies", report.damageDealtToEnemies);
            AppendMetric(builder, "DamageReceived", report.damageReceived);
            AppendMetric(builder, "HealingReceived", report.healingReceived);
            AppendMetric(builder, "EnemiesDefeated", report.enemiesDefeated);

            builder.AppendLine();
            builder.AppendLine("LevelUpOptions");
            builder.AppendLine("OfferId,OptionId,Level,OptionIndex,OptionType,DisplayName,Rarity,SkillDefinition,EffectSummary");
            if (report.levelUpOptions != null)
            {
                foreach (ExpeditionLevelUpOptionRecord option in report.levelUpOptions)
                {
                    builder.Append(EscapeCsv(option.offerId)).Append(",");
                    builder.Append(EscapeCsv(option.optionId)).Append(",");
                    builder.Append(option.level).Append(",");
                    builder.Append(option.optionIndex).Append(",");
                    builder.Append(EscapeCsv(option.optionType)).Append(",");
                    builder.Append(EscapeCsv(option.displayName)).Append(",");
                    builder.Append(EscapeCsv(option.rarity)).Append(",");
                    builder.Append(EscapeCsv(option.skillDefinition)).Append(",");
                    builder.AppendLine(EscapeCsv(option.effectSummary));
                }
            }

            builder.AppendLine();
            builder.AppendLine("LevelUpSelections");
            builder.AppendLine("OfferId,OptionId,Level,OptionType,DisplayName,SelectedAtUtc,Status,EffectType,DefinitionName,TargetSkill,TargetSlot,TargetStat,ModifierType,ConfiguredValue,PreviousValue,NewValue,Succeeded,EffectStatus");
            if (report.levelUpSelections != null)
            {
                foreach (ExpeditionLevelUpSelectionRecord selection in report.levelUpSelections)
                {
                    if (selection.effects == null || selection.effects.Count == 0)
                    {
                        builder.Append(EscapeCsv(selection.offerId)).Append(",");
                        builder.Append(EscapeCsv(selection.optionId)).Append(",");
                        builder.Append(selection.level).Append(",");
                        builder.Append(EscapeCsv(selection.optionType)).Append(",");
                        builder.Append(EscapeCsv(selection.displayName)).Append(",");
                        builder.Append(EscapeCsv(selection.selectedAtUtc)).Append(",");
                        builder.Append(EscapeCsv(selection.status));
                        // Keep the row aligned with the 18-column header.
                        for (int column = 7; column < 18; column++)
                            builder.Append(",");
                        builder.AppendLine();
                        continue;
                    }

                    foreach (ExpeditionLevelUpEffectRecord effect in selection.effects)
                    {
                        builder.Append(EscapeCsv(selection.offerId)).Append(",");
                        builder.Append(EscapeCsv(selection.optionId)).Append(",");
                        builder.Append(selection.level).Append(",");
                        builder.Append(EscapeCsv(selection.optionType)).Append(",");
                        builder.Append(EscapeCsv(selection.displayName)).Append(",");
                        builder.Append(EscapeCsv(selection.selectedAtUtc)).Append(",");
                        builder.Append(EscapeCsv(selection.status)).Append(",");
                        builder.Append(EscapeCsv(effect.effectType)).Append(",");
                        builder.Append(EscapeCsv(effect.definitionName)).Append(",");
                        builder.Append(EscapeCsv(effect.targetSkill)).Append(",");
                        builder.Append(effect.targetSlot).Append(",");
                        builder.Append(EscapeCsv(effect.targetStat)).Append(",");
                        builder.Append(EscapeCsv(effect.modifierType)).Append(",");
                        builder.Append(effect.configuredValue.ToString(CultureInfo.InvariantCulture)).Append(",");
                        builder.Append(effect.previousValue.ToString(CultureInfo.InvariantCulture)).Append(",");
                        builder.Append(effect.newValue.ToString(CultureInfo.InvariantCulture)).Append(",");
                        builder.Append(effect.succeeded).Append(",");
                        builder.AppendLine(EscapeCsv(effect.status));
                    }
                }
            }

            builder.AppendLine();
            builder.AppendLine("SkillStatsChanges");
            builder.AppendLine("SkillDefinitionId,SkillDisplayName,ChangeSource,ModifierName,ModifierType,ConfiguredValue,LevelBefore,LevelAfter,SessionSeconds,ExpeditionSeconds,HasPreviousStats,PreviousFinalDamage,CurrentFinalDamage,PreviousFinalCooldown,CurrentFinalCooldown,PreviousSpawnRadius,CurrentSpawnRadius,PreviousImpactRadius,CurrentImpactRadius,PreviousRange,CurrentRange,PreviousDuration,CurrentDuration,PreviousTickRate,CurrentTickRate,PreviousCount,CurrentCount,PreviousCriticalChance,CurrentCriticalChance,PreviousCriticalDamageBonus,CurrentCriticalDamageBonus,PreviousPenetration,CurrentPenetration,PreviousChain,CurrentChain,PreviousBounce,CurrentBounce,PreviousGrantsExplosion,CurrentGrantsExplosion,PreviousGrantsChaining,CurrentGrantsChaining,PreviousGrantsSplit,CurrentGrantsSplit,PreviousSpawnZoneOnHit,CurrentSpawnZoneOnHit,PreviousSpawnZoneChance,CurrentSpawnZoneChance");
            if (report.skillStatsChanges != null)
            {
                foreach (ExpeditionSkillStatsChangeRecord change in report.skillStatsChanges)
                {
                    if (change == null || change.currentStats == null) continue;
                    ExpeditionSkillStatsSnapshot previous = change.previousStats;
                    ExpeditionSkillStatsSnapshot current = change.currentStats;
                    builder.Append(EscapeCsv(change.skillDefinitionId)).Append(",");
                    builder.Append(EscapeCsv(change.skillDisplayName)).Append(",");
                    builder.Append(EscapeCsv(change.changeSource)).Append(",");
                    builder.Append(EscapeCsv(change.modifierName)).Append(",");
                    builder.Append(EscapeCsv(change.modifierType)).Append(",");
                    builder.Append(change.configuredValue.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(change.levelBefore).Append(",").Append(change.levelAfter).Append(",");
                    builder.Append(change.sessionSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(change.expeditionSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(change.hasPreviousStats).Append(",");
                    AppendSnapshotPair(builder, previous != null ? previous.finalDamage : 0f, current.finalDamage, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null ? previous.finalCooldown : 0f, current.finalCooldown, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null ? previous.finalSpawnRadius : 0f, current.finalSpawnRadius, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null ? previous.finalImpactRadius : 0f, current.finalImpactRadius, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null ? previous.finalRange : 0f, current.finalRange, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null ? previous.finalDuration : 0f, current.finalDuration, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null ? previous.finalTickRate : 0f, current.finalTickRate, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null ? previous.finalCount : 0, current.finalCount, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null ? previous.criticalChance : 0f, current.criticalChance, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null ? previous.criticalDamageBonus : 0f, current.criticalDamageBonus, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null ? previous.penetrationCount : 0, current.penetrationCount, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null ? previous.chainCount : 0, current.chainCount, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null ? previous.bounceCount : 0, current.bounceCount, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null && previous.grantsExplosion, current.grantsExplosion, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null && previous.grantsChaining, current.grantsChaining, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null && previous.grantsSplit, current.grantsSplit, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null && previous.spawnZoneOnHit, current.spawnZoneOnHit, change.hasPreviousStats);
                    AppendSnapshotPair(builder, previous != null ? previous.spawnZoneChance : 0f, current.spawnZoneChance, change.hasPreviousStats, true);
                }
            }

            builder.AppendLine();
            builder.AppendLine("PhaseRecords");
            builder.AppendLine("PhaseId,SourceId,SourceName,Status,HasStarted,HasCompleted,HasDuration,StartedAtExpeditionSeconds,CompletedAtExpeditionSeconds,DurationSeconds,StartedAtSessionSeconds,CompletedAtSessionSeconds,HasObservedDuration,ObservedDurationSeconds,SessionEndExpeditionSeconds,SessionEndSessionSeconds");

            if (report.phaseRecords != null)
            {
                foreach (ExpeditionPhaseRecord phase in report.phaseRecords)
                {
                    if (phase == null)
                        continue;

                    builder.Append(EscapeCsv(phase.phaseId)).Append(",");
                    builder.Append(EscapeCsv(phase.sourceId)).Append(",");
                    builder.Append(EscapeCsv(phase.sourceName)).Append(",");
                    builder.Append(EscapeCsv(phase.status)).Append(",");
                    builder.Append(phase.hasStarted).Append(",");
                    builder.Append(phase.hasCompleted).Append(",");
                    builder.Append(phase.hasDuration).Append(",");
                    builder.Append(phase.startedAtExpeditionSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(phase.completedAtExpeditionSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(phase.durationSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(phase.startedAtSessionSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(phase.completedAtSessionSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(phase.hasObservedDuration).Append(",");
                    builder.Append(phase.observedDurationSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(phase.sessionEndExpeditionSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.AppendLine(phase.sessionEndSessionSeconds.ToString(CultureInfo.InvariantCulture));
                }
            }

            builder.AppendLine();
            builder.AppendLine("EnemySkillContributions");
            builder.AppendLine("EnemyLifeId,EnemyTier,SkillDefinitionId,SkillId,SkillDisplayName,SourceCategory,LastExecutionId,Damage,HitCount,EnemyDefeated");
            if (report.enemySkillContributions != null)
            {
                foreach (ExpeditionEnemySkillContributionRecord contribution
                    in report.enemySkillContributions)
                {
                    if (contribution == null)
                        continue;

                    builder.Append(EscapeCsv(contribution.enemyLifeId)).Append(",");
                    builder.Append(EscapeCsv(contribution.enemyTier)).Append(",");
                    builder.Append(EscapeCsv(contribution.skillDefinitionId)).Append(",");
                    builder.Append(EscapeCsv(contribution.skillId)).Append(",");
                    builder.Append(EscapeCsv(contribution.skillDisplayName)).Append(",");
                    builder.Append(EscapeCsv(contribution.sourceCategory)).Append(",");
                    builder.Append(EscapeCsv(contribution.lastExecutionId)).Append(",");
                    builder.Append(contribution.damage.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(contribution.hitCount).Append(",");
                    builder.AppendLine(contribution.enemyDefeated.ToString());
                }
            }


            builder.AppendLine();
            builder.AppendLine("EnemyLifecycles");
            builder.AppendLine("EnemyLifeId,EnemyVariantId,EnemyDisplayName,EnemyTier,EnemyRoles,SpawnedAtExpeditionSeconds,DiedAtExpeditionSeconds,TTKSeconds,SessionEndExpeditionSeconds,Outcome");
            if (report.enemyLifecycles != null)
            {
                foreach (ExpeditionEnemyLifecycleRecord enemy in report.enemyLifecycles)
                {
                    if (enemy == null)
                        continue;

                    string roles = enemy.enemyRoles != null
                        ? string.Join("|", enemy.enemyRoles.ToArray())
                        : string.Empty;

                    builder.Append(EscapeCsv(enemy.enemyLifeId)).Append(",");
                    builder.Append(EscapeCsv(enemy.enemyVariantId)).Append(",");
                    builder.Append(EscapeCsv(enemy.enemyDisplayName)).Append(",");
                    builder.Append(EscapeCsv(enemy.enemyTier)).Append(",");
                    builder.Append(EscapeCsv(roles)).Append(",");
                    builder.Append(enemy.spawnedAtExpeditionSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(enemy.diedAtExpeditionSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(enemy.ttkSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(enemy.sessionEndExpeditionSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.AppendLine(EscapeCsv(enemy.outcome));
                }
            }

            builder.AppendLine();
            builder.AppendLine("SessionElapsedSeconds,ExpeditionElapsedSeconds,EventType,Value,IntegerValue,Detail");

            if (report.events != null)
            {
                foreach (ExpeditionTelemetryEvent telemetryEvent in report.events)
                {
                    builder.Append(telemetryEvent.elapsedSeconds.ToString(
                        CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(telemetryEvent.expeditionElapsedSeconds.ToString(
                        CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(EscapeCsv(telemetryEvent.eventType)).Append(",");
                    builder.Append(telemetryEvent.value.ToString(
                        CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(telemetryEvent.integerValue.ToString(
                        CultureInfo.InvariantCulture)).Append(",");
                    builder.AppendLine(EscapeCsv(telemetryEvent.detail));
                }
            }

            builder.AppendLine();
            builder.AppendLine("SkillTreeEffectProvenance");
            builder.AppendLine("NodeId,NodeDisplayName,OptionId,OptionDisplayName,EffectIndex,EffectType,DestinationDomain,Target,ModifierType,ConfiguredValue,SessionSeconds,ExpeditionSeconds");
            if (report.skillTreeEffects != null)
            {
                foreach (ExpeditionSkillTreeEffectRecord effect in report.skillTreeEffects)
                {
                    if (effect == null)
                        continue;

                    builder.Append(EscapeCsv(effect.nodeId)).Append(",");
                    builder.Append(EscapeCsv(effect.nodeDisplayName)).Append(",");
                    builder.Append(EscapeCsv(effect.optionId)).Append(",");
                    builder.Append(EscapeCsv(effect.optionDisplayName)).Append(",");
                    builder.Append(effect.effectIndex).Append(",");
                    builder.Append(EscapeCsv(effect.effectType)).Append(",");
                    builder.Append(EscapeCsv(effect.destinationDomain)).Append(",");
                    builder.Append(EscapeCsv(effect.target)).Append(",");
                    builder.Append(EscapeCsv(effect.modifierType)).Append(",");
                    builder.Append(effect.configuredValue.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(effect.sessionSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.AppendLine(effect.expeditionSeconds.ToString(CultureInfo.InvariantCulture));
                }
            }

            builder.AppendLine();
            builder.AppendLine("TelemetryBuckets");
            builder.AppendLine("SessionId,BucketIndex,ExpeditionStartSeconds,ExpeditionEndSeconds,SessionStartSeconds,SessionEndSeconds,DamageDealtTotal,DamageReceivedTotal,HealingReceivedTotal");
            if (report.buckets != null)
            {
                foreach (ExpeditionTelemetryBucket bucket in report.buckets)
                {
                    if (bucket == null) continue;
                    builder.Append(EscapeCsv(bucket.sessionId)).Append(",");
                    builder.Append(bucket.bucketIndex).Append(",");
                    builder.Append(bucket.expeditionStartSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(bucket.expeditionEndSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(bucket.sessionStartSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(bucket.sessionEndSeconds.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(bucket.damageDealtTotal.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.Append(bucket.damageReceivedTotal.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.AppendLine(bucket.healingReceivedTotal.ToString(CultureInfo.InvariantCulture));
                }
            }

            AppendBucketBreakdown(builder, "BucketDamageDealtBySkill", report.buckets,
                bucket => bucket.damageDealtBySkill);
            AppendBucketBreakdown(builder, "BucketDamageDealtBySource", report.buckets,
                bucket => bucket.damageDealtBySource);
            AppendBucketBreakdown(builder, "BucketDamageDealtByEnemyTier", report.buckets,
                bucket => bucket.damageDealtByEnemyTier);
            AppendBucketBreakdown(builder, "BucketDamageReceivedBySource", report.buckets,
                bucket => bucket.damageReceivedBySource);
            AppendBucketBreakdown(builder, "BucketHealingBySource", report.buckets,
                bucket => bucket.healingBySource);

            builder.AppendLine();
            builder.AppendLine("BucketDamageDealtBySkillAndTier");
            builder.AppendLine("BucketIndex,SessionId,SkillDefinitionId,SkillId,SkillDisplayName,EnemyTier,Damage,HitCount");
            if (report.buckets != null)
            {
                foreach (ExpeditionTelemetryBucket bucket in report.buckets)
                {
                    if (bucket == null || bucket.damageDealtBySkillAndTier == null) continue;
                    foreach (ExpeditionTelemetrySkillTierRecord item in bucket.damageDealtBySkillAndTier)
                    {
                        if (item == null) continue;
                        builder.Append(bucket.bucketIndex).Append(",");
                        builder.Append(EscapeCsv(bucket.sessionId)).Append(",");
                        builder.Append(EscapeCsv(item.skillDefinitionId)).Append(",");
                        builder.Append(EscapeCsv(item.skillId)).Append(",");
                        builder.Append(EscapeCsv(item.skillDisplayName)).Append(",");
                        builder.Append(EscapeCsv(item.enemyTier)).Append(",");
                        builder.Append(item.value.ToString(CultureInfo.InvariantCulture)).Append(",");
                        builder.AppendLine(item.hitCount.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }

            return builder.ToString();
        }

        private static void AppendBucketBreakdown(
            StringBuilder builder,
            string sectionName,
            List<ExpeditionTelemetryBucket> buckets,
            Func<ExpeditionTelemetryBucket, List<ExpeditionTelemetryBreakdownRecord>> selector)
        {
            builder.AppendLine();
            builder.AppendLine(sectionName);
            builder.AppendLine("BucketIndex,SessionId,Key,DisplayName,Value,HitCount");
            if (buckets == null || selector == null) return;

            foreach (ExpeditionTelemetryBucket bucket in buckets)
            {
                if (bucket == null) continue;
                List<ExpeditionTelemetryBreakdownRecord> records = selector(bucket);
                if (records == null) continue;
                foreach (ExpeditionTelemetryBreakdownRecord record in records)
                {
                    if (record == null) continue;
                    builder.Append(bucket.bucketIndex).Append(",");
                    builder.Append(EscapeCsv(bucket.sessionId)).Append(",");
                    builder.Append(EscapeCsv(record.key)).Append(",");
                    builder.Append(EscapeCsv(record.displayName)).Append(",");
                    builder.Append(record.value.ToString(CultureInfo.InvariantCulture)).Append(",");
                    builder.AppendLine(record.hitCount.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        private static void AppendMetric(
            StringBuilder builder,
            string name,
            double value)
        {
            builder.Append(EscapeCsv(name)).Append(",");
            builder.AppendLine(value.ToString(CultureInfo.InvariantCulture));
        }

        private static void AppendMetric(
            StringBuilder builder,
            string name,
            int value)
        {
            builder.Append(EscapeCsv(name)).Append(",");
            builder.AppendLine(value.ToString(CultureInfo.InvariantCulture));
        }

        private static void AppendSnapshotPair(
            StringBuilder builder, float previous, float current,
            bool hasPrevious, bool lastPair = false)
        {
            builder.Append(hasPrevious ? previous.ToString(CultureInfo.InvariantCulture) : string.Empty).Append(",");
            builder.Append(current.ToString(CultureInfo.InvariantCulture));
            if (!lastPair) builder.Append(",");
            else builder.AppendLine();
        }

        private static void AppendSnapshotPair(
            StringBuilder builder, int previous, int current,
            bool hasPrevious, bool lastPair = false)
        {
            builder.Append(hasPrevious ? previous.ToString(CultureInfo.InvariantCulture) : string.Empty).Append(",");
            builder.Append(current.ToString(CultureInfo.InvariantCulture));
            if (!lastPair) builder.Append(",");
            else builder.AppendLine();
        }

        private static void AppendSnapshotPair(
            StringBuilder builder, bool previous, bool current,
            bool hasPrevious, bool lastPair = false)
        {
            builder.Append(hasPrevious ? previous.ToString() : string.Empty).Append(",");
            builder.Append(current.ToString());
            if (!lastPair) builder.Append(",");
            else builder.AppendLine();
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private static string FormatDuration(float seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(Math.Max(0f, seconds));

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0:00}:{1:00}:{2:00}",
                (int)time.TotalHours,
                time.Minutes,
                time.Seconds);
        }
    }
}
#endif
