using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.MetaProgression.Persistent;
using UnityEngine;

namespace Chaosbound.Gameplay.Save
{
    public sealed class BootstrapContextSaveLoadTest : MonoBehaviour
    {
        [SerializeField]
        private ItemBaseData mainWeaponBaseData;

        [SerializeField]
        private ItemBaseData helmetBaseData;

        [ContextMenu("Run Bootstrap Context Save Load Test")]
        private void RunTest()
        {
            BootstrapContext context =
                BootstrapContext.Current;

            if (context == null)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "BootstrapContext.Current is null.");
                return;
            }

            if (mainWeaponBaseData == null ||
                helmetBaseData == null)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Missing test ItemBaseData references.");
                return;
            }

            if (mainWeaponBaseData.Category !=
                ItemCategory.Equipment ||
                helmetBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Test items must be Equipment.");
                return;
            }

            if (mainWeaponBaseData.EquipmentType ==
                EquipmentType.None ||
                helmetBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Test equipment requires valid EquipmentTypes.");
                return;
            }

            if (mainWeaponBaseData.EquipmentType ==
                helmetBaseData.EquipmentType)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Test equipment must use different slots.");
                return;
            }

            PersistentInventoryRuntime inventoryRuntime =
                context.PersistentInventoryRuntime;

            PersistentMetaRuntime metaRuntime =
                context.PersistentMetaRuntime;

            EquipmentLoadoutRuntime equipmentRuntime =
                context.EquipmentLoadoutRuntime;

            if (inventoryRuntime == null ||
                inventoryRuntime.State == null)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "PersistentInventoryRuntime is not ready.");
                return;
            }

            if (metaRuntime == null ||
                metaRuntime.State == null)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "PersistentMetaRuntime is not ready.");
                return;
            }

            if (equipmentRuntime == null)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "EquipmentLoadoutRuntime is not ready.");
                return;
            }

            //==================================================
            // CLEAN INITIAL STATE
            //==================================================

            inventoryRuntime.State.Items.Clear();
            equipmentRuntime.Clear();
            metaRuntime.State.Clear();

            //==================================================
            // CREATE RUNTIME DATA
            //==================================================

            ItemInstance weapon =
                new ItemInstance(
                    "bootstrap-test-weapon",
                    mainWeaponBaseData.ContentId,
                    ItemTier.Rare,
                    3);

            ItemInstance helmet =
                new ItemInstance(
                    "bootstrap-test-helmet",
                    helmetBaseData.ContentId,
                    ItemTier.Uncommon,
                    2);

            if (!inventoryRuntime.State.Items.TryAdd(weapon))
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Failed to add weapon.");
                return;
            }

            if (!inventoryRuntime.State.Items.TryAdd(helmet))
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Failed to add helmet.");
                return;
            }

            if (!equipmentRuntime.TryEquip(
                    weapon,
                    mainWeaponBaseData,
                    out ItemInstance replacedWeapon))
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Failed to equip weapon.");
                return;
            }

            if (replacedWeapon != null)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Unexpected replaced weapon.");
                return;
            }

            if (!equipmentRuntime.TryEquip(
                    helmet,
                    helmetBaseData,
                    out ItemInstance replacedHelmet))
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Failed to equip helmet.");
                return;
            }

            if (replacedHelmet != null)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Unexpected replaced helmet.");
                return;
            }

            metaRuntime.State.AddExperience(4200);

            //==================================================
            // SAVE
            //==================================================

            context.SavePersistentState();

            //==================================================
            // DESTROY CURRENT RUNTIME STATE
            //==================================================

            inventoryRuntime.State.Items.Clear();
            equipmentRuntime.Clear();
            metaRuntime.State.Clear();

            //==================================================
            // LOAD
            //==================================================

            if (!context.LoadPersistentState())
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "LoadPersistentState returned false.");
                return;
            }

            //==================================================
            // VALIDATE INVENTORY
            //==================================================

            if (inventoryRuntime.State.Items.Count != 2)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    $"Inventory count mismatch. " +
                    $"Expected=2, " +
                    $"Actual={inventoryRuntime.State.Items.Count}.");
                return;
            }

            ItemInstance loadedWeapon = null;
            ItemInstance loadedHelmet = null;

            foreach (ItemInstance item in
                     inventoryRuntime.State.Items.GetItems())
            {
                if (item == null)
                    continue;

                if (item.InstanceId == weapon.InstanceId)
                    loadedWeapon = item;

                if (item.InstanceId == helmet.InstanceId)
                    loadedHelmet = item;
            }

            if (loadedWeapon == null)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Weapon was not restored.");
                return;
            }

            if (loadedHelmet == null)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Helmet was not restored.");
                return;
            }

            //==================================================
            // VALIDATE EQUIPMENT
            //==================================================

            if (!equipmentRuntime.TryGetEquipped(
                    mainWeaponBaseData.EquipmentType,
                    out ItemInstance equippedWeapon))
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Weapon was not equipped after load.");
                return;
            }

            if (!equipmentRuntime.TryGetEquipped(
                    helmetBaseData.EquipmentType,
                    out ItemInstance equippedHelmet))
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Helmet was not equipped after load.");
                return;
            }

            if (equippedWeapon != loadedWeapon)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Equipment weapon is not the same ItemInstance " +
                    "held by Inventory.");
                return;
            }

            if (equippedHelmet != loadedHelmet)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    "Equipment helmet is not the same ItemInstance " +
                    "held by Inventory.");
                return;
            }

            //==================================================
            // VALIDATE META
            //==================================================

            if (metaRuntime.State.Experience != 4200)
            {
                Debug.LogError(
                    "[Bootstrap Context Save Load Test] " +
                    $"Meta experience mismatch. " +
                    $"Expected=4200, " +
                    $"Actual={metaRuntime.State.Experience}.");
                return;
            }

            Debug.Log(
                "[Bootstrap Context Save Load Test] " +
                "✓ COMPLETE — BootstrapContext successfully " +
                "saved and restored the real Inventory, " +
                "Equipment, and Meta runtime instances.");
        }
    }
}