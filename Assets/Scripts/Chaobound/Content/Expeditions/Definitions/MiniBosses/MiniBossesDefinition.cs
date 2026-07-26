using Chaosbound.Shared.Identifiers;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions.MiniBosses
{
    public sealed class MiniBossesDefinition
    {
        public MiniBossesDefinition(IReadOnlyList<ContentReference> content)
        {
            Content = content;
        }

        public IReadOnlyList<ContentReference> Content { get; }
    }
}