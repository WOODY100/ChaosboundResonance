using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Runtime
{
    /// <summary>
    /// Unity-facing driver for the MinimapRuntime.
    ///
    /// Provides the player's current world position
    /// to the minimap runtime every frame.
    /// </summary>
    public sealed class MinimapRuntimeUpdater :
        MonoBehaviour
    {
        private MinimapRuntime minimapRuntime;
        private Transform playerTransform;

        private bool initialized;

        public void Initialize(
            MinimapRuntime runtime,
            Transform playerTransform)
        {
            minimapRuntime =
                runtime
                ?? throw new ArgumentNullException(
                    nameof(runtime));

            this.playerTransform =
                playerTransform
                ?? throw new ArgumentNullException(
                    nameof(playerTransform));

            initialized = true;
        }

        private void Update()
        {
            if (!initialized)
                return;

            if (playerTransform == null)
                return;

            minimapRuntime.Update(
                playerTransform.position);
        }

        public void Clear()
        {
            minimapRuntime = null;
            playerTransform = null;
            initialized = false;
        }
    }
}