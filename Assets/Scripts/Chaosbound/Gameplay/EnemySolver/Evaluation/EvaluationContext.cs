using System;
using Chaosbound.Gameplay.Threat.ValueObjects;

namespace Chaosbound.Core.Runtime.Enemies.Evaluation
{
    /// <summary>
    /// Represents the immutable state used by evaluation rules when
    /// scoring enemy candidates.
    /// </summary>
    public sealed class EvaluationContext
    {
        /// <summary>
        /// Gets the composition currently being built.
        /// </summary>
        public EnemyComposition CurrentComposition { get; }

        /// <summary>
        /// Gets the remaining threat available for investment.
        /// </summary>
        public ThreatCost RemainingThreat { get; }

        /// <summary>
        /// Gets the active solver constraints.
        /// </summary>
        public SolverConstraints SolverConstraints { get; }

        /// <summary>
        /// Creates a new evaluation context.
        /// </summary>
        public EvaluationContext(
            EnemyComposition currentComposition,
            ThreatCost remainingThreat,
            SolverConstraints solverConstraints)
        {
            CurrentComposition = currentComposition
                ?? throw new ArgumentNullException(nameof(currentComposition));

            SolverConstraints = solverConstraints
                ?? throw new ArgumentNullException(nameof(solverConstraints));

            RemainingThreat = remainingThreat;
        }
    }
}