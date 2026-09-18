using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Items.UI.Tooltip
{
    public sealed class ItemTooltipView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text category;
        [SerializeField] private TMP_Text secondaryText;
        [SerializeField] private TMP_Text quantityText;
        [SerializeField] private TMP_Text description;

        private ItemTooltipPositioner positioner;
        private bool isVisible;

        public bool IsVisible => isVisible;

        private void Awake()
        {
            positioner =
                GetComponent<ItemTooltipPositioner>();

            Hide();
        }

        public void Show(TooltipContent content)
        {
            if (content == null)
            {
                Hide();
                return;
            }

            if (itemIcon != null)
            {
                itemIcon.sprite = content.Icon;
                itemIcon.enabled =
                    content.Icon != null;
            }

            if (itemName != null)
            {
                itemName.text =
                    content.DisplayName;

                itemName.gameObject.SetActive(
                    !string.IsNullOrEmpty(
                        content.DisplayName));
            }

            if (category != null)
            {
                category.text =
                    content.CategoryText;

                category.gameObject.SetActive(
                    !string.IsNullOrEmpty(
                        content.CategoryText));
            }

            if (secondaryText != null)
            {
                secondaryText.text =
                    content.SecondaryText;

                secondaryText.gameObject.SetActive(
                    !string.IsNullOrEmpty(
                        content.SecondaryText));
            }

            if (quantityText != null)
            {
                quantityText.text =
                    content.QuantityText;

                quantityText.gameObject.SetActive(
                    !string.IsNullOrEmpty(
                        content.QuantityText));
            }

            if (description != null)
            {
                description.text =
                    content.Description;

                description.gameObject.SetActive(
                    !string.IsNullOrEmpty(
                        content.Description));
            }

            isVisible = true;
            gameObject.SetActive(true);
        }

        public void PositionAtScreenPoint(
            Vector2 screenPoint)
        {
            if (positioner == null)
                return;

            positioner.PositionAtScreenPoint(
                screenPoint);
        }

        public void Hide()
        {
            isVisible = false;
            gameObject.SetActive(false);
        }
    }
}