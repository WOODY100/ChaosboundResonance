using System;
using System.Collections.Generic;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Inventory.Runtime
{
    public sealed class SecureInventoryState
    {
        private readonly InventorySlot[] slots;

        private int unlockedSlotCount;

        public int Capacity =>
            slots.Length;

        public int UnlockedSlotCount =>
            unlockedSlotCount;

        public SecureInventoryState()
        {
            slots = new InventorySlot[
                InventoryConstants.SecureSlotCount];

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new InventorySlot(i);
            }

            unlockedSlotCount =
                InventoryConstants.InitialUnlockedSecureSlots;
        }

        public bool IsUnlocked(int slotIndex)
        {
            return slotIndex >= 0 &&
                   slotIndex < unlockedSlotCount;
        }

        public bool TryStore(
            int slotIndex,
            ItemInstance item)
        {
            if (item == null)
                return false;

            if (!IsUnlocked(slotIndex))
                return false;

            return slots[slotIndex].TrySet(item);
        }

        public bool TryRemoveAt(
            int slotIndex,
            out ItemInstance item)
        {
            item = null;

            if (!IsUnlocked(slotIndex))
                return false;

            if (!slots[slotIndex].IsOccupied)
                return false;

            item = slots[slotIndex].Remove();
            return item != null;
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
            {
                return false;
            }

            if (!IsUnlocked(sourceIndex) ||
                !IsUnlocked(destinationIndex))
            {
                return false;
            }

            InventorySlot sourceSlot =
                slots[sourceIndex];

            InventorySlot destinationSlot =
                slots[destinationIndex];

            if (!sourceSlot.IsOccupied)
            {
                return false;
            }

            if (destinationSlot.IsOccupied)
            {
                return false;
            }

            ItemInstance item =
                sourceSlot.Remove();

            if (item == null)
            {
                return false;
            }

            if (destinationSlot.TrySet(item))
            {
                return true;
            }

            // Defensive rollback.
            if (!sourceSlot.TrySet(item))
            {
                throw new InvalidOperationException(
                    "Secure inventory rollback failed.");
            }

            return false;
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

            if (!IsUnlocked(sourceIndex) ||
                !IsUnlocked(destinationIndex))
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
                    "Secure inventory swap rollback failed.");
            }

            return false;
        }

        public bool TryGetAt(
            int slotIndex,
            out ItemInstance item)
        {
            item = null;

            if (!IsUnlocked(slotIndex))
                return false;

            item = slots[slotIndex].Item;
            return item != null;
        }

        public bool TryUnlockNextSlot()
        {
            if (unlockedSlotCount >= slots.Length)
                return false;

            unlockedSlotCount++;
            return true;
        }

        public IReadOnlyList<InventorySlot> GetSlots()
        {
            return slots;
        }
    }
}