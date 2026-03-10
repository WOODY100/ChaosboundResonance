using UnityEngine;

public class TorchFlicker : MonoBehaviour
{
    Light torchLight;

    public float baseIntensity = 2.2f;
    public float flickerAmount = 0.4f;
    public float flickerSpeed = 3f;

    float noiseOffset;

    void Awake()
    {
        torchLight = GetComponentInChildren<Light>();
    }

    void Start()
    {
        noiseOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (torchLight == null) return;

        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed + noiseOffset, 0f);
        torchLight.intensity = baseIntensity + (noise - 0.5f) * flickerAmount;
    }
}