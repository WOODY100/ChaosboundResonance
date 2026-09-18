using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class InventoryUITrashDropTarget :
        MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private GameObject background;

        [Header("Raycast")]
        [SerializeField] private Image dropAreaImage;

        [Header("Feedback")]
        [SerializeField] private Image backgroundImage;

        [SerializeField]
        private Color normalColor =
            new Color(1f, 1f, 1f, 0.15f);

        [SerializeField]
        private Color validColor =
            new Color(1f, 1f, 1f, 0.30f);

        private bool dragActive;

        private void Awake()
        {
            if (dropAreaImage == null)
            {
                dropAreaImage =
                    GetComponent<Image>();
            }

            if (backgroundImage == null &&
                background != null)
            {
                backgroundImage =
                    background.GetComponent<Image>();
            }

            SetDragVisual(false);
        }

        public void SetDragVisual(bool active)
        {
            dragActive = active;

            if (background != null)
                background.SetActive(active);

            if (dropAreaImage != null)
                dropAreaImage.raycastTarget = active;

            ResetVisual();
        }

        public void ShowValidFeedback()
        {
            if (!dragActive)
                return;

            if (backgroundImage == null)
                return;

            backgroundImage.color =
                validColor;
        }

        public void ClearFeedback()
        {
            if (!dragActive)
                return;

            ResetVisual();
        }

        private void ResetVisual()
        {
            if (backgroundImage == null)
                return;

            backgroundImage.color =
                normalColor;
        }
    }
}