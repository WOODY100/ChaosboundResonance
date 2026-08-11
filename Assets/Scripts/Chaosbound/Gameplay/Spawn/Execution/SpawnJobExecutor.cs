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
        private readonly EnemyScheduler scheduler;

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
            EnemyScheduler scheduler,
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
        public void Execute(
            EnemySchedulingContext schedulingContext)
        {
            if (schedulingContext == null)
                throw new ArgumentNullException(nameof(schedulingContext));

            IReadOnlyList<ScheduledSpawnTask> tasks =
                scheduler.Schedule(
                    schedulingContext);

            Debug.Log(
                $"[SpawnJobExecutor] Scheduled={tasks.Count}");

            SpawnJobRuntimeState runtimeState =
                runtimeStateFactory.Create(
                    schedulingContext.Job,
                    schedulingContext.ExpeditionRuntime);

            foreach (ScheduledSpawnTask task in tasks)
            {
                PlacementIntent placementIntent =
                placementIntentFactory.Create(
                    task,
                    schedulingContext.SpawnConfig);

                SpawnReferenceContext referenceContext =
                    referenceContextFactory.Create(
                        schedulingContext.SpawnConfig,
                        schedulingContext.References);

                SpawnReferenceResult reference =
                    referenceResolver.Resolve(
                        referenceContext);

                if (!reference.IsSuccess)
                {
                    continue;
                }

                PlacementContext placementContext =
                    placementContextFactory.Create(
                        placementIntent,
                        reference.Reference);

                PlacementResolution placement =
                    placementResolver.Resolve(
                        placementContext);

                ResolvedSpawnTask resolvedTask =
                    resolvedTaskFactory.Create(
                        task,
                        placement);

                taskExecutor.Execute(
                    resolvedTask,
                    runtimeState);
            }
        }
    }
}