using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Spawn.Domain;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    public sealed class SpawnJobRuntimeStateFactory
    {
        public SpawnJobRuntimeState Create(
            SpawnJob job,
            ExpeditionRuntimeState expeditionRuntime)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            if (expeditionRuntime == null)
                throw new ArgumentNullException(nameof(expeditionRuntime));

            return new SpawnJobRuntimeState(
                job,
                expeditionRuntime);
        }
    }
}