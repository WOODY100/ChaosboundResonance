using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Pipeline;
using Chaosbound.Gameplay.ExpeditionRuntime.Director;
using Chaosbound.Gameplay.ExpeditionRuntime.Pipeline;
using Chaosbound.Gameplay.Spawn.Bootstrap;
using Chaosbound.Gameplay.Spawn.Runtime;
using Chaosbound.Core.GameFlow;
using Chaosbound.Gameplay.ExpeditionRuntime.Exit;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Bootstrap
{
    /// <summary>
    /// Builds the dependency graph required to execute
    /// the Expedition Runtime.
    /// </summary>
    public sealed class ExpeditionRuntimeBootstrap
    {
        private readonly SceneTransitionService
            sceneTransitionService;

        public ExpeditionRuntimeBootstrap(
            SceneTransitionService sceneTransitionService)
        {
            this.sceneTransitionService =
                sceneTransitionService
                ?? throw new ArgumentNullException(
                    nameof(sceneTransitionService));
        }

        /// <summary>
        /// Builds a fully initialized Expedition Director.
        /// </summary>
        public ExpeditionDirector Build()
        {
            SpawnRuntime spawnRuntime =
                new SpawnRuntimeBootstrap()
                    .Build();

            ExpeditionRuntimePipeline runtimePipeline =
                BuildRuntimePipeline(
                    spawnRuntime);

            ExpeditionCleanupPipeline cleanupPipeline =
                BuildCleanupPipeline(
                    spawnRuntime);

            return new ExpeditionDirector(
                runtimePipeline,
                cleanupPipeline);
        }

        private ExpeditionRuntimePipeline
            BuildRuntimePipeline(
                SpawnRuntime spawnRuntime)
        {
            if (spawnRuntime == null)
                throw new ArgumentNullException(
                    nameof(spawnRuntime));

            ExpeditionRuntimePipelineFactory factory =
                new ExpeditionRuntimePipelineFactory();

            return factory.Create(
                spawnRuntime);
        }

        private ExpeditionCleanupPipeline
            BuildCleanupPipeline(
                SpawnRuntime spawnRuntime)
        {
            if (spawnRuntime == null)
                throw new ArgumentNullException(
                    nameof(spawnRuntime));

            ExpeditionCleanupPipelineFactory factory =
                new ExpeditionCleanupPipelineFactory();

            return factory.Create(
                spawnRuntime);
        }

        public ExpeditionExitService BuildExitService(
            ExpeditionDirector expeditionDirector,
            GameFlow gameFlow)
        {
            if (expeditionDirector == null)
                throw new ArgumentNullException(
                    nameof(expeditionDirector));

            if (gameFlow == null)
                throw new ArgumentNullException(
                    nameof(gameFlow));

            return new ExpeditionExitService(
                expeditionDirector,
                gameFlow,
                sceneTransitionService);
        }
    }
}