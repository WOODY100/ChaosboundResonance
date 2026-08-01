using System.Collections.Generic;
using Chaosbound.Shared.Identifiers;

namespace Chaosbound.Content.Expeditions.Runtime.ExpeditionEvents
{
    public sealed class RuntimeExpeditionEventsConfig
    {
        public RuntimeExpeditionEventsConfig(IReadOnlyList<ContentReference> content)
        {
            Content = content;
        }

        public IReadOnlyList<ContentReference> Content { get; }
    }
}