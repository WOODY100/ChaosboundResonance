using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents the complete comparison between two tactical profiles.
    /// </summary>
    public sealed class ProfileComparison
    {
        private readonly Dictionary<TacticalCapability, CapabilityDifference> _differences = new();

        /// <summary>
        /// Gets all registered capability differences.
        /// </summary>
        public IReadOnlyCollection<CapabilityDifference> Differences => _differences.Values;

        /// <summary>
        /// Gets the registered difference for the specified capability.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no difference exists for the specified capability.
        /// </exception>
        public CapabilityDifference GetDifference(TacticalCapability capability)
        {
            if (!TryGetDifference(capability, out CapabilityDifference difference))
            {
                throw new InvalidOperationException(
                    $"Capability '{capability}' is not registered.");
            }

            return difference;
        }

        /// <summary>
        /// Attempts to retrieve a capability difference.
        /// </summary>
        public bool TryGetDifference(
            TacticalCapability capability,
            out CapabilityDifference difference)
        {
            return _differences.TryGetValue(capability, out difference);
        }

        /// <summary>
        /// Adds a capability difference.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the supplied difference is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the capability is already registered.
        /// </exception>
        public void Add(CapabilityDifference difference)
        {
            if (difference == null)
            {
                throw new ArgumentNullException(nameof(difference));
            }

            if (_differences.ContainsKey(difference.Capability))
            {
                throw new InvalidOperationException(
                    $"Capability '{difference.Capability}' is already registered.");
            }

            _differences.Add(difference.Capability, difference);
        }

        /// <summary>
        /// Removes the specified capability difference.
        /// </summary>
        public bool Remove(TacticalCapability capability)
        {
            return _differences.Remove(capability);
        }

        /// <summary>
        /// Removes every registered capability difference.
        /// </summary>
        public void Clear()
        {
            _differences.Clear();
        }
    }
}