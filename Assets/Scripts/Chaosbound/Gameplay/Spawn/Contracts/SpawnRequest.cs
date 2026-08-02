using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Chaosbound.Gameplay.Spawn.Contracts
{
    /// <summary>
    /// Represents a declarative materialization request produced by any
    /// gameplay system.
    ///
    /// A SpawnRequest describes WHAT should exist in the world.
    /// It never specifies HOW the request will be executed.
    /// </summary>
    public sealed class SpawnRequest
    {
        private readonly IReadOnlyList<SpawnRequestEntry> m_Entries;

        /// <summary>
        /// Creates a new spawn request.
        /// </summary>
        /// <param name="entries">
        /// The requested materialization entries.
        /// </param>
        /// <param name="context">
        /// Global execution context.
        /// </param>
        /// <param name="metadata">
        /// Metadata describing the producer.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any argument is null.
        /// </exception>
        public SpawnRequest(
            IEnumerable<SpawnRequestEntry> entries,
            SpawnRequestContext context,
            SpawnRequestMetadata metadata)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            Context = context
                ?? throw new ArgumentNullException(nameof(context));

            Metadata = metadata
                ?? throw new ArgumentNullException(nameof(metadata));

            List<SpawnRequestEntry> list = entries.ToList();

            m_Entries =
                new ReadOnlyCollection<SpawnRequestEntry>(list);
        }

        /// <summary>
        /// Gets the requested materialization entries.
        /// </summary>
        public IReadOnlyList<SpawnRequestEntry> Entries
            => m_Entries;

        /// <summary>
        /// Gets the execution context.
        /// </summary>
        public SpawnRequestContext Context
        {
            get;
        }

        /// <summary>
        /// Gets request metadata.
        /// </summary>
        public SpawnRequestMetadata Metadata
        {
            get;
        }

        /// <summary>
        /// Gets whether the request contains no entries.
        /// </summary>
        public bool IsEmpty
            => m_Entries.Count == 0;

        /// <summary>
        /// Gets the number of requested entries.
        /// </summary>
        public int EntryCount
            => m_Entries.Count;

        /// <summary>
        /// Gets the total requested quantity.
        /// </summary>
        public int TotalQuantity
            => m_Entries.Sum(entry => entry.Quantity);
    }
}