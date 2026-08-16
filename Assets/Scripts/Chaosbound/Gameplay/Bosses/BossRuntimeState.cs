using Chaosbound.Content.Enemy.Bosses;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Mutable runtime state owned by the Boss Domain.
    /// </summary>
    public sealed class BossRuntimeState
    {
        /// <summary>
        /// Gets the current lifecycle state of the Boss Domain.
        /// </summary>
        public BossDomainState State { get; private set; }

        /// <summary>
        /// Gets the Boss selected for the current activation.
        /// </summary>
        public BossData SelectedBoss { get; private set; }

        public BossRuntimeState()
        {
            State = BossDomainState.Inactive;
            SelectedBoss = null;
        }

        public void Start(
            BossData selectedBoss)
        {
            SelectedBoss =
                selectedBoss;

            State =
                BossDomainState.Starting;
        }

        public void MarkActive()
        {
            State =
                BossDomainState.Active;
        }

        public void Complete()
        {
            State =
                BossDomainState.Completed;
        }
    }
}