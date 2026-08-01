using Chaosbound.Shared.Identifiers;
using UnityEngine;

namespace Chaosbound.Shared.Content.Resolution
{
    /// <summary>
    /// Resolves registered content assets by their content identifier.
    /// </summary>
    public interface IContentResolver
    {
        /// <summary>
        /// Resolves the specified content asset.
        /// </summary>
        /// <typeparam name="T">
        /// The expected Unity asset type.
        /// </typeparam>
        /// <param name="id">
        /// The content identifier.
        /// </param>
        /// <returns>
        /// The resolved asset.
        /// </returns>
        T Resolve<T>(ContentId id)
            where T : Object;

        /// <summary>
        /// Attempts to resolve the specified content asset.
        /// </summary>
        /// <typeparam name="T">
        /// The expected Unity asset type.
        /// </typeparam>
        /// <param name="id">
        /// The content identifier.
        /// </param>
        /// <param name="asset">
        /// When this method returns, contains the resolved asset if successful;
        /// otherwise, null.
        /// </param>
        /// <returns>
        /// True if the asset was successfully resolved; otherwise, false.
        /// </returns>
        bool TryResolve<T>(
            ContentId id,
            out T asset)
            where T : Object;
    }
}