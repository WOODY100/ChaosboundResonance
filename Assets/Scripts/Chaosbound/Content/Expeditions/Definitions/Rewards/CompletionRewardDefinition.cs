using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions.Rewards
{
    public sealed class CompletionRewardDefinition
    {
        public CompletionRewardDefinition(
            IReadOnlyList<string> itemContentIds,
            int metaExperience)
        {
            if (metaExperience < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(metaExperience),
                    metaExperience,
                    "Meta experience cannot be negative.");
            }

            List<string> validIds = new List<string>();

            if (itemContentIds != null)
            {
                for (int i = 0; i < itemContentIds.Count; i++)
                {
                    string contentId = itemContentIds[i];

                    if (string.IsNullOrWhiteSpace(contentId))
                        continue;

                    validIds.Add(contentId.Trim());
                }
            }

            ItemContentIds = validIds.AsReadOnly();
            MetaExperience = metaExperience;
        }

        public IReadOnlyList<string> ItemContentIds { get; }

        public int MetaExperience { get; }

        public bool HasItemReward =>
            ItemContentIds.Count > 0;

        public bool HasMetaExperience =>
            MetaExperience > 0;

        public bool HasAnyReward =>
            HasItemReward || HasMetaExperience;
    }
}