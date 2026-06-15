using UnityEngine;

public class BoundsChecker : MonoBehaviour
{
    [ContextMenu("Print Bounds")]
    void PrintBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        Debug.Log($"Size: {bounds.size}");
        Debug.Log($"Center: {bounds.center}");
        Debug.Log($"Min: {bounds.min}");
        Debug.Log($"Max: {bounds.max}");
    }
}