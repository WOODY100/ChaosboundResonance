using Chaosbound.Content.Enemy.MiniBosses;

namespace Chaosbound.Gameplay.MiniBosses
{
    /// <summary>
    /// Mutable runtime state owned by the MiniBoss Domain.
    /// </summary>
    public sealed class MiniBossRuntimeState
    {
        /// <summary>
        /// Gets the current lifecycle state of the MiniBoss Domain.
        /// </summary>
        public MiniBossDomainState State
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the MiniBoss selected for the current activation.
        /// </summary>
        public MiniBossData SelectedMiniBoss
        {
            get;
            private set;
        }

        public MiniBossRuntimeState()
        {
            State =
                MiniBossDomainState.Inactive;

            SelectedMiniBoss =
                null;
        }

        public void Start(
            MiniBossData selectedMiniBoss)
        {
            SelectedMiniBoss =
                selectedMiniBoss;

            State =
                MiniBossDomainState.Starting;
        }

        public void MarkActive()
        {
            State =
                MiniBossDomainState.Active;
        }

        public void Complete()
        {
            State =
                MiniBossDomainState.Completed;
        }
    }
}