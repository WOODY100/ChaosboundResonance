namespace Chaosbound.Content.Expeditions.Definitions.Rewards
{
    public sealed class RewardsDefinition
    {
        public RewardsDefinition(
            CompletionRewardDefinition completion)
        {
            Completion =
                completion;
        }

        public CompletionRewardDefinition Completion { get; }
    }
}