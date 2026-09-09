using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Materialization;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.Placement.Factories;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using Chaosbound.Gameplay.Spawn.Placement.Resolvers;
using Chaosbound.Gameplay.Spawn.Reference.Factories;
using Chaosbound.Gameplay.Spawn.Reference.Models;
using Chaosbound.Gameplay.Spawn.Reference.Resolvers;
using Chaosbound.Gameplay.Spawn.Runtime;
using Chaosbound.Gameplay.Spawn.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Execution
{
    /// <summary>
    /// Executes scheduled SpawnJobs.
    /// </summary>
    public sealed class SpawnJobExecutor
    {
        private readonly SpawnScheduler scheduler;

        private readonly SpawnJobRuntimeStateFactory
            runtimeStateFactory;

        private readonly ScheduledSpawnTaskExecutor
            taskExecutor;

        private readonly PlacementContextFactory
            placementContextFactory;

        private readonly PlacementResolver
            placementResolver;

        private readonly ResolvedSpawnTaskFactory
            resolvedTaskFactory;

        private readonly SpawnReferenceContextFactory
            referenceContextFactory;

        private readonly SpawnReferenceResolver
            referenceResolver;

        private readonly PlacementIntentFactory
            placementIntentFactory;

        public SpawnJobExecutor(
            SpawnScheduler scheduler,
            SpawnJobRuntimeStateFactory runtimeStateFactory,
            ScheduledSpawnTaskExecutor taskExecutor,
            PlacementIntentFactory placementIntentFactory,
            SpawnReferenceContextFactory referenceContextFactory,
            SpawnReferenceResolver referenceResolver,
            PlacementContextFactory placementContextFactory,
            PlacementResolver placementResolver,
            ResolvedSpawnTaskFactory resolvedTaskFactory)
        {
            this.scheduler =
                scheduler
                ?? throw new ArgumentNullException(nameof(scheduler));

            this.runtimeStateFactory =
                runtimeStateFactory
                ?? throw new ArgumentNullException(nameof(runtimeStateFactory));

            this.taskExecutor =
                taskExecutor
                ?? throw new ArgumentNullException(nameof(taskExecutor));

            this.placementIntentFactory =
                placementIntentFactory
                ?? throw new ArgumentNullException(nameof(placementIntentFactory));

            this.referenceContextFactory =
                referenceContextFactory
                ?? throw new ArgumentNullException(nameof(referenceContextFactory));

            this.referenceResolver =
                referenceResolver
                ?? throw new ArgumentNullException(nameof(referenceResolver));

            this.placementContextFactory =
                placementContextFactory
                ?? throw new ArgumentNullException(nameof(placementContextFactory));

            this.placementResolver =
                placementResolver
                ?? throw new ArgumentNullException(nameof(placementResolver));

            this.resolvedTaskFactory =
                resolvedTaskFactory
                ?? throw new ArgumentNullException(nameof(resolvedTaskFactory));
        }

        /// <summary>
        /// Executes the supplied scheduling context.
        /// </summary>
        public IReadOnlyList<GameObject> Execute(
            SpawnSchedulingContext schedulingContext,
            RuntimeSpawnConfig spawnConfig,
            RuntimeReferencesConfig references,
            ExpeditionRuntimeState expeditionRuntime,
            SpawnSpatialOrigin? requestSpatialOrigin)
        {
            if (schedulingContext == null)
                throw new ArgumentNullException(nameof(schedulingContext));

            if (spawnConfig == null)
                throw new ArgumentNullException(nameof(spawnConfig));

            if (references == null)
                throw new ArgumentNullException(nameof(references));

            if (expeditionRuntime == null)
                throw new ArgumentNullException(nameof(expeditionRuntime));

            List<GameObject> materializedObjects =
                new List<GameObject>();

            IReadOnlyList<ScheduledSpawnTask> tasks =
                scheduler.Schedule(
                    schedulingContext);

            SpawnJobRuntimeState runtimeState =
                runtimeStateFactory.Create(
                    schedulingContext.Job,
                    expeditionRuntime);

            foreach (ScheduledSpawnTask task in tasks)
            {
                PlacementIntent placementIntent =
                    placementIntentFactory.Create(
                        task,
                        spawnConfig);

                SpawnReferenceContext referenceContext =
                    referenceContextFactory.Create(
                        spawnConfig,
                        references,
                        expeditionRuntime,
                        requestSpatialOrigin);

                SpawnReferenceResult reference =
                    referenceResolver.Resolve(
                        referenceContext);

                if (!reference.IsSuccess)
                {
                    continue;
                }

                SpawnSpatialOrigin spatialOrigin;

                if (reference.SpatialOrigin.HasValue)
                {
                    spatialOrigin =
                        reference.SpatialOrigin.Value;
                }
                else
                {
                    if (reference.Reference == null)
                    {
                        continue;
                    }

                    spatialOrigin =
                        new SpawnSpatialOrigin(
                            reference.Reference.position);
                }

                PlacementContext placementContext =
                    placementContextFactory.Create(
                        placementIntent,
                        spatialOrigin);

                PlacementResolution placement =
                    placementResolver.Resolve(
                        placementContext,
                        spawnConfig.SpawnConstraints);

                if (!placement.IsSuccess)
                {
                    continue;
                }

                ResolvedSpawnTask resolvedTask =
                    resolvedTaskFactory.Create(
                        task,
                        placement);

                GameObject materializedObject =
                    taskExecutor.Execute(
                        resolvedTask,
                        runtimeState);

                if (materializedObject != null)
                {
                    materializedObjects.Add(
                        materializedObject);
                }
            }

            return materializedObjects;
        }
    }
}