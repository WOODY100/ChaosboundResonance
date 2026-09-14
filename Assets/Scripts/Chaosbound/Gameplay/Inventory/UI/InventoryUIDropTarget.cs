using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public enum InventoryUIDropTargetType
    {
        Main,
        Secure
    }

    public sealed class InventoryUIDropTarget : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private InventoryUIDropTargetType targetType;

        [Header("Feedback")]
        [SerializeField] private InventoryUIDropFeedback feedback;

        private int slotIndex = -1;

        public InventoryUIDropTargetType TargetType =>
            targetType;

        public int SlotIndex =>
            slotIndex;

        private void Awake()
        {
            if (feedback == null)
            {
                feedback =
                    GetComponent<InventoryUIDropFeedback>();
            }

            ClearFeedback();
        }

        public void Configure(
            InventoryUIDropTargetType type,
            int index)
        {
            if (index < 0)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(index));
            }

            targetType = type;
            slotIndex = index;
        }

        public void ShowSwapFeedback()
        {
            if (feedback == null)
            {
                return;
            }

            feedback.ShowSwap();
        }

        public void ShowValidFeedback()
        {
            if (feedback == null)
            {
                return;
            }

            feedback.ShowValid();
        }

        public void ShowInvalidFeedback()
        {
            if (feedback == null)
            {
                return;
            }

            feedback.ShowInvalid();
        }

        public void ClearFeedback()
        {
            if (feedback == null)
            {
                return;
            }

            feedback.Clear();
        }
    }
}