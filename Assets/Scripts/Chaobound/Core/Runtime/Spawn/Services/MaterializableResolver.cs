using Chaosbound.Core.Runtime.Spawn.Contracts;
using Chaosbound.Shared.Contracts;
using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Runtime.Spawn.Services
{
    /// <summary>
    /// Coordinates the resolution of materializable references into domain definitions.
    /// </summary>
    public sealed class MaterializableResolver : IMaterializableResolver
    {
        private readonly IReadOnlyCollection<IMaterializableReferenceResolver> _resolvers;

        public MaterializableResolver(
            IReadOnlyCollection<IMaterializableReferenceResolver> resolvers)
        {
            _resolvers = resolvers ?? throw new ArgumentNullException(nameof(resolvers));
        }

        public IDefinition ResolveDefinition(IMaterializableReference reference)
        {
            if (reference is null)
                throw new ArgumentNullException(nameof(reference));

            foreach (var resolver in _resolvers)
            {
                if (!resolver.CanResolve(reference))
                    continue;

                return resolver.ResolveDefinition(reference);
            }

            throw new InvalidOperationException(
                $"No materializable reference resolver is registered for reference type '{reference.GetType().Name}'.");
        }
    }
}