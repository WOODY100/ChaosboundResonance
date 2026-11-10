using System;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.MetaProgression.Persistent;
using Chaosbound.Gameplay.Currency.Persistent;

namespace Chaosbound.Gameplay.Save
{
    public static class SaveGameMapper
    {
        public static SaveGameData ToSaveData(
            int saveVersion,
            PersistentInventoryState inventoryState,
            EquipmentLoadoutRuntime equipmentLoadout,
            PersistentMetaState metaState,
            SkillTreeDatabase skillTreeDatabase,
            SkillTreeRuntimeState skillTreeState,
            ExpeditionProgressRuntime expeditionProgress,
            PersistentCurrencyState currencyState,
            SaveSanctuaryPlayerData sanctuaryPlayerData)
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

            if (skillTreeDatabase == null)
                throw new ArgumentNullException(nameof(skillTreeDatabase));

            if (skillTreeState == null)
                throw new ArgumentNullException(nameof(skillTreeState));

            if (expeditionProgress == null)
                throw new ArgumentNullException(nameof(expeditionProgress));

            if (currencyState == null)
                throw new ArgumentNullException(nameof(currencyState));

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

                SkillTree =
                    SaveSkillTreeMapper.ToSaveData(
                        skillTreeDatabase,
                        skillTreeState),

                ExpeditionProgress =
                    expeditionProgress.ToSaveData(),

                Currency =
                    SaveCurrencyMapper.ToSaveData(
                        currencyState),

                SanctuaryPlayer = sanctuaryPlayerData
            };
        }
    }
}
