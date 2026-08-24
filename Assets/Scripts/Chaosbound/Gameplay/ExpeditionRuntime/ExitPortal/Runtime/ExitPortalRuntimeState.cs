namespace Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Runtime
{
    /// <summary>
    /// Mutable runtime state owned by the Exit Portal Domain.
    /// </summary>
    public sealed class ExitPortalRuntimeState
    {
        /// <summary>
        /// Gets the current lifecycle state of the
        /// Exit Portal Domain.
        /// </summary>
        public ExitPortalDomainState State
        {
            get;
            private set;
        }

        public ExitPortalRuntimeState()
        {
            State =
                ExitPortalDomainState.Inactive;
        }

        public bool InteractionRequested
        {
            get;
            private set;
        }

        /// <summary>
        /// Activates the Exit Portal Domain for
        /// the current expedition.
        /// </summary>
        public void Start()
        {
            State =
                ExitPortalDomainState.Waiting;
        }

        /// <summary>
        /// Marks the Exit Portal as spawned.
        /// </summary>
        public void MarkSpawned()
        {
            State =
                ExitPortalDomainState.Spawned;
        }

        public void RequestInteraction()
        {
            InteractionRequested = true;
        }

        public void ClearInteractionRequest()
        {
            InteractionRequested = false;
        }
    }
}