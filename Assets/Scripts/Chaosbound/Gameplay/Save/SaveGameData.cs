using System;

namespace Chaosbound.Gameplay.Save
{
    [Serializable]
    public sealed class SaveGameData
    {
        public int SaveVersion;

        public SaveInventoryData Inventory;

        public SaveEquipmentData Equipment;

        public SaveMetaProgressionData MetaProgression;
    }
}