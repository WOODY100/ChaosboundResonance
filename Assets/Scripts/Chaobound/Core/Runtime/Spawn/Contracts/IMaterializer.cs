using Chaosbound.Shared.Contracts;

namespace Chaosbound.Core.Runtime.Spawn.Contracts
{
    /// <summary>
    /// Coordinates the materialization of declarative content.
    /// </summary>
    public interface IMaterializer
    {
        /// <summary>
        /// Materializes a runtime instance from the specified definition.
        /// </summary>
        IMaterializedInstance Materialize(IDefinition definition);
    }
}