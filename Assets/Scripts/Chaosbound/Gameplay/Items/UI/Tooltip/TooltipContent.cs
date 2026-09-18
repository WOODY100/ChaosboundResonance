using UnityEngine;

namespace Chaosbound.Gameplay.Items.UI.Tooltip
{
    public sealed class TooltipContent
    {
        public Sprite Icon { get; }

        public string DisplayName { get; }

        public string CategoryText { get; }

        public string SecondaryText { get; }

        public string QuantityText { get; }

        public string Description { get; }

        public TooltipContent(
            Sprite icon,
            string displayName,
            string categoryText,
            string secondaryText,
            string quantityText,
            string description)
        {
            Icon = icon;
            DisplayName = displayName;
            CategoryText = categoryText;
            SecondaryText = secondaryText;
            QuantityText = quantityText;
            Description = description;
        }
    }
}