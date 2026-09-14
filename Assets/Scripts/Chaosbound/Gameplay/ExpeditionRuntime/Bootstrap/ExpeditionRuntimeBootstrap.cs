using Chaosbound.Content.Items;
using Chaosbound.Core.GameFlow;
using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Pipeline;
using Chaosbound.Gameplay.ExpeditionRuntime.Composition;
using Chaosbound.Gameplay.ExpeditionRuntime.Director;
using Chaosbound.Gameplay.ExpeditionRuntime.Exit;
using Chaosbound.Gameplay.ExpeditionRuntime.Pipeline;
using Chaosbound.Gameplay.ExpeditionRuntime.Settlement;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.World.Integration;
using Chaosbound.Gameplay.Spawn.Bootstrap;
using Chaosbound.Gameplay.Spawn.Runtime;
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

        private readonly ExpeditionRuntimeCompositionContext
            compositionContext;

        private SpawnRuntime
            spawnRuntime;

        public ExpeditionRuntimeBootstrap(
            SceneTransitionService sceneTransitionService,
            ExpeditionRuntimeCompositionContext compositionContext)
        {
            this.sceneTransitionService =
                sceneTransitionService
                ?? throw new ArgumentNullException(
                    nameof(sceneTransitionService));

            this.compositionContext =
                compositionContext
                ?? throw new ArgumentNullException(
                    nameof(compositionContext));
        }

        /// <summary>
        /// Builds a fully initialized Expedition Director.
        /// </summary>
        public ExpeditionDirector Build()
        {
            spawnRuntime =
                new SpawnRuntimeBootstrap()
                    .Build();

            ExpeditionRuntimePipeline runtimePipeline =
                BuildRuntimePipeline(
                    spawnRuntime,
                    compositionContext);

            ExpeditionCleanupPipeline cleanupPipeline =
                BuildCleanupPipeline(
                    spawnRuntime);

            return new ExpeditionDirector(
                runtimePipeline,
                cleanupPipeline);
        }

        public ItemWorldDropService BuildItemWorldDropService()
        {
            if (spawnRuntime == null)
            {
                throw new InvalidOperationException(
                    "SpawnRuntime must be built before " +
                    "building ItemWorldDropService.");
            }

            if (compositionContext.ItemDatabase == null)
            {
                throw new InvalidOperationException(
                    "ItemDatabase is not available.");
            }

            ItemResolver itemResolver =
                new ItemResolver(
                    compositionContext.ItemDatabase);

            return new ItemWorldDropService(
                itemResolver,
                spawnRuntime);
        }

        private ExpeditionRuntimePipeline
            BuildRuntimePipeline(
                SpawnRuntime spawnRuntime,
                ExpeditionRuntimeCompositionContext compositionContext)
        {
            if (spawnRuntime == null)
                throw new ArgumentNullException(
                    nameof(spawnRuntime));

            if (compositionContext == null)
                throw new ArgumentNullException(
                    nameof(compositionContext));

            ExpeditionRuntimePipelineFactory factory =
                new ExpeditionRuntimePipelineFactory();

            return factory.Create(
                spawnRuntime,
                compositionContext);
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
            ExpeditionSettlementService settlementService,
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
                settlementService,
                gameFlow,
                sceneTransitionService);
        }
    }
}