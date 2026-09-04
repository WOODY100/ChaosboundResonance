using Chaosbound.Content.Expeditions.Authoring.SkillProgression;
using Chaosbound.Content.Expeditions.Definitions.SkillProgression;
using System;

namespace Chaosbound.Content.Expeditions.Builders.SkillProgression
{
    /// <summary>
    /// Builds the immutable skill progression definition
    /// from expedition authoring content.
    /// </summary>
    public static class SkillProgressionBuilder
    {
        public static SkillProgressionDefinition Build(
            SkillProgressionAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            return new SkillProgressionDefinition(
                authoring.MaxSkillLevel,
                authoring.EvolutionRequiredLevel);
        }
    }
}