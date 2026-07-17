using System;

namespace Chaosbound.Content.Expeditions
{
    /// <summary>
    /// Represents a player's request to start an expedition.
    /// </summary>
    public sealed class ExpeditionRequest
    {
        public ExpeditionDefinition Definition { get; }

        public DifficultyTier SelectedDifficulty { get; }

        public int Seed { get; }

        public ExpeditionRequest(
            ExpeditionDefinition Definition,
            DifficultyTier selectedDifficulty,
            int seed)
        {
            this.Definition = Definition
                ?? throw new ArgumentNullException(nameof(Definition));

            SelectedDifficulty = selectedDifficulty;

            Seed = seed;
        }
    }
}