using Chaosbound.Content.Materials;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class MaterialCostEntryUI : MonoBehaviour
    {
        [Header("Material")]
        [SerializeField] private Image materialIcon;
        [SerializeField] private TMP_Text materialName;

        [Header("Amounts")]
        [SerializeField] private TMP_Text ownedAmount;
        [SerializeField] private TMP_Text requiredAmount;

        public void SetData(
            MaterialDefinition definition,
            int owned,
            int required)
        {
            if (definition == null)
                return;

            if (materialIcon != null)
            {
                materialIcon.sprite = definition.Icon;
                materialIcon.enabled =
                    definition.Icon != null;
            }

            if (materialName != null)
                materialName.text = definition.DisplayName;

            if (ownedAmount != null)
            {
                ownedAmount.text = owned.ToString();

                ownedAmount.color =
                    owned >= required
                        ? Color.white
                        : Color.red;
            }

            if (requiredAmount != null)
                requiredAmount.text = required.ToString();
        }
    }
}