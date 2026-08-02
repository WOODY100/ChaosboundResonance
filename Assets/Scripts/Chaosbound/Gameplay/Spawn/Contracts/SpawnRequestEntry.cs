using Chaosbound.Gameplay.Spawn.Definitions;
using System;

namespace Chaosbound.Gameplay.Spawn.Contracts
{
    /// <summary>
    /// Represents a single declarative materialization request.
    ///
    /// Each entry describes one type of materializable that should
    /// exist in the world.
    /// </summary>
    public sealed class SpawnRequestEntry
    {
        /// <summary>
        /// Gets the requested materializable.
        /// </summary>
        public MaterializableDefinition Materializable { get; }

        /// <summary>
        /// Gets the requested quantity.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Gets entry-specific materialization options.
        /// </summary>
        public SpawnEntryOptions Options { get; }

        /// <summary>
        /// Creates a new spawn request entry.
        /// </summary>
        public SpawnRequestEntry(
            MaterializableDefinition materializable,
            int quantity,
            SpawnEntryOptions options)
        {
            Materializable = materializable
                ?? throw new ArgumentNullException(nameof(materializable));

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "Quantity must be greater than zero.");
            }

            Options = options
                ?? throw new ArgumentNullException(nameof(options));

            Quantity = quantity;
        }
    }
}