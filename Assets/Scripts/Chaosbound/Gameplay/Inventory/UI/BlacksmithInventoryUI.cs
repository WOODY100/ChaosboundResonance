using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.UI.Tooltip;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class BlacksmithInventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform slotContainer;
        [SerializeField] private InventorySlotUI slotPrefab;

        [Header("Capacity")]
        [SerializeField] private int minimumSlotCount = 20;
        [SerializeField] private int slotsPerExpansion = 5;

        [Header("Drag")]
        [SerializeField] private bool enableDragHandlers = false;

        [SerializeField]
        private BlacksmithItemDropTarget itemDropTarget;

        private ItemInstance selectedItem;

        private InventorySlotUI[] slotUIs;

        private PersistentInventoryRuntime inventoryRuntime;
        private PersistentItemInventoryState inventoryItems;
        private PersistentItemSeenState itemSeenState;

        private ItemTooltipService tooltipService;
        private ItemResolver itemResolver;

        private readonly List<FilteredItemEntry> filteredItems =
            new List<FilteredItemEntry>();

        private PersonalStashInventoryFilter currentFilter =
            PersonalStashInventoryFilter.All;

        private bool isInitialized;

        public int SlotCount =>
            slotUIs != null ? slotUIs.Length : 0;

        public PersonalStashInventoryFilter CurrentFilter =>
            currentFilter;

        private struct FilteredItemEntry
        {
            public ItemInstance Item;
            public int InventoryIndex;

            public FilteredItemEntry(
                ItemInstance item,
                int inventoryIndex)
            {
                Item = item;
                InventoryIndex = inventoryIndex;
            }
        }

        private void Awake()
        {
            ValidateConfiguration();

            EnsureSlotCapacity(minimumSlotCount);
        }

        private void Start()
        {
            ResolveInventory();
            ResolveItemResolver();
            ResolveTooltipService();

            SubscribeTooltipService();

            isInitialized = true;

            RefreshInventory();
        }

        private void OnEnable()
        {
            if (itemDropTarget == null)
                return;

            itemDropTarget.SelectionChanged -=
                HandleSelectionChanged;

            itemDropTarget.SelectionChanged +=
                HandleSelectionChanged;

            selectedItem = itemDropTarget.SelectedItem;

            RefreshSelectionHighlights();
        }

        private void OnDisable()
        {
            if (itemDropTarget != null)
            {
                itemDropTarget.SelectionChanged -=
                    HandleSelectionChanged;
            }
        }

        private void OnDestroy()
        {
            if (tooltipService != null)
            {
                tooltipService.OnItemSeen -= HandleItemSeen;
            }
        }

        private void HandleSelectionChanged(
            ItemInstance item,
            ItemBaseData itemData)
        {
            selectedItem = item;

            RefreshSelectionHighlights();
        }

        //==================================================
        // Initialization
        //==================================================

        private void ResolveInventory()
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithInventoryUI] " +
                    "BootstrapContext could not be resolved.",
                    this);

                return;
            }

            inventoryRuntime =
                bootstrapContext.PersistentInventoryRuntime;

            if (inventoryRuntime == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithInventoryUI] " +
                    "PersistentInventoryRuntime is unavailable.",
                    this);

                return;
            }

            PersistentInventoryState state =
                inventoryRuntime.State;

            if (state == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithInventoryUI] " +
                    "PersistentInventoryState is unavailable.",
                    this);

                return;
            }

            inventoryItems = state.Items;
            itemSeenState = state.ItemSeenState;
        }

        private void ResolveItemResolver()
        {
            GameContentContext contentContext =
                GameContentContext.Current;

            if (contentContext == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithInventoryUI] " +
                    "GameContentContext could not be resolved.",
                    this);

                return;
            }

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            if (contentResolver == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithInventoryUI] " +
                    "ItemContentResolver is unavailable.",
                    this);

                return;
            }

            itemResolver =
                new ItemResolver(contentResolver);
        }

        private void ResolveTooltipService()
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            tooltipService =
                bootstrapContext.ItemTooltipService;
        }

        private void SubscribeTooltipService()
        {
            if (tooltipService == null)
                return;

            tooltipService.OnItemSeen -= HandleItemSeen;
            tooltipService.OnItemSeen += HandleItemSeen;
        }

        private void RefreshSelectionHighlights()
        {
            if (slotUIs == null)
                return;

            for (int i = 0; i < slotUIs.Length; i++)
            {
                InventorySlotUI slot = slotUIs[i];

                if (slot == null)
                    continue;

                bool isSelected =
                    slot.gameObject.activeSelf &&
                    selectedItem != null &&
                    slot.CurrentItem == selectedItem;

                slot.SetSelected(isSelected);
            }
        }

        //==================================================
        // Slot Creation
        //==================================================

        public void EnsureSlotCapacity(
            int requiredSlotCount)
        {
            if (requiredSlotCount < 0)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithInventoryUI] " +
                    "Required slot count cannot be negative.",
                    this);

                return;
            }

            int targetCount =
                CalculateTargetSlotCount(requiredSlotCount);

            if (targetCount <= SlotCount)
                return;

            CreateSlots(targetCount);
        }

        private int CalculateTargetSlotCount(
            int requiredSlotCount)
        {
            int targetCount =
                Mathf.Max(
                    requiredSlotCount,
                    minimumSlotCount);

            int remainder =
                targetCount % slotsPerExpansion;

            if (remainder != 0)
            {
                targetCount +=
                    slotsPerExpansion - remainder;
            }

            return targetCount;
        }

        private void CreateSlots(int targetCount)
        {
            int previousCount = SlotCount;

            InventorySlotUI[] newSlots =
                new InventorySlotUI[targetCount];

            if (slotUIs != null)
            {
                System.Array.Copy(
                    slotUIs,
                    newSlots,
                    slotUIs.Length);
            }

            for (int i = previousCount; i < targetCount; i++)
            {
                InventorySlotUI slot =
                    Instantiate(
                        slotPrefab,
                        slotContainer);

                slot.SetSlotIndex(i);

                ConfigureDragHandler(slot);

                newSlots[i] = slot;
            }

            slotUIs = newSlots;
        }

        private void ConfigureDragHandler(
            InventorySlotUI slot)
        {
            InventorySlotUIDragHandler dragHandler =
                slot.GetComponent<InventorySlotUIDragHandler>();

            if (dragHandler != null)
            {
                dragHandler.enabled = enableDragHandlers;
            }
        }

        //==================================================
        // Inventory Refresh
        //==================================================

        public void RefreshInventory()
        {
            if (inventoryItems == null ||
                itemResolver == null)
            {
                return;
            }

            IReadOnlyList<ItemInstance> items =
                inventoryItems.GetItems();

            BuildFilteredItems(items);

            if (currentFilter ==
                PersonalStashInventoryFilter.All)
            {
                RefreshAllItems(items);
            }
            else
            {
                RefreshFilteredItems();
            }

            RefreshSelectionHighlights();
        }

        private void RefreshAllItems(
            IReadOnlyList<ItemInstance> items)
        {
            EnsureSlotCapacity(items.Count);

            for (int i = 0; i < slotUIs.Length; i++)
            {
                InventorySlotUI slot = slotUIs[i];

                slot.gameObject.SetActive(true);
                slot.SetSlotIndex(i);

                ItemInstance item = null;

                if (i < items.Count)
                    item = items[i];

                if (slot.CurrentItem != item)
                    slot.SetItem(item);

                UpdateNewIndicator(slot, item);
            }
        }

        private void RefreshFilteredItems()
        {
            EnsureSlotCapacity(filteredItems.Count);

            for (int i = 0; i < slotUIs.Length; i++)
            {
                InventorySlotUI slot = slotUIs[i];

                if (i >= filteredItems.Count)
                {
                    slot.gameObject.SetActive(false);
                    continue;
                }

                FilteredItemEntry entry =
                    filteredItems[i];

                slot.gameObject.SetActive(true);

                // Keep the original inventory index.
                slot.SetSlotIndex(entry.InventoryIndex);

                if (slot.CurrentItem != entry.Item)
                    slot.SetItem(entry.Item);

                UpdateNewIndicator(slot, entry.Item);
            }
        }

        private void UpdateNewIndicator(
            InventorySlotUI slot,
            ItemInstance item)
        {
            if (item == null)
            {
                slot.SetNewIndicator(false);
                return;
            }

            bool isNew =
                itemSeenState != null &&
                itemSeenState.IsNew(item.InstanceId);

            slot.SetNewIndicator(isNew);
        }

        //==================================================
        // Filtering
        //==================================================

        private void BuildFilteredItems(
            IReadOnlyList<ItemInstance> items)
        {
            filteredItems.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                ItemInstance item = items[i];

                if (item == null)
                    continue;

                if (!PassesCurrentFilter(item))
                    continue;

                filteredItems.Add(
                    new FilteredItemEntry(item, i));
            }
        }

        private bool PassesCurrentFilter(
            ItemInstance item)
        {
            if (item == null)
                return false;

            if (currentFilter ==
                PersonalStashInventoryFilter.All)
            {
                return true;
            }

            if (!itemResolver.TryResolve(
                    item.BaseDataId,
                    out ItemBaseData itemData))
            {
                return false;
            }

            switch (currentFilter)
            {
                case PersonalStashInventoryFilter.New:

                    return itemSeenState != null &&
                           itemSeenState.IsNew(item.InstanceId);

                case PersonalStashInventoryFilter.Weapons:

                    return itemData.Category ==
                               ItemCategory.Equipment &&
                           itemData.EquipmentType ==
                               EquipmentType.MainWeapon;

                case PersonalStashInventoryFilter.Armor:

                    return itemData.Category ==
                               ItemCategory.Equipment &&
                           IsArmorType(itemData.EquipmentType);

                case PersonalStashInventoryFilter.Accessories:

                    return itemData.Category ==
                               ItemCategory.Equipment &&
                           IsAccessoryType(itemData.EquipmentType);

                case PersonalStashInventoryFilter.Relics:

                    return itemData.Category ==
                               ItemCategory.Equipment &&
                           itemData.EquipmentType ==
                               EquipmentType.SpecialRelic;

                default:
                    return false;
            }
        }

        private bool IsArmorType(
            EquipmentType equipmentType)
        {
            switch (equipmentType)
            {
                case EquipmentType.Helmet:
                case EquipmentType.Armor:
                case EquipmentType.Pants:
                case EquipmentType.Boots:
                case EquipmentType.Gloves:
                    return true;

                default:
                    return false;
            }
        }

        private bool IsAccessoryType(
            EquipmentType equipmentType)
        {
            switch (equipmentType)
            {
                case EquipmentType.Pendant:
                case EquipmentType.Ring:
                    return true;

                default:
                    return false;
            }
        }

        public void SetFilter(
            PersonalStashInventoryFilter filter)
        {
            if (currentFilter == filter)
                return;

            currentFilter = filter;

            RefreshInventory();
        }

        //==================================================
        // Tooltip Integration
        //==================================================

        private void HandleItemSeen(ItemInstance item)
        {
            if (item == null)
                return;

            if (currentFilter ==
                PersonalStashInventoryFilter.New)
            {
                RefreshInventory();
                return;
            }

            if (slotUIs == null)
                return;

            for (int i = 0; i < slotUIs.Length; i++)
            {
                InventorySlotUI slot = slotUIs[i];

                if (slot == null)
                    continue;

                if (slot.CurrentItem != item)
                    continue;

                slot.SetNewIndicator(false);
                return;
            }
        }

        //==================================================
        // Validation
        //==================================================

        private void ValidateConfiguration()
        {
            if (slotContainer == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithInventoryUI] " +
                    "Slot Container reference is missing.",
                    this);
            }

            if (slotPrefab == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithInventoryUI] " +
                    "Slot Prefab reference is missing.",
                    this);
            }

            if (minimumSlotCount < 1)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithInventoryUI] " +
                    "Minimum Slot Count must be greater than zero.",
                    this);
            }

            if (slotsPerExpansion < 1)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithInventoryUI] " +
                    "Slots Per Expansion must be greater than zero.",
                    this);
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            ValidateConfiguration();
        }

#endif
    }
}