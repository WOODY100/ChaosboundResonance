using Chaosbound.Shared.Contracts;

namespace Chaosbound.Core.Runtime.Spawn.Contracts
{
    /// <summary>
    /// Represents a runtime instance created from declarative content.
    /// </summary>
    public interface IMaterializedInstance
    {
        /// <summary>
        /// Gets the identity of the materialized content.
        /// </summary>
        IIdentity Identity { get; }

        /// <summary>
        /// Gets whether the runtime instance is still valid.
        /// </summary>
        bool IsValid { get; }
    }
}