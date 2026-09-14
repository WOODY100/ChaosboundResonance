using Chaosbound.Gameplay.Inventory.Runtime;
using Chaosbound.Gameplay.Items.Runtime;
using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class ExpeditionInventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform slotContainer;
        [SerializeField] private InventorySlotUI slotPrefab;

        private InventorySlotUI[] slotUIs;
        private ExpeditionInventoryRuntime inventory;

        private void Awake()
        {
            CreateSlots();
        }

        private void Update()
        {
            ResolveInventory();

            if (inventory == null)
            {
                return;
            }

            Refresh();
        }

        private void ResolveInventory()
        {
            if (inventory != null)
            {
                return;
            }

            RunManager runManager = RunManager.Instance;

            if (runManager == null)
            {
                return;
            }

            if (runManager.ExpeditionRuntimeState == null)
            {
                return;
            }

            inventory =
                runManager.ExpeditionRuntimeState.Inventory;
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
                InventoryConstants.MainSlotCount;

            slotUIs =
                new InventorySlotUI[slotCount];

            for (int i = 0; i < slotCount; i++)
            {
                InventorySlotUI slot =
                    Instantiate(slotPrefab, slotContainer);

                slot.SetSlotIndex(i);

                InventoryUIDropTarget dropTarget =
                    slot.gameObject.GetComponent<InventoryUIDropTarget>();

                if (dropTarget == null)
                {
                    dropTarget =
                        slot.gameObject.AddComponent<InventoryUIDropTarget>();
                }

                dropTarget.Configure(
                    InventoryUIDropTargetType.Main,
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
                ItemInstance item;

                if (!inventory.TryGetAt(i, out item))
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