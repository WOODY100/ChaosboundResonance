using Chaosbound.Content.Enemy.MiniBosses;
using Chaosbound.Content.Expeditions.Definitions.Timeline;
using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Factories;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.MiniBosses.Integration.Spawn;
using Chaosbound.Gameplay.MiniBosses.Services;
using Chaosbound.Gameplay.Spawn.Bootstrap;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.MiniBosses.Tests
{
    public sealed class MiniBossDomainManualTest :
        MonoBehaviour
    {
        [ContextMenu("Run MiniBoss Domain Test")]
        private void RunTest()
        {
            BootstrapContext bootstrap =
                BootstrapContext.Current;

            if (bootstrap == null)
            {
                throw new InvalidOperationException(
                    "BootstrapContext is not available.");
            }

            RunSession runSession =
                bootstrap.RunSession;

            if (runSession == null)
            {
                throw new InvalidOperationException(
                    "RunSession is not available.");
            }

            RuntimeExpeditionConfig config =
                runSession.CurrentRun;

            if (config == null)
            {
                throw new InvalidOperationException(
                    "No RuntimeExpeditionConfig is available.");
            }

            if (config.MiniBosses == null)
            {
                throw new InvalidOperationException(
                    "RuntimeMiniBossesConfig is missing.");
            }

            if (config.MiniBosses.MiniBosses == null ||
                config.MiniBosses.MiniBosses.Count == 0)
            {
                throw new InvalidOperationException(
                    "No MiniBosses are configured for the expedition.");
            }

            Debug.Log(
                "[MiniBoss Domain Test] " +
                "Configured MiniBoss Count: " +
                $"{config.MiniBosses.MiniBosses.Count}");

            for (int i = 0;
                 i < config.MiniBosses.MiniBosses.Count;
                 i++)
            {
                MiniBossData miniBoss =
                    config.MiniBosses.MiniBosses[i];

                if (miniBoss == null)
                {
                    throw new InvalidOperationException(
                        $"MiniBoss at index {i} is null.");
                }

                Debug.Log(
                    "[MiniBoss Domain Test] " +
                    $"[{i}] " +
                    $"MiniBossId={miniBoss.Id} | " +
                    $"Name={miniBoss.DisplayName} | " +
                    $"Prefab={miniBoss.SpawnPrefab}");
            }

            ExpeditionRuntimeState state =
                new ExpeditionRuntimeState();

            ExpeditionRuntimeContextFactory contextFactory =
                new ExpeditionRuntimeContextFactory(
                    config,
                    state);

            ExpeditionRuntimeContext context =
                contextFactory.Create();

            SpawnRuntime spawnRuntime =
                new SpawnRuntimeBootstrap()
                    .Build();

            SpawnRequestEntryFactory entryFactory =
                new SpawnRequestEntryFactory(
                    new MaterializableReferenceFactory());

            MiniBossSpawnRequestTranslator
                spawnRequestTranslator =
                    new MiniBossSpawnRequestTranslator(
                        new SpawnRequestFactory(),
                        entryFactory);

            MiniBossSpawnPlanner spawnPlanner =
                new MiniBossSpawnPlanner(
                    new MiniBossSpawnPlanBuilder());

            MiniBossStage miniBossStage =
                new MiniBossStage(
                    new MiniBossDomainDirector(
                        spawnPlanner,
                        spawnRequestTranslator,
                        spawnRuntime));

            // -------------------------------------------------
            // TEST 1
            // No Timeline entry reached.
            // -------------------------------------------------

            miniBossStage.Execute(context);

            if (context.State.MiniBoss.State !=
                MiniBossDomainState.Inactive)
            {
                throw new Exception(
                    "MiniBoss Domain should remain Inactive " +
                    "when no Timeline entry has been reached.");
            }

            if (context.State.MiniBoss.SelectedMiniBoss != null)
            {
                throw new Exception(
                    "No MiniBoss should be selected before the " +
                    "MiniBoss Timeline trigger is reached.");
            }

            Debug.Log(
                "[MiniBoss Domain Test] " +
                "PASS 1: Domain remains Inactive without trigger.");

            // -------------------------------------------------
            // TEST 2
            // Simulate Timeline reaching miniboss.start.
            // -------------------------------------------------

            TimelineEntry miniBossEntry =
                FindMiniBossTimelineEntry(
                    config);

            if (miniBossEntry == null)
            {
                throw new InvalidOperationException(
                    "Could not find a MiniBoss Timeline entry.");
            }

            List<TimelineEntry> reachedEntries =
                new List<TimelineEntry>
                {
                    miniBossEntry
                };

            context.State.Timeline.SetEvaluation(
                reachedEntries,
                false);

            miniBossStage.Execute(context);

            // -------------------------------------------------
            // Validate MiniBoss Runtime State.
            // -------------------------------------------------

            if (context.State.MiniBoss.State !=
                MiniBossDomainState.Starting)
            {
                throw new Exception(
                    "MiniBoss Domain did not enter Starting state.");
            }

            if (context.State.MiniBoss.SelectedMiniBoss == null)
            {
                throw new Exception(
                    "MiniBoss Domain did not select a MiniBoss.");
            }

            Debug.Log(
                "[MiniBoss Domain Test] " +
                $"PASS 2: MiniBoss selected. " +
                $"MiniBossId={context.State.MiniBoss.SelectedMiniBoss.Id} | " +
                $"Name={context.State.MiniBoss.SelectedMiniBoss.DisplayName}");

            Debug.Log(
                "[MiniBoss Domain Test] " +
                "ALL TESTS PASSED.");
        }

        private static TimelineEntry FindMiniBossTimelineEntry(
            RuntimeExpeditionConfig config)
        {
            if (config.Timeline == null)
            {
                throw new InvalidOperationException(
                    "RuntimeTimelineConfig is missing.");
            }

            if (config.Timeline.Agenda == null)
            {
                throw new InvalidOperationException(
                    "Timeline Agenda is missing.");
            }

            IReadOnlyList<TimelineEntry> entries =
                config.Timeline.Agenda.Entries;

            foreach (TimelineEntry entry in entries)
            {
                if (entry == null)
                    continue;

                if (entry.TriggerReference == null)
                    continue;

                if (entry.TriggerReference.DomainId !=
                    "miniboss")
                {
                    continue;
                }

                if (entry.TriggerReference.ContentId !=
                    "miniboss.start")
                {
                    continue;
                }

                return entry;
            }

            return null;
        }
    }
}