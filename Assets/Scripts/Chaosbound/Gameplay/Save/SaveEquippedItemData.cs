using Chaosbound.Content.Items;
using System;

namespace Chaosbound.Gameplay.Save
{
    [Serializable]
    public sealed class SaveEquippedItemData
    {
        public EquipmentType EquipmentType;
        public string ItemInstanceId;
    }
}