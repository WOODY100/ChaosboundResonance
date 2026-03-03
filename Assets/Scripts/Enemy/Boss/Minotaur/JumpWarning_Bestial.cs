using UnityEngine;

public class JumpWarning_Bestial : MonoBehaviour
{
    [SerializeField] private Transform symbol;
    [SerializeField] private Transform ring;
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float ringPulseStrength = 0.15f;
    [SerializeField] private float symbolRotationSpeed = 30f;

    private float timer;
    private float rotation;
    private float intensityMultiplier = 1f;

    void Update()
    {
        timer += Time.deltaTime;

        // 🩸 Pulsación del anillo
        float pulse = 1f + Mathf.Sin(timer * pulseSpeed) * ringPulseStrength;
        ring.localScale = Vector3.one * pulse;

        // 🌀 Rotación base
        rotation -= symbolRotationSpeed * Time.deltaTime;

        // 🐂 Wobble bestial (muy sutil)
        float wobble = Mathf.Sin(timer * 3f) * 2f;

        // 🔥 Aplicar TODO en una sola rotación controlada
        symbol.localRotation = Quaternion.Euler(
            90f,
            wobble,
            rotation
        );
    }

    public void SetIntensity(float value)
    {
        intensityMultiplier = value;
    }

}