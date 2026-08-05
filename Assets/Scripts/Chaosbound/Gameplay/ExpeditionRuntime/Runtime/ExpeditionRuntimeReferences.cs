using System;
using Chaosbound.Content.Expeditions.Runtime.References;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Runtime
{
    /// <summary>
    /// Represents the shared runtime references available
    /// to every Expedition Runtime stage.
    /// </summary>
    public sealed class ExpeditionRuntimeReferences
    {
        /// <summary>
        /// Gets the immutable runtime references.
        /// </summary>
        public RuntimeReferencesConfig Runtime
        {
            get;
        }

        public ExpeditionRuntimeReferences(
            RuntimeReferencesConfig runtime)
        {
            Runtime =
                runtime
                ?? throw new ArgumentNullException(
                    nameof(runtime));
        }
    }
}