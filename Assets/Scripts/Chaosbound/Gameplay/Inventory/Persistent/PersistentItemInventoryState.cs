using Chaosbound.Gameplay.Items.Runtime;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Inventory.Persistent
{
    public sealed class PersistentItemInventoryState
    {
        private readonly List<ItemInstance> items =
            new List<ItemInstance>();

        public int Count =>
            items.Count;

        public bool IsEmpty =>
            items.Count == 0;

        public bool TryAdd(ItemInstance item)
        {
            if (item == null)
                return false;

            items.Add(item);

            return true;
        }

        public bool TryRemove(ItemInstance item)
        {
            if (item == null)
                return false;

            return items.Remove(item);
        }

        public bool TryRemoveAt(
            int index,
            out ItemInstance item)
        {
            item = null;

            if (index < 0 ||
                index >= items.Count)
                return false;

            item = items[index];

            items.RemoveAt(index);

            return item != null;
        }

        public bool TryGetAt(
            int index,
            out ItemInstance item)
        {
            item = null;

            if (index < 0 ||
                index >= items.Count)
                return false;

            item = items[index];

            return item != null;
        }

        public bool TryReorder(
            int sourceIndex,
            int destinationIndex)
        {
            if (sourceIndex < 0 ||
                sourceIndex >= items.Count)
            {
                return false;
            }

            if (destinationIndex < 0 ||
                destinationIndex >= items.Count)
            {
                return false;
            }

            if (sourceIndex == destinationIndex)
                return false;

            ItemInstance item =
                items[sourceIndex];

            if (item == null)
                return false;

            items.RemoveAt(sourceIndex);

            items.Insert(
                destinationIndex,
                item);

            return true;
        }

        public IReadOnlyList<ItemInstance> GetItems()
        {
            return items;
        }

        public bool Contains(ItemInstance item)
        {
            if (item == null)
                return false;

            return items.Contains(item);
        }

        public void Clear()
        {
            items.Clear();
        }
    }
}