using UnityEngine;

public sealed class SkillTreeEditorGraphView
{
    private const float DefaultZoom = 1f;
    private const float MinZoom = 0.25f;
    private const float MaxZoom = 2.5f;

    private float zoom = DefaultZoom;
    private Vector2 pan;

    public float Zoom
    {
        get { return zoom; }
    }

    public Vector2 Pan
    {
        get { return pan; }
    }

    public void SetZoom(float value)
    {
        zoom = Mathf.Clamp(
            value,
            MinZoom,
            MaxZoom);
    }

    public void SetPan(Vector2 value)
    {
        pan = value;
    }

    public Vector2 GraphToGUI(
        Vector2 graphPosition,
        Rect canvasRect)
    {
        Vector2 canvasCenter =
            canvasRect.center;

        Vector2 scaledPosition =
            graphPosition * zoom;

        return canvasCenter +
               pan +
               new Vector2(
                   scaledPosition.x,
                   -scaledPosition.y);
    }

    public Vector2 GUIToGraph(
        Vector2 guiPosition,
        Rect canvasRect)
    {
        Vector2 canvasCenter =
            canvasRect.center;

        Vector2 offset =
            guiPosition -
            canvasCenter -
            pan;

        return new Vector2(
            offset.x / zoom,
            -offset.y / zoom);
    }

    public void ZoomAt(
    float zoomFactor,
    Vector2 guiPosition,
    Rect canvasRect)
    {
        Vector2 graphPosition =
            GUIToGraph(
                guiPosition,
                canvasRect);

        float newZoom =
            Mathf.Clamp(
                zoom * zoomFactor,
                MinZoom,
                MaxZoom);

        if (Mathf.Approximately(
            newZoom,
            zoom))
        {
            return;
        }

        zoom = newZoom;

        Vector2 scaledGraphPosition =
            graphPosition * zoom;

        Vector2 guiOffset =
            new Vector2(
                scaledGraphPosition.x,
                -scaledGraphPosition.y);

        pan =
            guiPosition -
            canvasRect.center -
            guiOffset;
    }

    public void PanBy(Vector2 delta)
    {
        pan += delta;
    }

    public Vector2 SnapToGrid(
    Vector2 graphPosition,
    float gridSize)
    {
        if (gridSize <= 0f)
            return graphPosition;

        return new Vector2(
            Mathf.Round(graphPosition.x / gridSize) * gridSize,
            Mathf.Round(graphPosition.y / gridSize) * gridSize);
    }
}