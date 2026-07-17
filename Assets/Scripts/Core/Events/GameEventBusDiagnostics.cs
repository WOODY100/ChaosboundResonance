using System;
using UnityEngine;

namespace Chaosbound.Core.Events
{
    /// <summary>
    /// Centralized diagnostics and logging for the GameEventBus.
    /// Only active in the Editor and Development Builds.
    /// </summary>
    internal static class GameEventBusDiagnostics
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public static bool Enabled { get; set; } = true;
#else
        public static bool Enabled { get; set; } = false;
#endif

        /// <summary>
        /// Returns whether diagnostics are currently enabled.
        /// </summary>
        private static bool CanLog
        {
            get
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                return Enabled;
#else
        return false;
#endif
            }
        }

        /// <summary>
        /// Reports an exception thrown by an event subscriber.
        /// </summary>
        internal static void ReportSubscriberException(
            GameEventSubscriber subscriber,
            IGameEvent gameEvent,
            Exception exception)
        {
            if (!CanLog)
                return;
            Debug.LogError(
                $"An exception occurred while dispatching event '{subscriber.EventType.Name}' " +
                $"to subscriber '{subscriber.Owner?.GetType().Name ?? "Unknown"}'.");

            Debug.LogException(exception);
        }

        /// <summary>
        /// Reports a new subscription.
        /// </summary>
        internal static void LogSubscribe(
            GameEventSubscriber subscriber)
        {
            if (!CanLog)
                return;
        }

        /// <summary>
        /// Reports a subscription removal.
        /// </summary>
        internal static void LogUnsubscribe(
            long subscriptionId,
            Type eventType)
        {
            if (!CanLog)
                return;
        }

        /// <summary>
        /// Reports an event publication.
        /// </summary>
        internal static void LogPublish(
            IGameEvent gameEvent)
        {
            if (!CanLog)
                return;
        }

        /// <summary>
        /// Reports the creation of a registry.
        /// </summary>
        internal static void LogRegistryCreated(
            Type eventType)
        {
            if (!CanLog)
                return;
        }

        /// <summary>
        /// Reports the removal of a registry.
        /// </summary>
        internal static void LogRegistryRemoved(
            Type eventType)
        {
            if (!CanLog)
                return;
        }
    }
}