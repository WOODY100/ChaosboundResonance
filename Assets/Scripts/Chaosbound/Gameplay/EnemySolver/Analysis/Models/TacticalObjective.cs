using Chaosbound.Gameplay.EnemySolver.Enums;
using System;

namespace Chaosbound.Gameplay.EnemySolver.Analysis.Models
{
    /// <summary>
    /// Represents the tactical objective selected by the Enemy Solver.
    /// </summary>
    public sealed class TacticalObjective
    {
        /// <summary>
        /// Gets the tactical capability targeted by this objective.
        /// </summary>
        public TacticalCapability Capability { get; }

        /// <summary>
        /// Gets the difference between the current and desired capability values.
        /// A positive value indicates a deficit.
        /// A negative value indicates an excess.
        /// </summary>
        public int Difference { get; }

        /// <summary>
        /// Gets whether this objective represents a capability deficit.
        /// </summary>
        public bool IsDeficit
        {
            get
            {
                return Difference > 0;
            }
        }

        /// <summary>
        /// Gets whether this objective represents a capability excess.
        /// </summary>
        public bool IsExcess
        {
            get
            {
                return Difference < 0;
            }
        }

        /// <summary>
        /// Initializes a new tactical objective.
        /// </summary>
        /// <param name="capability">
        /// The tactical capability associated with the objective.
        /// </param>
        /// <param name="difference">
        /// The difference between the current and desired capability values.
        /// Cannot be zero.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the capability is invalid or the difference is zero.
        /// </exception>
        public TacticalObjective(
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
                    "A tactical objective must represent a non-zero difference.");
            }

            Capability = capability;
            Difference = difference;
        }
    }
}