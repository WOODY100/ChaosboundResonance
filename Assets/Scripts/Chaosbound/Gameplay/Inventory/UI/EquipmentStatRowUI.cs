using TMPro;
using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class EquipmentStatRowUI : MonoBehaviour
    {
        [Header("Text References")]
        [SerializeField]
        private TMP_Text statName;

        [SerializeField]
        private TMP_Text value;

        [Header("Highlight")]
        [SerializeField]
        private Color normalColor = Color.white;

        [SerializeField]
        private Color highlightedColor = Color.green;

        public void SetData(
            string displayName,
            string displayValue,
            bool isHighlighted = false)
        {
            Color targetColor =
                isHighlighted
                    ? highlightedColor
                    : normalColor;

            if (statName != null)
            {
                statName.text = displayName;
                statName.color = targetColor;
            }

            if (value != null)
            {
                value.text = displayValue;
                value.color = targetColor;
            }
        }
    }
}