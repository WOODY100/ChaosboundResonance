using System;
using Chaosbound.Content.Items;

namespace Chaosbound.Gameplay.Items.Runtime
{
    public sealed class ItemResolver
    {
        private readonly ItemDatabase database;

        public ItemResolver(ItemDatabase database)
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
            if (!TryResolve(contentId, out ItemBaseData item))
            {
                throw new InvalidOperationException(
                    $"Item with ContentId '{contentId}' " +
                    "could not be resolved.");
            }

            return item;
        }
    }
}