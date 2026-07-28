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
        /// Builds a runtime content registry.
        /// </summary>
        /// <param name="entries">
        /// The discovered content entries.
        /// </param>
        /// <returns>
        /// A fully validated immutable content registry.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the collection is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when duplicate ContentIds are detected.
        /// </exception>
        public ContentRegistry Build(
            IReadOnlyCollection<ContentEntry> entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            Dictionary<ContentId, UnityEngine.Object> assets =
                new Dictionary<ContentId, UnityEngine.Object>(entries.Count);

            foreach (ContentEntry entry in entries)
            {
                if (assets.ContainsKey(entry.Id))
                {
                    throw new ArgumentException(
                        $"Duplicate ContentId detected: '{entry.Id}'.",
                        nameof(entries));
                }

                assets.Add(
                    entry.Id,
                    entry.Asset);
            }

            return new ContentRegistry(assets);
        }
    }
}