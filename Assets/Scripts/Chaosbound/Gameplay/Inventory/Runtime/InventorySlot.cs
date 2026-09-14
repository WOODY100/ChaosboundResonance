using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Inventory.Runtime
{
    public sealed class InventorySlot
    {
        public int Index { get; }

        public ItemInstance Item { get; private set; }

        public bool IsOccupied =>
            Item != null;

        public InventorySlot(int index)
        {
            if (index < 0)
                throw new System.ArgumentOutOfRangeException(nameof(index));

            Index = index;
        }

        public bool TrySet(ItemInstance item)
        {
            if (IsOccupied)
                return false;

            if (item == null)
                return false;

            Item = item;
            return true;
        }

        public ItemInstance Remove()
        {
            ItemInstance item = Item;
            Item = null;
            return item;
        }

        public void Clear()
        {
            Item = null;
        }
    }
}