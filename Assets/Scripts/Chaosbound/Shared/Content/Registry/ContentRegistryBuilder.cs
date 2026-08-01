using System;
using System.Collections.Generic;
using Chaosbound.Shared.Content.Entries;
using Chaosbound.Shared.Identifiers;
using UnityEngine;

namespace Chaosbound.Shared.Content.Registry
{
    /// <summary>
    /// Builds immutable runtime content registries from discovered content entries.
    /// </summary>
    public sealed class ContentRegistryBuilder
    {
        /// <summary>
        /// Builds a fully validated immutable content registry.
        /// </summary>
        /// <param name="entries">
        /// Collection of discovered content entries.
        /// </param>
        /// <returns>
        /// An immutable runtime content registry.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the collection is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when invalid or duplicate entries are detected.
        /// </exception>
        public ContentRegistry Build(
            IReadOnlyCollection<Entries.ContentEntry> entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            Dictionary<ContentId, UnityEngine.Object> assets =
                new(entries.Count);

            foreach (Entries.ContentEntry entry in entries)
            {
                // Defensive validation.
                if (entry.Id.IsEmpty)
                {
                    throw new ArgumentException(
                        "ContentEntry contains an empty ContentId.",
                        nameof(entries));
                }

                if (entry.Asset == null)
                {
                    throw new ArgumentException(
                        $"Content '{entry.Id}' contains a null asset.",
                        nameof(entries));
                }

                if (!assets.TryAdd(entry.Id, entry.Asset))
                {
                    throw new ArgumentException(
                        $"Duplicate ContentId detected: '{entry.Id}'.",
                        nameof(entries));
                }
            }

            return new ContentRegistry(assets);
        }
    }
}