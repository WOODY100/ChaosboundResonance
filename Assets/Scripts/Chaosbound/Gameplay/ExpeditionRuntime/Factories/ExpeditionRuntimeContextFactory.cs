using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Factories
{
    /// <summary>
    /// Builds the shared runtime context used by the
    /// Expedition Runtime.
    /// </summary>
    public sealed class ExpeditionRuntimeContextFactory
    {
        private readonly RuntimeExpeditionConfig
            runtimeConfig;

        private readonly ExpeditionRuntimeState
            runtimeState;

        /// <summary>
        /// Creates a new runtime context.
        /// </summary>
        public ExpeditionRuntimeContext Create()
        {
            ExpeditionRuntimeReferences references =
                BuildReferences();

            ExpeditionRuntimeServices services =
                BuildServices();

            return new ExpeditionRuntimeContext(
                runtimeConfig,
                references,
                services,
                runtimeState);
        }

        private ExpeditionRuntimeReferences BuildReferences()
        {
            return new ExpeditionRuntimeReferences();
        }

        private ExpeditionRuntimeServices BuildServices()
        {
            return new ExpeditionRuntimeServices();
        }

        public ExpeditionRuntimeContextFactory(
            RuntimeExpeditionConfig runtimeConfig,
            ExpeditionRuntimeState runtimeState)
        {
            this.runtimeConfig =
                runtimeConfig
                ?? throw new ArgumentNullException(
                    nameof(runtimeConfig));
            this.runtimeState =
                runtimeState
                ?? throw new ArgumentNullException(
                    nameof(runtimeState));
        }
    }
}