using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Pipeline;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Factories;
using Chaosbound.Gameplay.ExpeditionRuntime.Pipeline;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Director
{
    /// <summary>
    /// Coordinates the execution of the Expedition Runtime.
    /// </summary>
    public sealed class ExpeditionDirector
    {
        private readonly ExpeditionRuntimePipeline
            runtimePipeline;

        private readonly ExpeditionCleanupPipeline
            cleanupPipeline;

        private readonly SceneTransitionService
            sceneTransitionService;

        private ExpeditionRuntimeContextFactory
            contextFactory;

        private ExpeditionRuntimeState
            runtimeState;

        private float
            debugTimer;

        public ExpeditionDirector(
            ExpeditionRuntimePipeline runtimePipeline,
            ExpeditionCleanupPipeline cleanupPipeline,
            SceneTransitionService sceneTransitionService)
        {
            this.runtimePipeline =
                runtimePipeline
                ?? throw new ArgumentNullException(
                    nameof(runtimePipeline));

            this.cleanupPipeline =
                cleanupPipeline
                ?? throw new ArgumentNullException(
                    nameof(cleanupPipeline));

            this.sceneTransitionService =
                sceneTransitionService
                ?? throw new ArgumentNullException(
                    nameof(sceneTransitionService));
        }

        public bool IsRunning
        {
            get;
            private set;
        }

        public void StartExpedition(
            RuntimeExpeditionConfig config)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException(
                    "An expedition is already running.");
            }

            if (config == null)
                throw new ArgumentNullException(
                    nameof(config));

            runtimeState =
                new ExpeditionRuntimeState();

            contextFactory =
                new ExpeditionRuntimeContextFactory(
                    config,
                    runtimeState);

            IsRunning = true;
        }

        /// <summary>
        /// Executes one runtime tick.
        /// </summary>
        public void Tick()
        {
            if (!IsRunning)
                return;

            ExpeditionRuntimeContext context =
                contextFactory.Create();

            runtimePipeline.Execute(
                context);

            if (runtimeState.ExitPortal.InteractionRequested)
            {
                CompleteExpedition();
                return;
            }

            debugTimer +=
                UnityEngine.Time.unscaledDeltaTime;
        }

        /// <summary>
        /// Completes the current expedition through
        /// the Exit Portal flow.
        /// </summary>
        private void CompleteExpedition()
        {
            if (!IsRunning)
                return;

            runtimeState.ExitPortal.ClearInteractionRequest();

            CleanupCurrentExpedition();

            sceneTransitionService.LoadScene(
                GameScene.Sanctuary);
        }

        /// <summary>
        /// Finishes the current expedition without
        /// transitioning scenes.
        /// </summary>
        public void FinishExpedition()
        {
            CleanupCurrentExpedition();
        }

        public void AbortExpedition()
        {
            CleanupCurrentExpedition();
        }

        private void CleanupCurrentExpedition()
        {
            if (!IsRunning)
                return;

            ExpeditionCleanupContext cleanupContext =
                new ExpeditionCleanupContext(
                    runtimeState);

            cleanupPipeline.Execute(
                cleanupContext);

            IsRunning = false;

            runtimeState = null;
            contextFactory = null;
        }

        public ExpeditionRuntimeState RuntimeState
        {
            get
            {
                return runtimeState;
            }
        }
    }
}