#if UNITY_EDITOR
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chaosbound.Development.Analytics.Editor
{
    /// <summary>
    /// Observes scene lifetime from the Editor without adding dependencies
    /// to Bootstrap, GameFlow, RunManager or Expedition gameplay systems.
    /// </summary>
    [InitializeOnLoad]
    internal static class ExpeditionAnalyticsEditorService
    {
        private const string ExpeditionSceneName = "Expedition";

        private static bool expeditionSceneLoaded;
        private static int lastKnownLevel = 1;

        static ExpeditionAnalyticsEditorService()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged += HandlePlayModeChanged;
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == ExpeditionSceneName)
            {
                expeditionSceneLoaded = true;
                lastKnownLevel = 1;

                if (!ExpeditionTelemetryCollector.IsRecording)
                {
                    ExpeditionTelemetryCollector.StartAutomaticRecording(
                        GetCurrentLevel(),
                        scene.name);
                }

                return;
            }

            if (expeditionSceneLoaded || ExpeditionTelemetryCollector.IsRecording)
            {
                // The Expedition player may already be destroyed here, or a
                // Sanctuary player may already be discoverable at level 1.
                // Preserve the last level observed while Expedition was active.
                ExpeditionTelemetryCollector.StopAutomaticRecording(
                    lastKnownLevel,
                    scene.name);
            }

            expeditionSceneLoaded = false;
        }

        private static void Tick()
        {
            if (!EditorApplication.isPlaying ||
                !expeditionSceneLoaded ||
                !ExpeditionTelemetryCollector.IsRecording)
                return;

            PlayerExperienceSystem experience =
                Object.FindObjectOfType<PlayerExperienceSystem>();

            if (experience != null)
                lastKnownLevel = Mathf.Max(1, experience.CurrentLevel);

            RunManager runManager = RunManager.Instance;
            if (runManager == null)
                return;

            ExpeditionRuntimeState runtimeState =
                runManager.ExpeditionRuntimeState;

            if (runtimeState == null)
                return;

            ExpeditionTelemetryCollector.UpdateExpeditionClock(
                (float)runtimeState.ElapsedTime.TotalSeconds,
                lastKnownLevel);
        }

        private static int GetCurrentLevel()
        {
            PlayerExperienceSystem experience =
                Object.FindObjectOfType<PlayerExperienceSystem>();

            if (experience != null)
                lastKnownLevel = Mathf.Max(1, experience.CurrentLevel);

            ExpeditionTelemetryReport report =
                ExpeditionTelemetryCollector.CurrentReport;

            return experience != null
                ? experience.CurrentLevel
                : (report != null ? report.finalLevel : lastKnownLevel);
        }

        private static void HandlePlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                expeditionSceneLoaded =
                    SceneManager.GetActiveScene().name == ExpeditionSceneName;

                if (expeditionSceneLoaded &&
                    !ExpeditionTelemetryCollector.IsRecording)
                {
                    ExpeditionTelemetryCollector.StartAutomaticRecording(
                        GetCurrentLevel(),
                        ExpeditionSceneName);
                }

                return;
            }

            if (state != PlayModeStateChange.ExitingPlayMode)
                return;

            if (ExpeditionTelemetryCollector.IsRecording)
            {
                ExpeditionTelemetryCollector.StopAutomaticRecording(
                    lastKnownLevel,
                    "PlayModeExit");
            }

            expeditionSceneLoaded = false;
        }
    }
}
#endif
