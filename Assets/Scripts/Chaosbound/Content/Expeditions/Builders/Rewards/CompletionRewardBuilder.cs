using Chaosbound.Content.Expeditions.Authoring.Rewards;
using Chaosbound.Content.Expeditions.Definitions.Rewards;
using System;

namespace Chaosbound.Content.Expeditions.Builders.Rewards
{
    public static class CompletionRewardBuilder
    {
        public static CompletionRewardDefinition Build(
            CompletionRewardAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(
                    nameof(authoring));

            return new CompletionRewardDefinition(
                authoring.ItemContentId,
                authoring.MetaExperience);
        }
    }
}