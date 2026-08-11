using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Infrastructure;
using Chaosbound.Gameplay.Spawn.Integration;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Materialization
{
    /// <summary>
    /// Materializes enemy spawn tasks into the game world.
    /// </summary>
    public sealed class EnemyMaterializer :
        ISpawnMaterializer
    {
        private readonly ISpawnInstantiationService
            instantiationService;

        public EnemyMaterializer(
            ISpawnInstantiationService instantiationService)
        {
            this.instantiationService =
                instantiationService
                ?? throw new ArgumentNullException(
                    nameof(instantiationService));
        }

        /// <summary>
        /// Materializes the supplied execution context.
        /// </summary>
        public void Materialize(
            SpawnExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context
                    .ResolvedTask
                    .ScheduledTask
                    .Task
                    .Entry
                    .Materializable
                    .Reference
                is not EnemyVariantData enemy)
            {
                throw new InvalidOperationException(
                    "EnemyMaterializer received an unsupported materializable reference.");
            }

            Debug.Log(
                $"[EnemyMaterializer] {enemy.name}");

            SpawnPlacement placement =
                context
                    .ResolvedTask
                    .Placement
                    .Placement;

            SpawnInstantiationRequest request =
                new SpawnInstantiationRequest(
                    enemy,
                    placement.Position,
                    placement.Rotation);

            GameObject spawnedObject =
                instantiationService.Spawn(request);

            EnemyRuntimeContext runtimeContext =
                spawnedObject.GetComponent<EnemyRuntimeContext>();

            if (runtimeContext == null)
            {
                throw new InvalidOperationException(
                    $"Enemy '{spawnedObject.name}' is missing an EnemyRuntimeContext component.");
            }

            runtimeContext.Initialize(
                enemy,
                context.RuntimeState.ExpeditionRuntime);

            context
                .RuntimeState
                .ExpeditionRuntime
                .RuntimeComposition
                .Increment(enemy);
        }
    }
}