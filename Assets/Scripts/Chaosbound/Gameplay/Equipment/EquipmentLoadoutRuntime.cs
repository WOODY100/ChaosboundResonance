using System;
using System.Collections.Generic;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentLoadoutRuntime
    {
        private readonly Dictionary<EquipmentType, ItemInstance> equippedItems =
            new Dictionary<EquipmentType, ItemInstance>();

        public event Action EquipmentChanged;

        public bool TryEquip(
            ItemInstance itemInstance,
            ItemBaseData baseData,
            out ItemInstance replacedItem)
        {
            replacedItem = null;

            if (itemInstance == null)
                return false;

            if (baseData == null)
                return false;

            if (baseData.Category != ItemCategory.Equipment)
                return false;

            EquipmentType equipmentType =
                baseData.EquipmentType;

            if (equipmentType == EquipmentType.None)
                return false;

            if (IsEquipped(itemInstance))
                return false;

            if (equippedItems.TryGetValue(
                    equipmentType,
                    out ItemInstance currentItem))
            {
                replacedItem = currentItem;
            }

            equippedItems[equipmentType] = itemInstance;

            EquipmentChanged?.Invoke();

            return true;
        }

        public bool TryUnequip(
            EquipmentType equipmentType,
            out ItemInstance removedItem)
        {
            removedItem = null;

            if (equipmentType == EquipmentType.None)
                return false;

            if (!equippedItems.TryGetValue(
                    equipmentType,
                    out ItemInstance itemInstance))
            {
                return false;
            }

            equippedItems.Remove(equipmentType);

            removedItem = itemInstance;

            EquipmentChanged?.Invoke();

            return true;
        }

        public bool TryGetEquipped(
            EquipmentType equipmentType,
            out ItemInstance itemInstance)
        {
            itemInstance = null;

            if (equipmentType == EquipmentType.None)
                return false;

            return equippedItems.TryGetValue(
                equipmentType,
                out itemInstance);
        }

        public bool IsEquipped(
            ItemInstance itemInstance)
        {
            if (itemInstance == null)
                return false;

            foreach (KeyValuePair<EquipmentType, ItemInstance> pair
                     in equippedItems)
            {
                ItemInstance equippedItem =
                    pair.Value;

                if (equippedItem == null)
                    continue;

                if (equippedItem.InstanceId ==
                    itemInstance.InstanceId)
                {
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            equippedItems.Clear();
        }

        public int EquippedCount =>
            equippedItems.Count;

        public IReadOnlyDictionary<EquipmentType, ItemInstance> EquippedItems =>
            equippedItems;
    }
}