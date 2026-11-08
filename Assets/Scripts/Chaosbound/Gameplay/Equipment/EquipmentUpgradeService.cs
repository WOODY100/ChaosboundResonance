using Chaosbound.Gameplay.Currency.Persistent;
using Chaosbound.Content.Items;
using Chaosbound.Content.Materials;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Save;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Equipment
{
    public sealed class EquipmentUpgradeService
    {
        private readonly ItemContentResolver itemContentResolver;
        private readonly MaterialResolver materialResolver;
        private readonly PersistentMaterialsState materialsState;
        private readonly EquipmentProgressionConfig progressionConfig;
        private readonly EquipmentProgressionRuntime progressionRuntime;
        private readonly IPersistentStateSaver persistentStateSaver;
        private readonly PersistentCurrencyState currencyState;

        private const string GoldContentId = "gold";

        public EquipmentUpgradeService(
            ItemContentResolver itemContentResolver,
            MaterialResolver materialResolver,
            PersistentMaterialsState materialsState,
            EquipmentProgressionConfig progressionConfig,
            EquipmentProgressionRuntime progressionRuntime,
            IPersistentStateSaver persistentStateSaver,
            PersistentCurrencyState currencyState)
        {
            this.persistentStateSaver =
                persistentStateSaver ??
                throw new ArgumentNullException(
                    nameof(persistentStateSaver));

            this.itemContentResolver =
                itemContentResolver ??
                throw new ArgumentNullException(
                    nameof(itemContentResolver));

            this.materialResolver =
                materialResolver ??
                throw new ArgumentNullException(
                    nameof(materialResolver));

            this.materialsState =
                materialsState ??
                throw new ArgumentNullException(
                    nameof(materialsState));

            this.progressionConfig =
                progressionConfig ??
                throw new ArgumentNullException(
                    nameof(progressionConfig));

            this.progressionRuntime =
                progressionRuntime ??
                throw new ArgumentNullException(
                    nameof(progressionRuntime));

            this.currencyState =
                currencyState ??
                throw new ArgumentNullException(nameof(currencyState));
        }

        //==========================================================
        // Read-Only Progression Queries
        //==========================================================

        public bool TryGetUpgradeCost(
            ItemInstance itemInstance,
            out ItemBaseData baseData,
            out IReadOnlyList<MaterialCost> costs)
        {
            baseData = null;
            costs = null;

            if (!TryResolveEquipment(
                    itemInstance,
                    out baseData))
            {
                return false;
            }

            if (!progressionRuntime.CanUpgrade(itemInstance))
                return false;

            if (!progressionConfig.TryGetUpgradeCost(
                    baseData.ContentId,
                    baseData.EquipmentType,
                    itemInstance.CurrentTier,
                    itemInstance.UpgradeLevel,
                    out costs))
            {
                return false;
            }

            if (!TryValidateMaterialCosts(costs))
            {
                costs = null;
                return false;
            }

            return true;
        }

        public bool TryGetRerollCost(
            ItemInstance itemInstance,
            out int goldCost)
        {
            goldCost = 0;

            if (!TryResolveEquipment(
                    itemInstance,
                    out _))
            {
                return false;
            }

            if (itemInstance == null ||
                !itemInstance.HasValidTierUpSession())
            {
                return false;
            }

            if (!progressionConfig.TryGetTierConfiguration(
                    itemInstance.CurrentTier,
                    out EquipmentTierConfiguration configuration))
            {
                return false;
            }

            if (configuration.RerollCost <= 0)
                return false;

            goldCost = configuration.RerollCost;

            return true;
        }

        public bool TryGetTierUpCost(
            ItemInstance itemInstance,
            out ItemBaseData baseData,
            out IReadOnlyList<MaterialCost> costs)
        {
            baseData = null;
            costs = null;

            if (!TryResolveEquipment(
                    itemInstance,
                    out baseData))
            {
                return false;
            }

            if (!progressionRuntime.IsReadyForTierUp(
                    itemInstance))
            {
                return false;
            }

            if (!progressionConfig.TryGetTierUpCost(
                    baseData.ContentId,
                    baseData.EquipmentType,
                    itemInstance.CurrentTier,
                    out costs))
            {
                return false;
            }

            if (!TryValidateMaterialCosts(costs))
            {
                costs = null;
                return false;
            }

            return true;
        }

        public bool TryGetMaterialDisplayData(
            string contentId,
            out MaterialDefinition definition,
            out int ownedAmount)
        {
            definition = null;
            ownedAmount = 0;

            if (string.IsNullOrWhiteSpace(contentId))
                return false;

            if (!materialResolver.TryResolve(
                    contentId,
                    out definition))
            {
                return false;
            }

            if (definition == null)
                return false;

            ownedAmount = materialsState.GetAmount(contentId);

            return true;
        }

        //==========================================================
        // Normal Upgrade
        //==========================================================

        public bool TryUpgrade(
            ItemInstance itemInstance)
        {
            if (!TryGetUpgradeCost(
                    itemInstance,
                    out _,
                    out IReadOnlyList<MaterialCost> costs))
            {
                return false;
            }

            if (!materialsState.TrySpend(costs))
                return false;

            if (!progressionRuntime.TryUpgrade(itemInstance))
            {
                RefundMaterials(costs);
                return false;
            }
            
            persistentStateSaver.SavePersistentState();

            return true;
        }

        //==========================================================
        // Tier Up Options
        //==========================================================

        public bool TryGenerateTierOptions(
            ItemInstance itemInstance,
            out List<EquipmentStatOption> options)
        {
            options = new List<EquipmentStatOption>();

            if (!TryResolveEquipment(
                    itemInstance,
                    out ItemBaseData baseData))
            {
                return false;
            }

            return progressionRuntime.TryGenerateTierOptions(
                baseData,
                itemInstance,
                out options);
        }

        //==========================================================
        // Tier Up
        //==========================================================

        public bool TryApplyTierUp(
            EquipmentUpgradeSession session,
            ItemInstance itemInstance,
            EquipmentStatOption selectedOption)
        {
            if (session == null ||
                !session.Authorizes(
                    this,
                    itemInstance,
                    selectedOption))
            {
                return false;
            }

            if (!TryGetTierUpCost(
                    itemInstance,
                    out _,
                    out IReadOnlyList<MaterialCost> costs))
            {
                return false;
            }

            if (!materialsState.TrySpend(costs))
                return false;

            if (!progressionRuntime.TryApplyTierUp(
                    itemInstance,
                    selectedOption))
            {
                RefundMaterials(costs);
                return false;
            }

            persistentStateSaver.SavePersistentState();

            return true;
        }

        //==========================================================
        // Reroll
        //==========================================================

        public bool TryRerollTierOptions(
            ItemInstance itemInstance,
            out List<EquipmentStatOption> options)
        {
            options = new List<EquipmentStatOption>();

            if (itemInstance == null)
                return false;

            // Validate the current Tier Up session and cost.
            if (!TryGetRerollCost(
                    itemInstance,
                    out int goldCost))
            {
                return false;
            }

            if (currencyState == null ||
                !currencyState.Has(GoldContentId, goldCost))
            {
                return false;
            }

            // Generate the replacement options before spending Gold.
            if (!TryGenerateTierOptions(
                    itemInstance,
                    out List<EquipmentStatOption> newOptions))
            {
                return false;
            }

            if (newOptions == null || newOptions.Count != 3)
                return false;

            // Preserve the previous session state for compensation.
            List<EquipmentStatOption> previousOptions =
                new List<EquipmentStatOption>(
                    itemInstance.TierUpOptions);

            bool hadPreviousSelection =
                itemInstance.HasSelectedTierUpOption;

            EquipmentStatOption previousSelection =
                itemInstance.SelectedTierUpOption;

            if (previousOptions.Count != 3)
                return false;

            // Replace options before consuming Gold.
            if (!itemInstance.TryReplaceTierUpOptions(newOptions))
                return false;

            // Spend Gold only after successful replacement.
            if (!currencyState.TrySpend(
                    GoldContentId,
                    goldCost))
            {
                RestorePreviousTierUpOptions(
                    itemInstance,
                    previousOptions,
                    hadPreviousSelection,
                    previousSelection);

                return false;
            }

            // Persist both the new options and the currency balance.
            if (!persistentStateSaver.SavePersistentState())
            {
                RestorePreviousTierUpOptions(
                    itemInstance,
                    previousOptions,
                    hadPreviousSelection,
                    previousSelection);

                currencyState.Add(GoldContentId, goldCost);

                return false;
            }

            options = new List<EquipmentStatOption>(newOptions);

            return true;
        }

        private static void RestorePreviousTierUpOptions(
            ItemInstance itemInstance,
            IReadOnlyList<EquipmentStatOption> previousOptions,
            bool hadPreviousSelection,
            EquipmentStatOption previousSelection)
        {
            if (!itemInstance.TryReplaceTierUpOptions(previousOptions))
                return;

            if (hadPreviousSelection)
            {
                itemInstance.TrySelectTierUpOption(previousSelection);
            }
        }

        //==========================================================
        // Equipment Resolution
        //==========================================================

        private bool TryResolveEquipment(
            ItemInstance itemInstance,
            out ItemBaseData baseData)
        {
            baseData = null;

            if (itemInstance == null)
                return false;

            if (!itemContentResolver.TryResolve(
                    itemInstance.BaseDataId,
                    out baseData))
            {
                return false;
            }

            if (baseData == null ||
                baseData.Category != ItemCategory.Equipment)
            {
                baseData = null;
                return false;
            }

            return true;
        }

        //==========================================================
        // Material Validation
        //==========================================================

        private bool TryValidateMaterialCosts(
            IReadOnlyList<MaterialCost> costs)
        {
            if (costs == null || costs.Count == 0)
                return false;

            HashSet<string> registeredIds =
                new HashSet<string>(
                    StringComparer.Ordinal);

            for (int i = 0; i < costs.Count; i++)
            {
                MaterialCost cost = costs[i];

                if (!cost.IsValid)
                    return false;

                string contentId =
                    cost.ContentId.Trim();

                if (!registeredIds.Add(contentId))
                    return false;

                if (!materialResolver.TryResolve(
                        contentId,
                        out MaterialDefinition definition))
                {
                    return false;
                }

                if (definition == null)
                    return false;
            }

            return true;
        }

        //==========================================================
        // Compensation
        //==========================================================

        private void RefundMaterials(
            IReadOnlyList<MaterialCost> costs)
        {
            if (costs == null)
                return;

            for (int i = 0; i < costs.Count; i++)
            {
                MaterialCost cost = costs[i];

                materialsState.Add(
                    cost.ContentId,
                    cost.Amount);
            }
        }
    }
}