using Chaosbound.Core.Audio;
using Chaosbound.Core.Composition;
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

        private UIAudioPlayer uiAudioPlayer;
        private InventoryTabUI activeTab;

        private void Start()
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext != null)
            {
                uiAudioPlayer =
                    FindAnyObjectByType<UIAudioPlayer>();
            }

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
            PlayTabSwitchIfNeeded(equipmentTab);

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
            PlayTabSwitchIfNeeded(statsTab);

            if (equipmentContent != null)
                equipmentContent.SetActive(false);

            if (statsContent != null)
                statsContent.SetActive(true);

            if (equipmentTab != null)
                equipmentTab.SetSelected(false);

            if (statsTab != null)
                statsTab.SetSelected(true);
        }

        private void PlayTabSwitchIfNeeded(
            InventoryTabUI selectedTab)
        {
            if (selectedTab == null)
                return;

            if (activeTab == selectedTab)
                return;

            if (activeTab != null && uiAudioPlayer != null)
            {
                uiAudioPlayer.PlayTabSwitch();
            }

            activeTab = selectedTab;
        }
    }
}