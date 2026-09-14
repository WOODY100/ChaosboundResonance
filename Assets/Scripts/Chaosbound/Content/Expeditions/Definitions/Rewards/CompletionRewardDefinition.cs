using System;

namespace Chaosbound.Content.Expeditions.Definitions.Rewards
{
    public sealed class CompletionRewardDefinition
    {
        public CompletionRewardDefinition(
            string itemContentId,
            int metaExperience)
        {
            if (metaExperience < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(metaExperience),
                    metaExperience,
                    "Meta experience cannot be negative.");

            ItemContentId =
                itemContentId ?? string.Empty;

            MetaExperience =
                metaExperience;
        }

        public string ItemContentId { get; }

        public int MetaExperience { get; }

        public bool HasItemReward =>
            !string.IsNullOrEmpty(ItemContentId);

        public bool HasMetaExperience =>
            MetaExperience > 0;

        public bool HasAnyReward =>
            HasItemReward ||
            HasMetaExperience;
    }
}