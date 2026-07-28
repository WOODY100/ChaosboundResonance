using Chaosbound.Shared.Identifiers;
using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents the set of candidates available for evaluation by the Enemy Solver.
    /// </summary>
    public sealed class CandidateSet
    {
        private readonly List<ContentReference> candidates =
            new List<ContentReference>();

        /// <summary>
        /// Gets the candidates contained in this set.
        /// </summary>
        public IReadOnlyList<ContentReference> Candidates
        {
            get
            {
                return candidates;
            }
        }

        /// <summary>
        /// Adds a candidate to the set.
        /// </summary>
        /// <param name="candidate">
        /// The candidate to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the candidate is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the candidate already exists in the set.
        /// </exception>
        public void Add(ContentReference candidate)
        {
            if (candidate == null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (Contains(candidate))
            {
                throw new InvalidOperationException(
                    "The candidate already exists in the candidate set.");
            }

            candidates.Add(candidate);
        }

        /// <summary>
        /// Removes a candidate from the set.
        /// </summary>
        /// <param name="candidate">
        /// The candidate to remove.
        /// </param>
        /// <returns>
        /// True if the candidate was removed; otherwise, false.
        /// </returns>
        public bool Remove(ContentReference candidate)
        {
            if (candidate == null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            return candidates.Remove(candidate);
        }

        /// <summary>
        /// Determines whether the candidate exists in the set.
        /// </summary>
        /// <param name="candidate">
        /// The candidate to search for.
        /// </param>
        /// <returns>
        /// True if the candidate exists; otherwise, false.
        /// </returns>
        public bool Contains(ContentReference candidate)
        {
            if (candidate == null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            return candidates.Contains(candidate);
        }

        /// <summary>
        /// Removes all candidates from the set.
        /// </summary>
        public void Clear()
        {
            candidates.Clear();
        }
    }
}