using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Inventory.Runtime;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.World.Integration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class ItemWorldDropConfirmationUI :
        MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject panelRoot;

        [Header("Content")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text description;

        [Header("Buttons")]
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button dropButton;

        private GameFlow gameFlow;
        private ItemWorldDropConfirmationService
            confirmationService;

        private ItemDatabase itemDatabase;
        private ItemResolver itemResolver;

        private void Start()
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            gameFlow =
                bootstrapContext.GameFlow;

            RunManager runManager =
                bootstrapContext.RunManager;

            if (runManager != null)
            {
                confirmationService =
                    runManager
                        .ItemWorldDropConfirmationService;
            }

            GameContentContext
                compositionContext =
                    GameContentContext.Current;

            if (compositionContext != null)
            {
                itemDatabase =
                    compositionContext.ItemDatabase;
            }

            if (itemDatabase != null)
            {
                itemResolver =
                    new ItemResolver(itemDatabase);
            }

            if (gameFlow != null)
            {
                gameFlow.OnContextChanged +=
                    HandleContextChanged;
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(
                    OnCancelPressed);
            }

            if (dropButton != null)
            {
                dropButton.onClick.AddListener(
                    OnDropPressed);
            }

            UpdateVisibility();
        }

        private void OnDestroy()
        {
            if (gameFlow != null)
            {
                gameFlow.OnContextChanged -=
                    HandleContextChanged;
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveListener(
                    OnCancelPressed);
            }

            if (dropButton != null)
            {
                dropButton.onClick.RemoveListener(
                    OnDropPressed);
            }
        }

        private void HandleContextChanged(
            GameFlowContext previous,
            GameFlowContext current)
        {
            UpdateVisibility();

            if (current ==
                GameFlowContext.Confirmation)
            {
                RefreshContent();
            }
        }

        private void UpdateVisibility()
        {
            if (panelRoot == null ||
                gameFlow == null)
            {
                return;
            }

            panelRoot.SetActive(
                gameFlow.CurrentContext ==
                GameFlowContext.Confirmation);
        }

        private void RefreshContent()
        {
            if (confirmationService == null)
                return;

            ItemInstance item =
                confirmationService.PendingItem;

            if (item == null)
            {
                ClearContent();
                return;
            }

            if (itemResolver == null)
            {
                ClearContent();
                return;
            }

            ItemBaseData itemData;

            if (!itemResolver.TryResolve(
                    item.BaseDataId,
                    out itemData))
            {
                ClearContent();
                return;
            }

            if (itemIcon != null)
            {
                itemIcon.sprite =
                    itemData.Icon;

                itemIcon.enabled =
                    itemData.Icon != null;
            }

            if (itemName != null)
            {
                itemName.text =
                    itemData.DisplayName;
            }

            if (description != null)
            {
                description.text =
                    "El objeto se dejará en el mundo.";
            }
        }

        private void ClearContent()
        {
            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }

            if (itemName != null)
            {
                itemName.text = string.Empty;
            }
        }

        private void OnCancelPressed()
        {
            if (confirmationService == null)
                return;

            confirmationService.Cancel();
        }

        private void OnDropPressed()
        {
            if (confirmationService == null)
                return;

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            RunManager runManager =
                bootstrapContext.RunManager;

            if (runManager == null)
                return;

            RuntimeExpeditionConfig config =
                runManager.CurrentRunConfig;

            ExpeditionSceneContext sceneContext =
                ExpeditionSceneContext.Current;

            if (config == null ||
                sceneContext == null ||
                sceneContext.Player == null)
            {
                return;
            }

            ExpeditionRuntimeState state =
                runManager.ExpeditionRuntimeState;

            if (state == null)
                return;

            ExpeditionInventoryRuntime inventory =
                state.Inventory;

            if (inventory == null)
                return;

            RuntimeReferencesConfig references =
                new RuntimeReferencesConfig(
                    sceneContext.Player.transform);

            confirmationService.Confirm(
                config,
                references,
                state,
                inventory);
        }
    }
}