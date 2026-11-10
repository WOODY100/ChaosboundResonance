using Chaosbound.Content.Items;
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
        [SerializeField] private Image tierFrame;
        [SerializeField] private TMP_Text upgradeLevelText;
        [SerializeField] private TMP_Text quantityText;
        [SerializeField] private TMP_Text description;

        [Header("Tier Presentation")]
        [SerializeField] private ItemTierPresentationDatabase tierPresentationDatabase;

        private ItemTooltipPositioner positioner;
        private bool isVisible;

        public bool IsVisible => isVisible;

        private void Awake()
        {
            positioner = GetComponent<ItemTooltipPositioner>();
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
                itemIcon.enabled = content.Icon != null;
            }

            SetText(itemName, content.DisplayName);
            SetText(category, content.CategoryText);
            SetText(secondaryText, content.SecondaryText);
            SetText(quantityText, content.QuantityText);
            SetText(description, content.Description);

            bool showProgression = content.HasEquipmentProgression;
            if (upgradeLevelText != null)
            {
                upgradeLevelText.text = content.UpgradeLevel.ToString();
                upgradeLevelText.gameObject.SetActive(showProgression);
            }

            if (tierFrame != null)
                tierFrame.gameObject.SetActive(showProgression);

            if (showProgression)
                ApplyTierPresentation(content.Tier);

            gameObject.SetActive(true);
            isVisible = true;
        }

        private static void SetText(TMP_Text target, string value)
        {
            if (target == null) return;
            target.text = value;
            target.gameObject.SetActive(!string.IsNullOrEmpty(value));
        }

        private void ApplyTierPresentation(ItemTier tier)
        {
            if (tierPresentationDatabase == null)
            {
                Debug.LogWarning(
                    "[ItemTooltipView] ItemTierPresentationDatabase is not assigned.", this);
                return;
            }

            if (!tierPresentationDatabase.TryGet(tier, out ItemTierPresentationDefinition definition))
            {
                Debug.LogWarning(
                    $"[ItemTooltipView] No presentation found for tier: {tier}.", this);
                return;
            }

            Color color = definition.NameColor;
            if (secondaryText != null) secondaryText.color = color;
            if (tierFrame != null) tierFrame.color = color;
        }

        public void PositionAtScreenPoint(Vector2 screenPoint)
        {
            if (positioner == null) return;
            positioner.PositionAtScreenPoint(screenPoint);
        }

        public void Hide()
        {
            isVisible = false;
            gameObject.SetActive(false);
        }
    }
}
