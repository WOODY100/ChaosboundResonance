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

        [Header("Persistent Stash")]
        [SerializeField] private InventoryUITrashDropTarget trashDropTarget;

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

            SetTrashDragVisual(
                sourceSlotUI,
                true);

            if (IsPointerOverTrash(eventData))
            {
                if (trashDropTarget != null)
                {
                    trashDropTarget.ShowValidFeedback();
                }

                ClearDropFeedback();
                return;
            }

            if (trashDropTarget != null)
            {
                trashDropTarget.ClearFeedback();
            }

            InventoryUIDropTarget target =
                ResolveDropTarget(eventData);

            if (target == null)
            {
                ClearDropFeedback();
                return;
            }

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

        private void SetTrashDragVisual(
            InventorySlotUI sourceSlotUI,
            bool active)
        {
            if (trashDropTarget == null)
                return;

            if (!IsPersistentStashSlot(sourceSlotUI))
            {
                trashDropTarget.SetDragVisual(false);
                return;
            }

            trashDropTarget.SetDragVisual(active);
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

            // =========================================================
            // PERSISTENT STASH
            // =========================================================

            if (target.TargetType ==
                    InventoryUIDropTargetType.Main &&
                IsPersistentStashSlot(sourceSlotUI))
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

                PersistentItemInventoryState inventory =
                    persistentInventoryRuntime
                        .State
                        .Items;

                if (inventory == null)
                    return DropFeedbackType.Invalid;

                int sourceIndex =
                    sourceSlotUI.SlotIndex;

                int destinationIndex =
                    target.SlotIndex;

                if (sourceIndex < 0 ||
                    sourceIndex >= inventory.Count)
                {
                    return DropFeedbackType.Invalid;
                }

                if (sourceIndex == destinationIndex)
                    return DropFeedbackType.Invalid;

                // A visual empty slot means
                // "move to the end".
                if (destinationIndex >= inventory.Count)
                    return DropFeedbackType.Valid;

                ItemInstance destinationItem;

                bool occupied =
                    inventory.TryGetAt(
                        destinationIndex,
                        out destinationItem);

                if (!occupied)
                    return DropFeedbackType.Invalid;

                return DropFeedbackType.Swap;
            }



            // =========================================================
            // EXPEDITION / SECURE
            // =========================================================

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

        private bool IsPersistentStashSlot(
            InventorySlotUI slotUI)
        {
            if (slotUI == null)
                return false;

            return slotUI.GetComponentInParent<
                PersonalStashInventoryUI>() != null;
        }

        // =========================================================
        // DROP RESOLUTION
        // =========================================================

        public void HandleDrop(
    InventorySlotUI sourceSlotUI,
    PointerEventData eventData)
        {
            ClearDropFeedback();

            SetTrashDragVisual(
                sourceSlotUI,
                false);

            if (sourceSlotUI == null)
                return;

            if (sourceSlotUI.CurrentItem == null)
                return;

            if (IsPointerOverTrash(eventData))
            {
                HandleTrashDrop(
                    sourceSlotUI);

                return;
            }

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

        private bool IsPointerOverTrash(
            PointerEventData eventData)
        {
            if (eventData == null ||
                trashDropTarget == null)
            {
                return false;
            }

            GameObject hitObject =
                eventData.pointerCurrentRaycast.gameObject;

            if (hitObject == null)
                return false;

            return hitObject.GetComponentInParent<
                InventoryUITrashDropTarget>() ==
                trashDropTarget;
        }

        // =========================================================
        // PERSISTENT TRASH
        // =========================================================

        private void HandleTrashDrop(
            InventorySlotUI sourceSlotUI)
        {
            if (sourceSlotUI == null)
                return;

            if (!IsPersistentStashSlot(sourceSlotUI))
                return;

            ItemInstance item =
                sourceSlotUI.CurrentItem;

            if (item == null)
                return;

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            PersistentItemTrashConfirmationService
                confirmationService =
                    bootstrapContext
                        .PersistentItemTrashConfirmationService;

            if (confirmationService == null)
                return;

            bool requested =
                confirmationService.Request(item);

            if (requested)
            {
                UnityEngine.Debug.Log(
                    $"Inventory trash confirmation requested: {item.InstanceId}");
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

            if (target.TargetType ==
                    InventoryUIDropTargetType.Main &&
                IsPersistentStashSlot(sourceSlotUI))
            {
                HandlePersistentStashReorder(
                    sourceSlotUI,
                    target);

                return;
            }

            RunManager runManager =
                bootstrapContext.RunManager;

            if (runManager == null)
                return;

            // Existing Expedition logic...

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

        private void HandlePersistentStashReorder(
            InventorySlotUI sourceSlotUI,
            InventoryUIDropTarget target)
        {
            if (sourceSlotUI == null ||
                target == null)
            {
                return;
            }

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            PersistentInventoryRuntime
                persistentInventoryRuntime =
                    bootstrapContext.PersistentInventoryRuntime;

            if (persistentInventoryRuntime == null ||
                persistentInventoryRuntime.State == null)
            {
                return;
            }

            PersistentItemInventoryState inventory =
                persistentInventoryRuntime
                    .State
                    .Items;

            if (inventory == null)
                return;

            int sourceIndex =
                sourceSlotUI.SlotIndex;

            if (sourceIndex < 0 ||
                sourceIndex >= inventory.Count)
            {
                return;
            }

            int destinationIndex =
                target.SlotIndex;

            if (sourceIndex == destinationIndex)
                return;

            // Any visual empty slot beyond the actual
            // inventory becomes "move to end".
            if (destinationIndex >= inventory.Count)
            {
                destinationIndex =
                    inventory.Count - 1;
            }

            if (destinationIndex < 0)
                return;

            bool reordered =
                inventory.TryReorder(
                    sourceIndex,
                    destinationIndex);

            if (!reordered)
                return;

            UnityEngine.Debug.Log(
                $"Inventory drag: Persistent {sourceIndex} -> {destinationIndex}");

            PersonalStashInventoryUI stashUI =
                sourceSlotUI.GetComponentInParent<
                    PersonalStashInventoryUI>();

            if (stashUI != null)
            {
                stashUI.RefreshInventory();
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