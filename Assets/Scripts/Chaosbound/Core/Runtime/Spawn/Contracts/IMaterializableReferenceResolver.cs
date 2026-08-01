using Chaosbound.Shared.Contracts;

namespace Chaosbound.Core.Runtime.Spawn.Contracts
{
    /// <summary>
    /// Resolves a specific materializable reference into its corresponding domain definition.
    /// </summary>
    public interface IMaterializableReferenceResolver
    {
        /// <summary>
        /// Determines whether this resolver supports the specified reference.
        /// </summary>
        bool CanResolve(IMaterializableReference reference);

        /// <summary>
        /// Resolves the specified materializable reference.
        /// </summary>
        IDefinition ResolveDefinition(IMaterializableReference reference);
    }
}