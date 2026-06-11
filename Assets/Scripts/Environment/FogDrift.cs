using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class FogDrift : MonoBehaviour
{
    [Header("Movement")]
    public Vector2 direction = new Vector2(1f, 1f);
    public float speed = 0.02f;

    [Header("Noise")]
    public float noiseStrength = 0.02f;
    public float noiseSpeed = 0.2f;

    [Header("Auto Tiling")]
    public float tileDensity = 0.1f;

    private Renderer rend;
    private MaterialPropertyBlock block;
    private float seed;

    private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

    void OnEnable()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
        seed = Random.Range(0f, 100f);

        ApplyAutoTiling();
    }

    void Update()
    {
        if (rend == null) return;

        Vector2 dir = direction.sqrMagnitude > 0.001f
            ? direction.normalized
            : Vector2.right;

        float time = Application.isPlaying ? Time.time : Time.realtimeSinceStartup;

        Vector2 baseOffset = dir * speed * time;

        float noiseX = Mathf.PerlinNoise(seed, time * noiseSpeed);
        float noiseY = Mathf.PerlinNoise(time * noiseSpeed, seed);

        Vector2 noise = new Vector2(
            noiseX - 0.5f,
            noiseY - 0.5f
        ) * noiseStrength;

        rend.GetPropertyBlock(block);
        block.SetVector("_BaseMap_ST", GetTextureST(baseOffset + noise));
        rend.SetPropertyBlock(block);
    }

    private Vector4 GetTextureST(Vector2 offset)
    {
        Vector3 scale = transform.localScale;

        float tileX = scale.x * tileDensity;
        float tileY = scale.z * tileDensity;

        return new Vector4(tileX, tileY, offset.x, offset.y);
    }

    private void ApplyAutoTiling()
    {
        if (rend == null) return;

        Vector3 scale = transform.localScale;

        float tileX = scale.x * tileDensity;
        float tileY = scale.z * tileDensity;

        rend.GetPropertyBlock(block);
        block.SetVector("_BaseMap_ST", new Vector4(tileX, tileY, 0f, 0f));
        rend.SetPropertyBlock(block);
    }

    void OnValidate()
    {
        if (!isActiveAndEnabled) return;

        rend = GetComponent<Renderer>();

        if (block == null)
            block = new MaterialPropertyBlock();

        ApplyAutoTiling();
    }
}