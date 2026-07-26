using System;

namespace Chaosbound.Content.Expeditions.Definitions.Scene
{
    /// <summary>
    /// Represents the immutable scene configuration of an expedition.
    /// </summary>
    public sealed class SceneDefinition
    {
        /// <summary>
        /// Gets the name of the scene associated with the expedition.
        /// </summary>
        public string SceneName { get; }

        public SceneDefinition(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
                throw new ArgumentException(
                    "Scene name cannot be null or empty.",
                    nameof(sceneName));

            SceneName = sceneName;
        }
    }
}