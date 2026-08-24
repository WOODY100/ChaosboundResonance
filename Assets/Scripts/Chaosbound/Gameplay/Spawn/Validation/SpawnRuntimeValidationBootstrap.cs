using Chaosbound.Gameplay.Spawn.Calculators;
using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Infrastructure;
using Chaosbound.Gameplay.Spawn.Integration;
using Chaosbound.Gameplay.Spawn.Materialization;
using Chaosbound.Gameplay.Spawn.Placement.Contracts;
using Chaosbound.Gameplay.Spawn.Placement.Factories;
using Chaosbound.Gameplay.Spawn.Placement.Resolvers;
using Chaosbound.Gameplay.Spawn.Placement.Strategies;
using Chaosbound.Gameplay.Spawn.Placement.Validation;
using Chaosbound.Gameplay.Spawn.Reference.Factories;
using Chaosbound.Gameplay.Spawn.Reference.Providers;
using Chaosbound.Gameplay.Spawn.Reference.Resolvers;
using Chaosbound.Gameplay.Spawn.Scheduling;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Validation
{
    /// <summary>
    /// Builds the dependency graph required to execute
    /// the Spawn Runtime validation pipeline.
    /// </summary>
    public sealed class SpawnRuntimeValidationBootstrap
    {
        /// <summary>
        /// Builds a fully initialized SpawnJobExecutor.
        /// </summary>
        public SpawnJobExecutor Build()
        {
            SpawnBatchCalculator batchCalculator =
                BuildBatchCalculator();

            SpawnTaskEntryFactory taskEntryFactory =
                BuildTaskEntryFactory();

            SpawnTaskFactory taskFactory =
                BuildTaskFactory();

            SpawnScheduler scheduler =
                BuildScheduler(
                    batchCalculator,
                    taskEntryFactory,
                    taskFactory);

            ScheduledSpawnTaskExecutor taskExecutor =
                BuildTaskExecutor();

            SpawnJobRuntimeStateFactory runtimeStateFactory =
                BuildRuntimeStateFactory();

            return new SpawnJobExecutor(
                scheduler,
                runtimeStateFactory,
                taskExecutor,

                BuildPlacementIntentFactory(),

                BuildReferenceContextFactory(),
                BuildReferenceResolver(),

                BuildPlacementContextFactory(),
                BuildPlacementResolver(),

                BuildResolvedSpawnTaskFactory());
        }

        private SpawnBatchCalculator BuildBatchCalculator()
        {
            return new SpawnBatchCalculator();
        }

        private SpawnTaskEntryFactory BuildTaskEntryFactory()
        {
            return new SpawnTaskEntryFactory();
        }

        private SpawnTaskFactory BuildTaskFactory()
        {
            return new SpawnTaskFactory();
        }

        private SpawnScheduler BuildScheduler(
            SpawnBatchCalculator batchCalculator,
            SpawnTaskEntryFactory taskEntryFactory,
            SpawnTaskFactory taskFactory)
        {
            SpawnSchedulingPolicyResolver resolver =
                BuildSchedulingPolicyResolver(
                    batchCalculator,
                    taskEntryFactory,
                    taskFactory);

            return new SpawnScheduler(resolver);
        }

        private SpawnSchedulingPolicyResolver
            BuildSchedulingPolicyResolver(
                SpawnBatchCalculator batchCalculator,
                SpawnTaskEntryFactory taskEntryFactory,
                SpawnTaskFactory taskFactory)
        {
            return new SpawnSchedulingPolicyResolver(
                batchCalculator,
                taskEntryFactory,
                taskFactory);
        }

        private ISpawnInstantiationService
            BuildInstantiationService()
        {
            return new PoolManagerSpawnInstantiationService();
        }

        private EnemyMaterializer
            BuildEnemyMaterializer()
        {
            return new EnemyMaterializer(
                BuildInstantiationService());
        }

        private BossMaterializer
            BuildBossMaterializer()
        {
            return new BossMaterializer(
                BuildInstantiationService());
        }

        private MiniBossMaterializer
            BuildMiniBossMaterializer()
        {
            return new MiniBossMaterializer(
                BuildInstantiationService());
        }

        private SpawnMaterializerResolver
            BuildMaterializerResolver()
        {
            return new SpawnMaterializerResolver(
                BuildEnemyMaterializer(),
                BuildBossMaterializer(),
                BuildMiniBossMaterializer(),
                BuildExitPortalMaterializer());
        }

        private SpawnExecutionContextFactory
            BuildExecutionContextFactory()
        {
            return new SpawnExecutionContextFactory();
        }

        private ScheduledSpawnTaskExecutor
            BuildTaskExecutor()
        {
            return new ScheduledSpawnTaskExecutor(
                BuildExecutionContextFactory(),
                BuildMaterializerResolver());
        }

        private SpawnJobRuntimeStateFactory
            BuildRuntimeStateFactory()
        {
            return new SpawnJobRuntimeStateFactory();
        }

        private PlacementIntentFactory
            BuildPlacementIntentFactory()
        {
            return new PlacementIntentFactory();
        }

        private SpawnReferenceContextFactory
            BuildReferenceContextFactory()
        {
            return new SpawnReferenceContextFactory();
        }

        private PlayerReferenceProvider
            BuildPlayerReferenceProvider()
        {
            return new PlayerReferenceProvider();
        }

        private ExitPortalMaterializer
            BuildExitPortalMaterializer()
        {
            return new ExitPortalMaterializer(
                BuildInstantiationService());
        }

        private CompletionOriginReferenceProvider
            BuildCompletionOriginReferenceProvider()
        {
            return new CompletionOriginReferenceProvider();
        }

        private SpawnReferenceResolver
            BuildReferenceResolver()
        {
            return new SpawnReferenceResolver(
                BuildPlayerReferenceProvider(),
                BuildCompletionOriginReferenceProvider());
        }

        private PlacementContextFactory
            BuildPlacementContextFactory()
        {
            return new PlacementContextFactory();
        }

        private AroundPlayerPlacementStrategy
            BuildAroundPlayerPlacementStrategy()
        {
            return new AroundPlayerPlacementStrategy();
        }

        private NearReferencePlacementStrategy
            BuildNearReferencePlacementStrategy()
        {
            return new NearReferencePlacementStrategy();
        }

        private IPlacementFootprintResolver
            BuildPlacementFootprintResolver()
        {
            return new PlacementFootprintResolver();
        }

        private PlacementValidator
            BuildPlacementValidator()
        {
            LayerMask obstacleLayer =
                LayerMask.GetMask("Obstacle");

            return new PlacementValidator(
                BuildPlacementFootprintResolver(),
                obstacleLayer);
        }

        private PlacementResolver
            BuildPlacementResolver()
        {
            return new PlacementResolver(
                BuildAroundPlayerPlacementStrategy(),
                BuildNearReferencePlacementStrategy(),
                BuildPlacementValidator());
        }

        private ResolvedSpawnTaskFactory
            BuildResolvedSpawnTaskFactory()
        {
            return new ResolvedSpawnTaskFactory();
        }
    }
}