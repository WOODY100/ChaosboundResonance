using System;
using Chaosbound.Shared.Content.Registry;
using Chaosbound.Shared.Identifiers;
using UnityEngine;

namespace Chaosbound.Shared.Content.Resolution
{
    /// <summary>
    /// Default implementation of <see cref="IContentResolver"/>.
    /// Resolves registered Unity assets by their content identifier.
    /// </summary>
    public sealed class UnityContentResolver : IContentResolver
    {
        private readonly IContentRegistry registry;

        public UnityContentResolver(
            IContentRegistry registry)
        {
            this.registry = registry
                ?? throw new ArgumentNullException(nameof(registry));
        }

        public T Resolve<T>(
            ContentId id)
            where T : UnityEngine.Object
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException(
                    "ContentId cannot be empty.",
                    nameof(id));
            }

            if (!registry.TryGetAsset(id, out UnityEngine.Object asset))
            {
                throw new InvalidOperationException(
                    $"No content is registered with ContentId '{id}'.");
            }

            if (asset is not T typedAsset)
            {
                throw new InvalidOperationException(
                    $"Content '{id}' is of type '{asset.GetType().Name}' but was requested as '{typeof(T).Name}'.");
            }

            return typedAsset;
        }

        public bool TryResolve<T>(
            ContentId id,
            out T asset)
            where T : UnityEngine.Object
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException(
                    "ContentId cannot be empty.",
                    nameof(id));
            }

            if (id.IsEmpty)
            {
                asset = null;
                return false;
            }

            asset = null;

            if (!registry.TryGetAsset(id, out UnityEngine.Object resolvedAsset))
            {
                return false;
            }

            if (resolvedAsset is not T typedAsset)
            {
                return false;
            }

            asset = typedAsset;
            return true;
        }
    }
}