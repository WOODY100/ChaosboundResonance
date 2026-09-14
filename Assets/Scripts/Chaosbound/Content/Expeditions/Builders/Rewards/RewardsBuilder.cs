using Chaosbound.Content.Expeditions.Authoring.Rewards;
using Chaosbound.Content.Expeditions.Definitions.Rewards;
using System;

namespace Chaosbound.Content.Expeditions.Builders.Rewards
{
    public static class RewardsBuilder
    {
        public static RewardsDefinition Build(
            RewardsAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(
                    nameof(authoring));

            CompletionRewardDefinition completion =
                CompletionRewardBuilder.Build(
                    authoring.Completion);

            return new RewardsDefinition(
                completion);
        }
    }
}