using Chaosbound.Shared.Identifiers;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Population
{
    public sealed class RuntimePopulationConfig
    {
        public IReadOnlyList<ContentReference> Content { get; }

        public RuntimePopulationConfig(
            IReadOnlyList<ContentReference> content)
        {
            Content = content;
        }
    }
}