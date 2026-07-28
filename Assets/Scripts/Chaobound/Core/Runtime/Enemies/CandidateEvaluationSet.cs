using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents a collection of candidate evaluations.
    /// </summary>
    public sealed class CandidateEvaluationSet
    {
        private readonly List<CandidateEvaluation> evaluations =
            new List<CandidateEvaluation>();

        /// <summary>
        /// Gets the candidate evaluations.
        /// </summary>
        public IReadOnlyList<CandidateEvaluation> Evaluations
        {
            get
            {
                return evaluations;
            }
        }

        /// <summary>
        /// Adds a candidate evaluation.
        /// </summary>
        /// <param name="evaluation">
        /// The evaluation to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the evaluation is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the evaluation already exists.
        /// </exception>
        public void Add(
            CandidateEvaluation evaluation)
        {
            if (evaluation == null)
            {
                throw new ArgumentNullException(nameof(evaluation));
            }

            if (evaluations.Contains(evaluation))
            {
                throw new InvalidOperationException(
                    "The candidate evaluation already exists.");
            }

            evaluations.Add(evaluation);
        }

        /// <summary>
        /// Removes a candidate evaluation.
        /// </summary>
        /// <param name="evaluation">
        /// The evaluation to remove.
        /// </param>
        /// <returns>
        /// True if the evaluation was removed; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the evaluation is null.
        /// </exception>
        public bool Remove(
            CandidateEvaluation evaluation)
        {
            if (evaluation == null)
            {
                throw new ArgumentNullException(nameof(evaluation));
            }

            return evaluations.Remove(evaluation);
        }

        /// <summary>
        /// Determines whether the specified evaluation exists.
        /// </summary>
        /// <param name="evaluation">
        /// The evaluation to locate.
        /// </param>
        /// <returns>
        /// True if the evaluation exists; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the evaluation is null.
        /// </exception>
        public bool Contains(
            CandidateEvaluation evaluation)
        {
            if (evaluation == null)
            {
                throw new ArgumentNullException(nameof(evaluation));
            }

            return evaluations.Contains(evaluation);
        }

        /// <summary>
        /// Removes all candidate evaluations.
        /// </summary>
        public void Clear()
        {
            evaluations.Clear();
        }
    }
}