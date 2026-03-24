using UnityEngine;

public class FogDrift : MonoBehaviour
{
    [Header("Movement")]
    public Vector2 direction = new Vector2(1f, 1f);
    public float speed = 0.02f;

    [Header("Noise")]
    public float noiseStrength = 0.02f;
    public float noiseSpeed = 0.2f;

    [Header("Auto Tiling")]
    public float tileDensity = 0.1f; // 🔥 controla tamaño del patrón

    private Material mat;
    private float seed;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        mat = rend.material;

        seed = Random.Range(0f, 100f);
        direction = direction.normalized;

        ApplyAutoTiling();
    }

    void Update()
    {
        float time = Time.time;

        // 🔥 Movimiento base
        Vector2 baseOffset = direction * speed * time;

        // 🌪 Noise
        float noiseX = Mathf.PerlinNoise(seed, time * noiseSpeed);
        float noiseY = Mathf.PerlinNoise(time * noiseSpeed, seed);

        Vector2 noise = new Vector2(
            (noiseX - 0.5f),
            (noiseY - 0.5f)
        ) * noiseStrength;

        // 🎯 Offset final
        mat.SetTextureOffset("_BaseMap", baseOffset + noise);
    }

    void ApplyAutoTiling()
    {
        Vector3 scale = transform.localScale;

        // ⚠️ usamos X y Z porque es un plano horizontal
        float tileX = scale.x * tileDensity;
        float tileY = scale.z * tileDensity;

        mat.SetTextureScale("_BaseMap", new Vector2(tileX, tileY));
    }
}