using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Gameplay.Bosses;
using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Infrastructure;
using Chaosbound.Gameplay.Spawn.Integration;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Materialization
{
    /// <summary>
    /// Materializes Boss spawn tasks into the game world.
    /// </summary>
    public sealed class BossMaterializer :
        ISpawnMaterializer
    {
        private const string BossDomainId =
            "boss";

        private readonly ISpawnInstantiationService
            instantiationService;

        public BossMaterializer(
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
                is not BossData boss)
            {
                throw new InvalidOperationException(
                    "BossMaterializer received an unsupported materializable reference.");
            }

            SpawnPlacement placement =
                context
                    .ResolvedTask
                    .Placement
                    .Placement;

            SpawnInstantiationRequest request =
                new SpawnInstantiationRequest(
                    boss,
                    placement.Position,
                    placement.Rotation);

            GameObject spawnedObject =
                instantiationService.Spawn(
                    request);

            if (spawnedObject == null)
            {
                throw new InvalidOperationException(
                    $"Boss '{boss.name}' could not be materialized.");
            }

            BossRuntimeContext runtimeContext =
                spawnedObject.GetComponent<BossRuntimeContext>();

            if (runtimeContext == null)
            {
                throw new InvalidOperationException(
                    $"Boss '{spawnedObject.name}' " +
                    "is missing a BossRuntimeContext component.");
            }

            runtimeContext.Initialize(
                boss,
                context.RuntimeState.ExpeditionRuntime);

            context
                .RuntimeState
                .ExpeditionRuntime
                .RuntimeReferences
                .Register(
                    BossDomainId,
                    boss.Id,
                    spawnedObject.transform);

            return spawnedObject;
        }
    }
}