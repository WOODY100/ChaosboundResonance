using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Content.Expeditions.Definitions.Timeline;
using Chaosbound.Content.Expeditions.Runtime.Bosses;
using Chaosbound.Gameplay.Bosses.Integration.Spawn;
using Chaosbound.Gameplay.Bosses.Models;
using Chaosbound.Gameplay.Bosses.Services;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Coordinates Boss Domain runtime behavior
    /// for the current expedition.
    /// </summary>
    public sealed class BossDomainDirector
    {
        private const string BossDomainId =
            "boss";

        private const string BossStartContentId =
            "boss.start";

        private readonly BossSpawnPlanner
            spawnPlanner;

        private readonly BossSpawnRequestTranslator
            spawnRequestTranslator;

        private readonly SpawnRuntime
            spawnRuntime;

        /// <summary>
        /// Executes the Boss Domain for the current
        /// Expedition Runtime tick.
        /// </summary>
        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            IReadOnlyList<TimelineEntry> reachedEntries =
                context.State
                    .Timeline
                    .ReachedEntriesThisTick;

            foreach (TimelineEntry entry in reachedEntries)
            {
                if (entry == null)
                    continue;

                if (entry.TriggerReference == null)
                    continue;

                if (entry.TriggerReference.DomainId !=
                    BossDomainId)
                {
                    continue;
                }

                HandleBossTrigger(
                    context,
                    entry);
            }
        }

        public BossDomainDirector(
            BossSpawnPlanner spawnPlanner,
            BossSpawnRequestTranslator spawnRequestTranslator,
            SpawnRuntime spawnRuntime)
        {
            this.spawnPlanner =
                spawnPlanner
                ?? throw new ArgumentNullException(
                    nameof(spawnPlanner));

            this.spawnRequestTranslator =
                spawnRequestTranslator
                ?? throw new ArgumentNullException(
                    nameof(spawnRequestTranslator));

            this.spawnRuntime =
                spawnRuntime
                ?? throw new ArgumentNullException(
                    nameof(spawnRuntime));
        }

        private void HandleBossTrigger(
            ExpeditionRuntimeContext context,
            TimelineEntry entry)
        {
            if (entry.TriggerReference.ContentId !=
                BossStartContentId)
            {
                throw new InvalidOperationException(
                    $"Unsupported Boss trigger ContentId: " +
                    $"'{entry.TriggerReference.ContentId}'.");
            }

            if (context.State.Boss.State !=
                BossDomainState.Inactive)
            {
                return;
            }

            BossData selectedBoss =
                SelectBoss(
                    context.Config.Bosses);

            context.State.Boss.Start(
                selectedBoss);

            BossSpawnPlan spawnPlan =
                spawnPlanner.Build(
                    selectedBoss);

            SpawnRequest spawnRequest =
                spawnRequestTranslator.Translate(
                    spawnPlan,
                    context.Config.Spawn);

            spawnRuntime.Execute(
                spawnRequest,
                context.Config.Spawn,
                context.References.Runtime,
                context.State);

            context.State.Boss.MarkActive();
        }

        private BossData SelectBoss(
            RuntimeBossesConfig runtimeBossesConfig)
        {
            if (runtimeBossesConfig == null)
            {
                throw new InvalidOperationException(
                    "Boss Domain requires " +
                    "RuntimeBossesConfig.");
            }

            IReadOnlyList<BossData> bosses =
                runtimeBossesConfig.Bosses;

            if (bosses == null ||
                bosses.Count == 0)
            {
                throw new InvalidOperationException(
                    "Boss Domain cannot start because " +
                    "the expedition contains no available Bosses.");
            }

            // Temporary deterministic selection.
            // The V1 Boss selection policy will be defined
            // independently from the Timeline Domain.
            return bosses[0];
        }
    }
}