using System;
using UnityEngine;
using UnityEngine.InputSystem;

using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Equipment;

namespace Chaosbound.Gameplay.Items.UI.Tooltip
{
    public sealed class ItemTooltipComparisonController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private ItemTooltipService tooltipService;

        [SerializeField]
        private EquipmentStatDatabase statDatabase;

        [Header("Input")]
        [SerializeField]
        private InputActionReference compareAction;

        private ItemComparisonService comparisonService;
        private EquipmentComparisonDisplayBuilder displayBuilder;

        private ItemInstance currentCandidateItem;
        private ItemInstance currentEquippedItem;

        private ItemComparisonResult currentComparisonResult;
        private EquipmentComparisonDisplayData currentDisplayData;

        public bool IsComparisonOpen { get; private set; }

        public ItemInstance CurrentCandidateItem =>
            currentCandidateItem;

        public ItemInstance CurrentEquippedItem =>
            currentEquippedItem;

        public ItemComparisonResult CurrentComparisonResult =>
            currentComparisonResult;

        public EquipmentComparisonDisplayData CurrentDisplayData =>
            currentDisplayData;

        public event Action ComparisonOpened;
        public event Action ComparisonClosed;

        private void Awake()
        {
            if (tooltipService == null)
            {
                Debug.LogError(
                    "[ItemTooltipComparisonController] " +
                    "ItemTooltipService reference is missing.",
                    this);
            }

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[ItemTooltipComparisonController] " +
                    "EquipmentStatDatabase reference is missing.",
                    this);
            }

            displayBuilder =
                new EquipmentComparisonDisplayBuilder();
        }

        private void OnEnable()
        {
            if (compareAction == null)
            {
                Debug.LogError(
                    "[ItemTooltipComparisonController] " +
                    "Compare Action reference is missing.",
                    this);

                return;
            }

            compareAction.action.Enable();

            compareAction.action.performed +=
                OnComparePerformed;
        }

        private void OnDisable()
        {
            if (compareAction == null)
                return;

            compareAction.action.performed -=
                OnComparePerformed;

            compareAction.action.Disable();
        }

        private void OnComparePerformed(
            InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            ToggleComparison();
        }

        public void ToggleComparison()
        {
            if (IsComparisonOpen)
            {
                CloseComparison();
                return;
            }

            TryOpenComparison();
        }

        public bool TryOpenComparison()
        {
            CloseComparisonInternal(false);

            if (tooltipService == null)
                return false;

            if (!tooltipService.IsVisible)
            {
                Debug.Log(
                    "[ItemTooltipComparisonController] " +
                    "Comparison ignored — tooltip is not visible.");

                return false;
            }

            ItemInstance candidateItem =
                tooltipService.CurrentItem;

            if (candidateItem == null)
            {
                Debug.Log(
                    "[ItemTooltipComparisonController] " +
                    "Comparison ignored — no current tooltip item.");

                return false;
            }

            if (statDatabase == null)
                return false;

            GameContentContext contentContext =
                GameContentContext.Current;

            if (contentContext == null)
            {
                Debug.LogError(
                    "[ItemTooltipComparisonController] " +
                    "GameContentContext is unavailable.",
                    this);

                return false;
            }

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            if (contentResolver == null)
            {
                Debug.LogError(
                    "[ItemTooltipComparisonController] " +
                    "ItemContentResolver is unavailable.",
                    this);

                return false;
            }

            if (!contentResolver.TryResolve(
                    candidateItem.BaseDataId,
                    out ItemBaseData candidateBaseData))
            {
                Debug.LogError(
                    "[ItemTooltipComparisonController] " +
                    "Could not resolve candidate ItemBaseData.",
                    this);

                return false;
            }

            if (candidateBaseData == null)
                return false;

            if (candidateBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.Log(
                    "[ItemTooltipComparisonController] " +
                    "Comparison ignored — current item is not Equipment.");

                return false;
            }

            EquipmentType equipmentType =
                candidateBaseData.EquipmentType;

            if (equipmentType == EquipmentType.None)
            {
                Debug.Log(
                    "[ItemTooltipComparisonController] " +
                    "Comparison ignored — EquipmentType is None.");

                return false;
            }

            BootstrapContext bootstrap =
                BootstrapContext.Current;

            if (bootstrap == null)
            {
                Debug.LogError(
                    "[ItemTooltipComparisonController] " +
                    "BootstrapContext is unavailable.",
                    this);

                return false;
            }

            EquipmentLoadoutRuntime loadout =
                bootstrap.EquipmentLoadoutRuntime;

            if (loadout == null)
            {
                Debug.LogError(
                    "[ItemTooltipComparisonController] " +
                    "EquipmentLoadoutRuntime is unavailable.",
                    this);

                return false;
            }

            if (!loadout.TryGetEquipped(
                    equipmentType,
                    out ItemInstance equippedItem))
            {
                Debug.Log(
                    "[ItemTooltipComparisonController] " +
                    "Comparison unavailable — no equipped item " +
                    "for EquipmentType: " +
                    equipmentType);

                return false;
            }

            if (equippedItem == null)
                return false;

            EquipmentStatResolver statResolver =
                new EquipmentStatResolver(
                    statDatabase);

            comparisonService =
                new ItemComparisonService(
                    contentResolver,
                    statResolver);

            if (!comparisonService.TryCompare(
                    candidateItem,
                    equippedItem,
                    out ItemComparisonResult comparisonResult))
            {
                Debug.LogError(
                    "[ItemTooltipComparisonController] " +
                    "ItemComparisonService rejected the comparison.",
                    this);

                return false;
            }

            EquipmentComparisonDisplayData displayData =
                displayBuilder.Build(
                    comparisonResult);

            if (displayData == null)
            {
                Debug.LogError(
                    "[ItemTooltipComparisonController] " +
                    "Comparison display data could not be created.",
                    this);

                return false;
            }

            currentCandidateItem =
                candidateItem;

            currentEquippedItem =
                equippedItem;

            currentComparisonResult =
                comparisonResult;

            currentDisplayData =
                displayData;

            tooltipService.ShowComparison(
                candidateItem,
                tooltipService.CurrentScreenPosition);

            IsComparisonOpen = true;

            ComparisonOpened?.Invoke();

            return true;
        }

        public void CloseComparison()
        {
            if (tooltipService != null)
            {
                tooltipService.HideComparison();
            }

            CloseComparisonInternal(true);
        }

        private void CloseComparisonInternal(
            bool notify)
        {
            bool wasOpen =
                IsComparisonOpen;

            IsComparisonOpen = false;

            currentCandidateItem = null;
            currentEquippedItem = null;
            currentComparisonResult = null;
            currentDisplayData = null;

            if (!wasOpen)
                return;

            if (notify)
                ComparisonClosed?.Invoke();
        }
    }
}