using Chaosbound.Content.Expeditions.Authoring.Bosses;
using Chaosbound.Content.Expeditions.Definitions.Bosses;
using Chaosbound.Shared.Authoring;
using Chaosbound.Shared.Builders;
using Chaosbound.Shared.Identifiers;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Builders.Bosses
{
    public static class BossesBuilder
    {
        public static BossesDefinition Build(
            BossesAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            var content = new List<ContentReference>();

            foreach (var entry in authoring.Content ?? Array.Empty<ContentReferenceAuthoring>())
            {
                content.Add(ContentReferenceBuilder.Build(entry));
            }

            return new BossesDefinition(content);
        }
    }
}