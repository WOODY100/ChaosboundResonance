using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Evaluates enemy candidates for a tactical objective.
    /// </summary>
    public sealed class CandidateEvaluator
    {
        /// <summary>
        /// Evaluates every candidate against the specified tactical objective.
        /// </summary>
        /// <param name="candidateSet">
        /// The candidate set to evaluate.
        /// </param>
        /// <param name="objective">
        /// The tactical objective.
        /// </param>
        /// <returns>
        /// A set containing the evaluation of every candidate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any argument is null.
        /// </exception>
        public CandidateEvaluationSet Evaluate(
            CandidateSet candidateSet,
            TacticalObjective objective)
        {
            if (candidateSet == null)
            {
                throw new ArgumentNullException(nameof(candidateSet));
            }

            if (objective == null)
            {
                throw new ArgumentNullException(nameof(objective));
            }

            List<CandidateEvaluation> evaluations =
                new List<CandidateEvaluation>();

            foreach (EnemyVariantData candidate in candidateSet.Candidates)
            {
                int score =
                    EvaluateCandidate(
                        candidate,
                        objective);

                evaluations.Add(
                    new CandidateEvaluation(
                        candidate,
                        score));
            }

            return new CandidateEvaluationSet(
                evaluations);
        }

        /// <summary>
        /// Evaluates a single enemy candidate.
        /// </summary>
        /// <param name="candidate">
        /// The candidate to evaluate.
        /// </param>
        /// <param name="objective">
        /// The tactical objective.
        /// </param>
        /// <returns>
        /// The tactical score assigned to the candidate.
        /// </returns>
        private static int EvaluateCandidate(
            EnemyVariantData candidate,
            TacticalObjective objective)
        {
            if (candidate == null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (objective == null)
            {
                throw new ArgumentNullException(nameof(objective));
            }

            if (candidate.TacticalCapabilities == null)
            {
                return 0;
            }

            int score = 0;

            foreach (TacticalCapability capability in candidate.TacticalCapabilities)
            {
                if (capability == objective.Capability)
                {
                    score++;
                }
            }

            return score;
        }
    }
}