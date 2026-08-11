using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Definitions;
using Chaosbound.Shared.Contracts;
using System;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates SpawnRequestEntry collections from gameplay spawn plans.
    /// </summary>
    public sealed class SpawnRequestEntryFactory
    {
        private readonly MaterializableReferenceFactory
            materializableReferenceFactory;

        public SpawnRequestEntryFactory(
            MaterializableReferenceFactory materializableReferenceFactory)
        {
            this.materializableReferenceFactory =
                materializableReferenceFactory
                ?? throw new ArgumentNullException(
                    nameof(materializableReferenceFactory));
        }

        public SpawnRequestEntry Create(
            IMaterializableReference reference,
            int quantity)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(
                    nameof(reference));
            }

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "Spawn quantity must be greater than zero.");
            }

            MaterializableDefinition materializable =
                new MaterializableDefinition(
                    reference);

            return new SpawnRequestEntry(
                materializable,
                quantity);
        }
    }
}