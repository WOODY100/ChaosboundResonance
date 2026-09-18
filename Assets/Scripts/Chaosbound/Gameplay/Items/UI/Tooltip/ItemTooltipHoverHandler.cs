using Chaosbound.Core.Composition;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Chaosbound.Gameplay.Items.UI.Tooltip
{
    public sealed class ItemTooltipHoverHandler :
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        private ITooltipSource tooltipSource;
        private ItemTooltipService tooltipService;
        private Coroutine hoverRoutine;

        private void Awake()
        {
            tooltipSource =
                GetComponent<ITooltipSource>();

            if (tooltipSource == null)
            {
                Debug.LogError(
                    "[ItemTooltipHoverHandler] " +
                    "ITooltipSource could not be resolved.",
                    this);
            }
        }

        private void Start()
        {
            ResolveTooltipService();
        }

        public void OnPointerEnter(
            PointerEventData eventData)
        {
            if (tooltipSource == null)
                return;

            if (tooltipSource.GetTooltipContent() == null)
                return;

            ResolveTooltipService();

            if (tooltipService == null)
                return;

            CancelHoverRoutine();

            hoverRoutine =
                StartCoroutine(
                    ShowAfterDelay());
        }

        public void OnPointerExit(
            PointerEventData eventData)
        {
            CancelHoverRoutine();

            if (tooltipService == null)
                ResolveTooltipService();

            if (tooltipService == null)
                return;

            tooltipService.Hide();
        }

        private IEnumerator ShowAfterDelay()
        {
            float delay =
                Mathf.Max(
                    0f,
                    tooltipService.HoverDelay);

            if (delay > 0f)
            {
                yield return new WaitForSecondsRealtime(
                    delay);
            }

            if (tooltipSource == null)
            {
                hoverRoutine = null;
                yield break;
            }

            TooltipContent content =
                tooltipSource.GetTooltipContent();

            if (content == null)
            {
                hoverRoutine = null;
                yield break;
            }

            Vector2 screenPosition =
                Mouse.current != null
                    ? Mouse.current.position.ReadValue()
                    : Vector2.zero;

            IItemTooltipSource itemTooltipSource =
                tooltipSource as IItemTooltipSource;

            if (itemTooltipSource != null &&
                itemTooltipSource.CurrentItem != null)
            {
                tooltipService.Show(
                    itemTooltipSource.CurrentItem,
                    screenPosition);
            }
            else
            {
                tooltipService.Show(
                    content,
                    screenPosition);
            }

            ITooltipSeenSource seenSource =
                tooltipSource as ITooltipSeenSource;

            if (seenSource != null)
            {
                seenSource.MarkAsSeen();
            }

            hoverRoutine = null;
        }

        private void CancelHoverRoutine()
        {
            if (hoverRoutine == null)
                return;

            StopCoroutine(
                hoverRoutine);

            hoverRoutine = null;
        }

        private void ResolveTooltipService()
        {
            if (tooltipService != null)
                return;

            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            tooltipService =
                bootstrapContext.ItemTooltipService;
        }

        private void OnDisable()
        {
            CancelHoverRoutine();

            if (tooltipService != null)
                tooltipService.Hide();
        }
    }
}