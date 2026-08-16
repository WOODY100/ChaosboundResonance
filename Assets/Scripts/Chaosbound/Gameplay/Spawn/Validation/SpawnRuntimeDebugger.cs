using Chaosbound.Gameplay.Spawn.Domain;
using Chaosbound.Gameplay.Spawn.Placement.Models;
using Chaosbound.Gameplay.Spawn.Scheduling;
using System;
using UnityEngine;

namespace Chaosbound.Debugging
{
    /// <summary>
    /// Utility logger used by Spawn Runtime validation.
    /// </summary>
    public static class SpawnRuntimeDebugger
    {
        private const string Prefix =
            "<color=#6BCBFF>[Spawn Runtime]</color>";

        public static void Step(string message)
        {
            Debug.Log($"{Prefix} {message}");
        }

        /*public static void Success(string message)
        {
            Debug.Log(
                $"<color=green>[Spawn Runtime]</color> {message}");
        }*/

        public static void Warning(string message)
        {
            Debug.LogWarning($"{Prefix} {message}");
        }

        public static void Error(string message)
        {
            Debug.LogError($"{Prefix} {message}");
        }

        public static void LogPlacement(
            PlacementContext context,
            PlacementResolution resolution)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (resolution == null)
                throw new ArgumentNullException(nameof(resolution));

            if (resolution.IsSuccess)
            {
                SpawnPlacement placement =
                    resolution.Placement;

                /*Success(
                    $"[Placement] " +
                    $"Policy={context.Intent.PlacementPolicy} | " +
                    $"Position={placement.Position} | " +
                    $"Rotation={placement.Rotation.eulerAngles}");
                */
            }
            else
            {
                Warning(
                    $"[Placement] Failed | " +
                    $"Reason={resolution.FailureReason}");
            }
        }

        /// <summary>
        /// Logs scheduling information.
        /// </summary>
        public static void LogScheduling(
            SpawnSchedulingContext context,
            int scheduledTaskCount)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            /*Success(
                $"[Scheduling] " +
                $"Job={context.Job.Identity} | " +
                $"Policy={context.EnemyConfig.SchedulingPolicy} | " +
                $"Tasks={scheduledTaskCount}");
            */
        }
    }
}