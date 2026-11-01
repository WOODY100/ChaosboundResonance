using Chaosbound.Content.Enemy.MiniBosses;
using Chaosbound.Gameplay.Bosses;
using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Infrastructure;
using Chaosbound.Gameplay.Spawn.Integration;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Materialization
{
    public sealed class MiniBossMaterializer : ISpawnMaterializer
    {
        private readonly ISpawnInstantiationService instantiationService;

        public MiniBossMaterializer(ISpawnInstantiationService instantiationService)
        {
            this.instantiationService = instantiationService
                ?? throw new ArgumentNullException(nameof(instantiationService));
        }

        public GameObject Materialize(SpawnExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.ResolvedTask.ScheduledTask.Task.Entry.Materializable.Reference
                is not MiniBossData miniBoss)
                throw new InvalidOperationException(
                    "MiniBossMaterializer received an unsupported materializable reference.");

            SpawnPlacement placement = context.ResolvedTask.Placement.Placement;
            SpawnInstantiationRequest request = new SpawnInstantiationRequest(
                miniBoss, placement.Position, placement.Rotation);

            GameObject spawnedObject = instantiationService.Spawn(request);
            if (spawnedObject == null)
                throw new InvalidOperationException(
                    $"MiniBoss '{miniBoss.name}' could not be materialized.");

            BossRuntimeContext runtimeContext =
                spawnedObject.GetComponent<BossRuntimeContext>();
            if (runtimeContext == null)
                throw new InvalidOperationException(
                    $"MiniBoss '{spawnedObject.name}' is missing BossRuntimeContext.");

            runtimeContext.Initialize(
                miniBoss,
                context.RuntimeState.ExpeditionRuntime);

            context.RuntimeState.ExpeditionRuntime.RuntimeReferences.Register(
                "miniboss",
                miniBoss.Id,
                spawnedObject.transform);

            return spawnedObject;
        }
    }
}
