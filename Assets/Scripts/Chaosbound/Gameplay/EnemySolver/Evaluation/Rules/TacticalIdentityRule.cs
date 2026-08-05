using Chaosbound.Gameplay.EnemySolver.Enums;
using Chaosbound.Gameplay.EnemySolver.Models;
using System;

namespace Chaosbound.Gameplay.EnemySolver.Evaluation.Rules
{
    /// <summary>
    /// Awards score to candidates whose tactical capabilities
    /// match the tactical identity configured for the expedition.
    /// </summary>
    public sealed class TacticalIdentityRule :
        IEnemyEvaluationRule
    {
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
                score +=
                    context.TacticalIdentity
                        .GetBonusScore(capability);
            }

            return score;
        }
    }
}