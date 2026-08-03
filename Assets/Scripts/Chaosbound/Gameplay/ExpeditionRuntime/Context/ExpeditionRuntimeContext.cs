using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Context
{
    /// <summary>
    /// Represents the shared runtime context passed through
    /// every Expedition Runtime stage.
    /// </summary>
    public sealed class ExpeditionRuntimeContext
    {
        /// <summary>
        /// Gets the immutable runtime configuration.
        /// </summary>
        public RuntimeExpeditionConfig Config { get; }

        /// <summary>
        /// Gets the runtime world references.
        /// </summary>
        public ExpeditionRuntimeReferences References { get; }

        /// <summary>
        /// Gets the runtime services.
        /// </summary>
        public ExpeditionRuntimeServices Services { get; }

        /// <summary>
        /// Gets the mutable runtime state.
        /// </summary>
        public ExpeditionRuntimeState State { get; }

        public ExpeditionRuntimeContext(
            RuntimeExpeditionConfig config,
            ExpeditionRuntimeReferences references,
            ExpeditionRuntimeServices services,
            ExpeditionRuntimeState state)
        {
            Config =
                config
                ?? throw new ArgumentNullException(
                    nameof(config));

            References =
                references
                ?? throw new ArgumentNullException(
                    nameof(references));

            Services =
                services
                ?? throw new ArgumentNullException(
                    nameof(services));

            State =
                state
                ?? throw new ArgumentNullException(
                    nameof(state));
        }
    }
}