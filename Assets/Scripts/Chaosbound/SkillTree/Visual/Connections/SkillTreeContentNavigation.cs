using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RectTransform))]
public sealed class SkillTreeContentNavigation : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    private RectTransform viewport;

    [Header("Pan")]

    [SerializeField]
    private float panSpeed = 1f;

    [Header("Zoom")]

    [SerializeField]
    private float minZoom = 0.5f;

    [SerializeField]
    private float maxZoom = 2.5f;

    [SerializeField]
    private float zoomSpeed = 0.1f;

    private RectTransform content;

    private bool isPanning;

    private Vector2 lastMousePosition;

    private void Awake()
    {
        content = GetComponent<RectTransform>();

        if (content == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeContentNavigation)}] " +
                "A RectTransform is required.",
                this);
        }
    }

    private void Update()
    {
        if (content == null)
            return;

        Mouse mouse = Mouse.current;

        if (mouse == null)
            return;

        if (viewport != null &&
            !RectTransformUtility.RectangleContainsScreenPoint(
                viewport,
                mouse.position.ReadValue()))
        {
            isPanning = false;
            return;
        }

        HandlePan(mouse);
        HandleZoom(mouse);
    }

    private void HandlePan(Mouse mouse)
    {
        if (mouse.middleButton.wasPressedThisFrame)
        {
            isPanning = true;
            lastMousePosition =
                mouse.position.ReadValue();
        }

        if (mouse.middleButton.wasReleasedThisFrame)
        {
            isPanning = false;
        }

        if (!isPanning)
            return;

        Vector2 currentMousePosition =
            mouse.position.ReadValue();

        Vector2 screenDelta =
            currentMousePosition -
            lastMousePosition;

        content.anchoredPosition +=
            screenDelta * panSpeed;

        lastMousePosition =
            currentMousePosition;
    }

    private void HandleZoom(Mouse mouse)
    {
        Vector2 scroll =
            mouse.scroll.ReadValue();

        if (Mathf.Abs(scroll.y) <= 0.0001f)
            return;

        float currentZoom =
            content.localScale.x;

        float zoomDelta =
            scroll.y * zoomSpeed;

        SetZoomAtScreenPosition(
            currentZoom + zoomDelta,
            mouse.position.ReadValue());
    }

    public void ZoomIn()
    {
        SetZoom(
            content.localScale.x +
            zoomSpeed);
    }

    public void ZoomOut()
    {
        SetZoom(
            content.localScale.x -
            zoomSpeed);
    }

    public void ResetView()
    {
        content.localScale =
            new Vector3(
                1f,
                1f,
                1f);

        content.anchoredPosition =
            Vector2.zero;
    }

    private void SetZoom(float targetZoom)
    {
        targetZoom =
            Mathf.Clamp(
                targetZoom,
                minZoom,
                maxZoom);

        content.localScale =
            new Vector3(
                targetZoom,
                targetZoom,
                1f);
    }

    private void SetZoomAtScreenPosition(
        float targetZoom,
        Vector2 screenPosition)
    {
        float currentZoom =
            content.localScale.x;

        targetZoom =
            Mathf.Clamp(
                targetZoom,
                minZoom,
                maxZoom);

        if (Mathf.Approximately(
            currentZoom,
            targetZoom))
        {
            return;
        }

        Vector2 localPoint;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            content,
            screenPosition,
            null,
            out localPoint))
        {
            return;
        }

        float scaleDelta =
            currentZoom -
            targetZoom;

        content.localScale =
            new Vector3(
                targetZoom,
                targetZoom,
                1f);

        content.anchoredPosition +=
            localPoint * scaleDelta;
    }
}