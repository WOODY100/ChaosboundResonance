using TMPro;
using UnityEngine;

namespace Chaosbound.Gameplay.Stats.UI
{
    public sealed class GlobalSkillModifierRowUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text modifierName;
        [SerializeField] private TMP_Text modifierValue;

        public void SetModifier(
            GlobalSkillModifierDisplayEntry entry)
        {
            if (entry == null)
                return;

            if (modifierName != null)
            {
                modifierName.text =
                    entry.DisplayName;
            }

            if (modifierValue != null)
            {
                modifierValue.text =
                    StatDisplayFormatter.Format(
                        entry.Value,
                        entry.Format);
            }
        }

        public void Clear()
        {
            if (modifierName != null)
                modifierName.text = string.Empty;

            if (modifierValue != null)
                modifierValue.text = string.Empty;
        }
    }
}