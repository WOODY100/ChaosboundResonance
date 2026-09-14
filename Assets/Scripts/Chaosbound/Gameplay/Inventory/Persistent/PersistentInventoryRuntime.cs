using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.Persistent
{
    public sealed class PersistentInventoryRuntime : MonoBehaviour
    {
        private PersistentInventoryState state;

        public PersistentInventoryState State =>
            state;

        private void Awake()
        {
            state = new PersistentInventoryState();
        }
    }
}