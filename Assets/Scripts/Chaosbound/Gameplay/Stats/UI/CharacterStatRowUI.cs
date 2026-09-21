using TMPro;
using UnityEngine;

namespace Chaosbound.Gameplay.Stats.UI
{
    public sealed class CharacterStatRowUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text statName;
        [SerializeField] private TMP_Text statValue;

        public void SetStat(
            CharacterStatDisplayEntry entry)
        {
            if (entry == null)
                return;

            if (statName != null)
            {
                statName.text =
                    StatDisplayNameFormatter.Format(
                        entry.StatType);
            }

            if (statValue != null)
            {
                statValue.text =
                    StatDisplayFormatter.Format(
                        entry.Value,
                        entry.Format);
            }
        }

        public void Clear()
        {
            if (statName != null)
                statName.text = string.Empty;

            if (statValue != null)
                statValue.text = string.Empty;
        }
    }
}