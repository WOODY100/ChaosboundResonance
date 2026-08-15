using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Factories;
using Chaosbound.Gameplay.ExpeditionRuntime.Pipeline;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Director
{
    /// <summary>
    /// Coordinates the execution of the Expedition Runtime.
    /// </summary>
    public sealed class ExpeditionDirector
    {
        private readonly ExpeditionRuntimePipeline
            pipeline;

        private ExpeditionRuntimeContextFactory
            contextFactory;

        private ExpeditionRuntimeState
            runtimeState;

        private float
            debugTimer;

        /// <summary>
        /// Creates a new Expedition Director.
        /// </summary>
        public ExpeditionDirector(
            ExpeditionRuntimePipeline pipeline)
        {
            this.pipeline =
                pipeline
                ?? throw new ArgumentNullException(
                    nameof(pipeline));
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
                throw new ArgumentNullException(nameof(config));

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

            pipeline.Execute(
                context);

            debugTimer +=
                UnityEngine.Time.unscaledDeltaTime;
        }

        public void FinishExpedition()
        {
            if (!IsRunning)
                return;

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