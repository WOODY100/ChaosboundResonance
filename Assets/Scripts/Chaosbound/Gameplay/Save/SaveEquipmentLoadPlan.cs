using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Items.Runtime;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveEquipmentLoadPlan
    {
        private readonly Dictionary<
            EquipmentType,
            ItemInstance> equippedItems =
            new Dictionary<
                EquipmentType,
                ItemInstance>();

        public IReadOnlyDictionary<
            EquipmentType,
            ItemInstance> EquippedItems =>
                equippedItems;

        public void Add(
            EquipmentType equipmentType,
            ItemInstance itemInstance)
        {
            equippedItems.Add(
                equipmentType,
                itemInstance);
        }
    }
}