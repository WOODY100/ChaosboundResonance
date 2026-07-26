using Chaosbound.Content.Expeditions.Enums;

namespace Chaosbound.Content.Expeditions.Definitions.General
{
    /// <summary>
    /// Describes the general gameplay characteristics of an expedition.
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