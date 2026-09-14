using Chaosbound.Gameplay.Items.Runtime;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Settlement
{
    public sealed class ExpeditionSettlementResult
    {
        private readonly IReadOnlyDictionary<string, int>
            settledMaterialAmounts;

        public ExpeditionSettlementResult(
            bool success,
            int settledItemCount,
            IReadOnlyDictionary<string, int> settledMaterialAmounts,
            ItemInstance completionRewardItem,
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

            CompletionRewardItem =
                completionRewardItem;

            MetaExperienceGranted =
                metaExperienceGranted;
        }

        public bool Success { get; }

        public int SettledItemCount { get; }

        public IReadOnlyDictionary<string, int>
            SettledMaterialAmounts =>
                settledMaterialAmounts;

        public ItemInstance CompletionRewardItem { get; }

        public bool HasCompletionRewardItem =>
            CompletionRewardItem != null;

        public int MetaExperienceGranted { get; }

        public bool HasMetaExperience =>
            MetaExperienceGranted > 0;
    }
}