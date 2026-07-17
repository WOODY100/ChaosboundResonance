using System;

namespace Chaosbound.Core.Events
{
    /// <summary>
    /// Strongly typed event subscriber.
    /// </summary>
    internal sealed class GameEventSubscriber<TEvent> : GameEventSubscriber
        where TEvent : IGameEvent
    {
        private readonly Action<TEvent> _callback;

        public override Type EventType => typeof(TEvent);

        internal GameEventSubscriber(
            long subscriptionId,
            Action<TEvent> callback,
            object owner = null,
            string debugName = null,
            int subscribeFrame = 0)
            : base(
                subscriptionId,
                owner,
                debugName,
                callback?.Method.Name,
                subscribeFrame)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        internal void Invoke(TEvent gameEvent)
        {
            _callback(gameEvent);
        }
    }
}