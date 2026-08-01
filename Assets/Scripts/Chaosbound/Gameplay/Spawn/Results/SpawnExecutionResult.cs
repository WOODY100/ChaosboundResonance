using Chaosbound.Gameplay.Spawn.Contracts;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Spawn.Results
{
    /// <summary>
    /// Represents the outcome of executing a SpawnJob.
    /// </summary>
    public sealed class SpawnExecutionResult
    {
        public IReadOnlyList<IMaterializedInstance> Instances { get; }

        public SpawnExecutionResult(
            IReadOnlyList<IMaterializedInstance> instances)
        {
            Instances = instances
                ?? throw new ArgumentNullException(nameof(instances));
        }
    }
}