using Chaosbound.Content.Expeditions.Enums;

namespace Chaosbound.Content.Expeditions.Definitions.General
{
    /// <summary>
    /// Describes the global rules and characteristics of an expedition.
    /// This definition contains only declarative data and no gameplay logic.
    /// </summary>
    public sealed class GeneralDefinition
    {
        public CompletionCondition CompletionCondition { get; }

        public DifficultyTier BaseDifficulty { get; }

        public GeneralDefinition(
            CompletionCondition completionCondition,
            DifficultyTier baseDifficulty)
        {
            CompletionCondition = completionCondition;
            BaseDifficulty = baseDifficulty;
        }
    }
}