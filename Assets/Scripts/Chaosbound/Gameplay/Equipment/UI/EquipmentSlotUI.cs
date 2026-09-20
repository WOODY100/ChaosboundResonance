using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Inventory.UI;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.UI.Tooltip;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Equipment.UI
{
    public sealed class EquipmentSlotUI :
        MonoBehaviour,
        IItemTooltipSource
    {
        [Header("References")]
        [SerializeField] private Image itemIcon;

        [SerializeField]
        private InventoryUIDropFeedback dropFeedback;

        [Header("Equipment")]
        [SerializeField] private EquipmentType equipmentType;

        private ItemResolver resolver;
        private ItemInstance currentItem;
        private Sprite emptySlotSprite;

        public EquipmentType EquipmentType =>
            equipmentType;

        public ItemInstance CurrentItem =>
            currentItem;

        public bool IsOccupied =>
            currentItem != null;

        private void Awake()
        {
            if (itemIcon != null)
            {
                emptySlotSprite =
                    itemIcon.sprite;
            }

            if (dropFeedback != null)
            {
                dropFeedback.Clear();
            }

            Clear();
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
                new ItemResolver(
                    contentResolver);

            itemResolver = resolver;

            return true;
        }

        public void SetItem(
            ItemInstance item)
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

            if (itemData.Category !=
                ItemCategory.Equipment)
            {
                Clear();
                return;
            }

            if (itemData.EquipmentType !=
                equipmentType)
            {
                Clear();
                return;
            }

            currentItem = item;

            if (itemIcon != null)
            {
                itemIcon.sprite =
                    itemData.Icon;

                itemIcon.enabled =
                    itemData.Icon != null;
            }
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

        public Sprite GetCurrentItemIcon()
        {
            if (itemIcon == null)
                return null;

            return itemIcon.sprite;
        }

        public void Clear()
        {
            currentItem = null;

            if (itemIcon != null)
            {
                itemIcon.sprite =
                    emptySlotSprite;

                itemIcon.enabled =
                    emptySlotSprite != null;
            }
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

        public void ShowValidDropFeedback()
        {
            if (dropFeedback == null)
                return;

            dropFeedback.ShowValid();
        }

        public void ShowSwapDropFeedback()
        {
            if (dropFeedback == null)
                return;

            dropFeedback.ShowSwap();
        }

        public void ShowInvalidDropFeedback()
        {
            if (dropFeedback == null)
                return;

            dropFeedback.ShowInvalid();
        }

        public void ClearDropFeedback()
        {
            if (dropFeedback == null)
                return;

            dropFeedback.Clear();
        }
    }
}