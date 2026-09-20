using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Chaosbound.Gameplay.Equipment;

namespace Chaosbound.Gameplay.Items.UI.Tooltip
{
    public sealed class EquipmentTooltipStatsView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private EquipmentTooltipStatRow statRowPrefab;

        [SerializeField]
        private RectTransform contentRoot;

        private readonly List<EquipmentTooltipStatRow> rows =
            new List<EquipmentTooltipStatRow>();

        public int RowCount => rows.Count;

        public void Show(
            IReadOnlyList<EquipmentTooltipStatData> stats)
        {
            Clear();

            if (stats == null)
                return;

            for (int i = 0; i < stats.Count; i++)
            {
                EquipmentTooltipStatData stat =
                    stats[i];

                EquipmentTooltipStatRow row =
                    CreateRow();

                if (row == null)
                    continue;

                row.SetStat(
                    GetStatDisplayName(stat.StatType),
                    FormatValue(
                        stat.ModifierType,
                        stat.Value));
            }
        }

        public void ShowComparison(
            IReadOnlyList<EquipmentComparisonDisplayEntry> entries,
            bool showCandidateValues)
        {
            Clear();

            if (entries == null)
                return;

            for (int i = 0; i < entries.Count; i++)
            {
                EquipmentComparisonDisplayEntry entry = entries[i];

                bool hasValue =
                    showCandidateValues
                        ? entry.HasCandidateValue
                        : entry.HasEquippedValue;

                if (!hasValue)
                    continue;

                ModifierType modifierType =
                    showCandidateValues
                        ? entry.CandidateModifierType
                        : entry.EquippedModifierType;

                float statValue =
                    showCandidateValues
                        ? entry.CandidateValue
                        : entry.EquippedValue;

                string displayValue =
                    FormatValue(
                        modifierType,
                        statValue);

                string differenceText =
                    BuildDifferenceText(
                        entry,
                        showCandidateValues);

                EquipmentComparisonDisplayState displayState =
                    ResolveDisplayState(
                        entry,
                        showCandidateValues);

                EquipmentTooltipStatRow row = CreateRow();

                if (row == null)
                    continue;

                row.SetComparisonStat(
                    GetStatDisplayName(entry.StatType),
                    displayValue,
                    differenceText,
                    displayState);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < rows.Count; i++)
            {
                EquipmentTooltipStatRow row =
                    rows[i];

                if (row == null)
                    continue;

                Destroy(row.gameObject);
            }

            rows.Clear();
        }

        private EquipmentTooltipStatRow CreateRow()
        {
            if (statRowPrefab == null)
            {
                Debug.LogError(
                    "[EquipmentTooltipStatsView] " +
                    "Stat Row Prefab is missing.",
                    this);

                return null;
            }

            if (contentRoot == null)
            {
                Debug.LogError(
                    "[EquipmentTooltipStatsView] " +
                    "Content Root is missing.",
                    this);

                return null;
            }

            EquipmentTooltipStatRow row =
                Instantiate(
                    statRowPrefab,
                    contentRoot);

            rows.Add(row);

            return row;
        }

        private static string BuildDifferenceText(
    EquipmentComparisonDisplayEntry entry,
    bool showCandidateValues)
        {
            if (!entry.HasDifference)
                return string.Empty;

            float difference =
                showCandidateValues
                    ? entry.Difference
                    : -entry.Difference;

            if (Mathf.Approximately(
                    difference,
                    0f))
            {
                return "0";
            }

            string sign =
                difference > 0f
                    ? "+"
                    : string.Empty;

            ModifierType modifierType =
                showCandidateValues
                    ? entry.CandidateModifierType
                    : entry.EquippedModifierType;

            if (modifierType == ModifierType.FinalMultiplier)
            {
                return sign +
                       difference.ToString("0.##");
            }

            return sign +
                   FormatValue(
                       modifierType,
                       difference);
        }

        private static EquipmentComparisonDisplayState
            ResolveDisplayState(
                EquipmentComparisonDisplayEntry entry,
                bool showCandidateValues)
        {
            if (!entry.HasDifference)
                return entry.State;

            if (Mathf.Approximately(
                    entry.Difference,
                    0f))
            {
                return EquipmentComparisonDisplayState.Unchanged;
            }

            if (showCandidateValues)
            {
                return entry.Difference > 0f
                    ? EquipmentComparisonDisplayState.Improved
                    : EquipmentComparisonDisplayState.Reduced;
            }

            return entry.Difference > 0f
                ? EquipmentComparisonDisplayState.Reduced
                : EquipmentComparisonDisplayState.Improved;
        }

        private static string GetStatDisplayName(
            StatType statType)
        {
            switch (statType)
            {
                case StatType.Damage:
                    return "Damage";

                case StatType.AttackSpeed:
                    return "Attack Speed";

                case StatType.MovementSpeed:
                    return "Movement Speed";

                case StatType.CritChance:
                    return "Crit Chance";

                case StatType.CritDamage:
                    return "Crit Damage";

                case StatType.MaxHP:
                    return "Max HP";

                case StatType.HPRegen:
                    return "HP Regen";

                case StatType.DamageReduction:
                    return "Damage Reduction";

                case StatType.Shield:
                    return "Shield";

                case StatType.PickupRadius:
                    return "Pickup Radius";

                case StatType.Luck:
                    return "Luck";

                case StatType.XPGain:
                    return "XP Gain";

                default:
                    return statType.ToString();
            }
        }

        private static string FormatValue(
            ModifierType modifierType,
            float value)
        {
            switch (modifierType)
            {
                case ModifierType.Flat:
                    return value.ToString("0.##");

                case ModifierType.Percent:
                    return (value * 100f).ToString("0.##") + "%";

                case ModifierType.FinalMultiplier:
                    return "x" + value.ToString("0.##");

                default:
                    return value.ToString("0.##");
            }
        }
    }
}