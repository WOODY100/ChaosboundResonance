using Chaosbound.Shared.Contracts;
using Chaosbound.Gameplay.Spawn.Contracts;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Spawn.Services
{
    /// <summary>
    /// Coordinates declarative content materialization by delegating
    /// to specialized content materializers.
    /// </summary>
    public sealed class Materializer : IMaterializer
    {
        private readonly IReadOnlyCollection<IContentMaterializer> materializers;

        public Materializer(IReadOnlyCollection<IContentMaterializer> materializers)
        {
            this.materializers = materializers
                ?? throw new ArgumentNullException(nameof(materializers));
        }

        public IMaterializedInstance Materialize(IDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            foreach (IContentMaterializer materializer in materializers)
            {
                if (!materializer.CanMaterialize(definition))
                {
                    continue;
                }

                return materializer.Materialize(definition);
            }

            throw new InvalidOperationException(
                $"No content materializer is registered for definition type '{definition.GetType().FullName}'.");
        }
    }
}