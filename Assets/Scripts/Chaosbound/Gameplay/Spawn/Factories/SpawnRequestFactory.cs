using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Spawn.Contracts;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Spawn.Factories
{
    /// <summary>
    /// Creates SpawnRequest aggregate roots.
    /// </summary>
    public sealed class SpawnRequestFactory
    {
        private readonly SpawnRequestEntryFactory entryFactory;
        private readonly SpawnRequestContextFactory contextFactory;
        private readonly SpawnRequestMetadataFactory metadataFactory;

        /// <summary>
        /// Creates a SpawnRequestFactory using the default
        /// factory implementations.
        /// </summary>
        public SpawnRequestFactory()
            : this(
                new SpawnRequestEntryFactory(
                new MaterializableReferenceFactory()),
                new SpawnRequestContextFactory(),
                new SpawnRequestMetadataFactory())
        {
        }

        /// <summary>
        /// Creates a SpawnRequestFactory with the specified
        /// specialized factories.
        /// </summary>
        /// <param name="entryFactory">
        /// Factory responsible for creating SpawnRequestEntry instances.
        /// </param>
        /// <param name="contextFactory">
        /// Factory responsible for creating SpawnRequestContext instances.
        /// </param>
        /// <param name="metadataFactory">
        /// Factory responsible for creating SpawnRequestMetadata instances.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any dependency is null.
        /// </exception>
        public SpawnRequestFactory(
            SpawnRequestEntryFactory entryFactory,
            SpawnRequestContextFactory contextFactory,
            SpawnRequestMetadataFactory metadataFactory)
        {
            this.entryFactory = entryFactory
                ?? throw new ArgumentNullException(nameof(entryFactory));

            this.contextFactory = contextFactory
                ?? throw new ArgumentNullException(nameof(contextFactory));

            this.metadataFactory = metadataFactory
                ?? throw new ArgumentNullException(nameof(metadataFactory));
        }        

        public SpawnRequest Create(
            IEnumerable<SpawnRequestEntry> entries,
            RuntimeSpawnConfig runtimeSpawnConfig,
            SpawnRequestOrigin origin)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(
                    nameof(entries));
            }

            if (runtimeSpawnConfig == null)
            {
                throw new ArgumentNullException(
                    nameof(runtimeSpawnConfig));
            }

            return new SpawnRequest(
                entries,
                contextFactory.Create(
                    runtimeSpawnConfig),
                metadataFactory.Create(
                    origin));
        }
    }
}