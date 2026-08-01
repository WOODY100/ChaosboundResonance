using Chaosbound.Shared.Contracts;

namespace Chaosbound.Gameplay.Spawn.Contracts
{
    /// <summary>
    /// Materializes a specific type of declarative content.
    /// </summary>
    public interface IContentMaterializer
    {
        /// <summary>
        /// Determines whether this materializer can materialize the specified definition.
        /// </summary>
        bool CanMaterialize(IDefinition definition);

        /// <summary>
        /// Materializes a runtime instance from the specified definition.
        /// </summary>
        IMaterializedInstance Materialize(IDefinition definition);
    }
}