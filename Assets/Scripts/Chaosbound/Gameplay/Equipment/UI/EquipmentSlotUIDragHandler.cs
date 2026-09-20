using Chaosbound.Gameplay.Inventory.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Equipment.UI
{
    public sealed class EquipmentSlotUIDragHandler :
        MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [Header("References")]
        [SerializeField] private EquipmentSlotUI slotUI;
        [SerializeField] private Canvas rootCanvas;

        private GameObject dragVisual;
        private Image dragImage;

        private void Awake()
        {
            if (slotUI == null)
                slotUI = GetComponent<EquipmentSlotUI>();

            if (rootCanvas == null)
                rootCanvas = GetComponentInParent<Canvas>();
        }

        public void OnBeginDrag(
            PointerEventData eventData)
        {
            if (slotUI == null)
                return;

            if (slotUI.CurrentItem == null)
                return;

            CreateDragVisual();
            UpdateDragVisual(eventData.position);

            InventoryUIDragController controller =
                InventoryUIDragController.Instance;

            if (controller != null)
            {
                controller.UpdateDropFeedback(
                    slotUI,
                    eventData);
            }
        }

        public void OnDrag(
            PointerEventData eventData)
        {
            if (dragVisual == null)
                return;

            UpdateDragVisual(eventData.position);

            InventoryUIDragController controller =
                InventoryUIDragController.Instance;

            if (controller != null)
            {
                controller.UpdateDropFeedback(
                    slotUI,
                    eventData);
            }
        }

        public void OnEndDrag(
            PointerEventData eventData)
        {
            InventoryUIDragController controller =
                InventoryUIDragController.Instance;

            if (controller != null)
            {
                controller.HandleDrop(
                    slotUI,
                    eventData);
            }

            DestroyDragVisual();
        }

        private void CreateDragVisual()
        {
            if (rootCanvas == null)
                return;

            Sprite icon =
                GetCurrentItemIcon();

            if (icon == null)
                return;

            dragVisual =
                new GameObject(
                    "EquipmentDragVisual",
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

            dragImage =
                dragVisual.GetComponent<Image>();

            dragImage.sprite = icon;
            dragImage.preserveAspect = true;
            dragImage.raycastTarget = false;
        }

        private void UpdateDragVisual(
            Vector2 screenPosition)
        {
            if (dragVisual == null)
                return;

            RectTransform canvasRect =
                rootCanvas.transform as RectTransform;

            if (canvasRect == null)
                return;

            Camera eventCamera =
                GetEventCamera();

            Vector2 localPoint;

            if (RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    screenPosition,
                    eventCamera,
                    out localPoint))
            {
                RectTransform dragRect =
                    dragVisual.transform as RectTransform;

                if (dragRect != null)
                {
                    dragRect.localPosition =
                        localPoint;
                }
            }
        }

        private Sprite GetCurrentItemIcon()
        {
            if (slotUI == null)
                return null;

            return slotUI.GetCurrentItemIcon();
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
            dragImage = null;
        }
    }
}