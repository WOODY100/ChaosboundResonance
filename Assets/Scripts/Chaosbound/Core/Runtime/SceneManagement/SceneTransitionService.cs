using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Chaosbound.UI.Loading;

namespace Chaosbound.Core.Runtime.SceneManagement
{
    /// <summary>
    /// Coordinates scene transitions.
    /// This is the only class allowed to interact with SceneManager.
    /// </summary>
    public sealed class SceneTransitionService
    {
        private readonly SceneTransitionHost host;
        private readonly LoadingScreenController loadingScreen;

        private bool isTransitioning;
        private bool sceneReady;

        public bool IsTransitioning => isTransitioning;

        public SceneTransitionService(
            SceneTransitionHost host,
            LoadingScreenController loadingScreen)
        {
            this.host = host;
            this.loadingScreen = loadingScreen;
        }

        public void LoadScene(GameScene scene)
        {
            LoadScene(scene, false);
        }

        /// <summary>
        /// Loads a scene with visual fades.
        /// Optionally waits for the destination to signal readiness.
        /// </summary>
        public void LoadScene(
            GameScene scene,
            bool waitForSceneReady)
        {
            if (isTransitioning)
            {
                Debug.LogWarning(
                    "[SceneTransitionService] A transition is already in progress.");

                return;
            }

            if (host == null || loadingScreen == null)
            {
                Debug.LogError(
                    "[SceneTransitionService] Host or LoadingScreenController is missing.");

                return;
            }

            string sceneName = SceneCatalog.GetSceneName(scene);

            isTransitioning = true;
            sceneReady = false;

            Coroutine coroutine = host.Run(
                TransitionRoutine(sceneName, waitForSceneReady));

            if (coroutine == null)
            {
                isTransitioning = false;
                loadingScreen.Hide();
            }
        }

        private IEnumerator TransitionRoutine(
            string sceneName,
            bool waitForSceneReady)
        {
            // Fade to the loading screen before changing scenes.
            yield return loadingScreen.FadeIn();

            float elapsed = 0f;
            float minimumTime = loadingScreen.MinimumLoadingTime;

            // Start loading immediately after the fade-in.
            AsyncOperation operation =
                SceneManager.LoadSceneAsync(sceneName);

            if (operation == null)
            {
                Debug.LogError(
                    "[SceneTransitionService] Failed to start loading scene: "
                    + sceneName);

                yield return loadingScreen.FadeOut();

                isTransitioning = false;
                yield break;
            }

            // Wait for both scene loading and the minimum loading duration.
            while (!operation.isDone || elapsed < minimumTime)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            // Some destinations need to finish their runtime initialization.
            if (waitForSceneReady)
            {
                while (!sceneReady)
                    yield return null;
            }

            // Reveal the destination scene gradually.
            yield return loadingScreen.FadeOut();

            isTransitioning = false;
        }

        /// <summary>
        /// Called by the destination scene after its required initialization.
        /// </summary>
        public void MarkSceneReady()
        {
            sceneReady = true;
        }
    }
}