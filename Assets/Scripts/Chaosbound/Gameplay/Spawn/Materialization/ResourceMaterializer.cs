using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Infrastructure;
using Chaosbound.Gameplay.Spawn.Integration;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using Chaosbound.Gameplay.Spawn.References;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Materialization
{
    /// <summary>
    /// Materializes physical Resource content
    /// into the game world.
    /// </summary>
    public sealed class ResourceMaterializer :
        ISpawnMaterializer
    {
        private readonly ISpawnInstantiationService
            instantiationService;

        public ResourceMaterializer(
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
                is not ResourceMaterializableReference resource)
            {
                throw new InvalidOperationException(
                    "ResourceMaterializer received an unsupported " +
                    "materializable reference.");
            }

            SpawnPlacement placement =
                context
                    .ResolvedTask
                    .Placement
                    .Placement;

            SpawnInstantiationRequest request =
                new SpawnInstantiationRequest(
                    resource,
                    placement.Position,
                    placement.Rotation);

            GameObject instance =
                instantiationService.Spawn(
                    request);

            if (instance == null)
            {
                throw new InvalidOperationException(
                    $"Resource '{resource.ContentId}' " +
                    "could not be materialized.");
            }

            IResourcePickup pickup =
                instance.GetComponent<IResourcePickup>();

            if (pickup == null)
            {
                throw new InvalidOperationException(
                    $"Resource prefab for '{resource.ContentId}' " +
                    "requires an IResourcePickup component.");
            }

            pickup.Initialize(
                resource.Amount);

            return instance;
        }
    }
}