using UnityEngine;

public class FloatingPickup : MonoBehaviour
{
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float rotationSpeed = 90f;

    [Header("Visual Pulse")]
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float pulseScaleAmount = 0.1f;

    private Vector3 startPos;
    private Vector3 baseScale;
    private bool isDisabled;

    void Start()
    {
        startPos = transform.position;
        baseScale = transform.localScale;
    }

    void Update()
    {
        if (isDisabled)
            return;

        // FLOAT
        transform.position = startPos + Vector3.up *
            Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        // ROTATION
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // 🔥 SCALE PULSE (CLAVE VISUAL)
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseScaleAmount + 1f;
        transform.localScale = baseScale * pulse;
    }

    public void DisableFloating()
    {
        isDisabled = true;
    }
}