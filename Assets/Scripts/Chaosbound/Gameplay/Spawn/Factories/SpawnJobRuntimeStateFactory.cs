using Chaosbound.Gameplay.Spawn.Domain;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    public sealed class SpawnJobRuntimeStateFactory
    {
        public SpawnJobRuntimeState Create(
            SpawnJob job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            return new SpawnJobRuntimeState(job);
        }
    }
}