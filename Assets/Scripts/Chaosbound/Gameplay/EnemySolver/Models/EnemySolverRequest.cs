using System;
using System.Collections.Generic;
using Chaosbound.Gameplay.Threat.ValueObjects;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents all information required to execute a complete
    /// EnemySolver pipeline.
    ///
    /// This object acts as the boundary between the Expedition Director
    /// and the EnemySolver.
    /// </summary>
    public sealed class EnemySolverRequest
    {
        /// <summary>
        /// Gets the enemy variants available for the current expedition.
        /// </summary>
        public IReadOnlyList<EnemyVariantData> AvailableEnemies { get; }

        /// <summary>
        /// Gets the current enemy composition.
        /// </summary>
        public EnemyComposition CurrentComposition { get; }

        /// <summary>
        /// Gets the available threat capacity.
        /// </summary>
        public ThreatCapacity AvailableThreat { get; }

        /// <summary>
        /// Gets the active solver constraints.
        /// </summary>
        public SolverConstraints Constraints { get; }

        /// <summary>
        /// Creates a new solver request.
        /// </summary>
        public EnemySolverRequest(
            IReadOnlyList<EnemyVariantData> availableEnemies,
            EnemyComposition currentComposition,
            ThreatCapacity availableThreat,
            SolverConstraints constraints)
        {
            AvailableEnemies =
                availableEnemies
                ?? throw new ArgumentNullException(nameof(availableEnemies));

            CurrentComposition =
                currentComposition
                ?? throw new ArgumentNullException(nameof(currentComposition));

            Constraints =
                constraints
                ?? throw new ArgumentNullException(nameof(constraints));

            AvailableThreat = availableThreat;
        }
    }
}