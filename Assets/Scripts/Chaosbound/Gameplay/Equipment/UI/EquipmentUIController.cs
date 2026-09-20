using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Items.Runtime;
using System.Collections;
using UnityEngine;

namespace Chaosbound.Gameplay.Equipment.UI
{
    public sealed class EquipmentUIController :
        MonoBehaviour
    {
        [Header("Equipment Slots")]
        [SerializeField] private EquipmentSlotUI mainWeaponSlot;
        [SerializeField] private EquipmentSlotUI helmetSlot;
        [SerializeField] private EquipmentSlotUI armorSlot;
        [SerializeField] private EquipmentSlotUI pantsSlot;
        [SerializeField] private EquipmentSlotUI bootsSlot;
        [SerializeField] private EquipmentSlotUI pendantSlot;
        [SerializeField] private EquipmentSlotUI ringSlot;
        [SerializeField] private EquipmentSlotUI glovesSlot;
        [SerializeField] private EquipmentSlotUI specialRelicSlot;

        private EquipmentLoadoutRuntime loadout;
        private bool initialized;

        private IEnumerator Start()
        {
            yield return null;

            TryInitialize();
        }

        private void OnDestroy()
        {
            if (loadout != null)
            {
                loadout.EquipmentChanged -=
                    HandleEquipmentChanged;
            }
        }

        private void TryInitialize()
        {
            if (initialized)
                return;

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            EquipmentLoadoutRuntime equipmentLoadout =
                bootstrapContext.EquipmentLoadoutRuntime;

            if (equipmentLoadout == null)
                return;

            loadout =
                equipmentLoadout;

            loadout.EquipmentChanged +=
                HandleEquipmentChanged;

            initialized = true;

            RefreshAll();
        }

        private void HandleEquipmentChanged()
        {
            RefreshAll();
        }

        private void RefreshAll()
        {
            RefreshSlot(
                mainWeaponSlot,
                EquipmentType.MainWeapon);

            RefreshSlot(
                helmetSlot,
                EquipmentType.Helmet);

            RefreshSlot(
                armorSlot,
                EquipmentType.Armor);

            RefreshSlot(
                pantsSlot,
                EquipmentType.Pants);

            RefreshSlot(
                bootsSlot,
                EquipmentType.Boots);

            RefreshSlot(
                pendantSlot,
                EquipmentType.Pendant);

            RefreshSlot(
                ringSlot,
                EquipmentType.Ring);

            RefreshSlot(
                glovesSlot,
                EquipmentType.Gloves);

            RefreshSlot(
                specialRelicSlot,
                EquipmentType.SpecialRelic);
        }

        private void RefreshSlot(
            EquipmentSlotUI slot,
            EquipmentType equipmentType)
        {
            if (slot == null)
                return;

            if (!loadout.TryGetEquipped(
                    equipmentType,
                    out ItemInstance item))
            {
                slot.Clear();
                return;
            }

            slot.SetItem(item);
        }
    }
}