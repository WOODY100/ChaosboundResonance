using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Items
{
    [CreateAssetMenu(
        menuName = "Chaosbound/Items/Item Database",
        fileName = "ItemDatabase")]
    public sealed class ItemDatabase : ScriptableObject
    {
        [SerializeField]
        private List<ItemBaseData> items = new();

        private Dictionary<string, ItemBaseData> itemsById;

        public IReadOnlyList<ItemBaseData> Items =>
            items;

        public bool TryGet(
            string contentId,
            out ItemBaseData item)
        {
            item = null;

            if (string.IsNullOrWhiteSpace(contentId))
                return false;

            BuildLookupIfNeeded();

            return itemsById.TryGetValue(
                contentId.Trim(),
                out item);
        }

        private void BuildLookupIfNeeded()
        {
            if (itemsById != null)
                return;

            itemsById =
                new Dictionary<string, ItemBaseData>(
                    System.StringComparer.Ordinal);

            for (int i = 0; i < items.Count; i++)
            {
                ItemBaseData item = items[i];

                if (item == null)
                    continue;

                string contentId = item.ContentId;

                if (string.IsNullOrWhiteSpace(contentId))
                    continue;

                if (itemsById.ContainsKey(contentId))
                {
                    Debug.LogError(
                        $"Duplicate Item ContentId '{contentId}' " +
                        $"found in database '{name}'.",
                        this);

                    continue;
                }

                itemsById.Add(contentId, item);
            }
        }

        private void OnValidate()
        {
            itemsById = null;

            HashSet<string> ids =
                new HashSet<string>(
                    System.StringComparer.Ordinal);

            for (int i = 0; i < items.Count; i++)
            {
                ItemBaseData item = items[i];

                if (item == null)
                    continue;

                string contentId = item.ContentId;

                if (string.IsNullOrWhiteSpace(contentId))
                    continue;

                if (!ids.Add(contentId))
                {
                    Debug.LogError(
                        $"Duplicate Item ContentId '{contentId}' " +
                        $"found in database '{name}'.",
                        this);
                }
            }
        }
    }
}