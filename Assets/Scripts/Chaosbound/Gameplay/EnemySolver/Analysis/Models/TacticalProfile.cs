using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents the current tactical capability state of an enemy composition.
    /// This model is purely descriptive and contains no decision-making logic.
    /// </summary>
    public sealed class TacticalProfile
    {
        private readonly Dictionary<TacticalCapability, int> _values = new();

        /// <summary>
        /// Gets a read-only view of all registered tactical capability values.
        /// </summary>
        public IReadOnlyDictionary<TacticalCapability, int> Values => _values;

        /// <summary>
        /// Gets the current value of a tactical capability.
        /// Returns zero when the capability has not been registered.
        /// </summary>
        public int GetValue(TacticalCapability capability)
        {
            return _values.TryGetValue(capability, out int value)
                ? value
                : 0;
        }

        /// <summary>
        /// Sets the value of a tactical capability.
        /// A value of zero removes the capability from the profile.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the supplied value is negative.
        /// </exception>
        public void SetValue(TacticalCapability capability, int value)
        {
            ValidateNonNegative(value);

            if (value == 0)
            {
                _values.Remove(capability);
                return;
            }

            _values[capability] = value;
        }

        /// <summary>
        /// Increments the value of a tactical capability.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the supplied amount is zero or negative.
        /// </exception>
        public void Increment(TacticalCapability capability, int amount = 1)
        {
            ValidatePositive(amount);

            SetValue(capability, GetValue(capability) + amount);
        }

        /// <summary>
        /// Removes every registered tactical capability.
        /// </summary>
        public void Clear()
        {
            _values.Clear();
        }

        private static void ValidateNonNegative(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "Capability values cannot be negative.");
            }
        }

        private static void ValidatePositive(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Increment amount must be greater than zero.");
            }
        }
    }
}