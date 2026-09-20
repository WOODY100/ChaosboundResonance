using UnityEngine;

namespace Chaosbound.Gameplay.Items.UI.Tooltip
{
    public sealed class ItemTooltipPositioner : MonoBehaviour
    {
        [Header("Position")]
        [SerializeField] private Vector2 cursorOffset = new Vector2(20f, -20f);

        [Header("Screen Padding")]
        [SerializeField] private float screenPadding = 10f;

        private RectTransform rectTransform;
        private Canvas canvas;

        private void Awake()
        {
            rectTransform =
                GetComponent<RectTransform>();

            canvas =
                GetComponentInParent<Canvas>();

            if (rectTransform == null)
            {
                Debug.LogError(
                    "[ItemTooltipPositioner] " +
                    "RectTransform reference could not be resolved.",
                    this);
            }

            if (canvas == null)
            {
                Debug.LogError(
                    "[ItemTooltipPositioner] " +
                    "Parent Canvas could not be resolved.",
                    this);
            }
        }

        public void PositionAtScreenPoint(
            Vector2 screenPoint)
        {
            PositionAtScreenPoint(
                screenPoint,
                Vector2.zero);
        }

        public void PositionAtScreenPoint(
            Vector2 screenPoint,
            Vector2 additionalOffset)
        {
            if (rectTransform == null)
                return;

            if (canvas == null)
                return;

            Camera eventCamera =
                canvas.renderMode ==
                RenderMode.ScreenSpaceOverlay
                    ? null
                    : canvas.worldCamera;

            Vector2 targetPosition =
                screenPoint +
                cursorOffset +
                additionalOffset;

            RectTransform canvasRect =
                canvas.transform as RectTransform;

            if (canvasRect == null)
                return;

            Vector2 localPoint;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    targetPosition,
                    eventCamera,
                    out localPoint))
            {
                return;
            }

            Vector2 tooltipSize =
                rectTransform.rect.size;

            Vector2 canvasSize =
                canvasRect.rect.size;

            Vector2 pivot =
                rectTransform.pivot;

            float left =
                localPoint.x -
                tooltipSize.x * pivot.x;

            float right =
                localPoint.x +
                tooltipSize.x * (1f - pivot.x);

            float bottom =
                localPoint.y -
                tooltipSize.y * pivot.y;

            float top =
                localPoint.y +
                tooltipSize.y * (1f - pivot.y);

            if (left < -canvasSize.x * 0.5f +
                screenPadding)
            {
                localPoint.x +=
                    (-canvasSize.x * 0.5f +
                     screenPadding) -
                    left;
            }

            if (right > canvasSize.x * 0.5f -
                screenPadding)
            {
                localPoint.x -=
                    right -
                    (canvasSize.x * 0.5f -
                     screenPadding);
            }

            if (bottom < -canvasSize.y * 0.5f +
                screenPadding)
            {
                localPoint.y +=
                    (-canvasSize.y * 0.5f +
                     screenPadding) -
                    bottom;
            }

            if (top > canvasSize.y * 0.5f -
                screenPadding)
            {
                localPoint.y -=
                    top -
                    (canvasSize.y * 0.5f -
                     screenPadding);
            }

            rectTransform.localPosition =
                localPoint;
        }
    }
}