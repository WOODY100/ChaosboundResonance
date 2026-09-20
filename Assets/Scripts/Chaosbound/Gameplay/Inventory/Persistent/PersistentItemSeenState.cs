using System.Collections.Generic;

namespace Chaosbound.Gameplay.Inventory.Persistent
{
    public sealed class PersistentItemSeenState
    {
        private readonly HashSet<string> seenItemInstanceIds =
            new HashSet<string>();

        public bool IsSeen(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId))
                return false;

            return seenItemInstanceIds.Contains(
                instanceId);
        }

        public bool IsNew(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId))
                return false;

            return !seenItemInstanceIds.Contains(
                instanceId);
        }

        public void MarkSeen(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId))
                return;

            seenItemInstanceIds.Add(
                instanceId);
        }

        public IReadOnlyCollection<string> GetSeenIds()
        {
            return seenItemInstanceIds;
        }

        public void Clear()
        {
            seenItemInstanceIds.Clear();
        }
    }
}