namespace Chaosbound.Gameplay.EnemySolver.Runtime.Synchronization
{
    /// <summary>
    /// Represents the type of synchronization required to align the
    /// materialized composition with the desired composition.
    /// </summary>
    public enum SynchronizationOperationType
    {
        /// <summary>
        /// Additional enemies must be materialized.
        /// </summary>
        Spawn,

        /// <summary>
        /// Existing enemies must be removed from the materialized composition.
        /// </summary>
        Despawn
    }
}