using System;

namespace Chaosbound.Core.Events
{
    /// <summary>
    /// Represents a public handle to an active GameEventBus subscription.
    /// Disposing the subscription safely unregisters it from the EventBus.
    /// </summary>
    public sealed class GameEventSubscription : IDisposable
    {
        private readonly Action _disposeAction;

        public long SubscriptionId { get; }

        public Type EventType { get; }

        public object Owner { get; }

        public bool IsDisposed { get; private set; }

        internal GameEventSubscription(
            long subscriptionId,
            Type eventType,
            object owner,
            Action disposeAction)
        {
            SubscriptionId = subscriptionId;
            EventType = eventType;
            Owner = owner;
            _disposeAction = disposeAction ?? throw new ArgumentNullException(nameof(disposeAction));
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            _disposeAction.Invoke();
        }
    }
}