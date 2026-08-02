using Chaosbound.Gameplay.Spawn.Execution;
using Chaosbound.Gameplay.Spawn.Infrastructure;
using Chaosbound.Gameplay.Spawn.Integration;
using Chaosbound.Gameplay.Spawn.References;
using System;

namespace Chaosbound.Gameplay.Spawn.Materialization
{
    /// <summary>
    /// Materializes enemy spawn tasks into the game world.
    /// </summary>
    public sealed class EnemyMaterializer :
        ISpawnMaterializer
    {
        private readonly ISpawnInstantiationService
            instantiationService;

        public EnemyMaterializer(
            ISpawnInstantiationService instantiationService)
        {
            this.instantiationService =
                instantiationService
                ?? throw new ArgumentNullException(
                    nameof(instantiationService));
        }

        /// <summary>
        /// Materializes the supplied execution context.
        /// </summary>
        public void Materialize(
            SpawnExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context
                    .ScheduledTask
                    .Task
                    .Entry
                    .Materializable
                    .Reference
                is not EnemyMaterializableReference reference)
            {
                throw new InvalidOperationException(
                    "EnemyMaterializer received an unsupported materializable reference.");
            }

            SpawnInstantiationRequest request =
                new SpawnInstantiationRequest(
                    reference);

            instantiationService.Spawn(request);
        }
    }
}