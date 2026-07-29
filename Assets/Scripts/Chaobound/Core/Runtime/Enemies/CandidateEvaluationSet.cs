using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents the complete evaluation of all enemy candidates.
    /// </summary>
    public sealed class CandidateEvaluationSet
    {
        /// <summary>
        /// Gets the candidate evaluations.
        /// </summary>
        public IReadOnlyList<CandidateEvaluation> Evaluations { get; }

        /// <summary>
        /// Initializes a new candidate evaluation set.
        /// </summary>
        /// <param name="evaluations">
        /// The candidate evaluations.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when evaluations is null.
        /// </exception>
        public CandidateEvaluationSet(
            IReadOnlyList<CandidateEvaluation> evaluations)
        {
            Evaluations = evaluations
                ?? throw new ArgumentNullException(nameof(evaluations));
        }
    }
}