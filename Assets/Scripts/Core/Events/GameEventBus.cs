using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Core.Events
{
    /// <summary>
    /// Central dispatcher responsible for coordinating all event registries.
    /// </summary>
    public static class GameEventBus
    {
        private static readonly Dictionary<Type, IGameEventRegistry> _registries = new();

        private static readonly List<Type> _registryCleanupBuffer = new(8);

        private static long _nextSubscriptionId = 1;

        /// <summary>
        /// Resets the EventBus when Unity initializes the runtime.
        /// Prevents static state from surviving when Domain Reload is disabled.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeRuntime()
        {
            Reset();
        }

        /// <summary>
        /// Generates the next unique subscription identifier.
        /// </summary>
        private static long NextSubscriptionId()
        {
            return _nextSubscriptionId++;
        }

        /// <summary>
        /// Gets an existing registry for the event type or creates a new one.
        /// </summary>
        private static GameEventRegistry<TEvent> GetOrCreateRegistry<TEvent>()
            where TEvent : IGameEvent
        {
            var eventType = typeof(TEvent);

            if (_registries.TryGetValue(eventType, out var registry))
                return (GameEventRegistry<TEvent>)registry;

            var newRegistry = new GameEventRegistry<TEvent>();

            _registries.Add(eventType, newRegistry);

            GameEventBusDiagnostics.LogRegistryCreated(eventType);

            return newRegistry;
        }

        /// <summary>
        /// Gets the registry for the specified event type.
        /// Returns null if no registry exists.
        /// </summary>
        private static GameEventRegistry<TEvent> GetRegistry<TEvent>()
            where TEvent : IGameEvent
        {
            var eventType = typeof(TEvent);

            return _registries.TryGetValue(eventType, out var registry)
                ? (GameEventRegistry<TEvent>)registry
                : null;
        }

        /// <summary>
        /// Subscribes a callback to the specified event type.
        /// </summary>
        public static GameEventSubscription Subscribe<TEvent>(
    Action<TEvent> callback,
    object owner = null)
    where TEvent : IGameEvent
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var registry = GetOrCreateRegistry<TEvent>();

            long subscriptionId = NextSubscriptionId();

            var subscriber = new GameEventSubscriber<TEvent>(
                subscriptionId,
                callback,
                owner);

            GameEventBusDiagnostics.LogSubscribe(subscriber);

            return new GameEventSubscription(
                subscriptionId,
                typeof(TEvent),
                owner,
                () => Unsubscribe(subscriptionId, typeof(TEvent)));
        }

        /// <summary>
        /// Unsubscribes a subscription from the specified event type.
        /// </summary>
        private static bool Unsubscribe(
    long subscriptionId,
    Type eventType)
        {
            if (!_registries.TryGetValue(eventType, out var registry))
                return false;

            bool removed = registry.Remove(subscriptionId);

            if (removed)
            {
                GameEventBusDiagnostics.LogUnsubscribe(
                    subscriptionId,
                    eventType);
            }

            if (!removed)
                return false;

            if (registry.IsEmpty)
                RemoveEmptyRegistries();

            return true;
        }

        private static void RemoveEmptyRegistries()
        {
            _registryCleanupBuffer.Clear();

            foreach (var pair in _registries)
            {
                if (pair.Value.IsEmpty)
                    _registryCleanupBuffer.Add(pair.Key);
            }

            foreach (var eventType in _registryCleanupBuffer)
            {
                _registries.Remove(eventType);

                GameEventBusDiagnostics.LogRegistryRemoved(eventType);
            }

            _registryCleanupBuffer.Clear();
        }

        /// <summary>
        /// Publishes an event to all subscribers of the specified event type.
        /// </summary>
        public static void Publish<TEvent>(TEvent gameEvent)
            where TEvent : IGameEvent
        {
            if (gameEvent == null)
                throw new ArgumentNullException(nameof(gameEvent));

            var registry = GetRegistry<TEvent>();

            if (registry == null)
                return;

            GameEventBusDiagnostics.LogPublish(gameEvent);
        }

        /// <summary>
        /// Removes every subscription owned by the specified object.
        /// </summary>
        public static int ClearOwner(object owner)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            int removed = 0;

            foreach (var registry in _registries.Values)
            {
                removed += registry.RemoveOwner(owner);
            }

            RemoveEmptyRegistries();

            return removed;
        }

        /// <summary>
        /// Removes every registry and every active subscription.
        /// </summary>
        public static void ClearAll()
        {
            Reset();
        }

        /// <summary>
        /// Clears all registries and restores the EventBus to its initial state.
        /// </summary>
        internal static void Reset()
        {
            foreach (var registry in _registries.Values)
            {
                registry.Clear();
            }

            _registries.Clear();
            _registryCleanupBuffer.Clear();

            _nextSubscriptionId = 1;
        }
    }
}