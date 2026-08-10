using System;

namespace Chaosbound.Content.Expeditions.Definitions.Combat.Replenishment
{
    public sealed class ReplenishmentDefinition
    {
        public float InitialDelay { get; }

        public float RecoveryInterval { get; }

        public ReplenishmentDefinition(
            float initialDelay,
            float recoveryInterval)
        {
            if (initialDelay < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(initialDelay),
                    "Initial delay cannot be negative.");
            }

            if (recoveryInterval <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(recoveryInterval),
                    "Recovery interval must be greater than zero.");
            }

            InitialDelay = initialDelay;
            RecoveryInterval = recoveryInterval;
        }
    }
}