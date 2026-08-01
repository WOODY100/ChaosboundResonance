using Chaosbound.Gameplay.Spawn.ValueObjects;
using Chaosbound.Gameplay.Spawn.Definitions;
using System;

namespace Chaosbound.Gameplay.Spawn.Domain
{
    /// <summary>
    /// Represents the complete declarative description of a spawn intention.
    /// </summary>
    public sealed class SpawnJob
    {
        public SpawnJobIdentity Identity { get; }

        public MaterializableDefinition Materializable { get; }

        public QuantityDefinition Quantity { get; }

        public PlacementDefinition Placement { get; }

        public ActivationDefinition Activation { get; }

        public ConstraintsDefinition Constraints { get; }

        public SpawnJob(
            SpawnJobIdentity identity,
            MaterializableDefinition materializable,
            QuantityDefinition quantity,
            PlacementDefinition placement,
            ActivationDefinition activation,
            ConstraintsDefinition constraints)
        {
            Identity = identity;

            Materializable = materializable
                ?? throw new ArgumentNullException(nameof(materializable));

            Quantity = quantity
                ?? throw new ArgumentNullException(nameof(quantity));

            Placement = placement
                ?? throw new ArgumentNullException(nameof(placement));

            Activation = activation
                ?? throw new ArgumentNullException(nameof(activation));

            Constraints = constraints
                ?? throw new ArgumentNullException(nameof(constraints));
        }
    }
}