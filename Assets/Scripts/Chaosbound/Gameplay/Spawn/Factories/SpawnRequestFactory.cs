using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Reference.Models;
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
            SpawnRequestOrigin origin,
            SpawnSpatialOrigin? spatialOrigin)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(
                    nameof(entries));
            }

            return new SpawnRequest(
                entries,
                contextFactory.Create(spatialOrigin),
                metadataFactory.Create(
                    origin));
        }
    }
}