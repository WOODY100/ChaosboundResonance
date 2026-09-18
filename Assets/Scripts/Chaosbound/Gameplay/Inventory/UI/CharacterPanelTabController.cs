using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class CharacterPanelTabController : MonoBehaviour
    {
        [Header("Tabs")]
        [SerializeField] private InventoryTabUI equipmentTab;
        [SerializeField] private InventoryTabUI statsTab;

        [Header("Content")]
        [SerializeField] private GameObject equipmentContent;
        [SerializeField] private GameObject statsContent;

        private void Start()
        {
            BindTabEvents();

            ShowEquipment();
        }

        private void OnDestroy()
        {
            UnbindTabEvents();
        }

        private void BindTabEvents()
        {
            if (equipmentTab != null && equipmentTab.Button != null)
            {
                equipmentTab.Button.onClick.AddListener(ShowEquipment);
            }

            if (statsTab != null && statsTab.Button != null)
            {
                statsTab.Button.onClick.AddListener(ShowStats);
            }
        }

        private void UnbindTabEvents()
        {
            if (equipmentTab != null && equipmentTab.Button != null)
            {
                equipmentTab.Button.onClick.RemoveListener(ShowEquipment);
            }

            if (statsTab != null && statsTab.Button != null)
            {
                statsTab.Button.onClick.RemoveListener(ShowStats);
            }
        }

        public void ShowEquipment()
        {
            if (equipmentContent != null)
                equipmentContent.SetActive(true);

            if (statsContent != null)
                statsContent.SetActive(false);

            if (equipmentTab != null)
                equipmentTab.SetSelected(true);

            if (statsTab != null)
                statsTab.SetSelected(false);
        }

        public void ShowStats()
        {
            if (equipmentContent != null)
                equipmentContent.SetActive(false);

            if (statsContent != null)
                statsContent.SetActive(true);

            if (equipmentTab != null)
                equipmentTab.SetSelected(false);

            if (statsTab != null)
                statsTab.SetSelected(true);
        }
    }
}