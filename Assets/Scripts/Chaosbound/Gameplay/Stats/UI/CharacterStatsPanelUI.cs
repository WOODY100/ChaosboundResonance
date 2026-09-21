using UnityEngine;

namespace Chaosbound.Gameplay.Stats.UI
{
    public sealed class CharacterStatsPanelUI : MonoBehaviour
    {
        [Header("Sections")]
        [SerializeField] private CharacterStatsSectionUI offenseSection;
        [SerializeField] private CharacterStatsSectionUI defenseSection;
        [SerializeField] private CharacterStatsSectionUI utilitySection;

        [SerializeField]
        private GlobalSkillModifierSectionUI globalSkillModifierSection;

        public void Show(CharacterStatsDisplayData displayData)
        {
            if (displayData == null)
            {
                Clear();
                return;
            }

            if (offenseSection != null)
            {
                offenseSection.Show(
                    displayData.Offense);
            }

            if (defenseSection != null)
            {
                defenseSection.Show(
                    displayData.Defense);
            }

            if (utilitySection != null)
            {
                utilitySection.Show(
                    displayData.Utility);
            }

            if (globalSkillModifierSection != null)
            {
                globalSkillModifierSection.Show(
                    displayData.GlobalSkillModifiers);
            }
        }

        public void Clear()
        {
            if (offenseSection != null)
                offenseSection.Clear();

            if (defenseSection != null)
                defenseSection.Clear();

            if (utilitySection != null)
                utilitySection.Clear();

            if (globalSkillModifierSection != null)
                globalSkillModifierSection.Clear();
        }
    }
}