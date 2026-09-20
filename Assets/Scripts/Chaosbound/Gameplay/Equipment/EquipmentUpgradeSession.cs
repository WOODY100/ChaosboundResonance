using System;
using System.Collections.Generic;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentUpgradeSession
    {
        private readonly EquipmentUpgradeService upgradeService;

        private ItemInstance itemInstance;

        private List<EquipmentStatOption> options =
            new List<EquipmentStatOption>();

        private bool hasSelection;
        private EquipmentStatOption selectedOption;

        public bool IsOpen =>
            itemInstance != null;

        public ItemInstance ItemInstance =>
            itemInstance;

        public IReadOnlyList<EquipmentStatOption> Options =>
            options;

        public bool HasSelection =>
            hasSelection;

        public EquipmentStatOption SelectedOption =>
            selectedOption;

        public EquipmentUpgradeSession(
            EquipmentUpgradeService upgradeService)
        {
            if (upgradeService == null)
                throw new ArgumentNullException(
                    nameof(upgradeService));

            this.upgradeService =
                upgradeService;
        }

        public bool Open(
            ItemInstance itemInstance)
        {
            Close();

            if (itemInstance == null)
                return false;

            if (!upgradeService.TryGenerateTierOptions(
                    itemInstance,
                    out List<EquipmentStatOption> generatedOptions))
            {
                return false;
            }

            this.itemInstance =
                itemInstance;

            options =
                generatedOptions;

            hasSelection = false;

            selectedOption =
                default;

            return true;
        }

        public bool TryReroll()
        {
            if (!IsOpen)
                return false;

            if (!upgradeService.TryRerollTierOptions(
                    itemInstance,
                    out List<EquipmentStatOption> rerolledOptions))
            {
                return false;
            }

            options =
                rerolledOptions;

            hasSelection = false;

            selectedOption =
                default;

            return true;
        }

        public bool TrySelect(
            EquipmentStatOption option)
        {
            if (!IsOpen)
                return false;

            for (int i = 0;
                 i < options.Count;
                 i++)
            {
                EquipmentStatOption availableOption =
                    options[i];

                if (!IsSameOption(
                        availableOption,
                        option))
                {
                    continue;
                }

                selectedOption =
                    availableOption;

                hasSelection = true;

                return true;
            }

            return false;
        }

        public bool TryConfirm()
        {
            if (!IsOpen)
                return false;

            if (!hasSelection)
                return false;

            if (!upgradeService.TryApplyTierUp(
                    itemInstance,
                    selectedOption))
            {
                return false;
            }

            Close();

            return true;
        }

        public void Close()
        {
            itemInstance = null;

            options =
                new List<EquipmentStatOption>();

            hasSelection = false;

            selectedOption =
                default;
        }

        private static bool IsSameOption(
            EquipmentStatOption first,
            EquipmentStatOption second)
        {
            return first.StatType ==
                       second.StatType &&
                   first.ModifierType ==
                       second.ModifierType &&
                   UnityEngine.Mathf.Approximately(
                       first.RolledValue,
                       second.RolledValue);
        }
    }
}