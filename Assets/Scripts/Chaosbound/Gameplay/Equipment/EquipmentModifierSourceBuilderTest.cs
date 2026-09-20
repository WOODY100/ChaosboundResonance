using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentModifierSourceBuilderTest : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private EquipmentStatDatabase statDatabase;
        [SerializeField] private ItemBaseData equipmentBaseData;

        [ContextMenu("Run Equipment Modifier Source Test")]
        private void RunTest()
        {
            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Modifier Test] Missing ItemDatabase.");
                return;
            }

            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Modifier Test] Missing EquipmentStatDatabase.");
                return;
            }

            if (equipmentBaseData == null)
            {
                Debug.LogError(
                    "[Equipment Modifier Test] Missing Equipment ItemBaseData.");
                return;
            }

            if (equipmentBaseData.Category !=
                ItemCategory.Equipment)
            {
                Debug.LogError(
                    "[Equipment Modifier Test] ItemBaseData is not Equipment.");
                return;
            }

            if (equipmentBaseData.EquipmentType ==
                EquipmentType.None)
            {
                Debug.LogError(
                    "[Equipment Modifier Test] EquipmentType is None.");
                return;
            }

            if (!itemDatabase.TryGet(
                    equipmentBaseData.ContentId,
                    out ItemBaseData resolvedBaseData))
            {
                Debug.LogError(
                    $"[Equipment Modifier Test] ItemDatabase could not " +
                    $"resolve '{equipmentBaseData.ContentId}'.");
                return;
            }

            if (resolvedBaseData != equipmentBaseData)
            {
                Debug.LogError(
                    "[Equipment Modifier Test] ItemDatabase resolved " +
                    "a different ItemBaseData.");
                return;
            }

            EquipmentLoadoutRuntime loadout =
                new EquipmentLoadoutRuntime();

            EquipmentStatResolver statResolver =
                new EquipmentStatResolver(
                    statDatabase);

            EquipmentModifierSourceBuilder builder =
                new EquipmentModifierSourceBuilder(
                    itemDatabase,
                    statResolver);

            ItemInstance itemInstance =
                new ItemInstance(
                    "equipment-modifier-test-instance",
                    equipmentBaseData.ContentId,
                    equipmentBaseData.BaseTier);

            if (!TryAddTestUnlockedStat(
                    itemInstance,
                    equipmentBaseData,
                    statDatabase,
                    out EquipmentStatDefinition unlockedDefinition))
            {
                Debug.LogError(
                    "[Equipment Modifier Test] Could not create " +
                    "test UnlockedStat.");
                return;
            }

            const float testRolledValue = 10f;

            itemInstance =
                CreateItemWithUnlockedStat(
                    equipmentBaseData,
                    unlockedDefinition,
                    testRolledValue);

            if (!loadout.TryEquip(
                    itemInstance,
                    equipmentBaseData,
                    out _))
            {
                Debug.LogError(
                    "[Equipment Modifier Test] Failed to equip test item.");
                return;
            }

            if (!builder.TryBuild(
                    loadout,
                    out ModifierSource source))
            {
                Debug.LogError(
                    "[Equipment Modifier Test] Failed to build " +
                    "Equipment ModifierSource.");
                return;
            }

            if (source == null)
            {
                Debug.LogError(
                    "[Equipment Modifier Test] ModifierSource is null.");
                return;
            }

            if (source.SourceID !=
                EquipmentModifierSourceBuilder.SourceId)
            {
                Debug.LogError(
                    $"[Equipment Modifier Test] Invalid SourceID. " +
                    $"Expected={EquipmentModifierSourceBuilder.SourceId}, " +
                    $"Actual={source.SourceID}");
                return;
            }

            Debug.Log(
                $"[Equipment Modifier Test] Source created: " +
                $"SourceID={source.SourceID}");

            Debug.Log(
                $"[Equipment Modifier Test] Modifier count: " +
                $"{source.Modifiers.Count}");

            TestBaseStats(
                source,
                equipmentBaseData);

            TestUnlockedStat(
                source,
                itemInstance,
                unlockedDefinition,
                testRolledValue);

            Debug.Log(
                "[Equipment Modifier Test] ✓ COMPLETE — " +
                "Equipment ModifierSource is working correctly.");
        }

        private static ItemInstance CreateItemWithUnlockedStat(
            ItemBaseData baseData,
            EquipmentStatDefinition definition,
            float rolledValue)
        {
            ItemInstance itemInstance =
                new ItemInstance(
                    "equipment-modifier-test-instance",
                    baseData.ContentId,
                    baseData.BaseTier);

            itemInstance.TryAddUnlockedStat(
                new EquipmentRolledStat(
                    definition.StatType,
                    definition.ModifierType,
                    rolledValue));

            return itemInstance;
        }

        private static bool TryAddTestUnlockedStat(
            ItemInstance itemInstance,
            ItemBaseData baseData,
            EquipmentStatDatabase statDatabase,
            out EquipmentStatDefinition definition)
        {
            definition = null;

            for (int i = 0;
                 i < statDatabase.Definitions.Count;
                 i++)
            {
                EquipmentStatDefinition candidate =
                    statDatabase.Definitions[i];

                if (candidate == null)
                    continue;

                if (candidate.Weight <= 0f)
                    continue;

                if (HasBaseStat(
                        baseData,
                        candidate.StatType))
                {
                    continue;
                }

                definition = candidate;

                return true;
            }

            return false;
        }

        private static void TestBaseStats(
            ModifierSource source,
            ItemBaseData baseData)
        {
            for (int i = 0;
                 i < baseData.BaseStats.Count;
                 i++)
            {
                EquipmentBaseStat expected =
                    baseData.BaseStats[i];

                bool found = false;

                for (int j = 0;
                     j < source.Modifiers.Count;
                     j++)
                {
                    StatModifier modifier =
                        source.Modifiers[j];

                    if (modifier.StatType !=
                        expected.StatType)
                    {
                        continue;
                    }

                    if (modifier.ModifierType !=
                        expected.ModifierType)
                    {
                        continue;
                    }

                    if (!Mathf.Approximately(
                            modifier.Value,
                            expected.Value))
                    {
                        continue;
                    }

                    found = true;
                    break;
                }

                if (!found)
                {
                    Debug.LogError(
                        $"[Equipment Modifier Test] BaseStat " +
                        $"'{expected.StatType}' was not found " +
                        "correctly in ModifierSource.");
                    return;
                }

                Debug.Log(
                    $"[Equipment Modifier Test] BaseStat verified: " +
                    $"{expected.StatType} | " +
                    $"{expected.ModifierType} | " +
                    $"{expected.Value}");
            }
        }

        private static void TestUnlockedStat(
            ModifierSource source,
            ItemInstance itemInstance,
            EquipmentStatDefinition definition,
            float rolledValue)
        {
            float expectedValue =
                rolledValue +
                (definition.UpgradeGrowth *
                 itemInstance.UpgradeLevel);

            for (int i = 0;
                 i < source.Modifiers.Count;
                 i++)
            {
                StatModifier modifier =
                    source.Modifiers[i];

                if (modifier.StatType !=
                    definition.StatType)
                {
                    continue;
                }

                if (modifier.ModifierType !=
                    definition.ModifierType)
                {
                    continue;
                }

                if (!Mathf.Approximately(
                        modifier.Value,
                        expectedValue))
                {
                    continue;
                }

                Debug.Log(
                    $"[Equipment Modifier Test] UnlockedStat verified: " +
                    $"{definition.StatType} | " +
                    $"{modifier.ModifierType} | " +
                    $"{modifier.Value}");

                return;
            }

            Debug.LogError(
                $"[Equipment Modifier Test] UnlockedStat " +
                $"'{definition.StatType}' was not found correctly.");
        }

        private static bool HasBaseStat(
            ItemBaseData baseData,
            StatType statType)
        {
            if (baseData.BaseStats == null)
                return false;

            for (int i = 0;
                 i < baseData.BaseStats.Count;
                 i++)
            {
                if (baseData.BaseStats[i].StatType ==
                    statType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}