using Chaosbound.Content.Expeditions.Runtime.Enemy.TacticalIdentity;
using Chaosbound.Gameplay.EnemySolver.Analysis;
using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.EnemySolver.ValueObjects;
using Chaosbound.Gameplay.Threat.ValueObjects;
using System;

namespace Chaosbound.Gameplay.EnemySolver.Evaluation
{
    /// <summary>
    /// Represents the immutable state used by evaluation rules when
    /// scoring enemy candidates.
    /// </summary>
    public sealed class EvaluationContext
    {
        /// <summary>
        /// Gets the target composition produced by the
        /// previous EnemySolver resolution.
        /// </summary>
        public EnemyComposition PreviousComposition { get; }

        /// <summary>
        /// Gets the remaining threat available for investment.
        /// </summary>
        public ThreatCost RemainingThreat { get; }

        /// <summary>
        /// Gets the active solver constraints.
        /// </summary>
        public SolverConstraints SolverConstraints { get; }

        /// <summary>
        /// Gets the tactical identity configured for the current expedition.
        /// </summary>
        public RuntimeTacticalIdentity TacticalIdentity { get; }

        /// <summary>
        /// Gets the tactical analysis of the current runtime composition.
        /// </summary>
        public CompositionAnalysis CompositionAnalysis { get; }

        /// <summary>
        /// Creates a new evaluation context.
        /// </summary>
        public EvaluationContext(
            EnemyComposition previousComposition,
            ThreatCost remainingThreat,
            SolverConstraints solverConstraints,
            RuntimeTacticalIdentity tacticalIdentity,
            CompositionAnalysis analysis)
        {
            PreviousComposition =
                previousComposition
                ?? throw new ArgumentNullException(nameof(previousComposition));

            SolverConstraints =
                solverConstraints
                ?? throw new ArgumentNullException(nameof(solverConstraints));

            TacticalIdentity =
                tacticalIdentity
                ?? throw new ArgumentNullException(nameof(tacticalIdentity));

            CompositionAnalysis =
                analysis
                ?? throw new ArgumentNullException(nameof(analysis));

            RemainingThreat = remainingThreat;
        }
    }
}