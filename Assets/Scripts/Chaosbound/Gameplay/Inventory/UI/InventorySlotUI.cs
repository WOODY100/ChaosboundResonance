using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
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

        [Header("Equipped Indicator")]
        [SerializeField] private GameObject equippedIndicator;

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
            Clear();
        }

        private void OnEnable()
        {
            SubscribeToEquipmentChanges();
            RefreshEquippedIndicator();
        }

        private void OnDisable()
        {
            UnsubscribeFromEquipmentChanges();
        }

        private void SubscribeToEquipmentChanges()
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            EquipmentLoadoutRuntime loadout =
                bootstrapContext.EquipmentLoadoutRuntime;

            if (loadout == null)
                return;

            loadout.EquipmentChanged -=
                HandleEquipmentChanged;

            loadout.EquipmentChanged +=
                HandleEquipmentChanged;
        }

        private void UnsubscribeFromEquipmentChanges()
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            EquipmentLoadoutRuntime loadout =
                bootstrapContext.EquipmentLoadoutRuntime;

            if (loadout == null)
                return;

            loadout.EquipmentChanged -=
                HandleEquipmentChanged;
        }

        private void HandleEquipmentChanged()
        {
            RefreshEquippedIndicator();
        }

        private void RefreshEquippedIndicator()
        {
            if (equippedIndicator == null)
                return;

            if (currentItem == null)
            {
                equippedIndicator.SetActive(false);
                return;
            }

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
            {
                equippedIndicator.SetActive(false);
                return;
            }

            EquipmentLoadoutRuntime loadout =
                bootstrapContext.EquipmentLoadoutRuntime;

            if (loadout == null)
            {
                equippedIndicator.SetActive(false);
                return;
            }

            bool isEquipped =
                loadout.IsEquipped(
                    currentItem);

            equippedIndicator.SetActive(
                isEquipped);
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

            currentItem = item;

            if (itemIcon != null)
            {
                itemIcon.sprite =
                    itemData.Icon;

                itemIcon.enabled =
                    itemData.Icon != null;
            }

            RefreshEquippedIndicator();
        }

        public Sprite GetCurrentItemIcon()
        {
            if (itemIcon == null)
                return null;

            return itemIcon.sprite;
        }

        public void SetNewIndicator(
            bool isNew)
        {
            if (newIndicator == null)
                return;

            newIndicator.SetActive(
                isNew);
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

            if (equippedIndicator != null)
            {
                equippedIndicator.SetActive(false);
            }
        }
    }
}