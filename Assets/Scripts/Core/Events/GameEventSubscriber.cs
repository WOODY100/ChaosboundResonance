using System;

namespace Chaosbound.Core.Events
{
    /// <summary>
    /// Base class for all event subscribers.
    /// Stores common subscription metadata.
    /// </summary>
    internal abstract class GameEventSubscriber
    {
        public long SubscriptionId { get; }

        public abstract Type EventType { get; }

        public object Owner { get; }

        public bool Enabled { get; private set; } = true;

        internal void SetEnabled(bool enabled)
        {
            if (Enabled == enabled)
                return;

            Enabled = enabled;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD

        public DateTime SubscribeTime { get; }

        public int SubscribeFrame { get; }

        public string DebugName { get; }

        public string CallbackMethod { get; }

#endif

        protected GameEventSubscriber(
            long subscriptionId,
            object owner = null,
            string debugName = null,
            string callbackMethod = null,
            int subscribeFrame = 0)
        {
            SubscriptionId = subscriptionId;
            Owner = owner;

#if UNITY_EDITOR || DEVELOPMENT_BUILD

            SubscribeTime = DateTime.UtcNow;
            SubscribeFrame = subscribeFrame;
            DebugName = debugName ?? owner?.GetType().Name ?? "Unknown";
            CallbackMethod = callbackMethod ?? "Unknown";

#endif
        }
    }
}