using Chaosbound.Content.Expeditions.Authoring.Completion;
using Chaosbound.Content.Expeditions.Definitions.Completion;
using System;

namespace Chaosbound.Content.Expeditions.Builders.Completion
{
    /// <summary>
    /// Builds declarative Completion content
    /// from Unity authoring data.
    /// </summary>
    public static class CompletionBuilder
    {
        public static CompletionDefinition Build(
            CompletionAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(
                    nameof(authoring));

            return new CompletionDefinition(
                authoring.DomainId,
                authoring.EventId);
        }
    }
}