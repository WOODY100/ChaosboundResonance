using UnityEngine;

public class FogDriftLocal : MonoBehaviour
{
    [Header("Drift")]
    public float speedX = 0.2f;
    public float speedY = 0.15f;

    [Header("Pulse")]
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.1f;

    [Header("Light Pulse")]
    public float lightPulseAmount = 0.5f;

    private Renderer rend;
    private Vector2 offset;
    private Vector3 baseScale;

    private Light lightComp;
    private float baseIntensity;

    private float randomOffset; // 🔥 clave

    void Start()
    {
        rend = GetComponent<Renderer>();
        baseScale = transform.localScale;

        lightComp = GetComponentInChildren<Light>();

        if (lightComp != null)
        {
            baseIntensity = lightComp.intensity;
        }

        // 🔥 cada instancia empieza en un tiempo diferente
        randomOffset = Random.Range(0f, 100f);
        speedX += Random.Range(-0.05f, 0.05f);
        speedY += Random.Range(-0.05f, 0.05f);
        pulseSpeed += Random.Range(-0.3f, 0.3f);
    }

    void Update()
    {
        float time = Time.time + randomOffset; // 🔥 clave

        // 🌫 Movimiento orgánico
        offset.x = Mathf.Sin(time * speedX) * 0.05f;
        offset.y = Mathf.Cos(time * speedY) * 0.05f;
        rend.material.SetTextureOffset("_BaseMap", offset); // ⚠️ CAMBIO AQUÍ

        // 🔥 Pulso (escala)
        float pulse = 1 + Mathf.Sin(time * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * pulse;

        // 🔥 Pulso de luz
        if (lightComp != null)
        {
            lightComp.intensity = baseIntensity + Mathf.Sin(time * pulseSpeed) * lightPulseAmount;
        }
    }
}