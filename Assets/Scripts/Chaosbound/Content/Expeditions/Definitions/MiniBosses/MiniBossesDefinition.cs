using Chaosbound.Shared.Content.Entries;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Definitions.MiniBosses
{
    public sealed class MiniBossesDefinition
    {
        public MiniBossesDefinition(
            IReadOnlyList<ContentEntry> entries)
        {
            Entries =
                new List<ContentEntry>(entries);
        }

        public IReadOnlyList<ContentEntry> Entries { get; }
    }
}