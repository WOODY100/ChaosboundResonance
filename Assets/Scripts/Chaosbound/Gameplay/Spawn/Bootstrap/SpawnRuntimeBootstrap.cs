using Chaosbound.Gameplay.Spawn.Calculators;
using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Infrastructure;
using Chaosbound.Gameplay.Spawn.Integration;
using Chaosbound.Gameplay.Spawn.Materialization;
using Chaosbound.Gameplay.Spawn.Placement.Factories;
using Chaosbound.Gameplay.Spawn.Placement.Resolvers;
using Chaosbound.Gameplay.Spawn.Placement.Strategies;
using Chaosbound.Gameplay.Spawn.Reference.Factories;
using Chaosbound.Gameplay.Spawn.Reference.Providers;
using Chaosbound.Gameplay.Spawn.Reference.Resolvers;
using Chaosbound.Gameplay.Spawn.Runtime;
using Chaosbound.Gameplay.Spawn.Scheduling;
using Chaosbound.Gameplay.Spawn.Services;

namespace Chaosbound.Gameplay.Spawn.Bootstrap
{
    /// <summary>
    /// Builds the production dependency graph of the
    /// Spawn Runtime.
    /// </summary>
    public sealed class SpawnRuntimeBootstrap
    {
        public SpawnRuntime Build()
        {
            SpawnJobExecutor jobExecutor =
                BuildSpawnJobExecutor();

            SpawnExecutionPlanExecutor
                executionPlanExecutor =
                    BuildSpawnExecutionPlanExecutor(
                        jobExecutor);

            SpawnExecutor executor =
                BuildSpawnExecutor();

            return new SpawnRuntime(
                executor,
                executionPlanExecutor);
        }

        private SpawnJobExecutor
            BuildSpawnJobExecutor()
        {
            SpawnBatchCalculator batchCalculator =
                BuildBatchCalculator();

            SpawnTaskEntryFactory taskEntryFactory =
                BuildSpawnTaskEntryFactory();

            SpawnTaskFactory taskFactory =
                BuildSpawnTaskFactory();

            SpawnScheduler scheduler =
                BuildSpawnScheduler(
                    batchCalculator,
                    taskEntryFactory,
                    taskFactory);

            ScheduledSpawnTaskExecutor taskExecutor =
                BuildScheduledSpawnTaskExecutor();

            SpawnJobRuntimeStateFactory runtimeStateFactory =
                BuildSpawnJobRuntimeStateFactory();

            return new SpawnJobExecutor(
                scheduler,
                runtimeStateFactory,
                taskExecutor,

                BuildPlacementIntentFactory(),

                BuildSpawnReferenceContextFactory(),
                BuildSpawnReferenceResolver(),

                BuildPlacementContextFactory(),
                BuildPlacementResolver(),

                BuildResolvedSpawnTaskFactory());
        }

        private SpawnExecutionPlanExecutor
            BuildSpawnExecutionPlanExecutor(
        SpawnJobExecutor jobExecutor)
        {
            return new SpawnExecutionPlanExecutor(
                BuildSpawnJobFactory(),
                BuildSpawnSchedulingContextFactory(),
                jobExecutor);
        }

        private SpawnExecutor
            BuildSpawnExecutor()
        {
            return new SpawnExecutor();
        }

        private SpawnJobFactory
            BuildSpawnJobFactory()
        {
            return new SpawnJobFactory();
        }

        private SpawnSchedulingContextFactory
            BuildSpawnSchedulingContextFactory()
        {
            return new SpawnSchedulingContextFactory();
        }

        private SpawnBatchCalculator
            BuildBatchCalculator()
        {
            return new SpawnBatchCalculator();
        }

        private SpawnTaskEntryFactory
            BuildSpawnTaskEntryFactory()
        {
            return new SpawnTaskEntryFactory();
        }

        private SpawnTaskFactory
            BuildSpawnTaskFactory()
        {
            return new SpawnTaskFactory();
        }

        private SpawnScheduler
            BuildSpawnScheduler(
                SpawnBatchCalculator batchCalculator,
                SpawnTaskEntryFactory taskEntryFactory,
                SpawnTaskFactory taskFactory)
        {
            SpawnSchedulingPolicyResolver resolver =
                BuildSchedulingPolicyResolver(
                    batchCalculator,
                    taskEntryFactory,
                    taskFactory);

            return new SpawnScheduler(
                resolver);
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

        private SpawnJobRuntimeStateFactory
            BuildSpawnJobRuntimeStateFactory()
        {
            return new SpawnJobRuntimeStateFactory();
        }

        private PlacementIntentFactory
            BuildPlacementIntentFactory()
        {
            return new PlacementIntentFactory();
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

        private PlacementResolver
            BuildPlacementResolver()
        {
            return new PlacementResolver(
                BuildAroundPlayerPlacementStrategy());
        }

        private SpawnReferenceContextFactory
            BuildSpawnReferenceContextFactory()
        {
            return new SpawnReferenceContextFactory();
        }

        private PlayerReferenceProvider
            BuildPlayerReferenceProvider()
        {
            return new PlayerReferenceProvider();
        }

        private SpawnReferenceResolver
            BuildSpawnReferenceResolver()
        {
            return new SpawnReferenceResolver(
                BuildPlayerReferenceProvider());
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

        private SpawnMaterializerResolver
            BuildMaterializerResolver()
        {
            return new SpawnMaterializerResolver(
                BuildEnemyMaterializer());
        }

        private SpawnExecutionContextFactory
            BuildSpawnExecutionContextFactory()
        {
            return new SpawnExecutionContextFactory();
        }

        private ScheduledSpawnTaskExecutor
    BuildScheduledSpawnTaskExecutor()
        {
            return new ScheduledSpawnTaskExecutor(
                BuildSpawnExecutionContextFactory(),
                BuildMaterializerResolver());
        }

        private ResolvedSpawnTaskFactory
    BuildResolvedSpawnTaskFactory()
        {
            return new ResolvedSpawnTaskFactory();
        }


    }
}