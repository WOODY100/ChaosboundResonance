namespace Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Runtime
{
    public sealed class ExitPortalRuntimeState
    {
        public ExitPortalDomainState State { get; private set; }

        public bool InteractionRequested { get; private set; }

        public bool ExitAccepted { get; private set; }

        public ExitPortalRuntimeState()
        {
            State = ExitPortalDomainState.Inactive;
        }

        public void Start()
        {
            State = ExitPortalDomainState.Waiting;
        }

        public void MarkSpawned()
        {
            State = ExitPortalDomainState.Spawned;
        }

        public void RequestInteraction()
        {
            InteractionRequested = true;
        }

        public void ClearInteractionRequest()
        {
            InteractionRequested = false;
        }

        public void MarkExitAccepted()
        {
            ExitAccepted = true;
        }

        public void ClearExitAccepted()
        {
            ExitAccepted = false;
        }
    }
}