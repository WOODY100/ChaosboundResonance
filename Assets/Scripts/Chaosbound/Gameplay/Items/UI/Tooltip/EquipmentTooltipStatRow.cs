using Chaosbound.Gameplay.Equipment;
using TMPro;
using UnityEngine;

namespace Chaosbound.Gameplay.Items.UI.Tooltip
{
    public sealed class EquipmentTooltipStatRow : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text statName;
        [SerializeField] private TMP_Text value;
        [SerializeField] private TMP_Text difference;

        [Header("Comparison Colors")]
        [SerializeField] private Color improvedColor = Color.green;
        [SerializeField] private Color reducedColor = Color.red;
        [SerializeField] private Color unchangedColor = Color.white;

        public void SetStat(
            string displayName,
            string displayValue)
        {
            if (statName != null)
                statName.text = displayName;

            if (value != null)
            {
                value.text = displayValue;
                value.color = Color.white;
            }

            ClearDifference();
        }

        public void SetComparisonStat(
            string displayName,
            string displayValue,
            string differenceText,
            EquipmentComparisonDisplayState state)
        {
            if (statName != null)
                statName.text = displayName;

            if (value != null)
            {
                value.text = displayValue;
                value.color = Color.white;
            }

            if (difference == null)
                return;

            if (string.IsNullOrEmpty(differenceText))
            {
                ClearDifference();
                return;
            }

            difference.text = differenceText;
            difference.gameObject.SetActive(true);

            switch (state)
            {
                case EquipmentComparisonDisplayState.Improved:
                    difference.color = improvedColor;
                    break;

                case EquipmentComparisonDisplayState.Reduced:
                    difference.color = reducedColor;
                    break;

                case EquipmentComparisonDisplayState.Unchanged:
                    difference.color = unchangedColor;
                    break;

                default:
                    difference.color = Color.white;
                    break;
            }
        }

        public void SetDifference(
            string differenceText)
        {
            if (difference == null)
                return;

            difference.text = differenceText;

            difference.gameObject.SetActive(
                !string.IsNullOrEmpty(differenceText));
        }

        public void ClearDifference()
        {
            if (difference == null)
                return;

            difference.text = string.Empty;
            difference.gameObject.SetActive(false);
        }
    }
}