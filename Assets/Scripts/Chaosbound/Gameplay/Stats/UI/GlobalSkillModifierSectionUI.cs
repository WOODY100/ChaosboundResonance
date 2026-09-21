using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.Stats.UI
{
    public sealed class GlobalSkillModifierSectionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GlobalSkillModifierRowUI modifierRowPrefab;
        [SerializeField] private RectTransform contentRoot;

        private readonly List<GlobalSkillModifierRowUI> rows =
            new List<GlobalSkillModifierRowUI>();

        public int RowCount => rows.Count;

        public void Show(
            IReadOnlyList<GlobalSkillModifierDisplayEntry> modifiers)
        {
            Clear();

            if (modifiers == null)
                return;

            for (int i = 0; i < modifiers.Count; i++)
            {
                GlobalSkillModifierDisplayEntry entry =
                    modifiers[i];

                if (entry == null)
                    continue;

                GlobalSkillModifierRowUI row =
                    CreateRow();

                if (row == null)
                    continue;

                row.SetModifier(entry);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < rows.Count; i++)
            {
                GlobalSkillModifierRowUI row = rows[i];

                if (row == null)
                    continue;

                Destroy(row.gameObject);
            }

            rows.Clear();
        }

        private GlobalSkillModifierRowUI CreateRow()
        {
            if (modifierRowPrefab == null)
            {
                Debug.LogError(
                    "[GlobalSkillModifierSectionUI] " +
                    "Modifier row prefab is not assigned.",
                    this);

                return null;
            }

            if (contentRoot == null)
            {
                Debug.LogError(
                    "[GlobalSkillModifierSectionUI] " +
                    "Content root is not assigned.",
                    this);

                return null;
            }

            GlobalSkillModifierRowUI row =
                Instantiate(
                    modifierRowPrefab,
                    contentRoot);

            rows.Add(row);

            return row;
        }
    }
}