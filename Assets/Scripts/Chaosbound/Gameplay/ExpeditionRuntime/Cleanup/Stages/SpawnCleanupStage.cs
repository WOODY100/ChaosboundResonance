using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Contracts;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Stages
{
    /// <summary>
    /// Cleans up gameplay objects materialized
    /// by the Spawn Runtime.
    /// </summary>
    public sealed class SpawnCleanupStage :
        IExpeditionCleanupStage
    {
        private readonly SpawnRuntime spawnRuntime;

        public SpawnCleanupStage(
            SpawnRuntime spawnRuntime)
        {
            this.spawnRuntime =
                spawnRuntime
                ?? throw new ArgumentNullException(
                    nameof(spawnRuntime));
        }

        public void Execute(
            ExpeditionCleanupContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(
                    nameof(context));
            }

            spawnRuntime.Cleanup();
        }
    }
}