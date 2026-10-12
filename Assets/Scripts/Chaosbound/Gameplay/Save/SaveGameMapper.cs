using System;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.MetaProgression.Persistent;

namespace Chaosbound.Gameplay.Save
{
    public static class SaveGameMapper
    {
        public static SaveGameData ToSaveData(
            int saveVersion,
            PersistentInventoryState inventoryState,
            EquipmentLoadoutRuntime equipmentLoadout,
            PersistentMetaState metaState,
            ExpeditionProgressRuntime expeditionProgress)
        {
            if (saveVersion < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(saveVersion));

            if (inventoryState == null)
                throw new ArgumentNullException(
                    nameof(inventoryState));

            if (equipmentLoadout == null)
                throw new ArgumentNullException(
                    nameof(equipmentLoadout));

            if (metaState == null)
                throw new ArgumentNullException(
                    nameof(metaState));

            if (expeditionProgress == null)
                throw new ArgumentNullException(
                    nameof(expeditionProgress));

            return new SaveGameData
            {
                SaveVersion = saveVersion,

                Inventory =
                    SaveInventoryMapper.ToSaveData(
                        inventoryState),

                Equipment =
                    SaveEquipmentMapper.ToSaveData(
                        equipmentLoadout),

                MetaProgression =
                    SaveMetaProgressionMapper.ToSaveData(
                        metaState),

                ExpeditionProgress =
                    expeditionProgress.ToSaveData()
            };
        }
    }
}
