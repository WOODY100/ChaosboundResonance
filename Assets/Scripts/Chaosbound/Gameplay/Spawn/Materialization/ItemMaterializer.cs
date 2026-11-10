using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.World;
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
        private readonly ItemResolver itemResolver;
        private readonly ItemInstanceFactory itemInstanceFactory;

        public ItemMaterializer(
            ISpawnInstantiationService instantiationService,
            ItemResolver itemResolver,
            ItemInstanceFactory itemInstanceFactory)
        {
            this.instantiationService =
                instantiationService
                ?? throw new ArgumentNullException(
                    nameof(instantiationService));

            this.itemResolver =
                itemResolver
                ?? throw new ArgumentNullException(
                    nameof(itemResolver));

            this.itemInstanceFactory =
                itemInstanceFactory
                ?? throw new ArgumentNullException(
                    nameof(itemInstanceFactory));
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

            ItemBaseData itemBaseData =
                itemResolver.Resolve(item.ContentId);

            if (itemBaseData == null)
            {
                throw new InvalidOperationException(
                    $"Item '{item.ContentId}' could not be resolved.");
            }

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

            WorldItem worldItem =
                instance.GetComponent<WorldItem>();

            if (worldItem == null)
            {
                throw new InvalidOperationException(
                    $"Item prefab '{instance.name}' for " +
                    $"ContentId '{item.ContentId}' " +
                    "requires a WorldItem component.");
            }

            ItemInstance itemInstance =
                itemInstanceFactory.Create(itemBaseData);

            worldItem.Initialize(
                itemInstance,
                itemBaseData);

            if (!worldItem.IsInitialized)
            {
                throw new InvalidOperationException(
                    $"WorldItem initialization failed for " +
                    $"ContentId '{item.ContentId}'.");
            }

            return instance;
        }
    }
}