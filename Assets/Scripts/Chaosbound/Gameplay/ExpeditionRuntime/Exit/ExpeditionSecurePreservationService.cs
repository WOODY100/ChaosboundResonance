using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Inventory.Runtime;
using Chaosbound.Gameplay.Items.Runtime;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Exit
{
    /// <summary>
    /// Preserves Items stored in Secure Inventory
    /// when an Expedition ends without successful completion.
    ///
    /// Secure Items are transferred to the persistent
    /// Item Inventory and removed from Secure Inventory.
    ///
    /// This service does not own Expedition lifecycle,
    /// GameFlow, scene transitions or cleanup.
    /// </summary>
    public sealed class ExpeditionSecurePreservationService
    {
        private readonly PersistentItemInventoryState persistentItems;
        private readonly SecureInventoryState secureInventory;

        public ExpeditionSecurePreservationService(
            PersistentItemInventoryState persistentItems,
            SecureInventoryState secureInventory)
        {
            this.persistentItems =
                persistentItems ??
                throw new ArgumentNullException(
                    nameof(persistentItems));

            this.secureInventory =
                secureInventory ??
                throw new ArgumentNullException(
                    nameof(secureInventory));
        }

        public bool TryPreserve()
        {
            List<ItemInstance> secureItems =
                CollectSecureItems();

            for (int i = 0; i < secureItems.Count; i++)
            {
                ItemInstance item =
                    secureItems[i];

                if (!persistentItems.TryAdd(item))
                {
                    return false;
                }
            }

            RemovePreservedSecureItems(
                secureItems);

            return true;
        }

        private List<ItemInstance> CollectSecureItems()
        {
            List<ItemInstance> items =
                new List<ItemInstance>();

            IReadOnlyList<InventorySlot> slots =
                secureInventory.GetSlots();

            for (int i = 0; i < slots.Count; i++)
            {
                InventorySlot slot =
                    slots[i];

                if (slot == null ||
                    !slot.IsOccupied)
                {
                    continue;
                }

                ItemInstance item =
                    slot.Item;

                if (item == null)
                {
                    throw new InvalidOperationException(
                        "Secure Inventory contains a null ItemInstance.");
                }

                items.Add(item);
            }

            return items;
        }

        private void RemovePreservedSecureItems(
            IReadOnlyList<ItemInstance> preservedItems)
        {
            for (int i = 0;
                 i < preservedItems.Count;
                 i++)
            {
                ItemInstance expectedItem =
                    preservedItems[i];

                IReadOnlyList<InventorySlot> slots =
                    secureInventory.GetSlots();

                bool removed = false;

                for (int slotIndex = 0;
                     slotIndex < slots.Count;
                     slotIndex++)
                {
                    InventorySlot slot =
                        slots[slotIndex];

                    if (slot == null ||
                        !slot.IsOccupied)
                    {
                        continue;
                    }

                    if (slot.Item != expectedItem)
                        continue;

                    if (!secureInventory.TryRemoveAt(
                            slotIndex,
                            out ItemInstance removedItem))
                    {
                        throw new InvalidOperationException(
                            "Failed to remove a preserved " +
                            "Secure Inventory item.");
                    }

                    if (removedItem != expectedItem)
                    {
                        throw new InvalidOperationException(
                            "Removed Secure Inventory item " +
                            "did not match the preserved item.");
                    }

                    removed = true;
                    break;
                }

                if (!removed)
                {
                    throw new InvalidOperationException(
                        "Could not locate a Secure Inventory " +
                        "item that was prepared for preservation.");
                }
            }
        }
    }
}