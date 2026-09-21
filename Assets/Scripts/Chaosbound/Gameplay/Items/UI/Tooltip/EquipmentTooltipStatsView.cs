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
                EquipmentComparisonDisplayEntry entry =
                    entries[i];

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

                EquipmentTooltipStatRow row =
                    CreateRow();

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

            ModifierType modifierType =
                showCandidateValues
                    ? entry.CandidateModifierType
                    : entry.EquippedModifierType;

            return FormatDifferenceValue(
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
                    return "DAMAGE";

                case StatType.AttackSpeed:
                    return "ATTACK SPEED";

                case StatType.MovementSpeed:
                    return "MOVEMENT SPEED";

                case StatType.CritChance:
                    return "CRIT CHANCE";

                case StatType.CritDamage:
                    return "CRIT DAMAGE";

                case StatType.MaxHP:
                    return "MAX HP";

                case StatType.HPRegen:
                    return "HP REGEN";

                case StatType.DamageReduction:
                    return "DAMAGE REDUCTION";

                case StatType.Shield:
                    return "SHIELD";

                case StatType.PickupRadius:
                    return "PICKUP RADIUS";

                case StatType.Luck:
                    return "LUCK";

                case StatType.XPGain:
                    return "XP GAIN";

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
                    return FormatSignedNumber(value);

                case ModifierType.Percent:
                    return FormatSignedPercent(value);

                case ModifierType.FinalMultiplier:
                    return FormatSignedMultiplier(value);

                default:
                    return value.ToString("0.##");
            }
        }

        private static string FormatDifferenceValue(
            ModifierType modifierType,
            float difference)
        {
            switch (modifierType)
            {
                case ModifierType.Flat:
                    return FormatSignedNumber(difference);

                case ModifierType.Percent:
                    return FormatSignedPercent(difference);

                case ModifierType.FinalMultiplier:
                    return FormatSignedMultiplierDifference(difference);

                default:
                    return FormatSignedNumber(difference);
            }
        }

        private static string FormatSignedNumber(
            float value)
        {
            if (Mathf.Approximately(value, 0f))
                return "0";

            return value > 0f
                ? "+" + value.ToString("0.##")
                : value.ToString("0.##");
        }

        private static string FormatSignedPercent(
            float value)
        {
            if (Mathf.Approximately(value, 0f))
                return "0%";

            float percentage =
                value * 100f;

            return percentage > 0f
                ? "+" + percentage.ToString("0.##") + "%"
                : percentage.ToString("0.##") + "%";
        }

        private static string FormatSignedMultiplier(
            float value)
        {
            if (Mathf.Approximately(value, 1f))
                return "0%";

            float percentage =
                (value - 1f) * 100f;

            return percentage > 0f
                ? "+" + percentage.ToString("0.##") + "%"
                : percentage.ToString("0.##") + "%";
        }

        private static string FormatSignedMultiplierDifference(
            float difference)
        {
            if (Mathf.Approximately(difference, 0f))
                return "0%";

            float percentage =
                difference * 100f;

            return percentage > 0f
                ? "+" + percentage.ToString("0.##") + "%"
                : percentage.ToString("0.##") + "%";
        }
    }
}