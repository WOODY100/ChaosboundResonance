using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class BlacksmithTierOptionSlotUI :
        MonoBehaviour
    {
        [Header("Selection")]
        [SerializeField] private Button selectionButton;
        [SerializeField] private GameObject selectionFrame;
        [SerializeField] private GameObject selectedText;

        [Header("Stat Presentation")]
        [SerializeField] private TMP_Text statName;
        [SerializeField] private TMP_Text statValue;
        [SerializeField] private TMP_Text statDescription;

        public Button SelectionButton =>
            selectionButton;

        public void SetData(
            string displayName,
            string displayValue,
            string description)
        {
            if (statName != null)
                statName.text = displayName;

            if (statValue != null)
                statValue.text = displayValue;

            if (statDescription != null)
                statDescription.text = description;
        }

        public void SetSelected(bool selected)
        {
            if (selectionFrame != null)
                selectionFrame.SetActive(selected);

            if (selectedText != null)
                selectedText.SetActive(selected);
        }

        private void Awake()
        {
            SetSelected(false);

            if (selectionButton == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithTierOptionSlotUI] " +
                    "Selection Button reference is missing.",
                    this);
            }
        }
    }
}