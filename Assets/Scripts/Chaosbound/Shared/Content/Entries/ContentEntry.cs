using System;
using Chaosbound.Shared.Identifiers;
using UnityEngine;

namespace Chaosbound.Shared.Content.Entries
{
    /// <summary>
    /// Associates a content identifier with its authored asset.
    /// Used as the transfer object between content discovery and
    /// runtime registry construction.
    /// </summary>
    public readonly struct ContentEntry
    {
        /// <summary>
        /// Gets the unique identifier of the content.
        /// </summary>
        public ContentId Id { get; }

        /// <summary>
        /// Gets the authored Unity asset.
        /// </summary>
        public UnityEngine.Object Asset { get; }

        /// <summary>
        /// Creates a new content entry.
        /// </summary>
        /// <param name="id">The content identifier.</param>
        /// <param name="asset">The associated Unity asset.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the identifier is empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the asset is null.
        /// </exception>
        public ContentEntry(
            ContentId id,
            UnityEngine.Object asset)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException(
                    "ContentEntry requires a valid ContentId.",
                    nameof(id));
            }

            Asset = asset ?? throw new ArgumentNullException(nameof(asset));
            Id = id;
        }
    }
}