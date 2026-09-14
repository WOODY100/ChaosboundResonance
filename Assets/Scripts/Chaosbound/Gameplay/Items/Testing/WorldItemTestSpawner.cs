using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Inventory;
using Chaosbound.Gameplay.Inventory.Runtime;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.World;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Items.Testing
{
    public sealed class WorldItemTestSpawner : MonoBehaviour
    {
        [SerializeField]
        private ItemDatabase itemDatabase;

        [SerializeField]
        private string itemContentId = "sword_basic";

        [SerializeField]
        private Transform spawnPoint;

        [Header("Repeated Spawn Test")]
        [SerializeField]
        private float repeatedSpawnSpacing = 1.5f;

        private int repeatedSpawnIndex;

        [Header("Inventory Full Test")]
        [SerializeField]
        private bool fillInventory;

        private void Start()
        {
            if (itemDatabase == null)
            {
                return;
            }

            ItemResolver resolver =
                new ItemResolver(itemDatabase);

            if (!resolver.TryResolve(
                    itemContentId,
                    out ItemBaseData itemData))
            {
                return;
            }

            if (itemData.WorldPrefab == null)
            {
                return;
            }

            RunManager runManager =
                RunManager.Instance;

            if (runManager == null)
            {
                return;
            }

            if (runManager.ExpeditionRuntimeState == null)
            {
                return;
            }

            if (fillInventory)
            {
                FillInventory(
                    runManager.ExpeditionRuntimeState.Inventory,
                    itemData);
            }

            Vector3 position =
                spawnPoint != null
                    ? spawnPoint.position
                    : transform.position;

            Quaternion rotation =
                spawnPoint != null
                    ? spawnPoint.rotation
                    : Quaternion.identity;

            SpawnWorldItem(
                itemData,
                position,
                rotation);
        }

        private void FillInventory(
            ExpeditionInventoryRuntime inventory,
            ItemBaseData itemData)
        {
            if (inventory == null)
            {
                return;
            }

            for (int i = 0; i < InventoryConstants.MainSlotCount; i++)
            {
                ItemInstance instance =
                    new ItemInstance(
                        Guid.NewGuid().ToString("N"),
                        itemData.ContentId,
                        itemData.BaseTier);

                if (!inventory.TryAdd(instance))
                {
                    return;
                }
            }
        }

        public void SpawnAnotherItem()
        {
            if (itemDatabase == null)
            {
                return;
            }

            ItemResolver resolver =
                new ItemResolver(itemDatabase);

            if (!resolver.TryResolve(
                    itemContentId,
                    out ItemBaseData itemData))
            {
                return;
            }

            if (itemData.WorldPrefab == null)
            {
                return;
            }

            Vector3 origin =
                spawnPoint != null
                    ? spawnPoint.position
                    : transform.position;

            Quaternion rotation =
                spawnPoint != null
                    ? spawnPoint.rotation
                    : Quaternion.identity;

            Vector3 position =
                origin +
                Vector3.right *
                (repeatedSpawnSpacing * repeatedSpawnIndex);

            repeatedSpawnIndex++;

            SpawnWorldItem(
                itemData,
                position,
                rotation);
        }

        private void SpawnWorldItem(
            ItemBaseData itemData,
            Vector3 position,
            Quaternion rotation)
        {
            WorldItem worldItem =
                PoolManager.Instance.Get<WorldItem>(
                    itemData.WorldPrefab,
                    position,
                    rotation);

            if (worldItem == null)
            {
                return;
            }

            ItemInstance instance =
                new ItemInstance(
                    Guid.NewGuid().ToString("N"),
                    itemData.ContentId,
                    itemData.BaseTier);

            worldItem.Initialize(
                instance,
                itemData);
        }
    }
}