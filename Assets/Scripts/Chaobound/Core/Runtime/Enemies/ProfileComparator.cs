using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Compares two tactical profiles and produces their capability differences.
    /// </summary>
    public sealed class ProfileComparator
    {
        /// <summary>
        /// Compares the current tactical profile against the desired tactical profile.
        /// </summary>
        public ProfileComparison Compare(
            TacticalProfile currentProfile,
            TacticalProfile desiredProfile)
        {
            if (currentProfile == null)
            {
                throw new ArgumentNullException(nameof(currentProfile));
            }

            if (desiredProfile == null)
            {
                throw new ArgumentNullException(nameof(desiredProfile));
            }

            ProfileComparison comparison = new ProfileComparison();

            foreach (TacticalCapability capability in Enum.GetValues(typeof(TacticalCapability)))
            {
                CapabilityDifference difference = new CapabilityDifference(
                    capability,
                    currentProfile.GetValue(capability),
                    desiredProfile.GetValue(capability));

                comparison.Add(difference);
            }

            return comparison;
        }
    }
}