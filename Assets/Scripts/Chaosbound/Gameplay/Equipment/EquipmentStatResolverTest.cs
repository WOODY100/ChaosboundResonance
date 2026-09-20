using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentStatResolverTest : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private EquipmentStatDatabase statDatabase;
        [SerializeField] private ItemBaseData testBaseData;

        [ContextMenu("Run Equipment Stat Resolver Test")]
        private void RunTest()
        {
            if (statDatabase == null)
            {
                Debug.LogError(
                    "[Equipment Resolver Test] Missing EquipmentStatDatabase.");
                return;
            }

            if (testBaseData == null)
            {
                Debug.LogError(
                    "[Equipment Resolver Test] Missing ItemBaseData.");
                return;
            }

            EquipmentStatResolver resolver =
                new EquipmentStatResolver(statDatabase);

            ItemInstance itemInstance =
                new ItemInstance(
                    "equipment-resolver-test-instance",
                    testBaseData.ContentId,
                    testBaseData.BaseTier);

            Debug.Log(
                $"[Equipment Resolver Test] Initial state: " +
                $"Tier={itemInstance.CurrentTier}, " +
                $"Upgrade={itemInstance.UpgradeLevel}");

            TestBaseStat(
                resolver,
                testBaseData,
                itemInstance);

            if (!TryFindUnlockedStatType(
                    testBaseData,
                    statDatabase,
                    out EquipmentStatDefinition definition))
            {
                Debug.LogError(
                    "[Equipment Resolver Test] Could not find a valid " +
                    "stat definition that is not already a BaseStat.");
                return;
            }

            const float testRolledValue = 10f;

            EquipmentRolledStat rolledStat =
                new EquipmentRolledStat(
                    definition.StatType,
                    definition.ModifierType,
                    testRolledValue);

            if (!itemInstance.TryAddUnlockedStat(rolledStat))
            {
                Debug.LogError(
                    "[Equipment Resolver Test] Failed to add test " +
                    "UnlockedStat.");
                return;
            }

            Debug.Log(
                $"[Equipment Resolver Test] Added UnlockedStat: " +
                $"{definition.StatType} | " +
                $"{definition.ModifierType} | " +
                $"RolledValue={testRolledValue} | " +
                $"UpgradeGrowth={definition.UpgradeGrowth}");

            TestUnlockedStat(
                resolver,
                testBaseData,
                itemInstance,
                definition,
                testRolledValue);

            TestMissingStat(
                resolver,
                testBaseData,
                itemInstance);

            Debug.Log(
                "[Equipment Resolver Test] ✓ COMPLETE — " +
                "Equipment stat resolution is working correctly.");
        }

        private static void TestBaseStat(
            EquipmentStatResolver resolver,
            ItemBaseData baseData,
            ItemInstance itemInstance)
        {
            if (baseData.BaseStats == null ||
                baseData.BaseStats.Count == 0)
            {
                Debug.LogWarning(
                    "[Equipment Resolver Test] Item has no BaseStats. " +
                    "BaseStat test skipped.");
                return;
            }

            EquipmentBaseStat baseStat =
                baseData.BaseStats[0];

            bool resolved =
                resolver.TryResolve(
                    baseData,
                    itemInstance,
                    baseStat.StatType,
                    out ModifierType modifierType,
                    out float value);

            if (!resolved)
            {
                Debug.LogError(
                    $"[Equipment Resolver Test] Failed to resolve " +
                    $"BaseStat '{baseStat.StatType}'.");
                return;
            }

            if (modifierType != baseStat.ModifierType)
            {
                Debug.LogError(
                    $"[Equipment Resolver Test] BaseStat ModifierType " +
                    $"mismatch. Expected={baseStat.ModifierType}, " +
                    $"Actual={modifierType}.");
                return;
            }

            if (!Mathf.Approximately(value, baseStat.Value))
            {
                Debug.LogError(
                    $"[Equipment Resolver Test] BaseStat value mismatch. " +
                    $"Expected={baseStat.Value}, Actual={value}.");
                return;
            }

            Debug.Log(
                $"[Equipment Resolver Test] BaseStat resolved: " +
                $"{baseStat.StatType} | " +
                $"{modifierType} | " +
                $"{value}");
        }

        private static void TestUnlockedStat(
            EquipmentStatResolver resolver,
            ItemBaseData baseData,
            ItemInstance itemInstance,
            EquipmentStatDefinition definition,
            float rolledValue)
        {
            bool resolvedAtZero =
                resolver.TryResolve(
                    baseData,
                    itemInstance,
                    definition.StatType,
                    out ModifierType modifierTypeAtZero,
                    out float valueAtZero);

            if (!resolvedAtZero)
            {
                Debug.LogError(
                    $"[Equipment Resolver Test] Failed to resolve " +
                    $"UnlockedStat '{definition.StatType}' at Upgrade 0.");
                return;
            }

            float expectedAtZero =
                rolledValue;

            if (!Mathf.Approximately(
                    valueAtZero,
                    expectedAtZero))
            {
                Debug.LogError(
                    $"[Equipment Resolver Test] Upgrade 0 mismatch. " +
                    $"Expected={expectedAtZero}, " +
                    $"Actual={valueAtZero}.");
                return;
            }

            Debug.Log(
                $"[Equipment Resolver Test] Upgrade 0: " +
                $"{definition.StatType} | " +
                $"{modifierTypeAtZero} | " +
                $"{valueAtZero}");

            itemInstance.TryIncreaseUpgradeLevel();
            itemInstance.TryIncreaseUpgradeLevel();

            float expectedAtTwo =
                rolledValue +
                (definition.UpgradeGrowth * 2);

            bool resolvedAtTwo =
                resolver.TryResolve(
                    baseData,
                    itemInstance,
                    definition.StatType,
                    out ModifierType modifierTypeAtTwo,
                    out float valueAtTwo);

            if (!resolvedAtTwo)
            {
                Debug.LogError(
                    $"[Equipment Resolver Test] Failed to resolve " +
                    $"UnlockedStat '{definition.StatType}' at Upgrade 2.");
                return;
            }

            if (modifierTypeAtTwo != definition.ModifierType)
            {
                Debug.LogError(
                    $"[Equipment Resolver Test] ModifierType mismatch " +
                    $"at Upgrade 2. Expected={definition.ModifierType}, " +
                    $"Actual={modifierTypeAtTwo}.");
                return;
            }

            if (!Mathf.Approximately(
                    valueAtTwo,
                    expectedAtTwo))
            {
                Debug.LogError(
                    $"[Equipment Resolver Test] Upgrade 2 mismatch. " +
                    $"Expected={expectedAtTwo}, " +
                    $"Actual={valueAtTwo}.");
                return;
            }

            Debug.Log(
                $"[Equipment Resolver Test] Upgrade 2: " +
                $"{definition.StatType} | " +
                $"{modifierTypeAtTwo} | " +
                $"{valueAtTwo} " +
                $"(Expected={expectedAtTwo})");
        }

        private static void TestMissingStat(
            EquipmentStatResolver resolver,
            ItemBaseData baseData,
            ItemInstance itemInstance)
        {
            StatType missingStat =
                FindMissingStat(
                    baseData,
                    itemInstance);

            bool resolved =
                resolver.TryResolve(
                    baseData,
                    itemInstance,
                    missingStat,
                    out _,
                    out _);

            if (resolved)
            {
                Debug.LogError(
                    $"[Equipment Resolver Test] Missing stat " +
                    $"'{missingStat}' should not resolve.");
                return;
            }

            Debug.Log(
                $"[Equipment Resolver Test] Missing stat correctly " +
                $"returned false: {missingStat}");
        }

        private static bool TryFindUnlockedStatType(
            ItemBaseData baseData,
            EquipmentStatDatabase database,
            out EquipmentStatDefinition definition)
        {
            definition = null;

            for (int i = 0;
                 i < database.Definitions.Count;
                 i++)
            {
                EquipmentStatDefinition candidate =
                    database.Definitions[i];

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
                if (baseData.BaseStats[i].StatType == statType)
                    return true;
            }

            return false;
        }

        private static StatType FindMissingStat(
            ItemBaseData baseData,
            ItemInstance itemInstance)
        {
            StatType[] statTypes =
                (StatType[])System.Enum.GetValues(
                    typeof(StatType));

            for (int i = 0; i < statTypes.Length; i++)
            {
                StatType candidate = statTypes[i];

                if (HasBaseStat(
                        baseData,
                        candidate))
                {
                    continue;
                }

                if (itemInstance.HasUnlockedStat(candidate))
                {
                    continue;
                }

                return candidate;
            }

            return StatType.Damage;
        }
    }
}