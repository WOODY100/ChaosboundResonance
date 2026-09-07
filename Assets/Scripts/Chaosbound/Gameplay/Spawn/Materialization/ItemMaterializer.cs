using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Infrastructure;
using Chaosbound.Gameplay.Spawn.Integration;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using Chaosbound.Gameplay.Spawn.References;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Materialization
{
    public sealed class ItemMaterializer : ISpawnMaterializer
    {
        private readonly ISpawnInstantiationService instantiationService;

        public ItemMaterializer(
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
                throw new ArgumentNullException(nameof(context));

            if (context
                    .ResolvedTask
                    .ScheduledTask
                    .Task
                    .Entry
                    .Materializable
                    .Reference
                is not ItemMaterializableReference item)
            {
                throw new InvalidOperationException(
                    "ItemMaterializer received an unsupported " +
                    "materializable reference.");
            }

            SpawnPlacement placement =
                context
                    .ResolvedTask
                    .Placement
                    .Placement;

            SpawnInstantiationRequest request =
                new SpawnInstantiationRequest(
                    item,
                    placement.Position,
                    placement.Rotation);

            GameObject instance =
                instantiationService.Spawn(request);

            if (instance == null)
            {
                throw new InvalidOperationException(
                    $"Item '{item.ContentId}' " +
                    "could not be materialized.");
            }

            return instance;
        }
    }
}