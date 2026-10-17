
using Chaosbound.Gameplay.Items.Runtime;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Settlement
{
    public sealed class ExpeditionSettlementResult
    {
        private readonly IReadOnlyDictionary<string, int>
            settledMaterialAmounts;

        private readonly IReadOnlyList<ItemInstance>
            completionRewardItems;

        public ExpeditionSettlementResult(
            bool success,
            int settledItemCount,
            IReadOnlyDictionary<string, int> settledMaterialAmounts,
            IReadOnlyList<ItemInstance> completionRewardItems,
            int metaExperienceGranted)
        {
            if (settledItemCount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(settledItemCount));

            if (metaExperienceGranted < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(metaExperienceGranted));

            Success = success;
            SettledItemCount = settledItemCount;

            this.settledMaterialAmounts =
                settledMaterialAmounts ??
                new Dictionary<string, int>();

            List<ItemInstance> validItems =
                new List<ItemInstance>();

            if (completionRewardItems != null)
            {
                for (int i = 0; i < completionRewardItems.Count; i++)
                {
                    ItemInstance item = completionRewardItems[i];

                    if (item != null)
                        validItems.Add(item);
                }
            }

            this.completionRewardItems =
                validItems.AsReadOnly();

            MetaExperienceGranted = metaExperienceGranted;
        }

        public bool Success { get; }

        public int SettledItemCount { get; }

        public IReadOnlyDictionary<string, int>
            SettledMaterialAmounts =>
                settledMaterialAmounts;

        public IReadOnlyList<ItemInstance>
            CompletionRewardItems =>
                completionRewardItems;

        public bool HasCompletionRewardItems =>
            completionRewardItems.Count > 0;

        public int MetaExperienceGranted { get; }

        public bool HasMetaExperience =>
            MetaExperienceGranted > 0;
    }
}
