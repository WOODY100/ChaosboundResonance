using Chaosbound.Shared.Identifiers;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions.Population
{
    /// <summary>
    /// Defines the content catalog available for an expedition.
    /// </summary>
    public sealed class PopulationDefinition
    {
        /// <summary>
        /// Gets the content available for this expedition.
        /// </summary>
        public IReadOnlyList<ContentReference> Content { get; }

        public PopulationDefinition(
            IReadOnlyList<ContentReference> content)
        {
            Content = content ??
                throw new ArgumentNullException(nameof(content));
        }
    }
}