using System;
using Chaosbound.Content.Items;

namespace Chaosbound.Gameplay.Items.Runtime
{
    public sealed class ItemResolver
    {
        private readonly ItemDatabase database;
        private readonly ItemContentResolver contentResolver;

        public ItemResolver(ItemDatabase database)
        {
            this.database =
                database ??
                throw new ArgumentNullException(
                    nameof(database));
        }

        public ItemResolver(
            ItemContentResolver contentResolver)
        {
            this.contentResolver =
                contentResolver ??
                throw new ArgumentNullException(
                    nameof(contentResolver));
        }

        public bool TryResolve(
            string contentId,
            out ItemBaseData item)
        {
            if (contentResolver != null)
            {
                return contentResolver.TryResolve(
                    contentId,
                    out item);
            }

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
                    $"Item with ContentId '{contentId}' " +
                    "could not be resolved.");
            }

            return item;
        }
    }
}