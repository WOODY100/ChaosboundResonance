namespace Chaosbound.Content.Expeditions.Runtime.Combat.Replenishment
{
    public sealed class RuntimeReplenishmentProfile
    {
        public float InitialDelay { get; }

        public float RecoveryInterval { get; }

        public RuntimeReplenishmentProfile(
            float initialDelay,
            float recoveryInterval)
        {
            InitialDelay = initialDelay;
            RecoveryInterval = recoveryInterval;
        }
    }
}