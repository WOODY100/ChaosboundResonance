using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Items.Runtime;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Save
{
    public static class SaveEquipmentMapper
    {
        public static SaveEquipmentData ToSaveData(
            EquipmentLoadoutRuntime loadout)
        {
            if (loadout == null)
                throw new ArgumentNullException(
                    nameof(loadout));

            SaveEquipmentData data =
                new SaveEquipmentData();

            IReadOnlyDictionary<
                EquipmentType,
                ItemInstance> equippedItems =
                loadout.EquippedItems;

            foreach (
                KeyValuePair<
                    EquipmentType,
                    ItemInstance> entry
                in equippedItems)
            {
                if (entry.Key == EquipmentType.None)
                    continue;

                ItemInstance item =
                    entry.Value;

                if (item == null)
                    continue;

                data.EquippedItems.Add(
                    new SaveEquippedItemData
                    {
                        EquipmentType = entry.Key,
                        ItemInstanceId = item.InstanceId
                    });
            }

            return data;
        }

        public static bool TryPrepareLoad(
            SaveEquipmentData data,
            SaveInventoryLoadPlan inventoryLoadPlan,
            ItemContentResolver contentResolver,
            out SaveEquipmentLoadPlan loadPlan)
        {
            loadPlan = null;

            if (data == null)
                return false;

            if (inventoryLoadPlan == null)
                return false;

            if (contentResolver == null)
                return false;

            if (data.EquippedItems == null)
                return false;

            SaveEquipmentLoadPlan plan =
                new SaveEquipmentLoadPlan();

            HashSet<string> usedInstanceIds =
                new HashSet<string>(
                    StringComparer.Ordinal);

            HashSet<EquipmentType> usedEquipmentTypes =
                new HashSet<EquipmentType>();

            IReadOnlyList<ItemInstance> inventoryItems =
                inventoryLoadPlan.Items;

            for (int i = 0;
                 i < data.EquippedItems.Count;
                 i++)
            {
                SaveEquippedItemData savedEquipment =
                    data.EquippedItems[i];

                if (savedEquipment == null)
                    return false;

                if (savedEquipment.EquipmentType ==
                    EquipmentType.None)
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(
                        savedEquipment.ItemInstanceId))
                {
                    return false;
                }

                if (!usedEquipmentTypes.Add(
                        savedEquipment.EquipmentType))
                {
                    return false;
                }

                if (!usedInstanceIds.Add(
                        savedEquipment.ItemInstanceId))
                {
                    return false;
                }

                ItemInstance itemInstance = null;

                for (int j = 0;
                     j < inventoryItems.Count;
                     j++)
                {
                    ItemInstance candidate =
                        inventoryItems[j];

                    if (candidate == null)
                        continue;

                    if (candidate.InstanceId ==
                        savedEquipment.ItemInstanceId)
                    {
                        itemInstance = candidate;
                        break;
                    }
                }

                if (itemInstance == null)
                    return false;

                if (!contentResolver.TryResolve(
                        itemInstance.BaseDataId,
                        out ItemBaseData baseData))
                {
                    return false;
                }

                if (baseData == null)
                    return false;

                if (baseData.Category !=
                    ItemCategory.Equipment)
                {
                    return false;
                }

                if (baseData.EquipmentType !=
                    savedEquipment.EquipmentType)
                {
                    return false;
                }

                plan.Add(
                    savedEquipment.EquipmentType,
                    itemInstance);
            }

            loadPlan = plan;
            return true;
        }

        public static void ApplyLoadPlan(
            SaveEquipmentLoadPlan loadPlan,
            EquipmentLoadoutRuntime loadout)
        {
            if (loadPlan == null)
                throw new ArgumentNullException(
                    nameof(loadPlan));

            if (loadout == null)
                throw new ArgumentNullException(
                    nameof(loadout));

            loadout.Clear();

            foreach (
                KeyValuePair<
                    EquipmentType,
                    ItemInstance> entry
                in loadPlan.EquippedItems)
            {
                EquipmentType equipmentType =
                    entry.Key;

                ItemInstance itemInstance =
                    entry.Value;

                if (equipmentType == EquipmentType.None)
                {
                    throw new InvalidOperationException(
                        "Load plan contains EquipmentType.None.");
                }

                if (itemInstance == null)
                {
                    throw new InvalidOperationException(
                        "Load plan contains a null ItemInstance.");
                }

                if (!loadout.TryEquip(
                        itemInstance,
                        ResolveEquipmentBaseData(
                            itemInstance),
                        out ItemInstance replacedItem))
                {
                    throw new InvalidOperationException(
                        $"Failed to restore equipment " +
                        $"for ItemInstance '{itemInstance.InstanceId}'.");
                }

                if (replacedItem != null)
                {
                    throw new InvalidOperationException(
                        $"Unexpected equipment replacement while restoring " +
                        $"ItemInstance '{itemInstance.InstanceId}'.");
                }
            }
        }

        private static ItemBaseData ResolveEquipmentBaseData(
            ItemInstance itemInstance)
        {
            if (itemInstance == null)
            {
                throw new ArgumentNullException(
                    nameof(itemInstance));
            }

            GameContentContext context =
                GameContentContext.Current;

            if (context == null)
            {
                throw new InvalidOperationException(
                    "GameContentContext.Current is null.");
            }

            if (context.ItemContentResolver == null)
            {
                throw new InvalidOperationException(
                    "GameContentContext ItemContentResolver is null.");
            }

            if (!context.ItemContentResolver.TryResolve(
                    itemInstance.BaseDataId,
                    out ItemBaseData baseData))
            {
                throw new InvalidOperationException(
                    $"Could not resolve ItemBaseData " +
                    $"'{itemInstance.BaseDataId}' " +
                    $"for ItemInstance '{itemInstance.InstanceId}'.");
            }

            if (baseData == null)
            {
                throw new InvalidOperationException(
                    $"Resolved ItemBaseData is null for " +
                    $"ItemInstance '{itemInstance.InstanceId}'.");
            }

            if (baseData.Category !=
                ItemCategory.Equipment)
            {
                throw new InvalidOperationException(
                    $"ItemInstance '{itemInstance.InstanceId}' " +
                    "does not reference Equipment content.");
            }

            if (baseData.EquipmentType ==
                EquipmentType.None)
            {
                throw new InvalidOperationException(
                    $"ItemInstance '{itemInstance.InstanceId}' " +
                    "has EquipmentType.None.");
            }

            return baseData;
        }
    }
}