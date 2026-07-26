using Chaosbound.Content.Expeditions.Authoring.Population;
using Chaosbound.Content.Expeditions.Definitions.Population;
using Chaosbound.Shared.Identifiers;
using Chaosbound.Shared.Builders;
using Chaosbound.Shared.Authoring;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Builders.Population
{
    public static class PopulationBuilder
    {
        public static PopulationDefinition Build(
            PopulationAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            List<ContentReference> content =
                BuildContent(authoring.Content);

            return new PopulationDefinition(content);
        }

        private static List<ContentReference> BuildContent(
            IReadOnlyList<ContentReferenceAuthoring> authoring)
        {
            List<ContentReference> result =
                new(authoring.Count);

            foreach (var entry in authoring)
            {
                result.Add(
                    ContentReferenceBuilder.Build(entry));
            }

            return result;
        }
    }
}