namespace Chaosbound.Content.Expeditions.Runtime.Rewards
{
    public sealed class RuntimeRewardsConfig
    {
        public RuntimeRewardsConfig(
            string itemContentId,
            int metaExperience)
        {
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