using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Inventory.Runtime;
using Chaosbound.Gameplay.Items.Runtime;
using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class SecureInventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform slotContainer;
        [SerializeField] private SecureInventorySlotUI slotPrefab;

        private SecureInventorySlotUI[] slotUIs;
        private SecureInventoryState secureInventory;

        private void Awake()
        {
            CreateSlots();
        }

        private void Update()
        {
            ResolveSecureInventory();

            if (secureInventory == null)
            {
                return;
            }

            Refresh();
        }

        private void ResolveSecureInventory()
        {
            if (secureInventory != null)
            {
                return;
            }

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
            {
                return;
            }

            PersistentInventoryRuntime
                persistentInventoryRuntime =
                    bootstrapContext.PersistentInventoryRuntime;

            if (persistentInventoryRuntime == null)
            {
                return;
            }

            if (persistentInventoryRuntime.State == null)
            {
                return;
            }

            secureInventory =
                persistentInventoryRuntime.State.SecureInventory;
        }

        private void CreateSlots()
        {
            if (slotContainer == null)
            {
                return;
            }

            if (slotPrefab == null)
            {
                return;
            }

            int slotCount =
                InventoryConstants.SecureSlotCount;

            slotUIs =
                new SecureInventorySlotUI[slotCount];

            for (int i = 0; i < slotCount; i++)
            {
                SecureInventorySlotUI slot =
                    Instantiate(
                        slotPrefab,
                        slotContainer);

                slot.SetSlotIndex(i);

                InventoryUIDropTarget dropTarget =
                    slot.gameObject.GetComponent<InventoryUIDropTarget>();

                if (dropTarget == null)
                {
                    dropTarget =
                        slot.gameObject.AddComponent<InventoryUIDropTarget>();
                }

                dropTarget.Configure(
                    InventoryUIDropTargetType.Secure,
                    i);

                slotUIs[i] = slot;
            }
        }

        private void Refresh()
        {
            if (slotUIs == null)
            {
                return;
            }

            for (int i = 0; i < slotUIs.Length; i++)
            {
                bool unlocked =
                    secureInventory.IsUnlocked(i);

                slotUIs[i].SetLocked(!unlocked);

                if (!unlocked)
                {
                    continue;
                }

                ItemInstance item;

                if (!secureInventory.TryGetAt(
                        i,
                        out item))
                {
                    item = null;
                }

                if (slotUIs[i].CurrentItem == item)
                {
                    continue;
                }

                slotUIs[i].SetItem(item);
            }
        }
    }
}