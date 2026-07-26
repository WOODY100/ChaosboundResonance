using System;

namespace Chaosbound.Content.Expeditions.Runtime.Scene
{
    /// <summary>
    /// Runtime configuration for the expedition scene.
    /// </summary>
    public sealed class RuntimeSceneConfig
    {
        /// <summary>
        /// Name of the Unity scene that will be loaded for this expedition.
        /// </summary>
        public string SceneName { get; }

        /// <summary>
        /// Creates a runtime scene configuration.
        /// </summary>
        /// <param name="sceneName">
        /// Unity scene name.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if the scene name is null or empty.
        /// </exception>
        public RuntimeSceneConfig(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
                throw new ArgumentException(
                    "Scene name cannot be null or empty.",
                    nameof(sceneName));

            SceneName = sceneName;
        }
    }
}