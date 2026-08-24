using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Context
{
    /// <summary>
    /// Provides the runtime context required during
    /// expedition cleanup.
    /// </summary>
    public sealed class ExpeditionCleanupContext
    {
        /// <summary>
        /// Gets the runtime state of the expedition
        /// being finalized.
        /// </summary>
        public ExpeditionRuntimeState RuntimeState
        {
            get;
        }

        public ExpeditionCleanupContext(
            ExpeditionRuntimeState runtimeState)
        {
            RuntimeState =
                runtimeState
                ?? throw new ArgumentNullException(
                    nameof(runtimeState));
        }
    }
}