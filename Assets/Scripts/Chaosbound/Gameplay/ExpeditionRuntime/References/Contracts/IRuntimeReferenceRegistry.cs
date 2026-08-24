using Chaosbound.Shared.Identifiers;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.References.Contracts
{
    /// <summary>
    /// Provides access to runtime world references
    /// registered during the current expedition.
    /// </summary>
    public interface IRuntimeReferenceRegistry
    {
        /// <summary>
        /// Registers a runtime world reference.
        /// </summary>
        void Register(
            string domainId,
            ContentId contentId,
            Transform transform);

        /// <summary>
        /// Removes a previously registered runtime world reference.
        /// </summary>
        void Unregister(
            string domainId,
            ContentId contentId);

        /// <summary>
        /// Attempts to resolve a runtime world reference.
        /// </summary>
        bool TryResolve(
            string domainId,
            ContentId contentId,
            out Transform transform);
    }
}