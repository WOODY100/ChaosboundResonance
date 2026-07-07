using UnityEngine;

public class ChargeGhostFade : PooledBehaviour
{
    [SerializeField] private float lifetime = 0.1f;

    private Renderer rend;
    private MaterialPropertyBlock propertyBlock;
    private Color baseColor;
    private float timer;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    protected override void Awake()
    {
        base.Awake();

        rend = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        if (rend != null && rend.sharedMaterial != null)
            baseColor = rend.sharedMaterial.GetColor(BaseColorId);
    }

    protected override void ResetPooledState()
    {
        timer = 0f;

        if (rend != null)
        {
            propertyBlock.Clear();
            propertyBlock.SetColor(BaseColorId, baseColor * 0.3f);
            rend.SetPropertyBlock(propertyBlock);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float t = timer / lifetime;
        float intensity = Mathf.Lerp(0.3f, 0f, t * t);

        if (rend != null)
        {
            propertyBlock.Clear();
            propertyBlock.SetColor(BaseColorId, baseColor * intensity);
            rend.SetPropertyBlock(propertyBlock);
        }

        if (timer >= lifetime)
            ReturnToPool();
    }
}