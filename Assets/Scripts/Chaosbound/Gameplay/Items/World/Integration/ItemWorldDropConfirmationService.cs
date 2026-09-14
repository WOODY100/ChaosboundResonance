using System;
using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Core.GameFlow;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Inventory.Runtime;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Items.World.Integration
{
    public sealed class ItemWorldDropConfirmationService
    {
        private readonly GameFlow gameFlow;
        private readonly ItemWorldDropService itemWorldDropService;

        private ItemInstance pendingItem;
        private int pendingSourceIndex = -1;

        public bool HasPendingRequest =>
            pendingItem != null &&
            pendingSourceIndex >= 0;

        public ItemInstance PendingItem =>
            pendingItem;

        public int PendingSourceIndex =>
            pendingSourceIndex;

        public ItemWorldDropConfirmationService(
            GameFlow gameFlow,
            ItemWorldDropService itemWorldDropService)
        {
            this.gameFlow =
                gameFlow
                ?? throw new ArgumentNullException(
                    nameof(gameFlow));

            this.itemWorldDropService =
                itemWorldDropService
                ?? throw new ArgumentNullException(
                    nameof(itemWorldDropService));
        }

        public bool Request(
            ItemInstance item,
            int sourceIndex)
        {
            if (item == null)
                return false;

            if (sourceIndex < 0)
                return false;

            if (HasPendingRequest)
                return false;

            pendingItem = item;
            pendingSourceIndex = sourceIndex;

            bool requested =
                gameFlow.Request(
                    GameFlowContext.Confirmation);

            if (!requested)
            {
                ClearPending();
                return false;
            }

            return true;
        }

        public bool Confirm(
            RuntimeExpeditionConfig config,
            RuntimeReferencesConfig references,
            ExpeditionRuntimeState state,
            ExpeditionInventoryRuntime inventory)
        {
            if (!HasPendingRequest)
                return false;

            if (config == null ||
                references == null ||
                state == null ||
                inventory == null)
            {
                return false;
            }

            ItemInstance currentItem;

            if (!inventory.TryGetAt(
                    pendingSourceIndex,
                    out currentItem))
            {
                return false;
            }

            if (currentItem == null)
                return false;

            if (!string.Equals(
                    currentItem.InstanceId,
                    pendingItem.InstanceId,
                    StringComparison.Ordinal))
            {
                return false;
            }

            bool dropped =
                itemWorldDropService.TryDrop(
                    pendingItem,
                    config,
                    references,
                    state);

            if (!dropped)
                return false;

            bool removed =
                inventory.TryRemoveAt(
                    pendingSourceIndex,
                    out ItemInstance removedItem);

            if (!removed ||
                removedItem == null ||
                !string.Equals(
                    removedItem.InstanceId,
                    pendingItem.InstanceId,
                    StringComparison.Ordinal))
            {
                return false;
            }

            ClearPending();

            gameFlow.Pop(
                GameFlowContext.Confirmation);

            return true;
        }

        public bool Cancel()
        {
            if (!HasPendingRequest)
                return false;

            ClearPending();

            return gameFlow.Pop(
                GameFlowContext.Confirmation);
        }

        public void ClearPending()
        {
            pendingItem = null;
            pendingSourceIndex = -1;
        }
    }
}