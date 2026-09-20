using Chaosbound.Content.Materials;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Items.UI.Tooltip
{
    public static class TooltipContentFactory
    {
        public static TooltipContent CreateItemContent(
            ItemInstance item,
            ItemBaseData itemData)
        {
            if (item == null || itemData == null)
                return null;

            return new TooltipContent(
                itemData.Icon,
                itemData.DisplayName,
                GetEquipmentTypeText(
                    itemData.EquipmentType),
                "TIER " + item.CurrentTier,
                string.Empty,
                itemData.Description);
        }

        public static TooltipContent CreateMaterialContent(
            MaterialDefinition material,
            int amount)
        {
            if (material == null)
                return null;

            return new TooltipContent(
                material.Icon,
                material.DisplayName,
                "MATERIAL",
                string.Empty,
                "x" + amount,
                material.Description);
        }

        public static string GetEquipmentTypeText(
            EquipmentType equipmentType)
        {
            switch (equipmentType)
            {
                case EquipmentType.MainWeapon:
                    return "MAIN WEAPON";

                case EquipmentType.Helmet:
                    return "HELMET";

                case EquipmentType.Armor:
                    return "ARMOR";

                case EquipmentType.Pants:
                    return "PANTS";

                case EquipmentType.Boots:
                    return "BOOTS";

                case EquipmentType.Pendant:
                    return "PENDANT";

                case EquipmentType.Ring:
                    return "RING";

                case EquipmentType.Gloves:
                    return "GLOVES";

                case EquipmentType.SpecialRelic:
                    return "SPECIAL RELIC";

                default:
                    return string.Empty;
            }
        }
    }
}