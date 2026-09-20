using System;
using System.Collections.Generic;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Save
{
    public static class SaveInventoryMapper
    {
        public static SaveInventoryData ToSaveData(
            PersistentInventoryState state)
        {
            if (state == null)
                throw new ArgumentNullException(
                    nameof(state));

            SaveInventoryData data =
                new SaveInventoryData();

            // Items
            IReadOnlyList<ItemInstance> items =
                state.Items.GetItems();

            for (int i = 0; i < items.Count; i++)
            {
                ItemInstance item = items[i];

                if (item == null)
                    continue;

                data.Items.Add(
                    SaveItemInstanceMapper.ToSaveData(item));
            }

            // Materials
            IReadOnlyDictionary<string, int> amounts =
                state.Materials.GetAmounts();

            foreach (
                KeyValuePair<string, int> entry
                in amounts)
            {
                data.Materials ??=
                    new SaveMaterialsData();

                data.Materials.Materials.Add(
                    new SaveMaterialAmountData
                    {
                        ContentId = entry.Key,
                        Amount = entry.Value
                    });
            }

            // Item Seen State
            IReadOnlyCollection<string> seenItemIds =
                state.ItemSeenState.GetSeenIds();

            if (seenItemIds.Count > 0)
            {
                data.ItemSeenState =
                    new SaveSeenIdsData();

                foreach (string instanceId in seenItemIds)
                {
                    data.ItemSeenState.Ids.Add(
                        instanceId);
                }
            }

            // Material Seen State
            IReadOnlyCollection<string> seenMaterialIds =
                state.MaterialSeenState.GetSeenIds();

            if (seenMaterialIds.Count > 0)
            {
                data.MaterialSeenState =
                    new SaveSeenIdsData();

                foreach (string materialId in seenMaterialIds)
                {
                    data.MaterialSeenState.Ids.Add(
                        materialId);
                }
            }

            return data;
        }

        public static void ApplyLoadPlan(
    SaveInventoryLoadPlan loadPlan,
    PersistentInventoryState state)
        {
            if (loadPlan == null)
                throw new ArgumentNullException(
                    nameof(loadPlan));

            if (state == null)
                throw new ArgumentNullException(
                    nameof(state));

            //==========================================================
            // Items
            //==========================================================

            state.Items.Clear();

            for (int i = 0;
                 i < loadPlan.Items.Count;
                 i++)
            {
                ItemInstance item =
                    loadPlan.Items[i];

                if (item == null)
                {
                    throw new InvalidOperationException(
                        "Load plan contains a null ItemInstance.");
                }

                if (!state.Items.TryAdd(item))
                {
                    throw new InvalidOperationException(
                        "Failed to add ItemInstance " +
                        $"'{item.InstanceId}' during inventory load.");
                }
            }

            //==========================================================
            // Materials
            //==========================================================

            state.Materials.Clear();

            foreach (
                KeyValuePair<string, int> entry
                in loadPlan.Materials)
            {
                if (!state.Materials.Add(
                        entry.Key,
                        entry.Value))
                {
                    throw new InvalidOperationException(
                        "Failed to restore material " +
                        $"'{entry.Key}' during inventory load.");
                }
            }

            //==========================================================
            // Item Seen State
            //==========================================================

            state.ItemSeenState.Clear();

            foreach (string instanceId
                     in loadPlan.SeenItemInstanceIds)
            {
                state.ItemSeenState.MarkSeen(
                    instanceId);
            }

            //==========================================================
            // Material Seen State
            //==========================================================

            state.MaterialSeenState.Clear();

            foreach (string materialId
                     in loadPlan.SeenMaterialIds)
            {
                state.MaterialSeenState.MarkSeen(
                    materialId);
            }

            //==========================================================
            // Secure Inventory
            //==========================================================
            //
            // Intentionally untouched.
            //
            // SecureInventory belongs to Expedition and is not part
            // of the persistent SaveGame inventory state.
            //
        }

        public static bool TryPrepareLoad(
            SaveInventoryData data,
            ItemContentResolver contentResolver,
            out SaveInventoryLoadPlan loadPlan)
        {
            loadPlan = null;

            if (data == null)
                return false;

            if (contentResolver == null)
                return false;

            SaveInventoryLoadPlan plan =
                new SaveInventoryLoadPlan();

            //==========================================================
            // Items
            //==========================================================

            if (data.Items == null)
                return false;

            HashSet<string> itemInstanceIds =
                new HashSet<string>(
                    StringComparer.Ordinal);

            for (int i = 0;
                 i < data.Items.Count;
                 i++)
            {
                SaveItemInstanceData savedItem =
                    data.Items[i];

                if (savedItem == null)
                    return false;

                if (string.IsNullOrWhiteSpace(
                        savedItem.InstanceId))
                {
                    return false;
                }

                if (!itemInstanceIds.Add(
                        savedItem.InstanceId))
                {
                    return false;
                }

                if (!SaveItemInstanceMapper.TryFromSaveData(
                        savedItem,
                        contentResolver,
                        out ItemInstance itemInstance))
                {
                    return false;
                }

                plan.Items.Add(itemInstance);
            }

            //==========================================================
            // Materials
            //==========================================================

            if (data.Materials != null)
            {
                HashSet<string> materialIds =
                    new HashSet<string>(
                        StringComparer.Ordinal);

                if (data.Materials.Materials == null)
                    return false;

                for (int i = 0;
                     i < data.Materials.Materials.Count;
                     i++)
                {
                    SaveMaterialAmountData savedMaterial =
                        data.Materials.Materials[i];

                    if (savedMaterial == null)
                        return false;

                    if (string.IsNullOrWhiteSpace(
                            savedMaterial.ContentId))
                    {
                        return false;
                    }

                    if (savedMaterial.Amount < 0)
                        return false;

                    if (!materialIds.Add(
                            savedMaterial.ContentId))
                    {
                        return false;
                    }

                    plan.Materials.Add(
                        savedMaterial.ContentId,
                        savedMaterial.Amount);
                }
            }

            //==========================================================
            // Item Seen State
            //==========================================================

            if (data.ItemSeenState != null)
            {
                if (data.ItemSeenState.Ids == null)
                    return false;

                foreach (string instanceId
                         in data.ItemSeenState.Ids)
                {
                    if (string.IsNullOrWhiteSpace(
                            instanceId))
                    {
                        return false;
                    }

                    if (!plan.SeenItemInstanceIds.Add(
                            instanceId))
                    {
                        return false;
                    }
                }
            }

            //==========================================================
            // Material Seen State
            //==========================================================

            if (data.MaterialSeenState != null)
            {
                if (data.MaterialSeenState.Ids == null)
                    return false;

                foreach (string materialId
                         in data.MaterialSeenState.Ids)
                {
                    if (string.IsNullOrWhiteSpace(
                            materialId))
                    {
                        return false;
                    }

                    if (!plan.SeenMaterialIds.Add(
                            materialId))
                    {
                        return false;
                    }
                }
            }

            loadPlan = plan;
            return true;
        }
    }
}