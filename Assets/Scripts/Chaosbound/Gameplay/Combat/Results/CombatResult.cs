using Chaosbound.Gameplay.Combat.Models;
using Chaosbound.Content.Expeditions.Runtime.Combat.SpawnPattern;
using System;

namespace Chaosbound.Gameplay.Combat.Results
{
    /// <summary>
    /// Represents the strategic result produced by the Combat Solver.
    /// </summary>
    public sealed class CombatResult
    {
        /// <summary>
        /// Gets the desired enemy composition produced by the solver.
        /// </summary>
        public CombatComposition Composition { get; }

        /// <summary>
        /// Gets the spawn pattern intent associated with
        /// the selected combat tactic.
        /// </summary>
        public RuntimeSpawnPatternProfile SpawnPattern { get; }

        /// <summary>
        /// Creates a new combat result.
        /// </summary>
        public CombatResult(
            CombatComposition composition,
            RuntimeSpawnPatternProfile spawnPattern)
        {
            Composition =
                composition
                ?? throw new ArgumentNullException(
                    nameof(composition));

            SpawnPattern =
                spawnPattern
                ?? throw new ArgumentNullException(
                    nameof(spawnPattern));
        }
    }
}