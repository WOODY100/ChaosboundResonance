using Chaosbound.Gameplay.Spawn.Calculators;
using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Infrastructure;
using Chaosbound.Gameplay.Spawn.Integration;
using Chaosbound.Gameplay.Spawn.Materialization;
using Chaosbound.Gameplay.Spawn.Scheduling;
using System;

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

            EnemyScheduler scheduler =
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
                taskExecutor);
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

        private EnemyScheduler BuildScheduler(
            SpawnBatchCalculator batchCalculator,
            SpawnTaskEntryFactory taskEntryFactory,
            SpawnTaskFactory taskFactory)
        {
            EnemySchedulingPolicyResolver resolver =
                BuildSchedulingPolicyResolver(
                    batchCalculator,
                    taskEntryFactory,
                    taskFactory);

            return new EnemyScheduler(resolver);
        }

        private EnemySchedulingPolicyResolver BuildSchedulingPolicyResolver(
            SpawnBatchCalculator batchCalculator,
            SpawnTaskEntryFactory taskEntryFactory,
            SpawnTaskFactory taskFactory)
        {
            return new EnemySchedulingPolicyResolver(
                batchCalculator,
                taskEntryFactory,
                taskFactory);
        }

        private ISpawnInstantiationService BuildInstantiationService()
        {
            return new PoolManagerSpawnInstantiationService();
        }

        private EnemyMaterializer BuildEnemyMaterializer()
        {
            return new EnemyMaterializer(
                BuildInstantiationService());
        }

        private SpawnMaterializerResolver BuildMaterializerResolver()
        {
            return new SpawnMaterializerResolver(
                BuildEnemyMaterializer());
        }

        private SpawnExecutionContextFactory BuildExecutionContextFactory()
        {
            return new SpawnExecutionContextFactory();
        }

        private ScheduledSpawnTaskExecutor BuildTaskExecutor()
        {
            return new ScheduledSpawnTaskExecutor(
                BuildExecutionContextFactory(),
                BuildMaterializerResolver());
        }

        private SpawnJobRuntimeStateFactory BuildRuntimeStateFactory()
        {
            return new SpawnJobRuntimeStateFactory();
        }
    }
}