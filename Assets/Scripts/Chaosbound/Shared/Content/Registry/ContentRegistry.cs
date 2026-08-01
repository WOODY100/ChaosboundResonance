using System;
using System.Collections.Generic;
using Chaosbound.Shared.Identifiers;
using UnityEngine;

namespace Chaosbound.Shared.Content.Registry
{
    /// <summary>
    /// Immutable runtime registry that stores every authored content asset
    /// available during gameplay.
    /// </summary>
    public sealed class ContentRegistry : IContentRegistry
    {
        private readonly Dictionary<ContentId, UnityEngine.Object> assets;

        /// <summary>
        /// Creates a new immutable content registry.
        /// </summary>
        /// <param name="assets">
        /// Collection of resolved content assets indexed by their ContentId.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the asset collection is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the collection contains invalid identifiers or null assets.
        /// </exception>
        public ContentRegistry(
            IReadOnlyDictionary<ContentId, UnityEngine.Object> assets)
        {
            if (assets == null)
                throw new ArgumentNullException(nameof(assets));

            this.assets = new Dictionary<ContentId, UnityEngine.Object>(assets.Count);

            foreach (KeyValuePair<ContentId, UnityEngine.Object> pair in assets)
            {
                if (pair.Key.IsEmpty)
                {
                    throw new ArgumentException(
                        "ContentRegistry cannot contain an empty ContentId.",
                        nameof(assets));
                }

                if (pair.Value == null)
                {
                    throw new ArgumentException(
                        $"Content '{pair.Key}' is null.",
                        nameof(assets));
                }

                this.assets.Add(
                    pair.Key,
                    pair.Value);
            }
        }

        /// <inheritdoc/>
        public bool TryGetAsset(
            ContentId id,
            out UnityEngine.Object asset)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException(
                    "ContentId cannot be empty.",
                    nameof(id));
            }

            return assets.TryGetValue(
                id,
                out asset);
        }
    }
}