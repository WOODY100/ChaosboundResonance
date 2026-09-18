using Chaosbound.Content.Items;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Inventory.Persistent;
using Chaosbound.Gameplay.Items.Runtime;
using UnityEngine;
using System;

namespace Chaosbound.Gameplay.Items.UI.Tooltip
{
    public sealed class ItemTooltipService : MonoBehaviour
    {
        [Header("Tooltip")]
        [SerializeField] private ItemTooltipView tooltipView;

        [Header("Hover")]
        [SerializeField] private float hoverDelay = 0.5f;

        private ItemResolver resolver;

        public float HoverDelay => hoverDelay;

        public event Action<ItemInstance> OnItemSeen;

        public bool IsVisible
        {
            get
            {
                return tooltipView != null &&
                       tooltipView.IsVisible;
            }
        }

        public ItemInstance CurrentItem { get; private set; }

        public void Show(
            ItemInstance item,
            Vector2 screenPosition)
        {
            if (item == null)
            {
                Hide();
                return;
            }

            if (tooltipView == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "ItemTooltipView reference is missing.",
                    this);
                return;
            }

            if (resolver == null)
            {
                ResolveItemDatabase();

                if (resolver == null)
                {
                    Debug.LogError(
                        "[ItemTooltipService] " +
                        "ItemResolver is not available.",
                        this);
                    return;
                }
            }

            if (!resolver.TryResolve(
                    item.BaseDataId,
                    out ItemBaseData itemData))
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "Could not resolve ItemBaseData for ItemInstance: " +
                    item.InstanceId,
                    this);
                return;
            }

            CurrentItem = item;

            TooltipContent content =
                TooltipContentFactory.CreateItemContent(
                    item,
                    itemData);

            if (content == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "Could not create TooltipContent for ItemInstance: " +
                    item.InstanceId,
                    this);

                return;
            }

            tooltipView.Show(content);

            tooltipView.PositionAtScreenPoint(
                screenPosition);
        }

        public void Show(
            TooltipContent content,
            Vector2 screenPosition)
        {
            if (content == null)
            {
                Hide();
                return;
            }

            if (tooltipView == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "ItemTooltipView reference is missing.",
                    this);
                return;
            }

            tooltipView.Show(content);

            tooltipView.PositionAtScreenPoint(
                screenPosition);
        }

        public void Hide()
        {
            CurrentItem = null;

            if (tooltipView == null)
                return;

            tooltipView.Hide();
        }

        public void CancelPendingShow()
        {
            // Hover timing will be implemented
            // by the hover interaction layer.
        }

        private void Awake()
        {
            if (hoverDelay < 0f)
            {
                Debug.LogWarning(
                    "[ItemTooltipService] " +
                    "Hover Delay cannot be negative. " +
                    "Clamping to zero.",
                    this);

                hoverDelay = 0f;
            }

            if (tooltipView == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "ItemTooltipView reference is missing.",
                    this);
            }
        }

        private void Start()
        {
            ResolveItemDatabase();
        }

        private void ResolveItemDatabase()
        {
            if (resolver != null)
                return;

            GameContentContext contentContext =
                GameContentContext.Current;

            if (contentContext == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "GameContentContext is not available.",
                    this);
                return;
            }

            if (contentContext.ItemDatabase == null)
            {
                Debug.LogError(
                    "[ItemTooltipService] " +
                    "ItemDatabase is not available in " +
                    "GameContentContext.",
                    this);
                return;
            }

            resolver =
                new ItemResolver(
                    contentContext.ItemDatabase);
        }
    }
}