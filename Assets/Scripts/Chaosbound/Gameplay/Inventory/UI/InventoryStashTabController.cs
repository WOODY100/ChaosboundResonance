using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class InventoryStashTabController : MonoBehaviour
    {
        [Header("Tabs")]
        [SerializeField] private InventoryTabUI inventoryTab;
        [SerializeField] private InventoryTabUI materialsTab;
        [SerializeField] private InventoryTabUI questItemsTab;

        [Header("Content")]
        [SerializeField] private GameObject inventoryContent;
        [SerializeField] private GameObject materialsContent;
        [SerializeField] private GameObject questItemsContent;

        [SerializeField] private GameObject trash;

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            UnsubscribeTabs();
        }

        private void Initialize()
        {
            ValidateReferences();
            SubscribeTabs();

            ShowInventory();
        }

        private void SubscribeTabs()
        {
            if (inventoryTab != null)
            {
                inventoryTab.Button.onClick.AddListener(
                    ShowInventory);
            }

            if (materialsTab != null)
            {
                materialsTab.Button.onClick.AddListener(
                    ShowMaterials);
            }

            if (questItemsTab != null)
            {
                questItemsTab.Button.onClick.AddListener(
                    ShowQuestItems);
            }
        }

        private void UnsubscribeTabs()
        {
            if (inventoryTab != null)
            {
                inventoryTab.Button.onClick.RemoveListener(
                    ShowInventory);
            }

            if (materialsTab != null)
            {
                materialsTab.Button.onClick.RemoveListener(
                    ShowMaterials);
            }

            if (questItemsTab != null)
            {
                questItemsTab.Button.onClick.RemoveListener(
                    ShowQuestItems);
            }
        }

        private void ShowInventory()
        {
            SetActiveContent(
                inventoryContent,
                materialsContent,
                questItemsContent);

            SetSelectedTab(
                inventoryTab,
                materialsTab,
                questItemsTab);

            if (trash != null)
                trash.SetActive(true);
        }

        private void ShowMaterials()
        {
            SetActiveContent(
                materialsContent,
                inventoryContent,
                questItemsContent);

            SetSelectedTab(
                materialsTab,
                inventoryTab,
                questItemsTab);

            if (trash != null)
                trash.SetActive(false);
        }

        private void ShowQuestItems()
        {
            SetActiveContent(
                questItemsContent,
                inventoryContent,
                materialsContent);

            SetSelectedTab(
                questItemsTab,
                inventoryTab,
                materialsTab);

            if (trash != null)
                trash.SetActive(false);
        }

        private void SetActiveContent(
            GameObject active,
            GameObject inactiveA,
            GameObject inactiveB)
        {
            if (active != null)
                active.SetActive(true);

            if (inactiveA != null)
                inactiveA.SetActive(false);

            if (inactiveB != null)
                inactiveB.SetActive(false);
        }

        private void SetSelectedTab(
            InventoryTabUI selected,
            InventoryTabUI inactiveA,
            InventoryTabUI inactiveB)
        {
            if (selected != null)
                selected.SetSelected(true);

            if (inactiveA != null)
                inactiveA.SetSelected(false);

            if (inactiveB != null)
                inactiveB.SetSelected(false);
        }

        private void ValidateReferences()
        {
            if (inventoryTab == null)
            {
                UnityEngine.Debug.LogError(
                    "[InventoryStashTabController] " +
                    "Inventory tab reference is missing.",
                    this);
            }

            if (materialsTab == null)
            {
                UnityEngine.Debug.LogError(
                    "[InventoryStashTabController] " +
                    "Materials tab reference is missing.",
                    this);
            }

            if (questItemsTab == null)
            {
                UnityEngine.Debug.LogError(
                    "[InventoryStashTabController] " +
                    "Quest Items tab reference is missing.",
                    this);
            }

            if (inventoryContent == null)
            {
                UnityEngine.Debug.LogError(
                    "[InventoryStashTabController] " +
                    "Inventory content reference is missing.",
                    this);
            }

            if (materialsContent == null)
            {
                UnityEngine.Debug.LogError(
                    "[InventoryStashTabController] " +
                    "Materials content reference is missing.",
                    this);
            }

            if (questItemsContent == null)
            {
                UnityEngine.Debug.LogError(
                    "[InventoryStashTabController] " +
                    "Quest Items content reference is missing.",
                    this);
            }

            if (trash == null)
            {
                UnityEngine.Debug.LogError(
                    "[InventoryStashTabController] " +
                    "Trash reference is missing.",
                    this);
            }
        }
    }
}