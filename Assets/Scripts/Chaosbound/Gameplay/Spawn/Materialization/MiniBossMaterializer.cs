using Chaosbound.Content.Enemy.MiniBosses;
using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Infrastructure;
using Chaosbound.Gameplay.Spawn.Integration;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using System;

namespace Chaosbound.Gameplay.Spawn.Materialization
{
    /// <summary>
    /// Materializes MiniBoss spawn tasks into the game world.
    /// </summary>
    public sealed class MiniBossMaterializer :
        ISpawnMaterializer
    {
        private readonly ISpawnInstantiationService
            instantiationService;

        public MiniBossMaterializer(
            ISpawnInstantiationService instantiationService)
        {
            this.instantiationService =
                instantiationService
                ?? throw new ArgumentNullException(
                    nameof(instantiationService));
        }

        public void Materialize(
            SpawnExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            if (context
                    .ResolvedTask
                    .ScheduledTask
                    .Task
                    .Entry
                    .Materializable
                    .Reference
                is not MiniBossData miniBoss)
            {
                throw new InvalidOperationException(
                    "MiniBossMaterializer received an unsupported materializable reference.");
            }

            SpawnPlacement placement =
                context
                    .ResolvedTask
                    .Placement
                    .Placement;

            SpawnInstantiationRequest request =
                new SpawnInstantiationRequest(
                    miniBoss,
                    placement.Position,
                    placement.Rotation);

            instantiationService.Spawn(
                request);
        }
    }
}