using System;
using Chaosbound.Content.Items;

namespace Chaosbound.Gameplay.Items.Runtime
{
    public sealed class ExpeditionRewardItemResolver
    {
        private readonly ExpeditionRewardItemDatabase database;

        public ExpeditionRewardItemResolver(
            ExpeditionRewardItemDatabase database)
        {
            this.database =
                database ??
                throw new ArgumentNullException(nameof(database));
        }

        public bool TryResolve(
            string contentId,
            out ItemBaseData item)
        {
            return database.TryGet(
                contentId,
                out item);
        }

        public ItemBaseData Resolve(
            string contentId)
        {
            if (!TryResolve(
                    contentId,
                    out ItemBaseData item))
            {
                throw new InvalidOperationException(
                    $"Expedition Reward Item with ContentId " +
                    $"'{contentId}' could not be resolved.");
            }

            return item;
        }
    }
}