using Chaosbound.Content.Expeditions.Authoring.MiniBosses;
using Chaosbound.Content.Expeditions.Definitions.MiniBosses;
using Chaosbound.Shared.Authoring;
using Chaosbound.Shared.Builders;
using Chaosbound.Shared.Identifiers;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Builders.MiniBosses
{
    public static class MiniBossesBuilder
    {
        public static MiniBossesDefinition Build(
            MiniBossesAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            var content = new List<ContentReference>();

            foreach (var entry in authoring.Content ?? Array.Empty<ContentReferenceAuthoring>())
            {
                content.Add(ContentReferenceBuilder.Build(entry));
            }

            return new MiniBossesDefinition(content);
        }
    }
}