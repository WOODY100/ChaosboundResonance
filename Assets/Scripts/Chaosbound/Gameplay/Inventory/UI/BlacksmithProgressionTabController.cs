using System;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class BlacksmithProgressionTabController :
        MonoBehaviour
    {
        private enum ProgressionTab
        {
            Upgrade,
            TierUp
        }

        [Header("Tab Buttons")]
        [SerializeField] private Button upgradeTabButton;
        [SerializeField] private Button tierUpTabButton;

        [Header("Tab Images")]
        [SerializeField] private Image upgradeTabImage;
        [SerializeField] private Image tierUpTabImage;

        [Header("Tab Sprites")]
        [SerializeField] private Sprite activeTabSprite;
        [SerializeField] private Sprite inactiveTabSprite;

        [Header("Tab Content")]
        [SerializeField] private GameObject upgradeContent;
        [SerializeField] private GameObject tierUpContent;

        private ProgressionTab currentTab =
            ProgressionTab.Upgrade;

        public event Action<bool> ActiveTabChanged;

        public bool IsTierUpActive =>
            currentTab == ProgressionTab.TierUp;

        private void Awake()
        {
            ValidateConfiguration();
        }

        private void OnEnable()
        {
            if (upgradeTabButton != null)
            {
                upgradeTabButton.onClick.RemoveListener(
                    ShowUpgradeTab);

                upgradeTabButton.onClick.AddListener(
                    ShowUpgradeTab);
            }

            if (tierUpTabButton != null)
            {
                tierUpTabButton.onClick.RemoveListener(
                    ShowTierUpTab);

                tierUpTabButton.onClick.AddListener(
                    ShowTierUpTab);
            }

            ShowUpgradeTab();
        }

        private void OnDisable()
        {
            if (upgradeTabButton != null)
            {
                upgradeTabButton.onClick.RemoveListener(
                    ShowUpgradeTab);
            }

            if (tierUpTabButton != null)
            {
                tierUpTabButton.onClick.RemoveListener(
                    ShowTierUpTab);
            }
        }

        public void ShowUpgradeTab()
        {
            SetActiveTab(ProgressionTab.Upgrade);
        }

        public void ShowTierUpTab()
        {
            SetActiveTab(ProgressionTab.TierUp);
        }

        private void SetActiveTab(ProgressionTab tab)
        {
            if (currentTab == tab)
            {
                UpdateTabVisuals(
                    currentTab == ProgressionTab.Upgrade);

                return;
            }

            currentTab = tab;

            bool upgradeActive =
                currentTab == ProgressionTab.Upgrade;

            if (upgradeContent != null)
                upgradeContent.SetActive(upgradeActive);

            if (tierUpContent != null)
                tierUpContent.SetActive(!upgradeActive);

            UpdateTabVisuals(upgradeActive);

            ActiveTabChanged?.Invoke(!upgradeActive);
        }

        private void UpdateTabVisuals(bool upgradeActive)
        {
            if (upgradeTabImage != null)
            {
                upgradeTabImage.sprite =
                    upgradeActive
                        ? activeTabSprite
                        : inactiveTabSprite;
            }

            if (tierUpTabImage != null)
            {
                tierUpTabImage.sprite =
                    upgradeActive
                        ? inactiveTabSprite
                        : activeTabSprite;
            }
        }

        private void ValidateConfiguration()
        {
            if (upgradeTabButton == null ||
                tierUpTabButton == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionTabController] " +
                    "Tab button references are missing.",
                    this);
            }

            if (upgradeTabImage == null ||
                tierUpTabImage == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionTabController] " +
                    "Tab image references are missing.",
                    this);
            }

            if (activeTabSprite == null ||
                inactiveTabSprite == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionTabController] " +
                    "Tab sprites are missing.",
                    this);
            }

            if (upgradeContent == null ||
                tierUpContent == null)
            {
                UnityEngine.Debug.LogError(
                    "[BlacksmithProgressionTabController] " +
                    "Tab content references are missing.",
                    this);
            }
        }
    }
}