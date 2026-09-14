using System;
using Chaosbound.Content.Items;

namespace Chaosbound.Gameplay.Items.Runtime
{
    [Serializable]
    public sealed class ItemInstance
    {
        public string InstanceId { get; }

        public string BaseDataId { get; }

        public ItemTier CurrentTier { get; private set; }

        public int UpgradeLevel { get; private set; }

        public ItemInstance(
            string instanceId,
            string baseDataId,
            ItemTier currentTier,
            int upgradeLevel = 0)
        {
            if (string.IsNullOrWhiteSpace(instanceId))
                throw new ArgumentException(
                    "InstanceId cannot be null, empty, or whitespace.",
                    nameof(instanceId));

            if (string.IsNullOrWhiteSpace(baseDataId))
                throw new ArgumentException(
                    "BaseDataId cannot be null, empty, or whitespace.",
                    nameof(baseDataId));

            if (upgradeLevel < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(upgradeLevel));

            InstanceId = instanceId.Trim();
            BaseDataId = baseDataId.Trim();
            CurrentTier = currentTier;
            UpgradeLevel = upgradeLevel;
        }
    }
}