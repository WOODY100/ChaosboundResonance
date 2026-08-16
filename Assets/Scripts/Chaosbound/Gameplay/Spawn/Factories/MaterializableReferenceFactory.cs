using System;
using Chaosbound.Shared.Contracts;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates materializable references from runtime content.
    /// </summary>
    public sealed class MaterializableReferenceFactory
    {
        public IMaterializableReference Create(
            IMaterializableReference reference)
        {
            if (reference == null)
                throw new ArgumentNullException(
                    nameof(reference));

            return reference;
        }
    }
}