using UnityEngine;

public class JumpWarning_Bestial : PooledBehaviour
{
    [SerializeField] private Transform symbol;
    [SerializeField] private Transform ring;
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float ringPulseStrength = 0.15f;
    [SerializeField] private float symbolRotationSpeed = 30f;

    private float timer;
    private float rotation;
    private float intensityMultiplier = 1f;

    protected override void ResetPooledState()
    {
        timer = 0f;
        rotation = 0f;
        intensityMultiplier = 1f;

        if (ring != null)
            ring.localScale = Vector3.one;

        if (symbol != null)
            symbol.localRotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (ring != null)
        {
            float pulse = 1f + Mathf.Sin(timer * pulseSpeed) * ringPulseStrength * intensityMultiplier;
            ring.localScale = Vector3.one * pulse;
        }

        if (symbol != null)
        {
            rotation -= symbolRotationSpeed * Time.deltaTime;

            float wobble = Mathf.Sin(timer * 3f) * 2f * intensityMultiplier;

            symbol.localRotation = Quaternion.Euler(
                90f,
                wobble,
                rotation
            );
        }
    }

    public void SetIntensity(float value)
    {
        intensityMultiplier = value;
    }
}