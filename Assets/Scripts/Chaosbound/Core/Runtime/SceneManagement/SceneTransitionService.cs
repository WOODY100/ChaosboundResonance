using UnityEngine.SceneManagement;

namespace Chaosbound.Core.Runtime.SceneManagement
{
    /// <summary>
    /// Handles scene transitions.
    /// This is the only class allowed to interact with Unity's SceneManager.
    /// </summary>
    public sealed class SceneTransitionService
    {
        public void LoadScene(GameScene scene)
        {
            string sceneName = SceneCatalog.GetSceneName(scene);

            SceneManager.LoadScene(sceneName);
        }
    }
}