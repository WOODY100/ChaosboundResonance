using UnityEngine;

namespace Chaosbound.Debugging
{
    /// <summary>
    /// Utility logger used by Spawn Runtime validation.
    /// </summary>
    public static class SpawnRuntimeLogger
    {
        private const string Prefix =
            "<color=#6BCBFF>[Spawn Runtime]</color>";

        public static void Step(string message)
        {
            Debug.Log($"{Prefix} {message}");
        }

        public static void Success(string message)
        {
            Debug.Log(
                $"<color=green>[Spawn Runtime]</color> {message}");
        }

        public static void Warning(string message)
        {
            Debug.LogWarning($"{Prefix} {message}");
        }

        public static void Error(string message)
        {
            Debug.LogError($"{Prefix} {message}");
        }
    }
}