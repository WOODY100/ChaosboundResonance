using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Save;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentUpgradeSession
    {
        private readonly EquipmentUpgradeService upgradeService;

        private ItemInstance itemInstance;

        private string sessionInstanceId;
        private ItemTier sessionTier;
        private int sessionUpgradeLevel;

        private List<EquipmentStatOption> options =
            new List<EquipmentStatOption>();

        private bool hasSelection;
        private EquipmentStatOption selectedOption;

        private readonly IPersistentStateSaver persistentStateSaver;

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
            EquipmentUpgradeService upgradeService,
            IPersistentStateSaver persistentStateSaver)
        {
            if (upgradeService == null)
                throw new ArgumentNullException(
                    nameof(upgradeService));

            this.upgradeService = upgradeService;

            this.persistentStateSaver =
                persistentStateSaver ??
                throw new ArgumentNullException(
                    nameof(persistentStateSaver));
        }

        //==========================================================
        // Open
        //==========================================================

        public bool Open(ItemInstance itemInstance)
        {
            Close();

            if (itemInstance == null)
                return false;

            List<EquipmentStatOption> sessionOptions;

            if (itemInstance.HasValidTierUpSession())
            {
                sessionOptions =
                    new List<EquipmentStatOption>(
                        itemInstance.TierUpOptions);
            }
            else
            {
                if (!upgradeService.TryGenerateTierOptions(
                        itemInstance,
                        out sessionOptions))
                {
                    return false;
                }

                if (sessionOptions == null ||
                    sessionOptions.Count != 3)
                {
                    return false;
                }

                if (!itemInstance.TrySetTierUpSession(
                        sessionOptions))
                {
                    return false;
                }

                persistentStateSaver.SavePersistentState();

                sessionOptions =
                    new List<EquipmentStatOption>(
                        itemInstance.TierUpOptions);
            }

            if (sessionOptions == null ||
                sessionOptions.Count != 3 ||
                !itemInstance.HasValidTierUpSession())
            {
                return false;
            }

            this.itemInstance = itemInstance;

            sessionInstanceId =
                itemInstance.InstanceId;

            sessionTier =
                itemInstance.CurrentTier;

            sessionUpgradeLevel =
                itemInstance.UpgradeLevel;

            options = sessionOptions;

            hasSelection =
                itemInstance.HasSelectedTierUpOption;

            selectedOption =
                hasSelection
                    ? itemInstance.SelectedTierUpOption
                    : default;

            if (hasSelection &&
                !IsAvailableOption(selectedOption))
            {
                Close();
                return false;
            }

            return true;
        }

        //==========================================================
        // Reroll
        //==========================================================

        public bool TryReroll()
        {
            if (!IsOpen || !IsSessionStateValid())
                return false;

            // The service validates the cost, generates the new options,
            // replaces the persisted options, spends Gold and saves.
            if (!upgradeService.TryRerollTierOptions(
                    itemInstance,
                    out List<EquipmentStatOption> newOptions))
            {
                return false;
            }

            if (newOptions == null || newOptions.Count != 3)
            {
                Debug.LogError(
                    "[EquipmentUpgradeSession] Reroll returned invalid options.");

                return false;
            }

            // Synchronize the active session with the persisted options.
            options = new List<EquipmentStatOption>(newOptions);

            // The previous selection no longer applies.
            hasSelection = false;
            selectedOption = default;

            return true;
        }

        //==========================================================
        // Selection
        //==========================================================

        public bool TrySelect(
            EquipmentStatOption option)
        {
            if (!IsOpen || !IsSessionStateValid())
                return false;

            for (int i = 0; i < options.Count; i++)
            {
                EquipmentStatOption availableOption =
                    options[i];

                if (!IsSameOption(
                        availableOption,
                        option))
                {
                    continue;
                }

                if (itemInstance.HasUnlockedStat(
                        availableOption.StatType))
                {
                    return false;
                }

                if (!itemInstance.TrySelectTierUpOption(
                        availableOption))
                {
                    return false;
                }

                persistentStateSaver.SavePersistentState();

                selectedOption = availableOption;
                hasSelection = true;

                return true;
            }

            return false;
        }

        //==========================================================
        // Authorization
        //==========================================================

        public bool Authorizes(
            EquipmentUpgradeService requestingService,
            ItemInstance candidateItem,
            EquipmentStatOption candidateOption)
        {
            if (requestingService == null ||
                !ReferenceEquals(
                    upgradeService,
                    requestingService))
            {
                return false;
            }

            if (!IsOpen ||
                !hasSelection ||
                !IsSessionStateValid())
            {
                return false;
            }

            if (candidateItem == null ||
                !ReferenceEquals(
                    itemInstance,
                    candidateItem))
            {
                return false;
            }

            if (!itemInstance.HasSelectedTierUpOption)
                return false;

            if (!IsSameOption(
                    itemInstance.SelectedTierUpOption,
                    selectedOption))
            {
                return false;
            }

            if (!IsAvailableOption(candidateOption))
                return false;

            if (!IsSameOption(
                    selectedOption,
                    candidateOption))
            {
                return false;
            }

            if (itemInstance.HasUnlockedStat(
                    selectedOption.StatType))
            {
                return false;
            }

            return true;
        }

        //==========================================================
        // Confirmation
        //==========================================================

        public bool TryConfirm()
        {
            if (!IsOpen || !hasSelection)
                return false;

            if (!IsSessionStateValid())
                return false;

            if (!IsAvailableOption(selectedOption))
                return false;

            if (!itemInstance.HasSelectedTierUpOption ||
                !IsSameOption(
                    itemInstance.SelectedTierUpOption,
                    selectedOption))
            {
                return false;
            }

            if (itemInstance.HasUnlockedStat(
                    selectedOption.StatType))
            {
                return false;
            }

            if (!upgradeService.TryApplyTierUp(
                    this,
                    itemInstance,
                    selectedOption))
            {
                // Keep the session open after failed payment
                // or failed progression.
                return false;
            }

            Close();

            return true;
        }

        //==========================================================
        // Session Validation
        //==========================================================

        private bool IsSessionStateValid()
        {
            if (itemInstance == null)
                return false;

            if (!string.Equals(
                    itemInstance.InstanceId,
                    sessionInstanceId,
                    StringComparison.Ordinal))
            {
                return false;
            }

            if (itemInstance.CurrentTier != sessionTier)
                return false;

            if (itemInstance.UpgradeLevel != sessionUpgradeLevel)
                return false;

            if (!itemInstance.HasValidTierUpSession())
                return false;

            return true;
        }

        private bool IsAvailableOption(
            EquipmentStatOption selected)
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (IsSameOption(
                        options[i],
                        selected))
                {
                    return true;
                }
            }

            return false;
        }

        //==========================================================
        // Close
        //==========================================================

        public void Close()
        {
            // Important:
            // Closing the UI session must not clear the
            // persisted Tier Up session stored in ItemInstance.

            itemInstance = null;

            sessionInstanceId = null;
            sessionTier = ItemTier.Common;
            sessionUpgradeLevel = 0;

            options = new List<EquipmentStatOption>();

            hasSelection = false;
            selectedOption = default;
        }

        //==========================================================
        // Option Comparison
        //==========================================================

        private static bool IsSameOption(
            EquipmentStatOption first,
            EquipmentStatOption second)
        {
            return first.StatType == second.StatType &&
                   first.ModifierType == second.ModifierType &&
                   Mathf.Approximately(
                       first.RolledValue,
                       second.RolledValue);
        }
    }
}