using UnityEngine;

public class TorchFlicker : MonoBehaviour
{
    public Light torchLight;
    public float baseIntensity = 4f;
    public float flickerAmount = 0.5f;
    public float flickerSpeed = 3f;

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
        torchLight.intensity = baseIntensity + (noise - 0.5f) * flickerAmount;
    }
}