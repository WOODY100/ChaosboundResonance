using System;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.MetaProgression.Persistent;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveGameService
    {
        private readonly ISaveStorage storage;
        private readonly int saveVersion;

        public SaveGameService(
            ISaveStorage storage,
            int saveVersion)
        {
            this.storage =
                storage ??
                throw new ArgumentNullException(
                    nameof(storage));

            if (saveVersion < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(saveVersion));
            }

            this.saveVersion =
                saveVersion;
        }

        public void Save(
            PersistentInventoryState inventoryState,
            EquipmentLoadoutRuntime equipmentLoadout,
            PersistentMetaState metaState)
        {
            SaveGameData saveData =
                SaveGameMapper.ToSaveData(
                    saveVersion,
                    inventoryState,
                    equipmentLoadout,
                    metaState);

            string json =
                SaveGameJsonSerializer.Serialize(
                    saveData);

            storage.Save(
                json);
        }

        public bool TryPrepareLoad(
            SaveGameData saveData,
            out SaveGameLoadPlan loadPlan)
        {
            loadPlan = null;

            if (saveData == null)
                return false;

            if (saveData.SaveVersion != saveVersion)
                return false;

            if (saveData.Inventory == null)
                return false;

            if (saveData.Equipment == null)
                return false;

            if (saveData.MetaProgression == null)
                return false;

            GameContentContext context =
                GameContentContext.Current;

            if (context == null)
                return false;

            if (context.ItemContentResolver == null)
                return false;

            if (!SaveInventoryMapper.TryPrepareLoad(
                    saveData.Inventory,
                    context.ItemContentResolver,
                    out SaveInventoryLoadPlan inventoryLoadPlan))
            {
                return false;
            }

            if (!SaveEquipmentMapper.TryPrepareLoad(
                    saveData.Equipment,
                    inventoryLoadPlan,
                    context.ItemContentResolver,
                    out SaveEquipmentLoadPlan equipmentLoadPlan))
            {
                return false;
            }

            if (saveData.MetaProgression.Experience < 0)
                return false;

            loadPlan =
                new SaveGameLoadPlan(
                    inventoryLoadPlan,
                    equipmentLoadPlan,
                    saveData.MetaProgression.Experience);

            return true;
        }

        public void ApplyLoadPlan(
            SaveGameLoadPlan loadPlan,
            PersistentInventoryState inventoryState,
            EquipmentLoadoutRuntime equipmentLoadout,
            PersistentMetaState metaState)
        {
            if (loadPlan == null)
                throw new ArgumentNullException(nameof(loadPlan));

            if (inventoryState == null)
                throw new ArgumentNullException(nameof(inventoryState));

            if (equipmentLoadout == null)
                throw new ArgumentNullException(nameof(equipmentLoadout));

            if (metaState == null)
                throw new ArgumentNullException(nameof(metaState));

            SaveInventoryMapper.ApplyLoadPlan(
                loadPlan.Inventory,
                inventoryState);

            SaveEquipmentMapper.ApplyLoadPlan(
                loadPlan.Equipment,
                equipmentLoadout);

            SaveMetaProgressionMapper.ApplyLoadPlan(
                loadPlan.MetaExperience,
                metaState);
        }

        public bool TryLoad(
    out SaveGameLoadPlan loadPlan)
        {
            loadPlan = null;

            if (!storage.TryLoad(out string json))
                return false;

            if (string.IsNullOrWhiteSpace(json))
                return false;

            SaveGameData saveData;

            try
            {
                saveData = SaveGameJsonSerializer.Deserialize(json);
            }
            catch
            {
                return false;
            }

            return TryPrepareLoad(
                saveData,
                out loadPlan);
        }
    }
}