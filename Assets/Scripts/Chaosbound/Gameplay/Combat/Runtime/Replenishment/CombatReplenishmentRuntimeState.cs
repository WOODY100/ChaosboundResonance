namespace Chaosbound.Gameplay.Combat.Runtime.Replenishment
{
    /// <summary>
    /// Represents the mutable runtime state of combat
    /// replenishment for the current expedition.
    /// </summary>
    public sealed class CombatReplenishmentRuntimeState
    {
        public enum Phase
        {
            Ready = 0,
            WaitingInitialDelay = 1,
            WaitingRecovery = 2
        }

        public Phase CurrentPhase
        {
            get;
            private set;
        }

        public float Timer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the index from which the next replenishment
        /// search should begin.
        /// </summary>
        public int NextRoleIndex
        {
            get;
            private set;
        }

        public CombatReplenishmentRuntimeState()
        {
            Reset();
        }

        public void SetPhase(
            Phase phase)
        {
            CurrentPhase = phase;
        }

        public void SetTimer(
            float timer)
        {
            Timer = timer;
        }

        public void SetNextRoleIndex(
            int index)
        {
            NextRoleIndex = index;
        }

        public void Reset()
        {
            CurrentPhase = Phase.Ready;
            Timer = 0f;
            NextRoleIndex = 0;
        }
    }
}