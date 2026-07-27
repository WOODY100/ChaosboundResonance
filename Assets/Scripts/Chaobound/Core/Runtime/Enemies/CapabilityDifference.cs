using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents the objective difference between the current and desired
    /// values of a single tactical capability.
    /// </summary>
    public sealed class CapabilityDifference
    {
        /// <summary>
        /// Gets the tactical capability being compared.
        /// </summary>
        public TacticalCapability Capability { get; }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public int CurrentValue { get; }

        /// <summary>
        /// Gets the desired value.
        /// </summary>
        public int DesiredValue { get; }

        /// <summary>
        /// Gets the difference between the desired and current values.
        /// A positive value indicates that additional contribution is required.
        /// A negative value indicates excess contribution.
        /// Zero indicates an exact match.
        /// </summary>
        public int Difference => DesiredValue - CurrentValue;

        public CapabilityDifference(
            TacticalCapability capability,
            int currentValue,
            int desiredValue)
        {
            ValidateCapability(capability);
            ValidateNonNegative(currentValue, nameof(currentValue));
            ValidateNonNegative(desiredValue, nameof(desiredValue));

            Capability = capability;
            CurrentValue = currentValue;
            DesiredValue = desiredValue;
        }

        private static void ValidateCapability(TacticalCapability capability)
        {
            if (!Enum.IsDefined(typeof(TacticalCapability), capability))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(capability),
                    capability,
                    "Invalid tactical capability.");
            }
        }

        private static void ValidateNonNegative(int value, string parameterName)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    value,
                    "Capability values cannot be negative.");
            }
        }
    }
}