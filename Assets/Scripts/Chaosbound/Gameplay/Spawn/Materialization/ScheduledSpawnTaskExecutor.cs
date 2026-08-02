using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;

namespace Chaosbound.Gameplay.Spawn.Materialization
{
    /// <summary>
    /// Executes scheduled spawn tasks.
    /// </summary>
    public sealed class ScheduledSpawnTaskExecutor
    {
        private readonly SpawnExecutionContextFactory
            contextFactory;

        private readonly SpawnMaterializerResolver
            materializerResolver;

        public ScheduledSpawnTaskExecutor(
            SpawnExecutionContextFactory contextFactory,
            SpawnMaterializerResolver materializerResolver)
        {
            this.contextFactory =
                contextFactory
                ?? throw new ArgumentNullException(nameof(contextFactory));

            this.materializerResolver =
                materializerResolver
                ?? throw new ArgumentNullException(nameof(materializerResolver));
        }

        /// <summary>
        /// Executes the supplied scheduled task.
        /// </summary>
        public void Execute(
            ScheduledSpawnTask scheduledTask,
            SpawnJobRuntimeState runtimeState)
        {
            if (scheduledTask == null)
                throw new ArgumentNullException(nameof(scheduledTask));

            SpawnExecutionContext context =
                contextFactory.Create(
                    scheduledTask,
                    runtimeState);

            ISpawnMaterializer materializer =
                materializerResolver.Resolve(
                    context);

            materializer.Materialize(
                context);
        }
    }
}