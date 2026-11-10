using Chaosbound.Content.Items;
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
        public bool HasEquipmentProgression { get; }
        public ItemTier Tier { get; }
        public int UpgradeLevel { get; }

        public TooltipContent(
            Sprite icon,
            string displayName,
            string categoryText,
            string secondaryText,
            string quantityText,
            string description,
            bool hasEquipmentProgression = false,
            ItemTier tier = ItemTier.Common,
            int upgradeLevel = 0)
        {
            Icon = icon;
            DisplayName = displayName;
            CategoryText = categoryText;
            SecondaryText = secondaryText;
            QuantityText = quantityText;
            Description = description;
            HasEquipmentProgression = hasEquipmentProgression;
            Tier = tier;
            UpgradeLevel = upgradeLevel;
        }
    }
}
