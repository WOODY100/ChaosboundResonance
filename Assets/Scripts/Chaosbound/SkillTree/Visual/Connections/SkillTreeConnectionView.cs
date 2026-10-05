using SimpleTalentTreeUi;
using UnityEngine;

[RequireComponent(typeof(CanvasRenderer))]
public sealed class SkillTreeConnectionView : MonoBehaviour
{
    private SkillTreeConnectionDescriptor descriptor;

    private SkillTreeNodeView sourceNode;

    private SkillTreeNodeView targetNode;

    private LineRendererUI lineRenderer;

    private RectTransform rectTransform;

    private Material lockedMaterial;

    private Material availableMaterial;

    private Material purchasedMaterial;

    private SkillTreeVisualState visualState;

    public SkillTreeConnectionDescriptor Descriptor
    {
        get { return descriptor; }
    }

    public SkillTreeNodeView SourceNode
    {
        get { return sourceNode; }
    }

    public SkillTreeNodeView TargetNode
    {
        get { return targetNode; }
    }

    public SkillTreeVisualState VisualState
    {
        get { return visualState; }
    }

    private void Awake()
    {
        rectTransform =
            GetComponent<RectTransform>();

        lineRenderer =
            GetComponent<LineRendererUI>();

        if (lineRenderer != null)
        {
            lineRenderer.raycastTarget = false;
        }

        if (rectTransform == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeConnectionView)}] " +
                $"'{name}' requires a RectTransform.",
                this);
        }

        if (lineRenderer == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeConnectionView)}] " +
                $"'{name}' requires a LineRendererUI.",
                this);
        }
    }

    public void Initialize(
        SkillTreeConnectionDescriptor connection,
        SkillTreeNodeView source,
        SkillTreeNodeView target)
    {
        descriptor = connection;
        sourceNode = source;
        targetNode = target;

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRendererUI>();

        if (lineRenderer != null)
        {
            lineRenderer.raycastTarget = false;
        }

        Refresh();
    }

    public void ConfigureMaterials(
    Material locked,
    Material available,
    Material purchased)
    {
        lockedMaterial = locked;
        availableMaterial = available;
        purchasedMaterial = purchased;
    }

    public void SetVisualState(
        SkillTreeVisualState state)
    {
        visualState = state;

        Material material = null;

        switch (state)
        {
            case SkillTreeVisualState.Locked:
                material = lockedMaterial;
                break;

            case SkillTreeVisualState.Available:
                material = availableMaterial;
                break;

            case SkillTreeVisualState.Purchased:
                material = purchasedMaterial;
                break;
        }

        SetMaterial(material);
    }

    public void SetMaterial(Material material)
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRendererUI>();

        if (lineRenderer == null)
            return;

        lineRenderer.material = material;
    }

    public bool TryGetConnectionPoints(
        out Vector2 sourcePoint,
        out Vector2 targetPoint)
    {
        sourcePoint = Vector2.zero;
        targetPoint = Vector2.zero;

        if (sourceNode == null ||
            targetNode == null)
        {
            return false;
        }

        if (sourceNode.RectTransform == null ||
            targetNode.RectTransform == null)
        {
            return false;
        }

        Vector2 direction =
            targetNode.RectTransform.anchoredPosition -
            sourceNode.RectTransform.anchoredPosition;

        if (direction.sqrMagnitude <= 0.000001f)
            return false;

        sourcePoint =
            sourceNode.GetConnectionPoint(
                direction);

        targetPoint =
            targetNode.GetConnectionPoint(
                -direction);

        return true;
    }

    public void Refresh()
    {
        if (lineRenderer == null)
            return;

        if (rectTransform == null)
            rectTransform =
                GetComponent<RectTransform>();

        if (rectTransform == null)
            return;

        if (!TryGetConnectionPoints(
            out Vector2 sourcePoint,
            out Vector2 targetPoint))
        {
            lineRenderer.Points = null;
            lineRenderer.SetVerticesDirty();
            return;
        }

        Vector3 sourceWorld =
            sourceNode.RectTransform.TransformPoint(
                sourcePoint);

        Vector3 targetWorld =
            targetNode.RectTransform.TransformPoint(
                targetPoint);

        Vector3 sourceLocal =
            rectTransform.InverseTransformPoint(
                sourceWorld);

        Vector3 targetLocal =
            rectTransform.InverseTransformPoint(
                targetWorld);

        Vector2 sourceNormalized =
            LocalToNormalized(
                sourceLocal);

        Vector2 targetNormalized =
            LocalToNormalized(
                targetLocal);

        lineRenderer.relativeSize = true;
        lineRenderer.lineThickness = 20f;
        lineRenderer.lineList = false;
        lineRenderer.lineCaps = false;
        lineRenderer.bezierMode =
            BezierType.None;

        lineRenderer.Points =
            new Vector2[]
            {
                sourceNormalized,
                targetNormalized
            };

        lineRenderer.SetVerticesDirty();
    }

    private Vector2 LocalToNormalized(
        Vector3 localPosition)
    {
        Rect rect =
            rectTransform.rect;

        if (rect.width <= 0.000001f ||
            rect.height <= 0.000001f)
        {
            return new Vector2(
                0.5f,
                0.5f);
        }

        float normalizedX =
            (localPosition.x - rect.x) /
            rect.width;

        float normalizedY =
            (localPosition.y - rect.y) /
            rect.height;

        return new Vector2(
            normalizedX,
            normalizedY);
    }

    public void RefreshVisualStateFromTarget()
    {
        if (targetNode == null)
            return;

        SetVisualState(
            targetNode.VisualState);
    }
}