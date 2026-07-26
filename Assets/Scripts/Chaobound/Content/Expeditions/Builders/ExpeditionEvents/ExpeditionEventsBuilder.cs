using Chaosbound.Shared.Builders;
using Chaosbound.Content.Expeditions.Authoring.ExpeditionEvents;
using Chaosbound.Content.Expeditions.Definitions.ExpeditionEvents;
using Chaosbound.Shared.Identifiers;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Builders.ExpeditionEvents
{
    public static class ExpeditionEventsBuilder
    {
        public static ExpeditionEventsDefinition Build(
            ExpeditionEventsAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            var content = new List<ContentReference>();

            foreach (var entry in authoring.Content)
            {
                content.Add(ContentReferenceBuilder.Build(entry));
            }

            return new ExpeditionEventsDefinition(content);
        }
    }
}