using Chaosbound.Content.Items;
using UnityEngine;

namespace Chaosbound.Gameplay.Equipment.UI
{
    public sealed class EquipmentUIDropTarget :
        MonoBehaviour
    {
        [Header("Equipment")]
        [SerializeField]
        private EquipmentType equipmentType;

        private EquipmentSlotUI equipmentSlotUI;

        public EquipmentType EquipmentType =>
            equipmentType;

        private void Awake()
        {
            equipmentSlotUI =
                GetComponent<EquipmentSlotUI>();
        }

        public void ShowValidFeedback()
        {
            if (equipmentSlotUI == null)
                return;

            equipmentSlotUI.ShowValidDropFeedback();
        }

        public void ShowSwapFeedback()
        {
            if (equipmentSlotUI == null)
                return;

            equipmentSlotUI.ShowSwapDropFeedback();
        }

        public void ShowInvalidFeedback()
        {
            if (equipmentSlotUI == null)
                return;

            equipmentSlotUI.ShowInvalidDropFeedback();
        }

        public void ClearFeedback()
        {
            if (equipmentSlotUI == null)
                return;

            equipmentSlotUI.ClearDropFeedback();
        }
    }
}