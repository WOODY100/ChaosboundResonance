using Chaosbound.Gameplay.EnemySolver.Enums;
using System;

namespace Chaosbound.Gameplay.EnemySolver.Analysis.Models
{
    /// <summary>
    /// Represents a tactical need identified during composition analysis.
    /// </summary>
    public sealed class CompositionNeed
    {
        /// <summary>
        /// Gets the tactical capability associated with this need.
        /// </summary>
        public TacticalCapability Capability { get; }

        /// <summary>
        /// Gets the difference between the current and desired capability values.
        /// A positive value indicates a deficit.
        /// A negative value indicates an excess.
        /// </summary>
        public int Difference { get; }

        /// <summary>
        /// Gets whether this need represents a capability deficit.
        /// </summary>
        public bool IsDeficit
        {
            get
            {
                return Difference > 0;
            }
        }

        /// <summary>
        /// Gets whether this need represents a capability excess.
        /// </summary>
        public bool IsExcess
        {
            get
            {
                return Difference < 0;
            }
        }

        /// <summary>
        /// Initializes a new tactical composition need.
        /// </summary>
        /// <param name="capability">
        /// The tactical capability associated with the need.
        /// </param>
        /// <param name="difference">
        /// The difference between the current and desired values.
        /// Cannot be zero.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the capability is invalid or the difference is zero.
        /// </exception>
        public CompositionNeed(
            TacticalCapability capability,
            int difference)
        {
            if (!Enum.IsDefined(typeof(TacticalCapability), capability))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(capability),
                    capability,
                    "The tactical capability is not valid.");
            }

            if (difference == 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(difference),
                    difference,
                    "A composition need must represent a non-zero difference.");
            }

            Capability = capability;
            Difference = difference;
        }
    }
}