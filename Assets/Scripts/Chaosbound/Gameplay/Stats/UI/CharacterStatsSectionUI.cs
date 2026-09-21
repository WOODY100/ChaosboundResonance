using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Stats.UI
{
    public sealed class CharacterStatsSectionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterStatRowUI statRowPrefab;
        [SerializeField] private RectTransform contentRoot;

        private readonly List<CharacterStatRowUI> rows =
            new List<CharacterStatRowUI>();

        public int RowCount => rows.Count;

        public void Show(
            IReadOnlyList<CharacterStatDisplayEntry> stats)
        {
            Clear();

            if (stats == null)
                return;

            for (int i = 0; i < stats.Count; i++)
            {
                CharacterStatDisplayEntry entry = stats[i];

                if (entry == null)
                    continue;

                CharacterStatRowUI row = CreateRow();

                if (row == null)
                    continue;

                row.SetStat(entry);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < rows.Count; i++)
            {
                CharacterStatRowUI row = rows[i];

                if (row == null)
                    continue;

                Destroy(row.gameObject);
            }

            rows.Clear();
        }

        private CharacterStatRowUI CreateRow()
        {
            if (statRowPrefab == null)
            {
                Debug.LogError(
                    "[CharacterStatsSectionUI] " +
                    "Stat row prefab is not assigned.",
                    this);

                return null;
            }

            if (contentRoot == null)
            {
                Debug.LogError(
                    "[CharacterStatsSectionUI] " +
                    "Content root is not assigned.",
                    this);

                return null;
            }

            CharacterStatRowUI row =
                Instantiate(
                    statRowPrefab,
                    contentRoot);

            rows.Add(row);

            return row;
        }
    }
}