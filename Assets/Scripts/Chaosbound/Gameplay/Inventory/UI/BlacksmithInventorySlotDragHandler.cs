using Chaosbound.Gameplay.Items.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class BlacksmithInventorySlotDragHandler :
        MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [Header("References")]
        [SerializeField] private InventorySlotUI slotUI;
        [SerializeField] private Canvas rootCanvas;

        private GameObject dragVisual;

        private void Awake()
        {
            if (slotUI == null)
                slotUI = GetComponent<InventorySlotUI>();

            if (rootCanvas == null)
                rootCanvas = GetComponentInParent<Canvas>();
        }

        //==================================================
        // Drag
        //==================================================

        public void OnBeginDrag(
            PointerEventData eventData)
        {
            if (slotUI == null ||
                slotUI.CurrentItem == null)
            {
                return;
            }

            CreateDragVisual();

            if (dragVisual == null)
                return;

            UpdateDragVisual(eventData.position);
        }

        public void OnDrag(
            PointerEventData eventData)
        {
            if (dragVisual == null)
                return;

            UpdateDragVisual(eventData.position);
        }

        public void OnEndDrag(
            PointerEventData eventData)
        {
            DestroyDragVisual();
        }

        public ItemInstance DraggedItem
        {
            get
            {
                return slotUI != null
                    ? slotUI.CurrentItem
                    : null;
            }
        }

        //==================================================
        // Drag Visual
        //==================================================

        private void CreateDragVisual()
        {
            if (rootCanvas == null)
                return;

            Sprite icon =
                slotUI.GetCurrentItemIcon();

            if (icon == null)
                return;

            dragVisual = new GameObject(
                "BlacksmithDragVisual",
                typeof(RectTransform),
                typeof(CanvasGroup),
                typeof(Image));

            dragVisual.transform.SetParent(
                rootCanvas.transform,
                false);

            RectTransform rect =
                dragVisual.GetComponent<RectTransform>();

            rect.sizeDelta =
                new Vector2(80f, 80f);

            CanvasGroup canvasGroup =
                dragVisual.GetComponent<CanvasGroup>();

            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0.85f;

            Image dragImage =
                dragVisual.GetComponent<Image>();

            dragImage.sprite = icon;
            dragImage.preserveAspect = true;
            dragImage.raycastTarget = false;
        }

        private void UpdateDragVisual(
            Vector2 screenPosition)
        {
            if (dragVisual == null ||
                rootCanvas == null)
            {
                return;
            }

            RectTransform canvasRect =
                rootCanvas.transform as RectTransform;

            if (canvasRect == null)
                return;

            Camera eventCamera =
                GetEventCamera();

            if (!RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    screenPosition,
                    eventCamera,
                    out Vector2 localPoint))
            {
                return;
            }

            RectTransform dragRect =
                dragVisual.transform as RectTransform;

            if (dragRect != null)
                dragRect.localPosition = localPoint;
        }

        private Camera GetEventCamera()
        {
            if (rootCanvas == null)
                return null;

            if (rootCanvas.renderMode ==
                RenderMode.ScreenSpaceOverlay)
            {
                return null;
            }

            return rootCanvas.worldCamera;
        }

        private void DestroyDragVisual()
        {
            if (dragVisual != null)
                Destroy(dragVisual);

            dragVisual = null;
        }
    }
}