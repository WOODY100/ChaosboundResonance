using Chaosbound.Gameplay.EnemySolver.Enums;
using Chaosbound.Gameplay.EnemySolver.Models;
using System;

namespace Chaosbound.Gameplay.EnemySolver.Evaluation.Rules
{
    /// <summary>
    /// Awards score to candidates that provide tactical capabilities
    /// currently missing from the runtime composition.
    /// </summary>
    public sealed class NeedCoverageRule :
        IEnemyEvaluationRule
    {
        /// <summary>
        /// Bonus awarded for each missing tactical capability
        /// provided by the candidate.
        /// </summary>
        private const float MissingCapabilityBonus = 100f;

        /// <inheritdoc/>
        public CandidateScore Evaluate(
            EnemyCandidate candidate,
            EvaluationContext context)
        {
            if (candidate == null)
                throw new ArgumentNullException(nameof(candidate));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return new CandidateScore(
                CalculateScore(
                    candidate,
                    context));
        }

        private static float CalculateScore(
            EnemyCandidate candidate,
            EvaluationContext context)
        {
            float score = 0f;

            foreach (TacticalCapability capability
                in candidate.TacticalCapabilities)
            {
                if (context.CompositionAnalysis.NeedsCapability(capability))
                {
                    score += MissingCapabilityBonus;
                }
            }

            return score;
        }
    }
}