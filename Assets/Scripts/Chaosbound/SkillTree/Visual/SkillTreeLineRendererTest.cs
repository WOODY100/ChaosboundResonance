using SimpleTalentTreeUi;
using UnityEngine;
using UnityEngine.UI;

public sealed class SkillTreeLineRendererTest : MonoBehaviour
{
    private LineRendererUI lineRenderer;

    private void Awake()
    {
        lineRenderer =
            GetComponent<LineRendererUI>();

        if (lineRenderer == null)
        {
            Debug.LogError(
                "[SkillTreeLineRendererTest] " +
                "LineRendererUI not found.",
                this);

            return;
        }

        lineRenderer.color =
            Color.red;

        lineRenderer.raycastTarget =
            false;

        lineRenderer.relativeSize =
            true;

        lineRenderer.lineList =
            false;

        lineRenderer.lineCaps =
            false;

        lineRenderer.bezierMode =
            BezierType.None;

        lineRenderer.lineThickness =
            15f;

        lineRenderer.Points =
            new Vector2[]
            {
                new Vector2(0f, 0.5f),
                new Vector2(1f, 0.5f)
            };

        lineRenderer.SetAllDirty();
    }
}