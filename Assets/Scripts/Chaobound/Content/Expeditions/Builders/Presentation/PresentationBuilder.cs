using System;

using Chaosbound.Content.Expeditions.Authoring.Presentation;
using Chaosbound.Content.Expeditions.Definitions.Presentation;

namespace Chaosbound.Content.Expeditions.Builders.Presentation
{
    /// <summary>
    /// Converts presentation authoring data into its domain representation.
    /// </summary>
    public static class PresentationBuilder
    {
        public static PresentationDefinition Build(
            PresentationAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            return new PresentationDefinition(
                authoring.DisplayName,
                authoring.Description,
                authoring.IconId);
        }
    }
}