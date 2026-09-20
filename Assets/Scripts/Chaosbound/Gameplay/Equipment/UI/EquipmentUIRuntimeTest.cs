using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Equipment.UI
{
    public sealed class EquipmentUIRuntimeTest :
        MonoBehaviour
    {
        [ContextMenu("Equip First Equipment Item")]
        private void EquipFirstEquipmentItem()
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
            {
                Debug.LogError(
                    "[Equipment UI Test] " +
                    "BootstrapContext is not available.",
                    this);

                return;
            }

            PersistentInventoryRuntime inventory =
                bootstrapContext.PersistentInventoryRuntime;

            if (inventory == null ||
                inventory.State == null)
            {
                Debug.LogError(
                    "[Equipment UI Test] " +
                    "PersistentInventoryRuntime is not available.",
                    this);

                return;
            }

            EquipmentInventoryService equipmentService =
                bootstrapContext.EquipmentInventoryService;

            if (equipmentService == null)
            {
                Debug.LogError(
                    "[Equipment UI Test] " +
                    "EquipmentInventoryService is not available.",
                    this);

                return;
            }

            GameContentContext contentContext =
                GameContentContext.Current;

            if (contentContext == null)
            {
                Debug.LogError(
                    "[Equipment UI Test] " +
                    "GameContentContext is not available.",
                    this);

                return;
            }

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            if (contentResolver == null)
            {
                Debug.LogError(
                    "[Equipment UI Test] " +
                    "ItemContentResolver is not available.",
                    this);

                return;
            }

            IReadOnlyList<ItemInstance> items =
                inventory.State.Items.GetItems();

            for (int i = 0; i < items.Count; i++)
            {
                ItemInstance item =
                    items[i];

                if (item == null)
                    continue;

                if (!contentResolver.TryResolve(
                        item.BaseDataId,
                        out ItemBaseData baseData))
                {
                    continue;
                }

                if (baseData == null)
                    continue;

                if (baseData.Category !=
                    ItemCategory.Equipment)
                {
                    continue;
                }

                if (baseData.EquipmentType ==
                    EquipmentType.None)
                {
                    continue;
                }

                if (!equipmentService.TryEquip(
                        item,
                        out ItemInstance replacedItem))
                {
                    Debug.LogError(
                        "[Equipment UI Test] " +
                        "Failed to equip first Equipment item.",
                        this);

                    return;
                }

                Debug.Log(
                    "[Equipment UI Test] " +
                    $"Equipped '{item.BaseDataId}' " +
                    $"into {baseData.EquipmentType}. " +
                    $"Replaced: " +
                    $"{(replacedItem != null ? replacedItem.BaseDataId : "None")}");

                return;
            }

            Debug.LogWarning(
                "[Equipment UI Test] " +
                "No Equipment ItemInstance was found in Inventory.",
                this);
        }
    }
}