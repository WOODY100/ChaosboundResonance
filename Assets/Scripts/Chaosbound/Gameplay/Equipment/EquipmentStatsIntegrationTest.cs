using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentStatsIntegrationTest : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private PlayerModifierSystem playerModifierSystem;

        [Header("Equipment")]
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private EquipmentStatDatabase statDatabase;
        [SerializeField] private EquipmentProgressionConfig progressionConfig;
        [SerializeField] private ItemBaseData equipmentBaseData;

        [ContextMenu("Run Equipment Stats Integration Test")]
        private void RunTest()
        {
            if (playerModifierSystem == null)
            {
                Debug.LogError(
                    "[Equipment Integration Test] Missing " +
                    "PlayerModifierSystem.");
                return;
            }

            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Integration Test] Missing ItemDatabase.");
                return;
            }

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Integration Test] Missing " +
                    "EquipmentStatDatabase.");
                return;
            }

            if (equipmentBaseData == null)
            {
                Debug.LogError(
                    "[Equipment Integration Test] Missing " +
                    "Equipment ItemBaseData.");
                return;
            }

            if (equipmentBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Equipment Integration Test] ItemBaseData " +
                    "is not Equipment.");
                return;
            }

            if (equipmentBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Equipment Integration Test] EquipmentType " +
                    "is None.");
                return;
            }

            if (!itemDatabase.TryGet(
                    equipmentBaseData.ContentId,
                    out ItemBaseData resolvedBaseData))
            {
                Debug.LogError(
                    $"[Equipment Integration Test] Could not resolve " +
                    $"'{equipmentBaseData.ContentId}' from ItemDatabase.");
                return;
            }

            if (resolvedBaseData != equipmentBaseData)
            {
                Debug.LogError(
                    "[Equipment Integration Test] ItemDatabase resolved " +
                    "a different ItemBaseData.");
                return;
            }

            if (progressionConfig == null)
            {
                Debug.LogError(
                    "[Equipment Integration Test] Missing " +
                    "EquipmentProgressionConfig.");
                return;
            }

            EquipmentLoadoutRuntime loadout =
                new EquipmentLoadoutRuntime();

            EquipmentStatResolver statResolver =
                new EquipmentStatResolver(
                    statDatabase);

            EquipmentModifierSourceBuilder sourceBuilder =
                new EquipmentModifierSourceBuilder(
                    itemDatabase,
                    statResolver);

            EquipmentTierOptionGenerator optionGenerator =
                new EquipmentTierOptionGenerator(
                    statDatabase,
                    progressionConfig.OptionsPerTier);

            EquipmentProgressionRuntime progressionRuntime =
                new EquipmentProgressionRuntime(
                    progressionConfig,
                    optionGenerator);

            EquipmentStatsIntegration integration =
                new EquipmentStatsIntegration(
                    playerModifierSystem,
                    loadout,
                    sourceBuilder,
                    progressionRuntime);

            try
            {
                RunEquipmentChangedTest(
                    playerModifierSystem,
                    loadout,
                    progressionRuntime,
                    equipmentBaseData);
            }
            finally
            {
                integration.Dispose();
            }
        }

        private static void RunEquipmentChangedTest(
    PlayerModifierSystem playerModifierSystem,
    EquipmentLoadoutRuntime loadout,
    EquipmentProgressionRuntime progressionRuntime,
    ItemBaseData equipmentBaseData)
        {
            ItemInstance equippedItem =
                new ItemInstance(
                    "equipment-equipped-test-instance",
                    equipmentBaseData.ContentId,
                    ItemTier.Common);

            float damageBefore =
                playerModifierSystem.GetStat(
                    StatType.Damage);

            Debug.Log(
                $"[Equipment Integration Test] Damage before Equipment = " +
                $"{damageBefore}");

            if (!loadout.TryEquip(
                    equippedItem,
                    equipmentBaseData,
                    out _))
            {
                Debug.LogError(
                    "[Equipment Integration Test] Failed to equip test item.");
                return;
            }

            float damageAfterEquip =
                playerModifierSystem.GetStat(
                    StatType.Damage);

            Debug.Log(
                $"[Equipment Integration Test] Damage after automatic " +
                $"Equip refresh = {damageAfterEquip}");

            float expectedDamage =
                damageBefore;

            for (int i = 0;
                 i < equipmentBaseData.BaseStats.Count;
                 i++)
            {
                EquipmentBaseStat baseStat =
                    equipmentBaseData.BaseStats[i];

                if (baseStat.StatType != StatType.Damage)
                    continue;

                if (baseStat.ModifierType != ModifierType.Flat)
                {
                    Debug.LogError(
                        "[Equipment Integration Test] Damage BaseStat " +
                        "must be Flat for this test.");
                    return;
                }

                expectedDamage +=
                    baseStat.Value;

                break;
            }

            if (!Mathf.Approximately(
                    damageAfterEquip,
                    expectedDamage))
            {
                Debug.LogError(
                    $"[Equipment Integration Test] Damage mismatch after " +
                    $"Equip. Expected={expectedDamage}, " +
                    $"Actual={damageAfterEquip}.");
                return;
            }

            Debug.Log(
                "[Equipment Integration Test] Automatic Equip refresh verified.");

            if (!progressionRuntime.TryUpgrade(equippedItem))
            {
                Debug.LogError(
                    "[Equipment Integration Test] Equipped item Upgrade failed.");
                return;
            }

            Debug.Log(
                "[Equipment Integration Test] Equipped item Upgrade completed. " +
                "ProgressionChanged should have triggered automatic Refresh.");

            if (!loadout.IsEquipped(equippedItem))
            {
                Debug.LogError(
                    "[Equipment Integration Test] Equipped item is no longer " +
                    "recognized as equipped.");
                return;
            }

            Debug.Log(
                "[Equipment Integration Test] Equipped item correctly remains " +
                "recognized by Loadout after Upgrade.");

            if (!loadout.TryUnequip(
                    equipmentBaseData.EquipmentType,
                    out ItemInstance removedItem))
            {
                Debug.LogError(
                    "[Equipment Integration Test] Failed to unequip test item.");
                return;
            }

            if (removedItem != equippedItem)
            {
                Debug.LogError(
                    "[Equipment Integration Test] Removed item does not match " +
                    "the equipped test instance.");
                return;
            }

            float damageAfterUnequip =
                playerModifierSystem.GetStat(
                    StatType.Damage);

            if (!Mathf.Approximately(
                    damageAfterUnequip,
                    damageBefore))
            {
                Debug.LogError(
                    $"[Equipment Integration Test] Damage did not return " +
                    $"to baseline after Unequip. " +
                    $"Expected={damageBefore}, " +
                    $"Actual={damageAfterUnequip}.");
                return;
            }

            Debug.Log(
                "[Equipment Integration Test] Automatic Unequip refresh verified.");

            ItemInstance inventoryItem =
                new ItemInstance(
                    "equipment-inventory-test-instance",
                    equipmentBaseData.ContentId,
                    ItemTier.Common);

            if (!progressionRuntime.TryUpgrade(inventoryItem))
            {
                Debug.LogError(
                    "[Equipment Integration Test] Inventory item Upgrade failed.");
                return;
            }

            if (loadout.IsEquipped(inventoryItem))
            {
                Debug.LogError(
                    "[Equipment Integration Test] Inventory item should not " +
                    "be recognized as equipped.");
                return;
            }

            Debug.Log(
                "[Equipment Integration Test] Non-equipped item correctly " +
                "remains outside the Equipment Loadout after Upgrade.");

            float damageAfterInventoryUpgrade =
                playerModifierSystem.GetStat(
                    StatType.Damage);

            if (!Mathf.Approximately(
                    damageAfterInventoryUpgrade,
                    damageBefore))
            {
                Debug.LogError(
                    $"[Equipment Integration Test] Player Damage changed after " +
                    $"upgrading a non-equipped item. " +
                    $"Expected={damageBefore}, " +
                    $"Actual={damageAfterInventoryUpgrade}.");
                return;
            }

            Debug.Log(
                "[Equipment Integration Test] Non-equipped item correctly " +
                "does not affect PlayerModifierSystem.");

            Debug.Log(
                "[Equipment Integration Test] ✓ COMPLETE — " +
                "Loadout and progression events are correctly filtered.");
        }

        private static bool TryCalculateExpectedDamage(
            float damageBefore,
            ItemBaseData baseData,
            out float expectedDamage)
        {
            expectedDamage = damageBefore;

            if (baseData.BaseStats == null)
                return true;

            for (int i = 0;
                 i < baseData.BaseStats.Count;
                 i++)
            {
                EquipmentBaseStat baseStat =
                    baseData.BaseStats[i];

                if (baseStat.StatType !=
                    StatType.Damage)
                {
                    continue;
                }

                if (baseStat.ModifierType !=
                    ModifierType.Flat)
                {
                    return false;
                }

                expectedDamage +=
                    baseStat.Value;

                break;
            }

            return true;
        }
    }
}