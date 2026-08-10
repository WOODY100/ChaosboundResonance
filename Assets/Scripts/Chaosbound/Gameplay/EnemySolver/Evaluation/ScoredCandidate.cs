using Chaosbound.Shared.Enums;
using Chaosbound.Gameplay.EnemySolver.Enums;
using Chaosbound.Gameplay.EnemySolver.Models;
using Chaosbound.Gameplay.Threat.ValueObjects;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.EnemySolver.Evaluation
{
    /// <summary>
    /// Represents the result of evaluating an enemy candidate.
    /// </summary>
    public sealed class ScoredCandidate :
        IComparable<ScoredCandidate>
    {
        /// <summary>
        /// Gets the evaluated candidate.
        /// </summary>
        public EnemyCandidate Candidate { get; }

        /// <summary>
        /// Gets the evaluation score.
        /// </summary>
        public CandidateScore Score { get; }

        /// <summary>
        /// Gets the underlying enemy variant.
        /// </summary>
        public EnemyVariantData Variant => Candidate.Variant;

        /// <summary>
        /// Gets the threat cost of the candidate.
        /// </summary>
        public ThreatCost ThreatCost => Candidate.ThreatCost;

        /// <summary>
        /// Gets the category of the candidate.
        /// </summary>
        public EnemyCategory Category => Candidate.Category;

        /// <summary>
        /// Gets the tactical roles of the candidate.
        /// </summary>
        public IReadOnlyList<EnemyRole> Roles => Candidate.Roles;

        /// <summary>
        /// Gets the tactical capabilities of the candidate.
        /// </summary>
        public IReadOnlyList<TacticalCapability> TacticalCapabilities =>
            Candidate.TacticalCapabilities;

        /// <summary>
        /// Creates a scored candidate.
        /// </summary>
        public ScoredCandidate(
            EnemyCandidate candidate,
            CandidateScore score)
        {
            Candidate = candidate
                ?? throw new ArgumentNullException(nameof(candidate));

            Score = score;
        }

        /// <inheritdoc/>
        public int CompareTo(ScoredCandidate other)
        {
            if (other == null)
                return 1;

            return Score.CompareTo(other.Score);
        }
    }
}