using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Events
{
    /// <summary>
    /// Stores and manages all subscribers for a single event type.
    /// </summary>
    internal sealed class GameEventRegistry<TEvent> : IGameEventRegistry
    where TEvent : IGameEvent
    {
        private readonly GameEventRegistryStatistics _statistics = new();

        private readonly List<GameEventSubscriber<TEvent>> _subscribers = new(8);

        private readonly Dictionary<long, GameEventSubscriber<TEvent>> _subscriberLookup = new(8);

        private readonly List<GameEventSubscriber<TEvent>> _publishBuffer = new(8);

        private int _publishDepth;

        public Type EventType => typeof(TEvent);

        public int ActiveSubscribers => _subscribers.Count;

        public int PeakSubscribers => _statistics.PeakSubscribers;

        public bool IsPublishing => _publishDepth > 0;

        public bool IsEmpty => _subscribers.Count == 0;

        public GameEventRegistryStatistics Statistics => _statistics;

        internal void Add(GameEventSubscriber<TEvent> subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));

            if (_subscriberLookup.ContainsKey(subscriber.SubscriptionId))
                return;

            _subscribers.Add(subscriber);
            _subscriberLookup.Add(subscriber.SubscriptionId, subscriber);

            _statistics.SubscribeCount++;
            _statistics.LastSubscribeUtc = DateTime.UtcNow;

            _statistics.PeakSubscribers =
                Math.Max(_statistics.PeakSubscribers, _subscribers.Count);
        }

        internal bool Remove(long subscriptionId)
        {
            if (!_subscriberLookup.TryGetValue(subscriptionId, out var subscriber))
                return false;

            _subscriberLookup.Remove(subscriptionId);
            _subscribers.Remove(subscriber);

            _statistics.UnsubscribeCount++;
            _statistics.LastUnsubscribeUtc = DateTime.UtcNow;

            return true;
        }

        internal void Publish(TEvent gameEvent)
        {

            _publishDepth++;

            try
            {
                _statistics.PublishCount++;
                _statistics.LastPublishUtc = DateTime.UtcNow;

                _publishBuffer.Clear();
                _publishBuffer.AddRange(_subscribers);

                foreach (var subscriber in _publishBuffer)
                {
                    if (!subscriber.Enabled)
                        continue;

                    try
                    {
                        subscriber.Invoke(gameEvent);
                    }
                    catch (Exception ex)
                    {
                        _statistics.ExceptionCount++;

                        GameEventBusDiagnostics.ReportSubscriberException(
                            subscriber,
                            gameEvent,
                            ex);
                    }
                }
            }
            finally
            {
                _publishDepth--;

                if (_publishDepth == 0)
                    _publishBuffer.Clear();
            }
        }

        bool IGameEventRegistry.Remove(long subscriptionId)
        {
            return Remove(subscriptionId);
        }

        int IGameEventRegistry.RemoveOwner(object owner)
        {
            if (owner == null)
                return 0;

            int removed = 0;

            for (int i = _subscribers.Count - 1; i >= 0; i--)
            {
                var subscriber = _subscribers[i];

                if (!ReferenceEquals(subscriber.Owner, owner))
                    continue;

                _subscriberLookup.Remove(subscriber.SubscriptionId);
                _subscribers.RemoveAt(i);
                
                _statistics.UnsubscribeCount++;
                _statistics.LastUnsubscribeUtc = DateTime.UtcNow;

                removed++;
            }

            return removed;
        }

        void IGameEventRegistry.Clear()
        {
            if (IsPublishing)
                throw new InvalidOperationException(
                    $"Cannot clear registry '{typeof(TEvent).Name}' while publishing.");

            _subscribers.Clear();
            _subscriberLookup.Clear();
            _publishBuffer.Clear();

            _statistics.Reset();
        }
    }
}