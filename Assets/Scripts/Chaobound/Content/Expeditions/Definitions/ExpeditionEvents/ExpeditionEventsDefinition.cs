using Chaosbound.Shared.Identifiers;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions.ExpeditionEvents
{
    public sealed class ExpeditionEventsDefinition
    {
        public ExpeditionEventsDefinition(IReadOnlyList<ContentReference> content)
        {
            Content = content;
        }

        public IReadOnlyList<ContentReference> Content { get; }
    }
}