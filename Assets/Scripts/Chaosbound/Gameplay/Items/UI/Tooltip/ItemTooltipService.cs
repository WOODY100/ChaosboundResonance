using System;
using System.Collections.Generic;
using UnityEngine;

using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Items.UI.Tooltip
{
    public sealed class ItemTooltipService : MonoBehaviour
    {
        [Header("Tooltip")]
        [SerializeField]
        private ItemTooltipView tooltipView;

        [Header("Hover")]
        [SerializeField]
        private float hoverDelay = 0.5f;

        [Header("Equipment Tooltip")]
        [SerializeField]
        private EquipmentTooltipStatsView equipmentStatsView;

        [Header("Equipment Comparison")]
        [SerializeField]
        private EquipmentComparisonTooltipView comparisonTooltipView;

        [SerializeField]
        private EquipmentStatDatabase equipmentStatDatabase;

        private ItemResolver resolver;

        private Vector2 currentScreenPosition;

        private EquipmentStatResolver equipmentStatResolver;
        private EquipmentTooltipStatBuilder equipmentTooltipStatBuilder;

        private ItemComparisonService itemComparisonService;
        private EquipmentComparisonDisplayBuilder displayBuilder;
        private EquipmentComparisonTooltipBuilder tooltipBuilder;

        public float HoverDelay => hoverDelay;

        public event Action<ItemInstance> OnItemSeen;

        public bool IsVisible
        {
            get
            {
                return tooltipView != null &&
                       tooltipView.IsVisible;
            }
        }

        public ItemInstance CurrentItem { get; private set; }

        public Vector2 CurrentScreenPosition => currentScreenPosition;

        public void Show(
            ItemInstance item,
            Vector2 screenPosition)
        {
            if (item == null)
            {
                Hide();
                return;
            }

            currentScreenPosition = screenPosition;

            if (comparisonTooltipView != null)
            {
                comparisonTooltipView.Hide();
            }

            if (tooltipView == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "ItemTooltipView reference is missing.",
                    this);

                return;
            }

            if (resolver == null)
            {
                ResolveItemDatabase();

                if (resolver == null)
                {
                    Debug.LogError(
                        "[ItemTooltipService] " +
                        "ItemResolver is not available.",
                        this);

                    return;
                }
            }

            if (!resolver.TryResolve(
                    item.BaseDataId,
                    out ItemBaseData itemData))
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "Could not resolve ItemBaseData for ItemInstance: " +
                    item.InstanceId,
                    this);

                return;
            }

            CurrentItem = item;

            TooltipContent content =
                TooltipContentFactory.CreateItemContent(
                    item,
                    itemData);

            if (content == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "Could not create TooltipContent for ItemInstance: " +
                    item.InstanceId,
                    this);

                return;
            }

            /*
             * Equipment stats
             *
             * Only equipment uses the EquipmentTooltipStatsView.
             * Other item categories clear the previous equipment stats.
             */
            if (itemData.Category == ItemCategory.Equipment)
            {
                if (equipmentStatsView != null &&
                    equipmentTooltipStatBuilder != null)
                {
                    List<EquipmentTooltipStatData> stats =
                        equipmentTooltipStatBuilder.Build(
                            itemData,
                            item);

                    equipmentStatsView.Show(stats);
                }
                else
                {
                    if (equipmentStatsView != null)
                    {
                        equipmentStatsView.Clear();
                    }
                }
            }
            else
            {
                if (equipmentStatsView != null)
                {
                    equipmentStatsView.Clear();
                }
            }

            tooltipView.Show(content);

            tooltipView.PositionAtScreenPoint(
                screenPosition);
        }

        public void ShowComparison(
            ItemInstance candidateItem,
            Vector2 screenPosition)
        {
            if (candidateItem == null)
            {
                Hide();
                return;
            }

            if (comparisonTooltipView == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "EquipmentComparisonTooltipView reference is missing.",
                    this);

                Show(
                    candidateItem,
                    screenPosition);

                return;
            }

            if (itemComparisonService == null ||
                displayBuilder == null ||
                tooltipBuilder == null)
            {
                ResolveEquipmentComparison();

                if (itemComparisonService == null)
                {
                    Debug.LogError(
                        "[ItemTooltipService] " +
                        "Equipment comparison services are not available.",
                        this);

                    Show(
                        candidateItem,
                        screenPosition);

                    return;
                }
            }

            if (resolver == null)
            {
                ResolveItemDatabase();

                if (resolver == null)
                {
                    Show(
                        candidateItem,
                        screenPosition);

                    return;
                }
            }

            if (!resolver.TryResolve(
                    candidateItem.BaseDataId,
                    out ItemBaseData candidateBaseData))
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "Could not resolve candidate ItemBaseData.",
                    this);

                Show(
                    candidateItem,
                    screenPosition);

                return;
            }

            if (candidateBaseData == null)
            {
                Show(
                    candidateItem,
                    screenPosition);

                return;
            }

            if (candidateBaseData.Category !=
                ItemCategory.Equipment)
            {
                Show(
                    candidateItem,
                    screenPosition);

                return;
            }

            if (candidateBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Show(
                    candidateItem,
                    screenPosition);

                return;
            }

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "BootstrapContext is not available.",
                    this);

                Show(
                    candidateItem,
                    screenPosition);

                return;
            }

            EquipmentLoadoutRuntime loadout =
                bootstrapContext.EquipmentLoadoutRuntime;

            if (loadout == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "EquipmentLoadoutRuntime is not available.",
                    this);

                Show(
                    candidateItem,
                    screenPosition);

                return;
            }

            if (!loadout.TryGetEquipped(
                    candidateBaseData.EquipmentType,
                    out ItemInstance equippedItem))
            {
                Show(
                    candidateItem,
                    screenPosition);

                return;
            }

            if (equippedItem == null)
            {
                Show(
                    candidateItem,
                    screenPosition);

                return;
            }

            if (!itemComparisonService.TryCompare(
                    candidateItem,
                    equippedItem,
                    out ItemComparisonResult comparisonResult))
            {
                Show(
                    candidateItem,
                    screenPosition);

                return;
            }

            EquipmentComparisonDisplayData displayData =
                displayBuilder.Build(
                    comparisonResult);

            if (displayData == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "Could not create EquipmentComparisonDisplayData.",
                    this);

                Show(
                    candidateItem,
                    screenPosition);

                return;
            }

            EquipmentComparisonTooltipData tooltipData =
                tooltipBuilder.Build(
                    displayData,
                    comparisonResult);

            if (tooltipData == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "Could not create EquipmentComparisonTooltipData.",
                    this);

                Show(
                    candidateItem,
                    screenPosition);

                return;
            }

            CurrentItem = candidateItem;

            comparisonTooltipView.Show(
                tooltipData);

            comparisonTooltipView.PositionAtScreenPoint(
                screenPosition);
        }

        public void Show(
            TooltipContent content,
            Vector2 screenPosition)
        {
            if (content == null)
            {
                Hide();
                return;
            }

            if (tooltipView == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "ItemTooltipView reference is missing.",
                    this);

                return;
            }

            tooltipView.Show(content);

            tooltipView.PositionAtScreenPoint(
                screenPosition);
        }

        public void HideComparison()
        {
            if (comparisonTooltipView == null)
                return;

            comparisonTooltipView.Hide();
        }

        public void Hide()
        {
            CurrentItem = null;

            if (tooltipView != null)
            {
                tooltipView.Hide();
            }

            if (equipmentStatsView != null)
            {
                equipmentStatsView.Clear();
            }

            if (comparisonTooltipView != null)
            {
                comparisonTooltipView.Hide();
            }
        }

        public void CancelPendingShow()
        {
            // Hover timing will be implemented
            // by the hover interaction layer.
        }

        private void Awake()
        {
            if (hoverDelay < 0f)
            {
                Debug.LogWarning(
                    "[ItemTooltipService] " +
                    "Hover Delay cannot be negative. " +
                    "Clamping to zero.",
                    this);

                hoverDelay = 0f;
            }

            if (tooltipView == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "ItemTooltipView reference is missing.",
                    this);
            }
        }

        private void Start()
        {
            ResolveItemDatabase();
            ResolveEquipmentComparison();
        }

        private void ResolveItemDatabase()
        {
            if (resolver != null)
                return;

            GameContentContext contentContext =
                GameContentContext.Current;

            if (contentContext == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "GameContentContext is not available.",
                    this);

                return;
            }

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            if (contentResolver == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "ItemContentResolver is not available in " +
                    "GameContentContext.",
                    this);

                return;
            }

            resolver =
                new ItemResolver(
                    contentResolver);
        }

        private void ResolveEquipmentComparison()
        {
            if (itemComparisonService != null)
                return;

            if (equipmentStatDatabase == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "EquipmentStatDatabase reference is missing.",
                    this);

                return;
            }

            GameContentContext contentContext =
                GameContentContext.Current;

            if (contentContext == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "GameContentContext is not available.",
                    this);

                return;
            }

            ItemContentResolver contentResolver =
                contentContext.ItemContentResolver;

            if (contentResolver == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "ItemContentResolver is not available.",
                    this);

                return;
            }

            equipmentStatResolver =
                new EquipmentStatResolver(
                    equipmentStatDatabase);

            equipmentTooltipStatBuilder =
                new EquipmentTooltipStatBuilder(
                    equipmentStatResolver);

            itemComparisonService =
                new ItemComparisonService(
                    contentResolver,
                    equipmentStatResolver);

            displayBuilder =
                new EquipmentComparisonDisplayBuilder();

            tooltipBuilder =
                new EquipmentComparisonTooltipBuilder();
        }
    }
}