using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.UI.Tooltip;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentComparisonTooltipView : MonoBehaviour
    {
        [Header("Item Information")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text category;
        [SerializeField] private TMP_Text tier;
        [SerializeField] private Image tierFrame;
        [SerializeField] private TMP_Text upgradeLevelText;
        [SerializeField] private TMP_Text quantity;

        [Header("Tier Presentation")]
        [SerializeField] private ItemTierPresentationDatabase tierPresentationDatabase;
        [SerializeField] private TMP_Text description;

        [Header("Comparison Stats")]
        [SerializeField] private EquipmentTooltipStatsView statsView;

        [Header("Comparison Position")]
        [SerializeField]
        private Vector2 comparisonOffset =
            new Vector2(340f, 0f);

        private ItemTooltipPositioner positioner;
        private bool isVisible;

        public bool IsVisible => isVisible;

        private void Awake()
        {
            positioner =
                GetComponent<ItemTooltipPositioner>();
        }

        public void Show(
            EquipmentComparisonTooltipData data,
            EquipmentStatDatabase statDatabase)
        {
            if (data == null)
            {
                Hide();
                return;
            }

            ApplyItemInformation(
                data.EquippedBaseData,
                data.EquippedItem);

            if (statsView != null)
            {
                IReadOnlyList<EquipmentComparisonDisplayEntry> entries =
                    BuildDisplayEntries(data.Rows);

                statsView.ShowComparison(
                    entries,
                    false,
                    statDatabase);
            }

            gameObject.SetActive(true);
            isVisible = true;
        }

        private IReadOnlyList<EquipmentComparisonDisplayEntry> BuildDisplayEntries(
            IReadOnlyList<EquipmentComparisonTooltipRowData> rows)
        {
            List<EquipmentComparisonDisplayEntry> entries =
                new List<EquipmentComparisonDisplayEntry>();

            if (rows == null)
                return entries;

            for (int i = 0; i < rows.Count; i++)
            {
                EquipmentComparisonTooltipRowData row = rows[i];

                EquipmentComparisonDisplayEntry entry =
                    new EquipmentComparisonDisplayEntry(
                        row.StatType,
                        row.HasEquippedValue,
                        row.HasCandidateValue,
                        row.EquippedModifierType,
                        row.CandidateModifierType,
                        row.EquippedValue,
                        row.CandidateValue,
                        row.HasDifference,
                        row.Difference,
                        row.State);

                entries.Add(entry);
            }

            return entries;
        }

        public void PositionAtScreenPoint(Vector2 screenPoint)
        {
            if (positioner == null)
                return;

            positioner.PositionAtScreenPoint(
                screenPoint,
                comparisonOffset);
        }

        public void Hide()
        {
            isVisible = false;
            gameObject.SetActive(false);
        }


        private void ApplyTierPresentation(ItemTier itemTier)
        {
            if (tierPresentationDatabase == null)
            {
                Debug.LogWarning(
                    "[EquipmentComparisonTooltipView] ItemTierPresentationDatabase is not assigned.",
                    this);
                return;
            }

            if (!tierPresentationDatabase.TryGet(
                    itemTier,
                    out ItemTierPresentationDefinition definition))
            {
                Debug.LogWarning(
                    $"[EquipmentComparisonTooltipView] No presentation found for tier: {itemTier}.",
                    this);
                return;
            }

            Color color = definition.NameColor;
            if (tier != null) tier.color = color;
            if (tierFrame != null) tierFrame.color = color;
        }

        private void ApplyItemInformation(
            ItemBaseData baseData,
            ItemInstance item)
        {
            if (baseData == null || item == null)
            {
                return;
            }

            if (itemIcon != null)
            {
                itemIcon.sprite = baseData.Icon;
            }

            if (itemName != null)
            {
                itemName.text = baseData.DisplayName;
            }

            if (category != null)
            {
                category.text =
                    TooltipContentFactory.GetEquipmentTypeText(
                        baseData.EquipmentType);
            }

            if (tier != null)
            {
                tier.text = "TIER " + item.CurrentTier;
            }

            if (upgradeLevelText != null)
            {
                upgradeLevelText.text = item.UpgradeLevel.ToString();
            }

            ApplyTierPresentation(item.CurrentTier);

            if (quantity != null)
            {
                quantity.text = string.Empty;
            }

            if (description != null)
            {
                description.text = baseData.Description;
            }
        }
    }
}