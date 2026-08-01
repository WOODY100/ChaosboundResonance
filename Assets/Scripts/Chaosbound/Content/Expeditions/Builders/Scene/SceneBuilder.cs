using System;
using Chaosbound.Content.Expeditions.Authoring.Scene;
using Chaosbound.Content.Expeditions.Definitions.Scene;

namespace Chaosbound.Content.Expeditions.Builders.Scene
{
    /// <summary>
    /// Converts authoring scene settings into their domain representation.
    /// </summary>
    public static class SceneBuilder
    {
        public static SceneDefinition Build(
            SceneAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            return new SceneDefinition(
                authoring.SceneName);
        }
    }
}