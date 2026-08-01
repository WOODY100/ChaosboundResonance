using Chaosbound.Shared.Contracts;

namespace Chaosbound.Gameplay.Spawn.Contracts
{
    /// <summary>
    /// Resolves a materializable reference into its corresponding domain definition.
    /// </summary>
    public interface IMaterializableResolver
    {
        /// <summary>
        /// Resolves the specified materializable reference.
        /// </summary>
        /// <param name="reference">
        /// The reference to resolve.
        /// </param>
        /// <returns>
        /// The corresponding domain definition.
        /// </returns>
        IDefinition ResolveDefinition(IMaterializableReference reference);
    }
}