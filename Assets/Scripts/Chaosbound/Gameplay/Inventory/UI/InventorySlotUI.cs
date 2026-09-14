using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class InventorySlotUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image itemIcon;

        [Header("Content")]
        [SerializeField] private ItemDatabase itemDatabase;

        private ItemResolver resolver;
        private ItemInstance currentItem;

        private int slotIndex = -1;

        public int SlotIndex => slotIndex;

        public void SetSlotIndex(
            int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(index));

            slotIndex = index;
        }

        public ItemInstance CurrentItem => currentItem;

        private void Awake()
        {
            if (itemDatabase != null)
            {
                resolver = new ItemResolver(itemDatabase);
            }

            Clear();
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