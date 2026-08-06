using System.Collections.Generic;
using System;

namespace Chaosbound.Gameplay.Diagnostics.Threat
{
    /// <summary>
    /// Immutable snapshot describing the current runtime
    /// state of the Threat Budget system.
    /// </summary>
    public sealed class ThreatBudgetDebugSnapshot
    {
        /// <summary>
        /// Current expedition time.
        /// </summary>
        public float Time { get; }

        /// <summary>
        /// Current pressure value.
        /// </summary>
        public float Pressure { get; }

        /// <summary>
        /// Maximum threat capacity.
        /// </summary>
        public float Capacity { get; }

        /// <summary>
        /// Threat currently invested in alive enemies.
        /// </summary>
        public float InvestedThreat { get; }

        /// <summary>
        /// Remaining available threat.
        /// </summary>
        public float AvailableThreat { get; }

        /// <summary>
        /// Number of alive enemies.
        /// </summary>
        public int AliveEnemies { get; }

        /// <summary>
        /// Gets the current runtime composition.
        /// </summary>
        public IReadOnlyList<RuntimeCompositionDebugEntry> Composition { get; }

        public ThreatBudgetDebugSnapshot(
            float time,
            float pressure,
            float capacity,
            float investedThreat,
            float availableThreat,
            int aliveEnemies,
            IReadOnlyList<RuntimeCompositionDebugEntry> composition)
        {
            Time = time;
            Pressure = pressure;
            Capacity = capacity;
            InvestedThreat = investedThreat;
            AvailableThreat = availableThreat;
            AliveEnemies = aliveEnemies;
            Composition = composition
                ?? throw new ArgumentNullException(nameof(composition));
        }
    }
}