using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents the tactical evaluation result of a single enemy candidate.
    /// </summary>
    public sealed class CandidateEvaluation
    {
        /// <summary>
        /// Gets the evaluated enemy candidate.
        /// </summary>
        public EnemyVariantData Candidate { get; }

        /// <summary>
        /// Gets the tactical score assigned to the candidate.
        /// Higher values indicate a better match for the current objective.
        /// </summary>
        public int Score { get; }

        /// <summary>
        /// Initializes a new candidate evaluation.
        /// </summary>
        /// <param name="candidate">
        /// The evaluated enemy candidate.
        /// </param>
        /// <param name="score">
        /// The tactical score assigned to the candidate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the candidate is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the score is negative.
        /// </exception>
        public CandidateEvaluation(
    EnemyVariantData candidate,
    int score)
        {
            Candidate = candidate
                ?? throw new ArgumentNullException(nameof(candidate));

            if (score < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(score),
                    score,
                    "Candidate score cannot be negative.");
            }

            Score = score;
        }
    }
}