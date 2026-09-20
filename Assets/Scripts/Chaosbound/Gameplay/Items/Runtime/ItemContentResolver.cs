using System;
using Chaosbound.Content.Items;

namespace Chaosbound.Gameplay.Items.Runtime
{
    public sealed class ItemContentResolver
    {
        private readonly ItemDatabase itemDatabase;
        private readonly ExpeditionRewardItemDatabase
            expeditionRewardItemDatabase;

        public ItemContentResolver(
            ItemDatabase itemDatabase,
            ExpeditionRewardItemDatabase expeditionRewardItemDatabase)
        {
            this.itemDatabase =
                itemDatabase ??
                throw new ArgumentNullException(
                    nameof(itemDatabase));

            this.expeditionRewardItemDatabase =
                expeditionRewardItemDatabase ??
                throw new ArgumentNullException(
                    nameof(expeditionRewardItemDatabase));
        }

        public bool TryResolve(
            string contentId,
            out ItemBaseData item)
        {
            item = null;

            if (string.IsNullOrWhiteSpace(contentId))
                return false;

            bool foundInItemDatabase =
                itemDatabase.TryGet(
                    contentId,
                    out ItemBaseData normalItem);

            bool foundInRewardDatabase =
                expeditionRewardItemDatabase.TryGet(
                    contentId,
                    out ItemBaseData rewardItem);

            if (foundInItemDatabase &&
                foundInRewardDatabase)
            {
                if (normalItem == rewardItem)
                {
                    item = normalItem;
                    return item != null;
                }

                return false;
            }

            if (foundInItemDatabase)
            {
                item = normalItem;
                return item != null;
            }

            if (foundInRewardDatabase)
            {
                item = rewardItem;
                return item != null;
            }

            return false;
        }

        public ItemBaseData Resolve(
            string contentId)
        {
            if (!TryResolve(
                    contentId,
                    out ItemBaseData item))
            {
                throw new InvalidOperationException(
                    $"Item with ContentId '{contentId}' " +
                    "could not be resolved uniquely.");
            }

            return item;
        }
    }
}