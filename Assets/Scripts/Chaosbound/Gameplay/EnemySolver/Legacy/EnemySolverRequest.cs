using Chaosbound.Content.Expeditions.Runtime.Enemy.TacticalIdentity;
using Chaosbound.Gameplay.EnemySolver.Runtime.Composition;
using Chaosbound.Gameplay.EnemySolver.ValueObjects;
using Chaosbound.Gameplay.Threat.ValueObjects;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.EnemySolver.Models
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
        /// Gets the target composition produced by the
        /// previous EnemySolver resolution.
        /// </summary>
        public EnemyComposition PreviousComposition { get; }
        
        /// <summary>
        /// Gets the current materialized runtime composition.
        /// </summary>
        public RuntimeCompositionState RuntimeComposition { get; }

        /// <summary>
        /// Gets the available threat capacity.
        /// </summary>
        public ThreatCapacity AvailableThreat { get; }

        /// <summary>
        /// Gets the active solver constraints.
        /// </summary>
        public SolverConstraints Constraints { get; }

        /// <summary>
        /// Gets the tactical identity configured for the current expedition.
        /// </summary>
        public RuntimeTacticalIdentity TacticalIdentity { get; }

        /// <summary>
        /// Creates a new solver request.
        /// </summary>
        public EnemySolverRequest(
                IReadOnlyList<EnemyVariantData> availableEnemies,
                EnemyComposition previousComposition,
                RuntimeCompositionState runtimeComposition,
                ThreatCapacity availableThreat,
                SolverConstraints constraints,
                RuntimeTacticalIdentity tacticalIdentity)
        {
            AvailableEnemies =
                availableEnemies
                ?? throw new ArgumentNullException(nameof(availableEnemies));

            PreviousComposition =
                previousComposition
                ?? throw new ArgumentNullException(nameof(previousComposition));

            RuntimeComposition =
                runtimeComposition
                ?? throw new ArgumentNullException(nameof(runtimeComposition));

            Constraints =
                constraints
                ?? throw new ArgumentNullException(nameof(constraints));

            TacticalIdentity =
                tacticalIdentity
                ?? throw new ArgumentNullException(nameof(tacticalIdentity));

            AvailableThreat = availableThreat;
        }
    }
}