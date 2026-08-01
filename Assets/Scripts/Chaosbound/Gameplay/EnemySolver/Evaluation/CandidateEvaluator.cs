using Chaosbound.Gameplay.EnemySolver.Models;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.EnemySolver.Evaluation
{
    /// <summary>
    /// Coordinates the execution of the tactical evaluation rules
    /// used by the EnemySolver.
    /// </summary>
    public sealed class CandidateEvaluator
    {
        private readonly IReadOnlyList<IEnemyEvaluationRule> rules;

        /// <summary>
        /// Creates a new candidate evaluator.
        /// </summary>
        public CandidateEvaluator(
            IReadOnlyList<IEnemyEvaluationRule> rules)
        {
            this.rules = rules
                ?? throw new ArgumentNullException(nameof(rules));
        }

        /// <summary>
        /// Evaluates a candidate by aggregating the contribution
        /// of every registered evaluation rule.
        /// </summary>
        public ScoredCandidate Evaluate(
            EnemyCandidate candidate,
            EvaluationContext context)
        {
            if (candidate == null)
                throw new ArgumentNullException(nameof(candidate));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            CandidateScore total = CandidateScore.Zero;

            foreach (IEnemyEvaluationRule rule in rules)
            {
                total += rule.Evaluate(candidate, context);
            }

            return new ScoredCandidate(
                candidate,
                total);
        }
    }
}