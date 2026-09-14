using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Content.Items;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Inventory.Runtime;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.MetaProgression.Persistent;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Settlement
{
    public sealed class ExpeditionSettlementService
    {
        private readonly PersistentItemInventoryState persistentItems;
        private readonly PersistentMaterialsState persistentMaterials;
        private readonly PersistentMetaState persistentMeta;

        private readonly ExpeditionRewardItemResolver
            expeditionRewardItemResolver;

        private readonly ItemInstanceFactory
            itemInstanceFactory;

        private bool isSettled;

        public ExpeditionSettlementService(
            PersistentItemInventoryState persistentItems,
            PersistentMaterialsState persistentMaterials,
            PersistentMetaState persistentMeta,
            ExpeditionRewardItemResolver expeditionRewardItemResolver,
            ItemInstanceFactory itemInstanceFactory)
        {
            this.persistentItems =
                persistentItems ??
                throw new ArgumentNullException(nameof(persistentItems));

            this.persistentMaterials =
                persistentMaterials ??
                throw new ArgumentNullException(nameof(persistentMaterials));

            this.persistentMeta =
                persistentMeta ??
                throw new ArgumentNullException(nameof(persistentMeta));

            this.expeditionRewardItemResolver =
                expeditionRewardItemResolver ??
                throw new ArgumentNullException(
                    nameof(expeditionRewardItemResolver));

            this.itemInstanceFactory =
                itemInstanceFactory ??
                throw new ArgumentNullException(
                    nameof(itemInstanceFactory));
        }

        public bool IsSettled =>
            isSettled;

        public bool TrySettle(
            ExpeditionRuntimeState expeditionState,
            RuntimeExpeditionConfig expeditionConfig,
            out ExpeditionSettlementResult result)
        {
            if (isSettled)
            {
                result = CreateFailureResult();
                return false;
            }

            if (expeditionState == null)
            {
                result = CreateFailureResult();
                return false;
            }

            if (expeditionConfig == null)
            {
                result = CreateFailureResult();
                return false;
            }

            SettlementPlan plan;

            if (!TryBuildPlan(
                    expeditionState,
                    expeditionConfig,
                    out plan))
            {
                result = CreateFailureResult();
                return false;
            }

            Commit(plan);

            isSettled = true;

            result =
                new ExpeditionSettlementResult(
                    true,
                    plan.ExpeditionItemCount,
                    plan.MaterialAmounts,
                    plan.CompletionRewardItem,
                    plan.MetaExperience);

            return true;
        }

        private bool TryBuildPlan(
            ExpeditionRuntimeState expeditionState,
            RuntimeExpeditionConfig expeditionConfig,
            out SettlementPlan plan)
        {
            plan = new SettlementPlan();

            if (!TryPrepareExpeditionItems(
                    expeditionState,
                    plan))
            {
                return false;
            }

            if (!TryPrepareMaterials(
                    expeditionState,
                    plan))
            {
                return false;
            }

            if (!TryPrepareCompletionReward(
                    expeditionConfig,
                    plan))
            {
                return false;
            }

            return true;
        }

        private bool TryPrepareExpeditionItems(
            ExpeditionRuntimeState expeditionState,
            SettlementPlan plan)
        {
            IReadOnlyList<InventorySlot> slots =
                expeditionState.Inventory.GetSlots();

            for (int i = 0; i < slots.Count; i++)
            {
                InventorySlot slot = slots[i];

                if (slot == null || !slot.IsOccupied)
                    continue;

                ItemInstance item = slot.Item;

                if (item == null)
                    return false;

                plan.ExpeditionItems.Add(item);
            }

            plan.ExpeditionItemCount =
                plan.ExpeditionItems.Count;

            return true;
        }

        private bool TryPrepareMaterials(
            ExpeditionRuntimeState expeditionState,
            SettlementPlan plan)
        {
            IReadOnlyDictionary<string, int> amounts =
                expeditionState.Materials.GetAmounts();

            foreach (
                KeyValuePair<string, int> entry
                in amounts)
            {
                string contentId = entry.Key;
                int amount = entry.Value;

                if (string.IsNullOrWhiteSpace(contentId))
                    return false;

                if (amount <= 0)
                    return false;

                int currentAmount =
                    persistentMaterials.GetAmount(
                        contentId);

                if (amount >
                    int.MaxValue - currentAmount)
                {
                    return false;
                }

                plan.MaterialAmounts[contentId.Trim()] =
                    amount;
            }

            return true;
        }

        private bool TryPrepareCompletionReward(
            RuntimeExpeditionConfig expeditionConfig,
            SettlementPlan plan)
        {
            if (expeditionConfig.Rewards == null)
                return false;

            string itemContentId =
                expeditionConfig.Rewards.ItemContentId;

            if (!string.IsNullOrWhiteSpace(itemContentId))
            {
                ItemBaseData itemData;

                if (!expeditionRewardItemResolver.TryResolve(
                        itemContentId,
                        out itemData))
                {
                    return false;
                }

                if (itemData == null)
                    return false;

                plan.CompletionRewardItem =
                    itemInstanceFactory.Create(
                        itemData);
            }

            int metaExperience =
                expeditionConfig.Rewards.MetaExperience;

            if (metaExperience < 0)
                return false;

            if (!persistentMeta.CanAddExperience(
                    metaExperience))
            {
                return false;
            }

            plan.MetaExperience =
                metaExperience;

            return true;
        }

        private void Commit(
            SettlementPlan plan)
        {
            for (int i = 0; i < plan.ExpeditionItems.Count; i++)
            {
                ItemInstance item =
                    plan.ExpeditionItems[i];

                if (!persistentItems.TryAdd(item))
                {
                    throw new InvalidOperationException(
                        "Settlement commit failed while " +
                        "adding an expedition item.");
                }
            }

            foreach (
                KeyValuePair<string, int> entry
                in plan.MaterialAmounts)
            {
                if (!persistentMaterials.Add(
                        entry.Key,
                        entry.Value))
                {
                    throw new InvalidOperationException(
                        "Settlement commit failed while " +
                        "adding expedition materials.");
                }
            }

            if (plan.CompletionRewardItem != null)
            {
                if (!persistentItems.TryAdd(
                        plan.CompletionRewardItem))
                {
                    throw new InvalidOperationException(
                        "Settlement commit failed while " +
                        "adding the completion reward item.");
                }
            }

            if (plan.MetaExperience > 0)
            {
                persistentMeta.AddExperience(
                    plan.MetaExperience);
            }
        }

        private ExpeditionSettlementResult
            CreateFailureResult()
        {
            return new ExpeditionSettlementResult(
                false,
                0,
                new Dictionary<string, int>(),
                null,
                0);
        }

        private sealed class SettlementPlan
        {
            public readonly List<ItemInstance>
                ExpeditionItems =
                    new List<ItemInstance>();

            public readonly Dictionary<string, int>
                MaterialAmounts =
                    new Dictionary<string, int>(
                        StringComparer.Ordinal);

            public int ExpeditionItemCount;

            public ItemInstance CompletionRewardItem;

            public int MetaExperience;
        }
    }
}