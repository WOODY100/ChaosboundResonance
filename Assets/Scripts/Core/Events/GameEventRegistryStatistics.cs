using System;

namespace Chaosbound.Core.Events
{
    /// <summary>
    /// Runtime statistics collected for a single event registry.
    /// </summary>
    internal sealed class GameEventRegistryStatistics
    {
        /// <summary>
        /// Total number of published events.
        /// </summary>
        public long PublishCount { get; internal set; }

        /// <summary>
        /// Total number of subscriptions created.
        /// </summary>
        public long SubscribeCount { get; internal set; }

        /// <summary>
        /// Total number of subscriptions removed.
        /// </summary>
        public long UnsubscribeCount { get; internal set; }

        /// <summary>
        /// Total number of subscriber exceptions.
        /// </summary>
        public long ExceptionCount { get; internal set; }

        /// <summary>
        /// Highest number of simultaneous subscribers.
        /// </summary>
        public int PeakSubscribers { get; internal set; }

        /// <summary>
        /// Last time an event was published.
        /// </summary>
        public DateTime? LastPublishUtc { get; internal set; }

        /// <summary>
        /// Last time a subscriber was added.
        /// </summary>
        public DateTime? LastSubscribeUtc { get; internal set; }

        /// <summary>
        /// Last time a subscriber was removed.
        /// </summary>
        public DateTime? LastUnsubscribeUtc { get; internal set; }

        /// <summary>
        /// Restores all statistics to their initial state.
        /// </summary>
        internal void Reset()
        {
            PublishCount = 0;
            SubscribeCount = 0;
            UnsubscribeCount = 0;
            ExceptionCount = 0;

            PeakSubscribers = 0;

            LastPublishUtc = null;
            LastSubscribeUtc = null;
            LastUnsubscribeUtc = null;
        }
    }
}