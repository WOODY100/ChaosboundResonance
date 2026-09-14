using System;
using System.Collections.Generic;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Inventory.Runtime
{
    public sealed class ExpeditionInventoryRuntime
    {
        private readonly InventorySlot[] slots;

        public int Capacity =>
            slots.Length;

        public int OccupiedCount
        {
            get
            {
                int count = 0;

                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].IsOccupied)
                        count++;
                }

                return count;
            }
        }

        public bool IsFull =>
            OccupiedCount >= Capacity;

        public ExpeditionInventoryRuntime()
        {
            slots = new InventorySlot[InventoryConstants.MainSlotCount];

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new InventorySlot(i);
            }
        }

        public bool TryAdd(ItemInstance item)
        {
            if (item == null)
                return false;

            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsOccupied)
                {
                    return slots[i].TrySet(item);
                }
            }

            return false;
        }

        public bool TryRemoveAt(
            int slotIndex,
            out ItemInstance item)
        {
            item = null;

            if (!IsValidIndex(slotIndex))
                return false;

            if (!slots[slotIndex].IsOccupied)
                return false;

            item = slots[slotIndex].Remove();
            return item != null;
        }

        public bool TryGetAt(
            int slotIndex,
            out ItemInstance item)
        {
            item = null;

            if (!IsValidIndex(slotIndex))
                return false;

            item = slots[slotIndex].Item;
            return item != null;
        }

        public IReadOnlyList<InventorySlot> GetSlots()
        {
            return slots;
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 &&
                   index < slots.Length;
        }

        public bool TrySwap(
            int sourceIndex,
            int destinationIndex)
        {
            if (sourceIndex < 0 ||
                sourceIndex >= slots.Length)
            {
                return false;
            }

            if (destinationIndex < 0 ||
                destinationIndex >= slots.Length)
            {
                return false;
            }

            if (sourceIndex == destinationIndex)
            {
                return false;
            }

            InventorySlot sourceSlot =
                slots[sourceIndex];

            InventorySlot destinationSlot =
                slots[destinationIndex];

            if (!sourceSlot.IsOccupied ||
                !destinationSlot.IsOccupied)
            {
                return false;
            }

            ItemInstance sourceItem =
                sourceSlot.Item;

            ItemInstance destinationItem =
                destinationSlot.Item;

            if (sourceItem == null ||
                destinationItem == null)
            {
                return false;
            }

            sourceSlot.Remove();
            destinationSlot.Remove();

            if (destinationSlot.TrySet(sourceItem) &&
                sourceSlot.TrySet(destinationItem))
            {
                return true;
            }

            // Defensive rollback.
            sourceSlot.Clear();
            destinationSlot.Clear();

            bool sourceRestored =
                sourceSlot.TrySet(sourceItem);

            bool destinationRestored =
                destinationSlot.TrySet(destinationItem);

            if (!sourceRestored ||
                !destinationRestored)
            {
                throw new InvalidOperationException(
                    "Inventory swap rollback failed.");
            }

            return false;
        }

        public bool TryMove(
            int sourceIndex,
            int destinationIndex)
        {
            if (sourceIndex < 0 ||
                sourceIndex >= slots.Length)
            {
                return false;
            }

            if (destinationIndex < 0 ||
                destinationIndex >= slots.Length)
            {
                return false;
            }

            if (sourceIndex == destinationIndex)
                return false;

            InventorySlot sourceSlot =
                slots[sourceIndex];

            InventorySlot destinationSlot =
                slots[destinationIndex];

            if (!sourceSlot.IsOccupied)
                return false;

            if (destinationSlot.IsOccupied)
                return false;

            ItemInstance item =
                sourceSlot.Item;

            if (item == null)
                return false;

            sourceSlot.Remove();

            if (destinationSlot.TrySet(item))
                return true;

            // Defensive rollback.
            sourceSlot.TrySet(item);

            return false;
        }

        public bool TryMoveToSecure(
            int sourceIndex,
            SecureInventoryState secureInventory,
            int secureSlotIndex)
        {
            if (secureInventory == null)
                return false;

            if (sourceIndex < 0 ||
                sourceIndex >= slots.Length)
            {
                return false;
            }

            if (!secureInventory.IsUnlocked(secureSlotIndex))
                return false;

            InventorySlot sourceSlot =
                slots[sourceIndex];

            if (!sourceSlot.IsOccupied)
                return false;

            if (secureInventory.TryGetAt(
                secureSlotIndex,
                out ItemInstance _))
            {
                return false;
            }

            ItemInstance item =
                sourceSlot.Item;

            if (item == null)
                return false;

            if (!secureInventory.TryStore(
                secureSlotIndex,
                item))
            {
                return false;
            }

            ItemInstance removedItem =
                sourceSlot.Remove();

            if (removedItem == item)
                return true;

            // Defensive rollback.
            secureInventory.TryRemoveAt(
                secureSlotIndex,
                out ItemInstance rollbackItem);

            if (rollbackItem != null)
                sourceSlot.TrySet(rollbackItem);

            return false;
        }

        public bool TryMoveFromSecure(
            SecureInventoryState secureInventory,
            int secureSlotIndex,
            int destinationIndex)
        {
            if (secureInventory == null)
                return false;

            if (destinationIndex < 0 ||
                destinationIndex >= slots.Length)
            {
                return false;
            }

            if (!secureInventory.IsUnlocked(secureSlotIndex))
                return false;

            InventorySlot destinationSlot =
                slots[destinationIndex];

            if (destinationSlot.IsOccupied)
                return false;

            if (!secureInventory.TryGetAt(
                secureSlotIndex,
                out ItemInstance item))
            {
                return false;
            }

            if (item == null)
                return false;

            if (!destinationSlot.TrySet(item))
                return false;

            ItemInstance removedItem;

            if (secureInventory.TryRemoveAt(
                secureSlotIndex,
                out removedItem))
            {
                if (removedItem == item)
                    return true;

                // Defensive rollback.
                destinationSlot.Remove();

                if (removedItem != null)
                {
                    secureInventory.TryStore(
                        secureSlotIndex,
                        removedItem);
                }

                return false;
            }

            // Defensive rollback.
            destinationSlot.Remove();

            return false;
        }
    }
}