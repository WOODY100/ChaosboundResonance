
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Rewards
{
    public sealed class RuntimeRewardsConfig
    {
        public RuntimeRewardsConfig(
            IReadOnlyList<string> itemContentIds,
            int metaExperience)
        {
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
            MetaExperience = metaExperience < 0 ? 0 : metaExperience;
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
