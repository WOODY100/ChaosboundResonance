using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Runtime.References
{
    /// <summary>
    /// Immutable runtime references exposed to gameplay systems.
    /// </summary>
    public sealed class RuntimeReferencesConfig
    {
        /// <summary>
        /// Gets the player transform.
        /// </summary>
        public Transform Player { get; }

        public RuntimeReferencesConfig(
            Transform player)
        {
            Player =
                player
                ?? throw new ArgumentNullException(nameof(player));
        }
    }
}