using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class BlacksmithItemDropTarget : MonoBehaviour, IDropHandler
    {
        private ItemResolver itemResolver;

        public ItemInstance SelectedItem { get; private set; }

        public ItemBaseData SelectedItemData { get; private set; }

        public event Action<ItemInstance, ItemBaseData>
            SelectionChanged;

        private void Awake()
        {
            ResolveItemResolver();
        }

        //==================================================
        // Selection
        //==================================================

        public bool TryAcceptItem(ItemInstance item)
        {
            if (item == null)
                return false;

            if (!TryResolveEquipment(
                    item,
                    out ItemBaseData itemData))
            {
                return false;
            }

            SelectedItem = item;
            SelectedItemData = itemData;

            SelectionChanged?.Invoke(
                SelectedItem,
                SelectedItemData);

            return true;
        }

        public void ClearSelection()
        {
            if (SelectedItem == null)
                return;

            SelectedItem = null;
            SelectedItemData = null;

            SelectionChanged?.Invoke(
                null,
                null);
        }

        //==================================================
        // Validation
        //==================================================

        private bool TryResolveEquipment(
            ItemInstance item,
            out ItemBaseData itemData)
        {
            itemData = null;

            if (itemResolver == null)
                return false;

            if (!itemResolver.TryResolve(
                    item.BaseDataId,
                    out itemData))
            {
                return false;
            }

            if (itemData == null)
                return false;

            return itemData.Category ==
                ItemCategory.Equipment;
        }

        //==================================================
        // Resolver
        //==================================================

        private void ResolveItemResolver()
        {
            Chaosbound.Core.Composition.GameContentContext
                contentContext =
                    Chaosbound.Core.Composition.GameContentContext.Current;

            if (contentContext == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithItemDropTarget] " +
                    "GameContentContext is unavailable.",
                    this);

                return;
            }

            if (contentContext.ItemContentResolver == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithItemDropTarget] " +
                    "ItemContentResolver is unavailable.",
                    this);

                return;
            }

            itemResolver = new ItemResolver(
                contentContext.ItemContentResolver);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData == null ||
                eventData.pointerDrag == null)
            {
                return;
            }

            BlacksmithInventorySlotDragHandler dragHandler =
                eventData.pointerDrag.GetComponent<
                    BlacksmithInventorySlotDragHandler>();

            if (dragHandler == null)
                return;

            ItemInstance item = dragHandler.DraggedItem;

            if (item == null)
                return;

            if (!TryAcceptItem(item))
            {
                UnityEngine.Debug.Log(
                    "[BlacksmithItemDropTarget] " +
                    "The selected item was rejected.",
                    this);
            }
        }
    }
}