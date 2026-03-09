using UnityEngine;

public class FadeWall : MonoBehaviour
{
    private Renderer[] renderers;
    private MaterialPropertyBlock block;

    private float currentAlpha = 1f;
    private float targetAlpha = 1f;

    [SerializeField] private float fadeSpeed = 8f;
    [SerializeField] private float hiddenAlpha = 0.2f;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        block = new MaterialPropertyBlock();

        // Asegura que todos empiecen visibles
        foreach (var r in renderers)
        {
            r.GetPropertyBlock(block);
            block.SetFloat("_Alpha", 1f);
            r.SetPropertyBlock(block);
        }
    }

    void Update()
    {
        // interpolación suave
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);

        foreach (var r in renderers)
        {
            r.GetPropertyBlock(block);
            block.SetFloat("_Alpha", currentAlpha);
            r.SetPropertyBlock(block);
        }
    }

    public void Hide()
    {
        targetAlpha = hiddenAlpha;
    }

    public void Show()
    {
        targetAlpha = 1f;
    }
}