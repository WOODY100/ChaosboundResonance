namespace Chaosbound.Gameplay.Spawn.Runtime
{
    /// <summary>
    /// Represents the execution lifecycle of a SpawnTask.
    /// </summary>
    public enum SpawnTaskLifecycleState
    {
        Pending = 0,

        Running = 1,

        Completed = 2,

        Cancelled = 3
    }
}