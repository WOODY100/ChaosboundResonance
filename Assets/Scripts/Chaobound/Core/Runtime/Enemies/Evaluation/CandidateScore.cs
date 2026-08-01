using System;

namespace Chaosbound.Core.Runtime.Enemies.Evaluation
{
    /// <summary>
    /// Represents the tactical score assigned to an enemy candidate during
    /// the evaluation process.
    ///
    /// Scores are relative values whose only purpose is comparing candidates.
    /// Higher values represent better tactical choices.
    /// </summary>
    public readonly struct CandidateScore :
        IEquatable<CandidateScore>,
        IComparable<CandidateScore>
    {
        /// <summary>
        /// Represents a score of zero.
        /// </summary>
        public static readonly CandidateScore Zero = new(0f);

        /// <summary>
        /// Gets the numeric value of this score.
        /// </summary>
        public float Value { get; }

        /// <summary>
        /// Creates a new candidate score.
        /// </summary>
        public CandidateScore(float value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        public int CompareTo(CandidateScore other)
        {
            return Value.CompareTo(other.Value);
        }

        /// <inheritdoc/>
        public bool Equals(CandidateScore other)
        {
            return Value.Equals(other.Value);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is CandidateScore other &&
                   Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString("0.##");
        }

        public static CandidateScore operator +(
            CandidateScore left,
            CandidateScore right)
        {
            return new CandidateScore(left.Value + right.Value);
        }

        public static CandidateScore operator -(
            CandidateScore left,
            CandidateScore right)
        {
            return new CandidateScore(left.Value - right.Value);
        }

        public static bool operator >(
            CandidateScore left,
            CandidateScore right)
        {
            return left.Value > right.Value;
        }

        public static bool operator <(
            CandidateScore left,
            CandidateScore right)
        {
            return left.Value < right.Value;
        }

        public static bool operator >=(
            CandidateScore left,
            CandidateScore right)
        {
            return left.Value >= right.Value;
        }

        public static bool operator <=(
            CandidateScore left,
            CandidateScore right)
        {
            return left.Value <= right.Value;
        }

        public static bool operator ==(
            CandidateScore left,
            CandidateScore right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(
            CandidateScore left,
            CandidateScore right)
        {
            return !left.Equals(right);
        }
    }
}