using Chaosbound.Content.Expeditions.Definitions.Completion;
using Chaosbound.Content.Expeditions.Runtime.Completion;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using System;

namespace Chaosbound.Content.Expeditions.Runtime.Builders
{
    /// <summary>
    /// Builds the runtime completion configuration
    /// from declarative expedition content.
    /// </summary>
    public sealed class RuntimeCompletionBuilder
    {
        /// <summary>
        /// Builds the runtime completion configuration.
        /// </summary>
        public RuntimeCompletionConfig BuildCompletion(
            CompletionDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(
                    nameof(definition));

            CompletionRequirement requirement =
                new CompletionRequirement(
                    definition.DomainId,
                    definition.EventId);

            return new RuntimeCompletionConfig(
                requirement,
                definition.ExitPortal);
        }
    }
}