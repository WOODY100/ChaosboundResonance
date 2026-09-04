using System;

namespace Chaosbound.Content.Expeditions.Definitions.SkillProgression
{
    /// <summary>
    /// Defines the skill progression rules for an expedition.
    /// </summary>
    public sealed class SkillProgressionDefinition
    {
        /// <summary>
        /// Gets the maximum level a skill can reach during the expedition.
        /// </summary>
        public int MaxSkillLevel { get; }

        /// <summary>
        /// Gets the skill level required to unlock evolution availability.
        /// </summary>
        public int EvolutionRequiredLevel { get; }

        public SkillProgressionDefinition(
            int maxSkillLevel,
            int evolutionRequiredLevel)
        {
            if (maxSkillLevel <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(maxSkillLevel),
                    "Max skill level must be greater than zero.");

            if (evolutionRequiredLevel <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(evolutionRequiredLevel),
                    "Evolution required level must be greater than zero.");

            if (evolutionRequiredLevel > maxSkillLevel)
                throw new ArgumentException(
                    "Evolution required level cannot exceed max skill level.");

            MaxSkillLevel = maxSkillLevel;
            EvolutionRequiredLevel = evolutionRequiredLevel;
        }
    }
}