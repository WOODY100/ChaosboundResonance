using System;
using Chaosbound.Content.Items;

namespace Chaosbound.Gameplay.Items.Runtime
{
    public sealed class ItemInstanceFactory
    {
        public ItemInstance Create(ItemBaseData itemData)
        {
            if (itemData == null)
                throw new ArgumentNullException(nameof(itemData));

            if (string.IsNullOrWhiteSpace(itemData.ContentId))
                throw new InvalidOperationException(
                    $"Cannot create ItemInstance from ItemBaseData " +
                    $"'{itemData.name}' because its ContentId is empty.");

            string instanceId =
                ItemInstanceIdGenerator.Generate();

            return new ItemInstance(
                instanceId,
                itemData.ContentId,
                itemData.BaseTier,
                0);
        }
    }
}