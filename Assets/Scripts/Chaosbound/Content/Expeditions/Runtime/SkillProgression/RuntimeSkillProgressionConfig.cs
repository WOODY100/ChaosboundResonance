using Chaosbound.Content.Expeditions.Definitions.SkillProgression;

namespace Chaosbound.Content.Expeditions.Runtime.Configs
{
    /// <summary>
    /// Runtime configuration for skill progression during an expedition.
    /// </summary>
    public sealed class RuntimeSkillProgressionConfig
    {
        /// <summary>
        /// Gets the maximum level a skill can reach during the expedition.
        /// </summary>
        public int MaxSkillLevel { get; }

        /// <summary>
        /// Gets the skill level required to unlock evolution availability.
        /// </summary>
        public int EvolutionRequiredLevel { get; }

        public RuntimeSkillProgressionConfig(
            SkillProgressionDefinition definition)
        {
            if (definition == null)
                throw new System.ArgumentNullException(
                    nameof(definition));

            MaxSkillLevel = definition.MaxSkillLevel;
            EvolutionRequiredLevel = definition.EvolutionRequiredLevel;
        }
    }
}