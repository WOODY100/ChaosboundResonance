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
    public sealed class SecureInventorySlotUI :
        MonoBehaviour,
        IItemTooltipSource
    {
        [Header("References")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private GameObject lockedIcon;

        private ItemResolver resolver;
        private ItemInstance currentItem;

        public ItemInstance CurrentItem => currentItem;

        private int slotIndex = -1;

        public int SlotIndex => slotIndex;

        public void SetSlotIndex(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index));
            }

            slotIndex = index;
        }

        private bool TryGetResolver(
            out ItemResolver itemResolver)
        {
            itemResolver = resolver;

            if (itemResolver != null)
                return true;

            GameContentContext contentContext =
                GameContentContext.Current;

            if (contentContext == null)
                return false;

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            if (contentResolver == null)
                return false;

            resolver =
                new ItemResolver(contentResolver);

            itemResolver = resolver;

            return true;
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
        }

        public Sprite GetCurrentItemIcon()
        {
            if (itemIcon == null)
            {
                return null;
            }

            return itemIcon.sprite;
        }

        private void Awake()
        {
            Clear();
            SetLocked(true);
        }

        public TooltipContent GetTooltipContent()
        {
            if (currentItem == null)
                return null;

            if (!TryGetResolver(
                    out ItemResolver itemResolver))
            {
                return null;
            }

            if (!itemResolver.TryResolve(
                    currentItem.BaseDataId,
                    out ItemBaseData itemData))
            {
                return null;
            }

            return TooltipContentFactory.CreateItemContent(
                currentItem,
                itemData);
        }

        public void SetLocked(bool locked)
        {
            if (lockedIcon != null)
            {
                lockedIcon.SetActive(locked);
            }

            if (locked)
            {
                Clear();
            }
        }

        public void SetItem(ItemInstance item)
        {
            if (item == null)
            {
                Clear();
                return;
            }

            if (!TryGetResolver(
                    out ItemResolver itemResolver))
            {
                return;
            }

            if (!itemResolver.TryResolve(
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

        public void Clear()
        {
            currentItem = null;

            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }
        }
    }
}