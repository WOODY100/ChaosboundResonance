using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.EnemySolver.ValueObjects;
using System;

namespace Chaosbound.Gameplay.EnemySolver.Services
{
    /// <summary>
    /// Validates whether an enemy candidate satisfies the active
    /// solver constraints.
    /// </summary>
    public sealed class CandidateValidator
    {
        /// <summary>
        /// Determines whether the specified candidate is valid.
        /// </summary>
        public bool Validate(
            EnemyCandidate candidate,
            SolverConstraints constraints)
        {
            if (candidate == null)
                throw new ArgumentNullException(nameof(candidate));

            if (constraints == null)
                throw new ArgumentNullException(nameof(constraints));

            // V1:
            // No filtering yet.
            // Future versions will evaluate:
            //
            // - Expedition restrictions
            // - Threat limits
            // - Biome restrictions
            // - Difficulty restrictions
            // - Wave restrictions
            // - Special events
            //
            return true;
        }
    }
}