using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.UI.Tooltip;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class InventorySlotUI :
        MonoBehaviour,
        IItemTooltipSource
    {
        [Header("References")]
        [SerializeField] private Image itemIcon;

        [Header("New Indicator")]
        [SerializeField] private GameObject newIndicator;

        [Header("Content")]
        [SerializeField] private ItemDatabase itemDatabase;

        private ItemResolver resolver;
        private ItemInstance currentItem;

        private int slotIndex = -1;

        public int SlotIndex => slotIndex;

        public ItemInstance CurrentItem => currentItem;

        public void SetSlotIndex(
            int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(index));

            slotIndex = index;
        }

        private void Awake()
        {
            if (itemDatabase != null)
            {
                resolver = new ItemResolver(itemDatabase);
            }

            Clear();
        }

        public void MarkAsSeen()
        {
            if (currentItem == null)
                return;

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            PersistentInventoryRuntime inventoryRuntime =
                bootstrapContext.PersistentInventoryRuntime;

            if (inventoryRuntime == null)
                return;

            PersistentInventoryState state =
                inventoryRuntime.State;

            if (state == null)
                return;

            state.ItemSeenState.MarkSeen(
                currentItem.InstanceId);

            SetNewIndicator(false);
        }

        public TooltipContent GetTooltipContent()
        {
            if (currentItem == null)
                return null;

            if (resolver == null)
                return null;

            if (!resolver.TryResolve(
                    currentItem.BaseDataId,
                    out ItemBaseData itemData))
            {
                return null;
            }

            return TooltipContentFactory.CreateItemContent(
                currentItem,
                itemData);
        }

        public void SetItem(ItemInstance item)
        {
            if (item == null)
            {
                Clear();
                return;
            }

            if (resolver == null)
            {
                return;
            }

            if (!resolver.TryResolve(
                    item.BaseDataId,
                    out ItemBaseData itemData))
            {
                Clear();
                return;
            }

            currentItem = item;

            if (itemIcon != null)
            {
                itemIcon.sprite = itemData.Icon;
                itemIcon.enabled = itemData.Icon != null;
            }
        }

        public Sprite GetCurrentItemIcon()
        {
            if (itemIcon == null)
                return null;

            return itemIcon.sprite;
        }

        public void SetNewIndicator(bool isNew)
        {
            if (newIndicator == null)
                return;

            newIndicator.SetActive(isNew);
        }

        public void Clear()
        {
            currentItem = null;

            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }

            SetNewIndicator(false);
        }
    }
}