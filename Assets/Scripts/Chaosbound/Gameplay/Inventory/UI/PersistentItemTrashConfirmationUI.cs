using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class PersistentItemTrashConfirmationUI :
        MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject panelRoot;

        [Header("Stash")]
        [SerializeField]
        private PersonalStashInventoryUI stashInventoryUI;

        [Header("Content")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text description;

        [Header("Buttons")]
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button deleteButton;

        private GameFlow gameFlow;

        private PersistentItemTrashConfirmationService
            confirmationService;

        private ItemResolver itemResolver;

        private void Start()
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            gameFlow =
                bootstrapContext.GameFlow;

            confirmationService =
                bootstrapContext
                    .PersistentItemTrashConfirmationService;

            GameContentContext compositionContext =
                GameContentContext.Current;

            if (compositionContext != null &&
                compositionContext.ItemDatabase != null)
            {
                itemResolver =
                    new ItemResolver(
                        compositionContext.ItemDatabase);
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

            if (deleteButton != null)
            {
                deleteButton.onClick.AddListener(
                    OnDeletePressed);
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

            if (deleteButton != null)
            {
                deleteButton.onClick.RemoveListener(
                    OnDeletePressed);
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
            if (panelRoot == null)
                return;

            bool visible =
                gameFlow != null &&
                confirmationService != null &&
                gameFlow.CurrentContext ==
                    GameFlowContext.Confirmation &&
                confirmationService.HasPendingItem;

            panelRoot.SetActive(visible);
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

            if (!itemResolver.TryResolve(
                    item.BaseDataId,
                    out ItemBaseData itemData))
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
                    "El objeto será eliminado permanentemente.";
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

            if (description != null)
            {
                description.text = string.Empty;
            }
        }

        private void OnCancelPressed()
        {
            if (confirmationService == null)
                return;

            confirmationService.Cancel();
        }

        private void OnDeletePressed()
        {
            if (confirmationService == null)
            {
                return;
            }

            bool confirmed =
                confirmationService.Confirm();

            if (!confirmed)
                return;

            if (stashInventoryUI == null)
            {
                return;
            }

            stashInventoryUI.RefreshInventory();
        }
    }
}