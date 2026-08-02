using System;
using Chaosbound.Gameplay.Spawn.Domain;

namespace Chaosbound.Gameplay.Spawn.Models
{
    /// <summary>
    /// Represents a SpawnTask scheduled for future execution.
    /// </summary>
    public sealed class ScheduledSpawnTask
    {
        /// <summary>
        /// Gets the task to execute.
        /// </summary>
        public SpawnTask Task { get; }

        /// <summary>
        /// Gets the relative delay before execution.
        /// </summary>
        public TimeSpan Delay { get; }

        public ScheduledSpawnTask(
            SpawnTask task,
            TimeSpan delay)
        {
            Task = task
                ?? throw new ArgumentNullException(nameof(task));

            if (delay < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(delay),
                    "Delay cannot be negative.");
            }

            Delay = delay;
        }
    }
}