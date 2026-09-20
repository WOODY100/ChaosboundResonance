using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveEquipmentMapperTest
        : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private ItemBaseData mainWeaponBaseData;
        [SerializeField] private ItemBaseData helmetBaseData;

        [ContextMenu("Run Save Equipment Mapper Test")]
        private void RunTest()
        {
            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Missing ItemDatabase.");
                return;
            }

            if (mainWeaponBaseData == null)
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Missing Main Weapon ItemBaseData.");
                return;
            }

            if (helmetBaseData == null)
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Missing Helmet ItemBaseData.");
                return;
            }

            //==================================================
            // Validate Test Content
            //==================================================

            if (!itemDatabase.TryGet(
                    mainWeaponBaseData.ContentId,
                    out ItemBaseData resolvedWeapon))
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    $"Could not resolve weapon " +
                    $"'{mainWeaponBaseData.ContentId}'.");
                return;
            }

            if (resolvedWeapon != mainWeaponBaseData)
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Weapon resolved to a different " +
                    "ItemBaseData.");
                return;
            }

            if (!itemDatabase.TryGet(
                    helmetBaseData.ContentId,
                    out ItemBaseData resolvedHelmet))
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    $"Could not resolve helmet " +
                    $"'{helmetBaseData.ContentId}'.");
                return;
            }

            if (resolvedHelmet != helmetBaseData)
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Helmet resolved to a different " +
                    "ItemBaseData.");
                return;
            }

            if (mainWeaponBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Main weapon has EquipmentType.None.");
                return;
            }

            if (helmetBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Helmet has EquipmentType.None.");
                return;
            }

            if (mainWeaponBaseData.EquipmentType ==
                helmetBaseData.EquipmentType)
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Test items use the same EquipmentType.");
                return;
            }

            //==================================================
            // Create ItemInstances
            //==================================================

            ItemInstance weapon =
                new ItemInstance(
                    "equipment-save-test-weapon",
                    mainWeaponBaseData.ContentId,
                    ItemTier.Rare,
                    3);

            ItemInstance helmet =
                new ItemInstance(
                    "equipment-save-test-helmet",
                    helmetBaseData.ContentId,
                    ItemTier.Uncommon,
                    2);

            //==================================================
            // Create Loadout
            //==================================================

            EquipmentLoadoutRuntime loadout =
                new EquipmentLoadoutRuntime();

            if (!loadout.TryEquip(
                    weapon,
                    mainWeaponBaseData,
                    out ItemInstance replacedWeapon))
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Failed to equip weapon.");
                return;
            }

            if (replacedWeapon != null)
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Weapon unexpectedly replaced another item.");
                return;
            }

            if (!loadout.TryEquip(
                    helmet,
                    helmetBaseData,
                    out ItemInstance replacedHelmet))
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Failed to equip helmet.");
                return;
            }

            if (replacedHelmet != null)
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Helmet unexpectedly replaced another item.");
                return;
            }

            //==================================================
            // Map
            //==================================================

            SaveEquipmentData saveData =
                SaveEquipmentMapper.ToSaveData(
                    loadout);

            if (saveData == null)
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Save data is null.");
                return;
            }

            //==================================================
            // Validate Count
            //==================================================

            if (saveData.EquippedItems == null)
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "EquippedItems is null.");
                return;
            }

            if (saveData.EquippedItems.Count != 2)
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    $"Invalid equipped item count. " +
                    $"Expected=2, " +
                    $"Actual={saveData.EquippedItems.Count}.");
                return;
            }

            //==================================================
            // Validate Weapon
            //==================================================

            if (!ContainsEquipment(
                    saveData,
                    mainWeaponBaseData.EquipmentType,
                    weapon.InstanceId))
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Weapon equipment reference was not saved " +
                    "correctly.");
                return;
            }

            //==================================================
            // Validate Helmet
            //==================================================

            if (!ContainsEquipment(
                    saveData,
                    helmetBaseData.EquipmentType,
                    helmet.InstanceId))
            {
                Debug.LogError(
                    "[Save Equipment Mapper Test] " +
                    "Helmet equipment reference was not saved " +
                    "correctly.");
                return;
            }

            //==================================================
            // Complete
            //==================================================

            Debug.Log(
                "[Save Equipment Mapper Test] " +
                "✓ COMPLETE — Equipment loadout was mapped " +
                "correctly to ItemInstance references.");
        }

        private static bool ContainsEquipment(
            SaveEquipmentData data,
            EquipmentType equipmentType,
            string instanceId)
        {
            for (int i = 0;
                 i < data.EquippedItems.Count;
                 i++)
            {
                SaveEquippedItemData entry =
                    data.EquippedItems[i];

                if (entry == null)
                    continue;

                if (entry.EquipmentType ==
                        equipmentType &&
                    entry.ItemInstanceId ==
                        instanceId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}