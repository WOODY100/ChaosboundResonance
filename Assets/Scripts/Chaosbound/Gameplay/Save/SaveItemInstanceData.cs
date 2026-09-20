using Chaosbound.Content.Items;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Save
{
    [Serializable]
    public sealed class SaveItemInstanceData
    {
        public string InstanceId;
        public string BaseDataId;
        public ItemTier CurrentTier;
        public int UpgradeLevel;

        public List<SaveEquipmentRolledStatData> UnlockedStats =
            new List<SaveEquipmentRolledStatData>();
    }
}