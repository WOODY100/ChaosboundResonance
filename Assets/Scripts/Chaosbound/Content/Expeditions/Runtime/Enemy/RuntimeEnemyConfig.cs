using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Enemy
{
    /// <summary>
    /// Immutable runtime configuration containing every enemy
    /// available for the current expedition.
    /// </summary>
    public sealed class RuntimeEnemyConfig
    {
        /// <summary>
        /// Gets every enemy available for the expedition.
        /// </summary>
        public IReadOnlyList<EnemyVariantData> Enemies { get; }

        /// <summary>
        /// Gets the tactical identity configured for the expedition.
        /// </summary>

        public RuntimeEnemyConfig(
            IReadOnlyList<EnemyVariantData> enemies)
        {
            Enemies = new List<EnemyVariantData>(enemies);
        }
    }
}