using Chaosbound.Gameplay.Bosses;
using Chaosbound.Content.Enemy.MiniBosses;
using Chaosbound.Content.Expeditions.Definitions.Timeline;
using Chaosbound.Content.Expeditions.Runtime.MiniBosses;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.MiniBosses.Integration.Spawn;
using Chaosbound.Gameplay.MiniBosses.Models;
using Chaosbound.Gameplay.MiniBosses.Services;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.MiniBosses
{
    /// <summary>
    /// Coordinates MiniBoss Domain runtime behavior
    /// for the current expedition.
    /// </summary>
    public sealed class MiniBossDomainDirector
    {
        private const string MiniBossDomainId =
            "miniboss";

        private const string MiniBossStartContentId =
            "miniboss.start";

        private readonly MiniBossSpawnPlanner
            spawnPlanner;

        private readonly MiniBossSpawnRequestTranslator
            spawnRequestTranslator;

        private readonly SpawnRuntime
            spawnRuntime;

        private readonly BossEncounterOrchestrator
            encounterOrchestrator;

        /// <summary>
        /// Executes the MiniBoss Domain for the current
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
                    MiniBossDomainId)
                {
                    continue;
                }

                HandleMiniBossTrigger(
                    context,
                    entry);
            }
        }

        public MiniBossDomainDirector(
            MiniBossSpawnPlanner spawnPlanner,
            MiniBossSpawnRequestTranslator spawnRequestTranslator,
            SpawnRuntime spawnRuntime)
            : this(spawnPlanner, spawnRequestTranslator, spawnRuntime,
                new BossEncounterOrchestrator(spawnRuntime))
        {
        }

        public MiniBossDomainDirector(
            MiniBossSpawnPlanner spawnPlanner,
            MiniBossSpawnRequestTranslator spawnRequestTranslator,
            SpawnRuntime spawnRuntime,
            BossEncounterOrchestrator encounterOrchestrator)
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

            this.encounterOrchestrator =
                encounterOrchestrator
                ?? throw new ArgumentNullException(
                    nameof(encounterOrchestrator));
        }

        private void HandleMiniBossTrigger(
            ExpeditionRuntimeContext context,
            TimelineEntry entry)
        {
            if (entry.TriggerReference.ContentId !=
                MiniBossStartContentId)
            {
                throw new InvalidOperationException(
                    $"Unsupported MiniBoss trigger ContentId: " +
                    $"'{entry.TriggerReference.ContentId}'.");
            }

            if (context.State.MiniBoss.State !=
                MiniBossDomainState.Inactive)
            {
                return;
            }

            MiniBossData selectedMiniBoss =
                SelectMiniBoss(
                    context.Config.MiniBosses);

            context.State.MiniBoss.Start(
                selectedMiniBoss);

            MiniBossSpawnPlan spawnPlan =
                spawnPlanner.Build(
                    selectedMiniBoss);

            SpawnRequest spawnRequest =
                spawnRequestTranslator.Translate(
                    spawnPlan);

            IReadOnlyList<UnityEngine.GameObject> spawnedObjects =
                spawnRuntime.Execute(
                    spawnRequest,
                    context.Config.Spawn,
                    context.References.Runtime,
                    context.State);

            if (!encounterOrchestrator.PrepareAndStart(
                spawnedObjects,
                out string preparationError))
            {
                context.State.MiniBoss.Fail();
                UnityEngine.Debug.LogError(
                    "MiniBoss encounter preparation failed: " + preparationError);
                return;
            }

            context.State.MiniBoss.MarkActive();
        }

        private MiniBossData SelectMiniBoss(
            RuntimeMiniBossesConfig runtimeMiniBossesConfig)
        {
            if (runtimeMiniBossesConfig == null)
            {
                throw new InvalidOperationException(
                    "MiniBoss Domain requires " +
                    "RuntimeMiniBossesConfig.");
            }

            IReadOnlyList<MiniBossData> miniBosses =
                runtimeMiniBossesConfig.MiniBosses;

            if (miniBosses == null ||
                miniBosses.Count == 0)
            {
                throw new InvalidOperationException(
                    "MiniBoss Domain cannot start because " +
                    "the expedition contains no available MiniBosses.");
            }

            // Temporary deterministic selection.
            // The V1 MiniBoss selection policy will be defined
            // independently from the Timeline Domain.
            return miniBosses[0];
        }
    }
}