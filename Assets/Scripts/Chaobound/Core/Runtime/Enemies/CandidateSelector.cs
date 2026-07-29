using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Selects the best evaluated enemy candidate.
    /// </summary>
    public sealed class CandidateSelector
    {
        /// <summary>
        /// Selects the highest-scoring candidate evaluation.
        /// </summary>
        /// <param name="evaluationSet">
        /// The evaluation set.
        /// </param>
        /// <returns>
        /// The best candidate evaluation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the evaluation set is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the evaluation set is empty.
        /// </exception>
        public CandidateEvaluation Select(
            CandidateEvaluationSet evaluationSet)
        {
            if (evaluationSet == null)
            {
                throw new ArgumentNullException(nameof(evaluationSet));
            }

            if (evaluationSet.Evaluations.Count == 0)
            {
                throw new InvalidOperationException(
                    "The evaluation set is empty.");
            }

            CandidateEvaluation best =
                evaluationSet.Evaluations[0];

            for (int i = 1; i < evaluationSet.Evaluations.Count; i++)
            {
                CandidateEvaluation current =
                    evaluationSet.Evaluations[i];

                if (current.Score > best.Score)
                {
                    best = current;
                }
            }

            return best;
        }
    }
}