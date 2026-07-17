using System;

namespace Chaosbound.Core.Events
{
    /// <summary>
    /// Non-generic contract implemented by all event registries.
    /// Allows GameEventBus to manage registries without knowing
    /// their concrete event type.
    /// </summary>
    internal interface IGameEventRegistry
    {
        Type EventType { get; }

        GameEventRegistryStatistics Statistics { get; }

        int ActiveSubscribers { get; }

        bool IsPublishing { get; }

        bool IsEmpty { get; }

        bool Remove(long subscriptionId);

        int RemoveOwner(object owner);

        void Clear();
    }
}