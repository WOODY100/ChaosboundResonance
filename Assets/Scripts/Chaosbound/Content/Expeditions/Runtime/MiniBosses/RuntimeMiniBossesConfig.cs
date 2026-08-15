using Chaosbound.Content.Enemy.MiniBosses;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.MiniBosses
{
    /// <summary>
    /// Immutable runtime configuration containing every mini boss
    /// available for the current expedition.
    /// </summary>
    public sealed class RuntimeMiniBossesConfig
    {
        /// <summary>
        /// Gets every mini boss available for the expedition.
        /// </summary>
        public IReadOnlyList<MiniBossData> MiniBosses { get; }

        public RuntimeMiniBossesConfig(
            IReadOnlyList<MiniBossData> miniBosses)
        {
            MiniBosses =
                new List<MiniBossData>(miniBosses);
        }
    }
}