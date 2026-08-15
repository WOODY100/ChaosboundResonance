using Chaosbound.Content.Enemy.Bosses;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Bosses
{
    /// <summary>
    /// Immutable runtime configuration containing every boss
    /// available for the current expedition.
    /// </summary>
    public sealed class RuntimeBossesConfig
    {
        /// <summary>
        /// Gets every boss available for the expedition.
        /// </summary>
        public IReadOnlyList<BossData> Bosses { get; }

        public RuntimeBossesConfig(
            IReadOnlyList<BossData> bosses)
        {
            Bosses =
                new List<BossData>(bosses);
        }
    }
}