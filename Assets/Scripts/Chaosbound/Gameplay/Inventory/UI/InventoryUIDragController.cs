using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Core.Composition;
using Chaosbound.Core.Settings;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Inventory.Runtime;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.World.Integration;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class InventoryUIDragController : MonoBehaviour
    {
        public static InventoryUIDragController Instance { get; private set; }

        private InventoryUIDropTarget activeFeedbackTarget;

        private enum DropFeedbackType
        {
            Invalid,
            Valid,
            Swap
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            ClearDropFeedback();

            if (Instance == this)
                Instance = null;
        }

        // =========================================================
        // DRAG FEEDBACK
        // =========================================================

        public void UpdateDropFeedback(
            InventorySlotUI sourceSlotUI,
            PointerEventData eventData)
        {
            if (sourceSlotUI == null)
            {
                ClearDropFeedback();
                return;
            }

            if (sourceSlotUI.CurrentItem == null)
            {
                ClearDropFeedback();
                return;
            }

            InventoryUIDropTarget target =
                ResolveDropTarget(eventData);

            if (target == null)
            {
                ClearDropFeedback();
                return;
            }

            if (activeFeedbackTarget != target)
            {
                ClearDropFeedback();

                activeFeedbackTarget =
                    target;
            }

            DropFeedbackType feedbackType =
                GetDropFeedbackType(
                    sourceSlotUI,
                    target);

            ApplyDropFeedback(
                target,
                feedbackType);
        }

        private DropFeedbackType GetDropFeedbackType(
            InventorySlotUI sourceSlotUI,
            InventoryUIDropTarget target)
        {
            if (sourceSlotUI == null ||
                target == null)
            {
                return DropFeedbackType.Invalid;
            }

            if (target.TargetType ==
                InventoryUIDropTargetType.Main)
            {
                BootstrapContext bootstrapContext =
                    BootstrapContext.Current;

                if (bootstrapContext == null)
                    return DropFeedbackType.Invalid;

                RunManager runManager =
                    bootstrapContext.RunManager;

                if (runManager == null)
                    return DropFeedbackType.Invalid;

                ExpeditionRuntimeState state =
                    runManager.ExpeditionRuntimeState;

                if (state == null ||
                    state.Inventory == null)
                {
                    return DropFeedbackType.Invalid;
                }

                int sourceIndex =
                    sourceSlotUI.SlotIndex;

                int destinationIndex =
                    target.SlotIndex;

                if (sourceIndex == destinationIndex)
                    return DropFeedbackType.Invalid;

                ItemInstance destinationItem;

                bool occupied =
                    state.Inventory.TryGetAt(
                        destinationIndex,
                        out destinationItem);

                if (!occupied)
                    return DropFeedbackType.Valid;

                return DropFeedbackType.Swap;
            }

            if (target.TargetType ==
                InventoryUIDropTargetType.Secure)
            {
                BootstrapContext bootstrapContext =
                    BootstrapContext.Current;

                if (bootstrapContext == null)
                    return DropFeedbackType.Invalid;

                PersistentInventoryRuntime
                    persistentInventoryRuntime =
                        bootstrapContext.PersistentInventoryRuntime;

                if (persistentInventoryRuntime == null ||
                    persistentInventoryRuntime.State == null)
                {
                    return DropFeedbackType.Invalid;
                }

                SecureInventoryState secureInventory =
                    persistentInventoryRuntime
                        .State
                        .SecureInventory;

                if (secureInventory == null)
                    return DropFeedbackType.Invalid;

                int destinationIndex =
                    target.SlotIndex;

                if (!secureInventory.IsUnlocked(
                        destinationIndex))
                {
                    return DropFeedbackType.Invalid;
                }

                ItemInstance destinationItem;

                bool occupied =
                    secureInventory.TryGetAt(
                        destinationIndex,
                        out destinationItem);

                if (occupied)
                {
                    // Main -> Secure cannot swap.
                    return DropFeedbackType.Invalid;
                }

                return DropFeedbackType.Valid;
            }

            return DropFeedbackType.Invalid;
        }

        private DropFeedbackType GetDropFeedbackType(
            SecureInventorySlotUI sourceSlotUI,
            InventoryUIDropTarget target)
        {
            if (sourceSlotUI == null ||
                target == null)
            {
                return DropFeedbackType.Invalid;
            }

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return DropFeedbackType.Invalid;

            PersistentInventoryRuntime
                persistentInventoryRuntime =
                    bootstrapContext.PersistentInventoryRuntime;

            if (persistentInventoryRuntime == null ||
                persistentInventoryRuntime.State == null)
            {
                return DropFeedbackType.Invalid;
            }

            SecureInventoryState secureInventory =
                persistentInventoryRuntime
                    .State
                    .SecureInventory;

            if (secureInventory == null)
                return DropFeedbackType.Invalid;

            int sourceIndex =
                sourceSlotUI.SlotIndex;

            int destinationIndex =
                target.SlotIndex;

            if (target.TargetType ==
                InventoryUIDropTargetType.Main)
            {
                RunManager runManager =
                    bootstrapContext.RunManager;

                if (runManager == null)
                    return DropFeedbackType.Invalid;

                ExpeditionRuntimeState state =
                    runManager.ExpeditionRuntimeState;

                if (state == null ||
                    state.Inventory == null)
                {
                    return DropFeedbackType.Invalid;
                }

                ItemInstance destinationItem;

                bool occupied =
                    state.Inventory.TryGetAt(
                        destinationIndex,
                        out destinationItem);

                if (occupied)
                {
                    // Secure -> Main cannot swap.
                    return DropFeedbackType.Invalid;
                }

                return DropFeedbackType.Valid;
            }

            if (target.TargetType ==
                InventoryUIDropTargetType.Secure)
            {
                if (sourceIndex == destinationIndex)
                    return DropFeedbackType.Invalid;

                if (!secureInventory.IsUnlocked(sourceIndex) ||
                    !secureInventory.IsUnlocked(destinationIndex))
                {
                    return DropFeedbackType.Invalid;
                }

                ItemInstance sourceItem;

                if (!secureInventory.TryGetAt(
                        sourceIndex,
                        out sourceItem))
                {
                    return DropFeedbackType.Invalid;
                }

                ItemInstance destinationItem;

                bool occupied =
                    secureInventory.TryGetAt(
                        destinationIndex,
                        out destinationItem);

                if (!occupied)
                    return DropFeedbackType.Valid;

                return DropFeedbackType.Swap;
            }

            return DropFeedbackType.Invalid;
        }

        private void ApplyDropFeedback(
            InventoryUIDropTarget target,
            DropFeedbackType feedbackType)
        {
            if (target == null)
                return;

            switch (feedbackType)
            {
                case DropFeedbackType.Valid:
                    target.ShowValidFeedback();
                    break;

                case DropFeedbackType.Swap:
                    target.ShowSwapFeedback();
                    break;

                default:
                    target.ShowInvalidFeedback();
                    break;
            }
        }

        public void UpdateDropFeedback(
            SecureInventorySlotUI sourceSlotUI,
            PointerEventData eventData)
        {
            if (sourceSlotUI == null)
            {
                ClearDropFeedback();
                return;
            }

            if (sourceSlotUI.CurrentItem == null)
            {
                ClearDropFeedback();
                return;
            }

            InventoryUIDropTarget target =
                ResolveDropTarget(eventData);

            if (target == null)
            {
                ClearDropFeedback();
                return;
            }

            if (activeFeedbackTarget != target)
            {
                ClearDropFeedback();

                activeFeedbackTarget =
                    target;
            }

            DropFeedbackType feedbackType =
                GetDropFeedbackType(
                    sourceSlotUI,
                    target);

            ApplyDropFeedback(
                target,
                feedbackType);
        }

        public void ClearDropFeedback()
        {
            if (activeFeedbackTarget != null)
            {
                activeFeedbackTarget.ClearFeedback();
                activeFeedbackTarget = null;
            }
        }

        // =========================================================
        // VALIDATION
        // =========================================================

        private bool IsValidDropTarget(
            InventorySlotUI sourceSlotUI,
            InventoryUIDropTarget target)
        {
            if (sourceSlotUI == null ||
                target == null)
            {
                return false;
            }

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return false;

            RunManager runManager =
                bootstrapContext.RunManager;

            if (runManager == null)
                return false;

            ExpeditionRuntimeState state =
                runManager.ExpeditionRuntimeState;

            if (state == null)
                return false;

            int sourceIndex =
                sourceSlotUI.SlotIndex;

            int destinationIndex =
                target.SlotIndex;

            if (sourceIndex < 0 ||
                destinationIndex < 0)
            {
                return false;
            }

            if (target.TargetType ==
                InventoryUIDropTargetType.Main)
            {
                ExpeditionInventoryRuntime inventory =
                    state.Inventory;

                if (inventory == null)
                    return false;

                // Main -> Main
                return IsValidMainTarget(
                    inventory,
                    sourceIndex,
                    destinationIndex);
            }

            if (target.TargetType ==
                InventoryUIDropTargetType.Secure)
            {
                PersistentInventoryRuntime
                    persistentInventoryRuntime =
                        bootstrapContext
                            .PersistentInventoryRuntime;

                if (persistentInventoryRuntime == null)
                    return false;

                if (persistentInventoryRuntime.State == null)
                    return false;

                SecureInventoryState secureInventory =
                    persistentInventoryRuntime
                        .State
                        .SecureInventory;

                if (secureInventory == null)
                    return false;

                // Main -> Secure
                return IsValidSecureTarget(
                    secureInventory,
                    destinationIndex);
            }

            return false;
        }

        private bool IsValidDropTarget(
            SecureInventorySlotUI sourceSlotUI,
            InventoryUIDropTarget target)
        {
            if (sourceSlotUI == null ||
                target == null)
            {
                return false;
            }

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return false;

            RunManager runManager =
                bootstrapContext.RunManager;

            if (runManager == null)
                return false;

            ExpeditionRuntimeState state =
                runManager.ExpeditionRuntimeState;

            if (state == null)
                return false;

            int sourceIndex =
                sourceSlotUI.SlotIndex;

            int destinationIndex =
                target.SlotIndex;

            if (sourceIndex < 0 ||
                destinationIndex < 0)
            {
                return false;
            }

            PersistentInventoryRuntime
                persistentInventoryRuntime =
                    bootstrapContext
                        .PersistentInventoryRuntime;

            if (persistentInventoryRuntime == null)
                return false;

            if (persistentInventoryRuntime.State == null)
                return false;

            SecureInventoryState secureInventory =
                persistentInventoryRuntime
                    .State
                    .SecureInventory;

            if (secureInventory == null)
                return false;

            if (target.TargetType ==
                InventoryUIDropTargetType.Main)
            {
                ExpeditionInventoryRuntime inventory =
                    state.Inventory;

                if (inventory == null)
                    return false;

                // Secure -> Main
                return IsValidMainTarget(
                    inventory,
                    -1,
                    destinationIndex);
            }

            if (target.TargetType ==
                InventoryUIDropTargetType.Secure)
            {
                // Secure -> Secure
                return IsValidSecureToSecureTarget(
                    secureInventory,
                    sourceIndex,
                    destinationIndex);
            }

            return false;
        }

        private bool IsValidMainTarget(
            ExpeditionInventoryRuntime inventory,
            int sourceIndex,
            int destinationIndex)
        {
            if (inventory == null)
                return false;

            if (destinationIndex < 0 ||
                destinationIndex >= inventory.Capacity)
            {
                return false;
            }

            if (sourceIndex >= 0 &&
                sourceIndex == destinationIndex)
            {
                return false;
            }

            ItemInstance destinationItem;

            bool destinationOccupied =
                inventory.TryGetAt(
                    destinationIndex,
                    out destinationItem);

            // Empty destination:
            // normal move is valid.
            if (!destinationOccupied)
            {
                return true;
            }

            // Occupied destination:
            // valid only when this is a Main -> Main drag.
            if (sourceIndex >= 0)
            {
                return true;
            }

            // Secure -> Main cannot swap across containers.
            return false;
        }

        private bool IsValidSecureTarget(
            SecureInventoryState secureInventory,
            int destinationIndex)
        {
            if (secureInventory == null)
                return false;

            if (!secureInventory.IsUnlocked(
                    destinationIndex))
            {
                return false;
            }

            ItemInstance destinationItem;

            if (secureInventory.TryGetAt(
                    destinationIndex,
                    out destinationItem))
            {
                // Destination occupied.
                return false;
            }

            return true;
        }

        private bool IsValidSecureToSecureTarget(
            SecureInventoryState secureInventory,
            int sourceIndex,
            int destinationIndex)
        {
            if (secureInventory == null)
                return false;

            if (sourceIndex == destinationIndex)
                return false;

            if (!secureInventory.IsUnlocked(sourceIndex))
                return false;

            if (!secureInventory.IsUnlocked(destinationIndex))
                return false;

            ItemInstance sourceItem;

            if (!secureInventory.TryGetAt(
                    sourceIndex,
                    out sourceItem))
            {
                return false;
            }

            if (sourceItem == null)
                return false;

            // Empty or occupied destination are both valid
            // for Secure -> Secure.
            return true;
        }

        // =========================================================
        // DROP RESOLUTION
        // =========================================================

        public void HandleDrop(
            InventorySlotUI sourceSlotUI,
            PointerEventData eventData)
        {
            ClearDropFeedback();

            if (sourceSlotUI == null)
                return;

            if (sourceSlotUI.CurrentItem == null)
                return;

            InventoryUIDropTarget target =
                ResolveDropTarget(eventData);

            if (target != null)
            {
                HandleInventoryDrop(
                    sourceSlotUI,
                    target);

                return;
            }

            HandleWorldDrop(sourceSlotUI);
        }

        public void HandleDrop(
            SecureInventorySlotUI sourceSlotUI,
            PointerEventData eventData)
        {
            ClearDropFeedback();

            if (sourceSlotUI == null)
            {
                return;
            }

            if (sourceSlotUI.CurrentItem == null)
            {
                return;
            }

            InventoryUIDropTarget target =
                ResolveDropTarget(eventData);

            if (target == null)
            {
                // Secure items cannot be dropped into the world.
                return;
            }

            if (target.TargetType ==
                InventoryUIDropTargetType.Main)
            {
                HandleSecureToMainDrop(
                    sourceSlotUI,
                    target);

                return;
            }

            if (target.TargetType ==
                InventoryUIDropTargetType.Secure)
            {
                HandleSecureToSecureDrop(
                    sourceSlotUI,
                    target);

                return;
            }
        }

        // =========================================================
        // SECURE -> SECURE
        // =========================================================

        private void HandleSecureToSecureDrop(
            SecureInventorySlotUI sourceSlotUI,
            InventoryUIDropTarget target)
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
            {
                return;
            }

            RunManager runManager =
                bootstrapContext.RunManager;

            if (runManager == null)
            {
                return;
            }

            ExpeditionRuntimeState state =
                runManager.ExpeditionRuntimeState;

            if (state == null)
            {
                return;
            }

            PersistentInventoryRuntime
                persistentInventoryRuntime =
                    bootstrapContext.PersistentInventoryRuntime;

            if (persistentInventoryRuntime == null)
            {
                return;
            }

            if (persistentInventoryRuntime.State == null)
            {
                return;
            }

            SecureInventoryState secureInventory =
                persistentInventoryRuntime
                    .State
                    .SecureInventory;

            int sourceIndex =
                sourceSlotUI.SlotIndex;

            int destinationIndex =
                target.SlotIndex;

            ItemInstance destinationItem;

            bool destinationOccupied =
                secureInventory.TryGetAt(
                    destinationIndex,
                    out destinationItem);

            if (!destinationOccupied)
            {
                bool moved =
                    secureInventory.TryMove(
                        sourceIndex,
                        destinationIndex);

                if (moved)
                {
                    UnityEngine.Debug.Log(
                        $"Inventory drag: Secure {sourceIndex} -> Secure {destinationIndex}");
                }

                return;
            }

            bool swapped =
                secureInventory.TrySwap(
                    sourceIndex,
                    destinationIndex);

            if (swapped)
            {
                UnityEngine.Debug.Log(
                    $"Inventory drag: Secure {sourceIndex} <-> Secure {destinationIndex}");
            }
        }

        // =========================================================
        // SECURE -> MAIN
        // =========================================================

        private void HandleSecureToMainDrop(
            SecureInventorySlotUI sourceSlotUI,
            InventoryUIDropTarget target)
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
            {
                return;
            }

            RunManager runManager =
                bootstrapContext.RunManager;

            if (runManager == null)
            {
                return;
            }

            ExpeditionRuntimeState state =
                runManager.ExpeditionRuntimeState;

            if (state == null)
            {
                return;
            }

            ExpeditionInventoryRuntime inventory =
                state.Inventory;

            if (inventory == null)
            {
                return;
            }

            PersistentInventoryRuntime
                persistentInventoryRuntime =
                    bootstrapContext.PersistentInventoryRuntime;

            if (persistentInventoryRuntime == null)
            {
                return;
            }

            if (persistentInventoryRuntime.State == null)
            {
                return;
            }

            SecureInventoryState secureInventory =
                persistentInventoryRuntime
                    .State
                    .SecureInventory;

            int sourceIndex =
                sourceSlotUI.SlotIndex;

            int destinationIndex =
                target.SlotIndex;

            bool moved =
                inventory.TryMoveFromSecure(
                    secureInventory,
                    sourceIndex,
                    destinationIndex);

            if (moved)
            {
                UnityEngine.Debug.Log(
                    $"Inventory drag: Secure {sourceIndex} -> Main {destinationIndex}");
            }
        }

        // =========================================================
        // TARGET RESOLUTION
        // =========================================================

        private InventoryUIDropTarget ResolveDropTarget(
            PointerEventData eventData)
        {
            if (eventData == null)
                return null;

            GameObject hitObject =
                eventData.pointerCurrentRaycast.gameObject;

            if (hitObject == null)
                return null;

            return hitObject.GetComponentInParent<
                InventoryUIDropTarget>();
        }

        // =========================================================
        // MAIN INVENTORY DROP
        // =========================================================

        private void HandleInventoryDrop(
            InventorySlotUI sourceSlotUI,
            InventoryUIDropTarget target)
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            RunManager runManager =
                bootstrapContext.RunManager;

            if (runManager == null)
                return;

            ExpeditionRuntimeState state =
                runManager.ExpeditionRuntimeState;

            if (state == null)
                return;

            ExpeditionInventoryRuntime inventory =
                state.Inventory;

            if (inventory == null)
                return;

            int sourceIndex =
                sourceSlotUI.SlotIndex;

            int destinationIndex =
                target.SlotIndex;

            if (target.TargetType ==
                InventoryUIDropTargetType.Main)
            {
                ItemInstance destinationItem;

                bool destinationOccupied =
                    inventory.TryGetAt(
                        destinationIndex,
                        out destinationItem);

                if (!destinationOccupied)
                {
                    bool moved =
                        inventory.TryMove(
                            sourceIndex,
                            destinationIndex);

                    if (moved)
                    {
                        UnityEngine.Debug.Log(
                            $"Inventory drag: Main {sourceIndex} -> Main {destinationIndex}");
                    }

                    return;
                }

                bool swapped =
                    inventory.TrySwap(
                        sourceIndex,
                        destinationIndex);

                if (swapped)
                {
                    UnityEngine.Debug.Log(
                        $"Inventory drag: Main {sourceIndex} <-> Main {destinationIndex}");
                }

                return;
            }

            SecureInventoryState secureInventory =
                bootstrapContext
                    .PersistentInventoryRuntime
                    .State
                    .SecureInventory;

            bool movedToSecure =
                inventory.TryMoveToSecure(
                    sourceIndex,
                    secureInventory,
                    destinationIndex);

            if (movedToSecure)
            {
                UnityEngine.Debug.Log(
                    $"Inventory drag: Main {sourceIndex} -> Secure {destinationIndex}");
            }
        }

        // =========================================================
        // WORLD DROP
        // =========================================================

        private void HandleWorldDrop(
            InventorySlotUI sourceSlotUI)
        {
            if (sourceSlotUI == null)
            {
                return;
            }

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
            {
                return;
            }

            RunManager runManager =
                bootstrapContext.RunManager;

            if (runManager == null)
            {
                return;
            }

            ExpeditionRuntimeState state =
                runManager.ExpeditionRuntimeState;

            if (state == null)
            {
                return;
            }

            ExpeditionInventoryRuntime inventory =
                state.Inventory;

            if (inventory == null)
            {
                return;
            }

            int sourceIndex =
                sourceSlotUI.SlotIndex;

            ItemInstance item;

            if (!inventory.TryGetAt(
                    sourceIndex,
                    out item))
            {
                return;
            }

            if (item == null)
            {
                return;
            }

            GameSettingsRuntime settingsRuntime =
                bootstrapContext.GameSettingsRuntime;

            if (settingsRuntime == null ||
                settingsRuntime.Settings == null)
            {
                return;
            }

            GameSettings settings =
                settingsRuntime.Settings;

            if (settings.ConfirmWorldItemDrop)
            {
                RequestWorldDropConfirmation(
                    runManager,
                    item,
                    sourceIndex);

                return;
            }

            ExecuteWorldDropDirectly(
                runManager,
                state,
                inventory,
                item,
                sourceIndex);
        }

        private void RequestWorldDropConfirmation(
            RunManager runManager,
            ItemInstance item,
            int sourceIndex)
        {
            if (runManager == null)
            {
                return;
            }

            ItemWorldDropConfirmationService
                confirmationService =
                    runManager.ItemWorldDropConfirmationService;

            if (confirmationService == null)
            {
                return;
            }

            confirmationService.Request(
                item,
                sourceIndex);
        }

        private void ExecuteWorldDropDirectly(
            RunManager runManager,
            ExpeditionRuntimeState state,
            ExpeditionInventoryRuntime inventory,
            ItemInstance item,
            int sourceIndex)
        {
            if (runManager == null ||
                state == null ||
                inventory == null ||
                item == null)
            {
                return;
            }

            RuntimeExpeditionConfig config =
                runManager.CurrentRunConfig;

            if (config == null)
            {
                return;
            }

            ExpeditionSceneContext sceneContext =
                ExpeditionSceneContext.Current;

            if (sceneContext == null ||
                sceneContext.Player == null)
            {
                return;
            }

            ItemWorldDropService
                itemWorldDropService =
                    runManager.ItemWorldDropService;

            if (itemWorldDropService == null)
            {
                return;
            }

            RuntimeReferencesConfig references =
                new RuntimeReferencesConfig(
                    sceneContext.Player.transform);

            bool dropped =
                itemWorldDropService.TryDrop(
                    item,
                    config,
                    references,
                    state);

            if (!dropped)
            {
                return;
            }

            ItemInstance currentItem;

            if (!inventory.TryGetAt(
                    sourceIndex,
                    out currentItem))
            {
                return;
            }

            if (currentItem == null)
            {
                return;
            }

            if (!string.Equals(
                    currentItem.InstanceId,
                    item.InstanceId,
                    System.StringComparison.Ordinal))
            {
                return;
            }

            if (!inventory.TryRemoveAt(
                    sourceIndex,
                    out ItemInstance removedItem))
            {
                return;
            }

            if (removedItem == null ||
                !string.Equals(
                    removedItem.InstanceId,
                    item.InstanceId,
                    System.StringComparison.Ordinal))
            {
                return;
            }

            UnityEngine.Debug.Log(
                $"Inventory world drop: Main {sourceIndex} -> World");
        }
    }
}