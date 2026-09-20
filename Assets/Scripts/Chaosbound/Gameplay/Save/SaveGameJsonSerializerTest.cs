using Chaosbound.Content.Items;
using UnityEngine;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveGameJsonSerializerTest
        : MonoBehaviour
    {
        [ContextMenu("Run Save Game JSON Serializer Test")]
        private void RunTest()
        {
            //==================================================
            // Build SaveGameData
            //==================================================

            SaveGameData original =
                new SaveGameData
                {
                    SaveVersion = 1,

                    Inventory =
                        new SaveInventoryData(),

                    Equipment =
                        new SaveEquipmentData(),

                    MetaProgression =
                        new SaveMetaProgressionData
                        {
                            Experience = 2500
                        }
                };

            //==================================================
            // Inventory
            //==================================================

            original.Inventory.Materials =
                new SaveMaterialsData();

            original.Inventory.Materials.Materials.Add(
                new SaveMaterialAmountData
                {
                    ContentId = "wood",
                    Amount = 125
                });

            original.Inventory.ItemSeenState =
                new SaveSeenIdsData();

            original.Inventory.ItemSeenState.Ids.Add(
                "serializer-test-item");

            original.Inventory.MaterialSeenState =
                new SaveSeenIdsData();

            original.Inventory.MaterialSeenState.Ids.Add(
                "wood");

            //==================================================
            // Equipment
            //==================================================

            original.Equipment.EquippedItems.Add(
                new SaveEquippedItemData
                {
                    EquipmentType =
                        EquipmentType.MainWeapon,

                    ItemInstanceId =
                        "serializer-test-item"
                });

            //==================================================
            // Serialize
            //==================================================

            string json;

            try
            {
                json =
                    SaveGameJsonSerializer.Serialize(
                        original);
            }
            catch (System.Exception exception)
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Serialization failed. " +
                    exception);
                return;
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Serialized JSON is empty.");
                return;
            }

            //==================================================
            // Deserialize
            //==================================================

            SaveGameData restored;

            try
            {
                restored =
                    SaveGameJsonSerializer.Deserialize(
                        json);
            }
            catch (System.Exception exception)
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Deserialization failed. " +
                    exception);
                return;
            }

            if (restored == null)
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Restored SaveGameData is null.");
                return;
            }

            //==================================================
            // Save Version
            //==================================================

            if (restored.SaveVersion !=
                original.SaveVersion)
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "SaveVersion mismatch.");
                return;
            }

            //==================================================
            // Root Domains
            //==================================================

            if (restored.Inventory == null)
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Inventory was not restored.");
                return;
            }

            if (restored.Equipment == null)
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Equipment was not restored.");
                return;
            }

            if (restored.MetaProgression == null)
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Meta Progression was not restored.");
                return;
            }

            //==================================================
            // Inventory Materials
            //==================================================

            if (restored.Inventory.Materials == null)
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Inventory Materials were not restored.");
                return;
            }

            if (restored.Inventory.Materials.Materials.Count != 1)
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Material count mismatch.");
                return;
            }

            SaveMaterialAmountData restoredMaterial =
                restored.Inventory.Materials.Materials[0];

            if (restoredMaterial.ContentId != "wood" ||
                restoredMaterial.Amount != 125)
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Material data mismatch.");
                return;
            }

            //==================================================
            // Inventory Seen State
            //==================================================

            if (restored.Inventory.ItemSeenState == null ||
                restored.Inventory.ItemSeenState.Ids.Count != 1 ||
                restored.Inventory.ItemSeenState.Ids[0] !=
                    "serializer-test-item")
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Item Seen State mismatch.");
                return;
            }

            if (restored.Inventory.MaterialSeenState == null ||
                restored.Inventory.MaterialSeenState.Ids.Count != 1 ||
                restored.Inventory.MaterialSeenState.Ids[0] !=
                    "wood")
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Material Seen State mismatch.");
                return;
            }

            //==================================================
            // Equipment
            //==================================================

            if (restored.Equipment.EquippedItems.Count != 1)
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Equipment count mismatch.");
                return;
            }

            SaveEquippedItemData restoredEquipment =
                restored.Equipment.EquippedItems[0];

            if (restoredEquipment.ItemInstanceId !=
                "serializer-test-item")
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Equipment ItemInstanceId mismatch.");
                return;
            }

            if (restoredEquipment.EquipmentType !=
                EquipmentType.MainWeapon)
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "EquipmentType mismatch.");
                return;
            }

            //==================================================
            // Meta Progression
            //==================================================

            if (restored.MetaProgression.Experience !=
                2500)
            {
                Debug.LogError(
                    "[Save Game JSON Serializer Test] " +
                    "Meta Experience mismatch.");
                return;
            }

            //==================================================
            // Complete
            //==================================================

            Debug.Log(
                "[Save Game JSON Serializer Test] " +
                "✓ COMPLETE — SaveGameData survived JSON " +
                "serialization and deserialization with all " +
                "tested persistent data preserved.");
        }
    }
}