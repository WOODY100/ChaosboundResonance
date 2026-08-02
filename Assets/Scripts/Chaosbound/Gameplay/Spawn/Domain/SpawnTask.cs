using System;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.ValueObjects;

namespace Chaosbound.Gameplay.Spawn.Domain
{
    /// <summary>
    /// Represents an immutable immediate execution task
    /// produced by the Spawn Runtime.
    /// </summary>
    public sealed class SpawnTask
    {
        /// <summary>
        /// Gets the immutable task identity.
        /// </summary>
        public SpawnTaskIdentity Identity { get; }

        /// <summary>
        /// Gets the task entry represented by this task.
        /// </summary>
        public SpawnTaskEntry Entry { get; }

        public SpawnTask(
            SpawnTaskIdentity identity,
            SpawnTaskEntry entry)
        {
            Identity = identity;

            Entry = entry
                ?? throw new ArgumentNullException(nameof(entry));
        }
    }
}