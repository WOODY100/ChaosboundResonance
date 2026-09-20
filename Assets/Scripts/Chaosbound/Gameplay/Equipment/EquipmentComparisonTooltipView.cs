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
        [SerializeField] private TMP_Text quantity;
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
            positioner = GetComponent<ItemTooltipPositioner>();
            Hide();
        }

        public void Show(EquipmentComparisonTooltipData data)
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
                    false);
            }

            isVisible = true;
            gameObject.SetActive(true);
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