using System;

using Chaosbound.Content.Expeditions.Authoring.General;
using Chaosbound.Content.Expeditions.Definitions.General;

namespace Chaosbound.Content.Expeditions.Builders.General
{
    /// <summary>
    /// Converts authoring general settings into their domain representation.
    /// </summary>
    public static class GeneralBuilder
    {
        public static GeneralDefinition Build(
            GeneralAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            return new GeneralDefinition(
                authoring.CompletionCondition,
                authoring.BaseDifficulty);
        }
    }
}