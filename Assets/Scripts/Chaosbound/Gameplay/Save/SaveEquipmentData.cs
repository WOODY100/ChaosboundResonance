using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Save
{
    [Serializable]
    public sealed class SaveEquipmentData
    {
        public List<SaveEquippedItemData> EquippedItems =
            new List<SaveEquippedItemData>();
    }
}