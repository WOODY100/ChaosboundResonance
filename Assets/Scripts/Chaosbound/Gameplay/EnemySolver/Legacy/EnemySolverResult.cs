using System;

namespace Chaosbound.Gameplay.EnemySolver.Models
{
    /// <summary>
    /// Represents the result produced by the EnemySolver.
    /// </summary>
    public sealed class EnemySolverResult
    {
        /// <summary>
        /// Gets the target enemy composition.
        /// </summary>
        public EnemyComposition Composition { get; }

        /// <summary>
        /// Gets the declarative spawn plan produced by the EnemySolver.
        /// This plan will later be translated by the Spawn Runtime.
        /// </summary>
        public SpawnPlan SpawnPlan { get; }

        /// <summary>
        /// Creates a new solver result.
        /// </summary>
        public EnemySolverResult(
            EnemyComposition composition,
            SpawnPlan spawnPlan)
        {
            Composition =
                composition
                ?? throw new ArgumentNullException(nameof(composition));

            SpawnPlan =
                spawnPlan
                ?? throw new ArgumentNullException(nameof(spawnPlan));
        }
    }
}