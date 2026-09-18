using System.Collections.Generic;

namespace Chaosbound.Gameplay.Inventory.Persistent
{
    public sealed class PersistentMaterialSeenState
    {
        private readonly HashSet<string> seenMaterialIds =
            new HashSet<string>();

        public bool IsSeen(string materialId)
        {
            if (string.IsNullOrEmpty(materialId))
                return false;

            return seenMaterialIds.Contains(
                materialId);
        }

        public bool IsNew(string materialId)
        {
            if (string.IsNullOrEmpty(materialId))
                return false;

            return !seenMaterialIds.Contains(
                materialId);
        }

        public void MarkSeen(string materialId)
        {
            if (string.IsNullOrEmpty(materialId))
                return;

            seenMaterialIds.Add(
                materialId);
        }

        public void Clear()
        {
            seenMaterialIds.Clear();
        }
    }
}