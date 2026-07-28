using Chaosbound.Shared.Identifiers;
using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents the evaluation of a candidate for a tactical objective.
    /// </summary>
    public sealed class CandidateEvaluation
    {
        /// <summary>
        /// Gets the evaluated candidate.
        /// </summary>
        public ContentReference Candidate
        {
            get;
        }

        /// <summary>
        /// Gets the tactical objective used during evaluation.
        /// </summary>
        public TacticalObjective Objective
        {
            get;
        }

        /// <summary>
        /// Gets the evaluation score.
        /// </summary>
        public int Score
        {
            get;
        }

        /// <summary>
        /// Initializes a new candidate evaluation.
        /// </summary>
        /// <param name="candidate">
        /// The evaluated candidate.
        /// </param>
        /// <param name="objective">
        /// The tactical objective.
        /// </param>
        /// <param name="score">
        /// The evaluation score.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when candidate or objective is null.
        /// </exception>
        public CandidateEvaluation(
            ContentReference candidate,
            TacticalObjective objective,
            int score)
        {
            if (candidate == null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (objective == null)
            {
                throw new ArgumentNullException(nameof(objective));
            }

            Candidate = candidate;
            Objective = objective;
            Score = score;
        }
    }
}