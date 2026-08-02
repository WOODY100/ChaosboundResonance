using System;
using Chaosbound.Gameplay.Spawn.Domain;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.ValueObjects;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates immutable SpawnTask instances.
    /// </summary>
    public sealed class SpawnTaskFactory
    {
        /// <summary>
        /// Creates a SpawnTask.
        /// </summary>
        /// <param name="entry">
        /// Task entry to materialize.
        /// </param>
        /// <returns>
        /// Immutable SpawnTask.
        /// </returns>
        public SpawnTask Create(
            SpawnTaskEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            return new SpawnTask(
                SpawnTaskIdentity.New(),
                entry);
        }
    }
}