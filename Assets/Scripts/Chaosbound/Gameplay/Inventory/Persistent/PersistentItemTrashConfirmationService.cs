using Chaosbound.Core.GameFlow;
using Chaosbound.Gameplay.Items.Runtime;
using System;

namespace Chaosbound.Gameplay.Inventory.Persistent
{
    public sealed class PersistentItemTrashConfirmationService
    {
        private readonly PersistentInventoryRuntime
            persistentInventoryRuntime;

        private readonly GameFlow gameFlow;

        private ItemInstance pendingItem;

        public ItemInstance PendingItem =>
            pendingItem;

        public bool HasPendingItem =>
            pendingItem != null;

        public PersistentItemTrashConfirmationService(
            PersistentInventoryRuntime persistentInventoryRuntime,
            GameFlow gameFlow)
        {
            if (persistentInventoryRuntime == null)
            {
                throw new ArgumentNullException(
                    nameof(persistentInventoryRuntime));
            }

            if (gameFlow == null)
            {
                throw new ArgumentNullException(
                    nameof(gameFlow));
            }

            this.persistentInventoryRuntime =
                persistentInventoryRuntime;

            this.gameFlow =
                gameFlow;
        }

        public bool Request(
            ItemInstance item)
        {
            if (item == null)
                return false;

            if (pendingItem != null)
                return false;

            pendingItem = item;

            gameFlow.Request(
                GameFlowContext.Confirmation);

            return true;
        }

        public bool Confirm()
        {
            if (pendingItem == null)
            {
                UnityEngine.Debug.Log(
                    "PersistentItemTrashConfirmationService: No pending item.");
                return false;
            }

            PersistentInventoryState state =
                persistentInventoryRuntime.State;

            if (state == null ||
                state.Items == null)
            {
                UnityEngine.Debug.Log(
                    "PersistentItemTrashConfirmationService: Inventory state is null.");
                return false;
            }

            bool removed =
                state.Items.TryRemove(
                    pendingItem);

            UnityEngine.Debug.Log(
                $"PersistentItemTrashConfirmationService: " +
                $"Remove result = {removed}, " +
                $"Item = {pendingItem.InstanceId}");

            if (!removed)
                return false;

            pendingItem = null;

            gameFlow.Pop(
                GameFlowContext.Confirmation);

            return true;
        }

        public void Cancel()
        {
            if (pendingItem == null)
                return;

            pendingItem = null;

            gameFlow.Pop(
                GameFlowContext.Confirmation);
        }
    }
}