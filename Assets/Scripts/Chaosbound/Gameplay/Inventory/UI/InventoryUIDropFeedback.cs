using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class InventoryUIDropFeedback : MonoBehaviour
    {
        public enum FeedbackState
        {
            None,
            Valid,
            Swap,
            Invalid
        }

        [Header("References")]
        [SerializeField] private Image highlightImage;

        [Header("Visual")]
        [SerializeField] private Color validColor = Color.green;
        [SerializeField]
        private Color swapColor =
            new Color(1f, 0.5f, 0f, 1f);
        [SerializeField] private Color invalidColor = Color.red;

        [Header("Alpha")]
        [SerializeField, Range(0f, 1f)]
        private float highlightAlpha = 0.35f;

        private void Awake()
        {
            Clear();
        }

        public void ShowValid()
        {
            Show(validColor);
        }

        public void ShowSwap()
        {
            Show(swapColor);
        }

        public void ShowInvalid()
        {
            Show(invalidColor);
        }

        public void Clear()
        {
            if (highlightImage == null)
            {
                return;
            }

            highlightImage.enabled = false;
        }

        private void Show(Color color)
        {
            if (highlightImage == null)
            {
                return;
            }

            Color finalColor = color;
            finalColor.a = highlightAlpha;

            highlightImage.color = finalColor;
            highlightImage.enabled = true;
        }
    }
}