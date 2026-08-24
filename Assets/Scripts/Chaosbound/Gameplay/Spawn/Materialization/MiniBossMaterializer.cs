using Chaosbound.Content.Enemy.MiniBosses;
using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Infrastructure;
using Chaosbound.Gameplay.Spawn.Integration;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using System;
using UnityEngine;

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

        public GameObject Materialize(
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

            GameObject spawnedObject =
                instantiationService.Spawn(
                    request);

            if (spawnedObject == null)
            {
                throw new InvalidOperationException(
                    $"MiniBoss '{miniBoss.name}' could not be materialized.");
            }

            return spawnedObject;
        }
    }
}