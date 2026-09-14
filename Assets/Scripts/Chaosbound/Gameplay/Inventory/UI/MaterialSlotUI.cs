using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class MaterialSlotUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Image itemIcon;

        [SerializeField]
        private TMP_Text amountText;

        private string materialId;

        public string MaterialId =>
            materialId;

        public void SetMaterial(
            string materialId,
            Sprite icon,
            int amount)
        {
            if (string.IsNullOrEmpty(materialId))
            {
                Clear();
                return;
            }

            this.materialId = materialId;

            if (itemIcon != null)
            {
                itemIcon.sprite = icon;
                itemIcon.enabled = icon != null;
            }

            if (amountText != null)
            {
                amountText.text = amount.ToString();
            }
        }

        public void Clear()
        {
            materialId = null;

            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }

            if (amountText != null)
            {
                amountText.text = string.Empty;
            }
        }
    }
}