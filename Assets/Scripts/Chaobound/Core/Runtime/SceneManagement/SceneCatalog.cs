using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Runtime.SceneManagement
{
    /// <summary>
    /// Resolves logical game scenes to Unity scene names.
    /// </summary>
    public static class SceneCatalog
    {
        private static readonly Dictionary<GameScene, string> SceneNames =
            new()
            {
                { GameScene.Bootstrap, "Bootstrap" },
                { GameScene.MainMenu, "MainMenu" },
                { GameScene.Sanctuary, "Sanctuary" },
                { GameScene.Expedition, "Expedition" }
            };

        public static string GetSceneName(GameScene scene)
        {
            if (SceneNames.TryGetValue(scene, out string sceneName))
            {
                return sceneName;
            }

            throw new InvalidOperationException(
                $"Scene '{scene}' is not registered in {nameof(SceneCatalog)}.");
        }
    }
}