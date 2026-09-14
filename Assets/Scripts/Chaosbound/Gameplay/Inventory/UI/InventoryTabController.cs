using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class InventoryTabController : MonoBehaviour
    {
        [Header("Tabs")]
        [SerializeField] private InventoryTabUI inventoryTab;
        [SerializeField] private InventoryTabUI materialsTab;
        [SerializeField] private InventoryTabUI questItemsTab;

        [Header("Content")]
        [SerializeField] private GameObject inventoryContent;
        [SerializeField] private GameObject secureInventoryContent;
        [SerializeField] private GameObject materialsContent;
        [SerializeField] private GameObject questItemsContent;

        private void Awake()
        {
            RegisterTabListeners();
        }

        private void Start()
        {
            SelectInventory();
        }

        private void RegisterTabListeners()
        {
            if (inventoryTab != null && inventoryTab.Button != null)
                inventoryTab.Button.onClick.AddListener(SelectInventory);

            if (materialsTab != null && materialsTab.Button != null)
                materialsTab.Button.onClick.AddListener(SelectMaterials);

            if (questItemsTab != null && questItemsTab.Button != null)
                questItemsTab.Button.onClick.AddListener(SelectQuestItems);
        }

        public void SelectInventory()
        {
            SelectTab(
                inventoryTab,
                materialsTab,
                questItemsTab,
                inventoryContent,
                materialsContent,
                questItemsContent);
        }

        public void SelectMaterials()
        {
            SelectTab(
                materialsTab,
                inventoryTab,
                questItemsTab,
                materialsContent,
                inventoryContent,
                questItemsContent);
        }

        public void SelectQuestItems()
        {
            SelectTab(
                questItemsTab,
                inventoryTab,
                materialsTab,
                questItemsContent,
                inventoryContent,
                materialsContent);
        }

        private void SelectTab(
    InventoryTabUI selectedTab,
    InventoryTabUI otherTabA,
    InventoryTabUI otherTabB,
    GameObject selectedContent,
    GameObject otherContentA,
    GameObject otherContentB)
        {
            if (selectedTab != null)
                selectedTab.SetSelected(true);

            if (otherTabA != null)
                otherTabA.SetSelected(false);

            if (otherTabB != null)
                otherTabB.SetSelected(false);

            if (inventoryContent != null)
                inventoryContent.SetActive(false);

            if (secureInventoryContent != null)
                secureInventoryContent.SetActive(false);

            if (materialsContent != null)
                materialsContent.SetActive(false);

            if (questItemsContent != null)
                questItemsContent.SetActive(false);

            if (selectedContent != null)
                selectedContent.SetActive(true);

            if (selectedContent == inventoryContent &&
                secureInventoryContent != null)
            {
                secureInventoryContent.SetActive(true);
            }
        }
    }
}