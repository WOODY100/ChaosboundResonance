using Chaosbound.Gameplay.Inventory.Runtime;

namespace Chaosbound.Gameplay.Inventory.Persistent
{
    public sealed class PersistentInventoryState
    {
        private readonly PersistentItemInventoryState items =
            new PersistentItemInventoryState();

        private readonly SecureInventoryState secureInventory =
            new SecureInventoryState();

        private readonly PersistentMaterialsState materials =
            new PersistentMaterialsState();

        public PersistentItemInventoryState Items =>
            items;

        public SecureInventoryState SecureInventory =>
            secureInventory;

        public PersistentMaterialsState Materials =>
            materials;
    }
}