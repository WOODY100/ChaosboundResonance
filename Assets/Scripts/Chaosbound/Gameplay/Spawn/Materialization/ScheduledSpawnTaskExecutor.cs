using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;
using UnityEngine;

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
        public GameObject Execute(
            ResolvedSpawnTask resolvedTask,
            SpawnJobRuntimeState runtimeState)
        {
            if (resolvedTask == null)
                throw new ArgumentNullException(nameof(resolvedTask));

            if (runtimeState == null)
                throw new ArgumentNullException(nameof(runtimeState));

            SpawnExecutionContext context =
                contextFactory.Create(
                    resolvedTask,
                    runtimeState);

            ISpawnMaterializer materializer =
                materializerResolver.Resolve(
                    context);

            return materializer.Materialize(
                context);
        }
    }
}