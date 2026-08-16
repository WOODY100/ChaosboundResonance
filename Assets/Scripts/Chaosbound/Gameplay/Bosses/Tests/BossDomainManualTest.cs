using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Content.Expeditions.Definitions.Timeline;
using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Bosses.Integration.Spawn;
using Chaosbound.Gameplay.Bosses.Services;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Factories;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Spawn.Bootstrap;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Bosses.Tests
{
    public sealed class BossDomainManualTest :
        MonoBehaviour
    {
        [ContextMenu("Run Boss Domain Test")]
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

            if (config.Bosses == null)
            {
                throw new InvalidOperationException(
                    "RuntimeBossesConfig is missing.");
            }

            if (config.Bosses.Bosses == null ||
                config.Bosses.Bosses.Count == 0)
            {
                throw new InvalidOperationException(
                    "No Bosses are configured for the expedition.");
            }

            Debug.Log(
                "[Boss Domain Test] " +
                $"Configured Boss Count: " +
                $"{config.Bosses.Bosses.Count}");

            for (int i = 0;
                 i < config.Bosses.Bosses.Count;
                 i++)
            {
                BossData boss =
                    config.Bosses.Bosses[i];

                if (boss == null)
                {
                    throw new InvalidOperationException(
                        $"Boss at index {i} is null.");
                }

                Debug.Log(
                    "[Boss Domain Test] " +
                    $"[{i}] " +
                    $"BossId={boss.Id} | " +
                    $"Name={boss.DisplayName} | " +
                    $"Prefab={boss.SpawnPrefab}");
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

            BossSpawnRequestTranslator
                spawnRequestTranslator =
                    new BossSpawnRequestTranslator(
                        new SpawnRequestFactory(),
                        entryFactory);

            BossSpawnPlanner spawnPlanner =
                new BossSpawnPlanner();

            BossStage bossStage =
                new BossStage(
                    new BossDomainDirector(
                        spawnPlanner,
                        spawnRequestTranslator,
                        spawnRuntime));

            // -------------------------------------------------
            // TEST 1
            // No Timeline entry reached.
            // -------------------------------------------------

            bossStage.Execute(context);

            if (context.State.Boss.State !=
                BossDomainState.Inactive)
            {
                throw new Exception(
                    "Boss Domain did not enter Inactive state " +
                    "after successful materialization.");
            }

            if (context.State.Boss.SelectedBoss != null)
            {
                throw new Exception(
                    "No Boss should be selected before the " +
                    "Boss Timeline trigger is reached.");
            }

            Debug.Log(
                "[Boss Domain Test] " +
                "PASS 1: Domain remains Inactive without trigger.");

            // -------------------------------------------------
            // TEST 2
            // Simulate Timeline reaching boss.start.
            // -------------------------------------------------

            TimelineEntry bossEntry =
                FindBossTimelineEntry(
                    config);

            if (bossEntry == null)
            {
                throw new InvalidOperationException(
                    "Could not find a Boss Timeline entry.");
            }

            List<TimelineEntry> reachedEntries =
                new List<TimelineEntry>
                {
                    bossEntry
                };

            context.State.Timeline.SetEvaluation(
                reachedEntries,
                false);

            bossStage.Execute(context);


            // -------------------------------------------------
            // Validate Boss Runtime State.
            // -------------------------------------------------

            if (context.State.Boss.State !=
                BossDomainState.Active)
            {
                throw new Exception(
                    "Boss Domain did not enter Active state.");
            }

            if (context.State.Boss.SelectedBoss == null)
            {
                throw new Exception(
                    "Boss Domain did not select a Boss.");
            }

            Debug.Log(
                "[Boss Domain Test] " +
                $"PASS 2: Boss materialized and became Active. " +
                $"BossId={context.State.Boss.SelectedBoss.Id} | " +
                $"Name={context.State.Boss.SelectedBoss.DisplayName}");

            Debug.Log(
                "[Boss Domain Test] " +
                "ALL TESTS PASSED.");


            // -------------------------------------------------
            // TEST 3
            // Boss dies and Boss Runtime Lifecycle completes
            // the Boss Domain.
            // -------------------------------------------------

            BossRuntimeContext bossRuntimeContext =
                FindMaterializedBoss(
                    context.State.Boss.SelectedBoss);

            if (bossRuntimeContext == null)
            {
                throw new InvalidOperationException(
                    "Could not find the materialized Boss Runtime Context.");
            }

            BossHealth bossHealth =
                bossRuntimeContext.GetComponent<BossHealth>();

            if (bossHealth == null)
            {
                throw new InvalidOperationException(
                    "Materialized Boss is missing BossHealth.");
            }

            BossRuntimeLifecycle bossLifecycle =
                bossRuntimeContext.GetComponent<BossRuntimeLifecycle>();

            if (bossLifecycle == null)
            {
                throw new InvalidOperationException(
                    "Materialized Boss is missing BossRuntimeLifecycle.");
            }

            if (bossHealth.IsDead)
            {
                throw new InvalidOperationException(
                    "Boss is already dead before lifecycle death test.");
            }

            bossHealth.TakeDamage(
                new DamageData
                {
                    amount = bossHealth.CurrentHealth,
                    isCrit = false
                });

            if (!bossHealth.IsDead)
            {
                throw new Exception(
                    "BossHealth did not enter the Dead state.");
            }

            if (context.State.Boss.State !=
                BossDomainState.Completed)
            {
                throw new Exception(
                    "Boss Domain did not enter Completed state " +
                    "after Boss death.");
            }

            Debug.Log(
                "[Boss Domain Test] " +
                "PASS 3: Boss death propagated through " +
                "BossRuntimeLifecycle and completed the Boss Domain.");

            Debug.Log(
                "[Boss Domain Test] " +
                "ALL TESTS PASSED.");
        }

        private static TimelineEntry FindBossTimelineEntry(
            RuntimeExpeditionConfig config)
        {
            if (config.Timeline == null)
                throw new InvalidOperationException(
                    "RuntimeTimelineConfig is missing.");

            if (config.Timeline.Agenda == null)
                throw new InvalidOperationException(
                    "Timeline Agenda is missing.");

            IReadOnlyList<TimelineEntry> entries =
                config.Timeline.Agenda.Entries;

            foreach (TimelineEntry entry in entries)
            {
                if (entry == null)
                    continue;

                if (entry.TriggerReference == null)
                    continue;

                if (entry.TriggerReference.DomainId !=
                    "boss")
                {
                    continue;
                }

                if (entry.TriggerReference.ContentId !=
                    "boss.start")
                {
                    continue;
                }

                return entry;
            }

            return null;
        }

        private static BossRuntimeContext FindMaterializedBoss(
            BossData expectedBoss)
        {
            BossRuntimeContext[] contexts =
                FindObjectsByType<BossRuntimeContext>(
                    FindObjectsSortMode.None);

            foreach (BossRuntimeContext runtimeContext in contexts)
            {
                if (runtimeContext == null)
                    continue;

                if (!runtimeContext.IsInitialized)
                    continue;

                if (runtimeContext.Boss != expectedBoss)
                    continue;

                return runtimeContext;
            }

            return null;
        }
    }
}