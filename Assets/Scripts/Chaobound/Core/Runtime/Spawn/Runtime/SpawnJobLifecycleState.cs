namespace Chaosbound.Core.Runtime.Spawn.Runtime
{
    /// <summary>
    /// Represents the execution lifecycle of a SpawnJob during a run.
    /// </summary>
    public enum SpawnJobLifecycleState
    {
        /// <summary>
        /// The SpawnJob has been created but has not started executing.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// The SpawnJob is currently executing.
        /// </summary>
        Running = 1,

        /// <summary>
        /// The SpawnJob finished successfully.
        /// </summary>
        Completed = 2,

        /// <summary>
        /// The SpawnJob was permanently cancelled before completion.
        /// </summary>
        Cancelled = 3
    }
}