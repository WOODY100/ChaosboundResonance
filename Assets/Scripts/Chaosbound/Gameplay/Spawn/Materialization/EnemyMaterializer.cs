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
        public GameObject Materialize(
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

            if (spawnedObject == null)
            {
                throw new InvalidOperationException(
                    $"Enemy '{enemy.name}' could not be materialized.");
            }

            EnemyRuntimeContext runtimeContext =
                spawnedObject.GetComponent<EnemyRuntimeContext>();

            if (runtimeContext == null)
            {
                throw new InvalidOperationException(
                    $"Enemy '{spawnedObject.name}' " +
                    "is missing an EnemyRuntimeContext component.");
            }

            runtimeContext.Initialize(
                enemy,
                context.RuntimeState.ExpeditionRuntime);

            EnemyRuntimeStats runtimeStats =
                spawnedObject.GetComponent<EnemyRuntimeStats>();

            if (runtimeStats == null)
            {
                throw new InvalidOperationException(
                    $"Enemy '{spawnedObject.name}' " +
                    "is missing an EnemyRuntimeStats component.");
            }

            runtimeStats.Initialize();

            EnemyHealth health =
                spawnedObject.GetComponent<EnemyHealth>();

            if (health == null)
            {
                throw new InvalidOperationException(
                    $"Enemy '{spawnedObject.name}' " +
                    "is missing an EnemyHealth component.");
            }

            health.Initialize();

            EnemyReward reward =
                spawnedObject.GetComponent<EnemyReward>();

            if (reward == null)
            {
                throw new InvalidOperationException(
                    $"Enemy '{spawnedObject.name}' " +
                    "is missing an EnemyReward component.");
            }

            reward.Initialize();

            EnemyRuntimePresentation presentation =
                spawnedObject.GetComponent<EnemyRuntimePresentation>();

            if (presentation == null)
            {
                throw new InvalidOperationException(
                    $"Enemy '{spawnedObject.name}' " +
                    "is missing an EnemyRuntimePresentation component.");
            }

            presentation.Initialize();

            EnemyRuntimeTargeting targeting =
                spawnedObject.GetComponent<EnemyRuntimeTargeting>();

            if (targeting == null)
            {
                throw new InvalidOperationException(
                    $"Enemy '{spawnedObject.name}' " +
                    "is missing an EnemyRuntimeTargeting component.");
            }

            targeting.Initialize(
                new ScenePlayerTargetProvider());

            EnemyRuntimeNavigation navigation =
                spawnedObject.GetComponent<EnemyRuntimeNavigation>();

            if (navigation == null)
            {
                throw new InvalidOperationException(
                    $"Enemy '{spawnedObject.name}' " +
                    "is missing an EnemyRuntimeNavigation component.");
            }

            navigation.Initialize();

            EnemyCombat combat =
                spawnedObject.GetComponent<EnemyCombat>();

            if (combat == null)
            {
                throw new InvalidOperationException(
                    $"Enemy '{spawnedObject.name}' " +
                    "is missing an EnemyCombat component.");
            }

            combat.Initialize();

            EnemyRuntimeBehavior behavior =
                spawnedObject.GetComponent<EnemyRuntimeBehavior>();

            if (behavior == null)
            {
                throw new InvalidOperationException(
                    $"Enemy '{spawnedObject.name}' " +
                    "is missing an EnemyRuntimeBehavior component.");
            }

            IEnemyMovementPolicy movementPolicy =
                EnemyMovementPolicyResolver.Resolve(
                    enemy.MovementPolicy);

            behavior.Initialize(
                movementPolicy);

            EnemyRuntimeBehaviorScheduler scheduler =
                spawnedObject.GetComponent<
                    EnemyRuntimeBehaviorScheduler>();

            if (scheduler == null)
            {
                throw new InvalidOperationException(
                    $"Enemy '{spawnedObject.name}' " +
                    "is missing an EnemyRuntimeBehaviorScheduler component.");
            }

            scheduler.Initialize();

            context
                .RuntimeState
                .ExpeditionRuntime
                .RuntimeComposition
                .Increment(enemy);

            return spawnedObject;
        }
    }
}