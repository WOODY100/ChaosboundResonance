namespace Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Runtime
{
    /// <summary>
    /// Represents the lifecycle state of the Exit Portal
    /// during the current expedition.
    /// </summary>
    public enum ExitPortalDomainState
    {
        Inactive = 0,

        Waiting = 1,

        Spawned = 2
    }
}