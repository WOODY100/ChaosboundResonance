using Chaosbound.Content.Expeditions.Enums.Spawn;
using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using Chaosbound.Gameplay.Spawn.Reference.Models;
using Chaosbound.Gameplay.Spawn.References;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;

namespace Chaosbound.Gameplay.Items.World.Integration
{
    /// <summary>
    /// Materializes an existing ItemInstance as a WorldItem
    /// through the shared Spawn Runtime.
    /// </summary>
    public sealed class ItemWorldDropService
    {
        private readonly ItemResolver itemResolver;
        private readonly SpawnRuntime spawnRuntime;

        private readonly SpawnRequestFactory
            spawnRequestFactory;

        private readonly SpawnRequestEntryFactory
            spawnRequestEntryFactory;

        public ItemWorldDropService(
            ItemResolver itemResolver,
            SpawnRuntime spawnRuntime)
        {
            this.itemResolver =
                itemResolver
                ?? throw new ArgumentNullException(
                    nameof(itemResolver));

            this.spawnRuntime =
                spawnRuntime
                ?? throw new ArgumentNullException(
                    nameof(spawnRuntime));

            spawnRequestFactory =
                new SpawnRequestFactory();

            spawnRequestEntryFactory =
                new SpawnRequestEntryFactory(
                    new MaterializableReferenceFactory());
        }

        /// <summary>
        /// Attempts to materialize an existing item instance
        /// as a WorldItem at the current player position.
        /// </summary>
        public bool TryDrop(
            ItemInstance item,
            RuntimeExpeditionConfig config,
            RuntimeReferencesConfig references,
            ExpeditionRuntimeState state)
        {
            if (item == null)
                return false;

            if (config == null)
                return false;

            if (references == null)
                return false;

            if (references.Player == null)
                return false;

            if (state == null)
                return false;

            ItemBaseData itemData;

            if (!itemResolver.TryResolve(
                item.BaseDataId,
                out itemData))
            {
                return false;
            }

            if (itemData == null)
                return false;

            if (itemData.WorldPrefab == null)
                return false;

            ItemMaterializableReference materializableReference =
                new ItemMaterializableReference(
                    itemData.ContentId,
                    itemData.WorldPrefab);

            SpawnRequestEntry entry =
                spawnRequestEntryFactory.Create(
                    materializableReference,
                    1);

            SpawnSpatialOrigin spatialOrigin =
                new SpawnSpatialOrigin(
                    references.Player.position);

            SpawnRequest request =
                spawnRequestFactory.Create(
                    new[] { entry },
                    SpawnRequestOrigin.ItemWorldDrop,
                    spatialOrigin);

            RuntimeSpawnConfig spawnConfig =
                config.Spawn.WithPlacement(
                    SpawnPlacementPolicy.AtOrigin);

            var spawnedObjects =
                spawnRuntime.Execute(
                    request,
                    spawnConfig,
                    references,
                    state);

            if (spawnedObjects == null ||
                spawnedObjects.Count != 1)
            {
                return false;
            }

            WorldItem worldItem =
                spawnedObjects[0].GetComponent<WorldItem>();

            if (worldItem == null)
                return false;

            worldItem.Initialize(
                item,
                itemData);

            return worldItem.IsInitialized;
        }
    }
}