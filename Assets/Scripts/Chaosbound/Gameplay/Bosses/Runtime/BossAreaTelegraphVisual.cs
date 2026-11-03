using UnityEngine;

/// <summary>
/// Pooled visual warning for a Boss area attack.
/// Its visual footprint is scaled to the configured damage radius.
/// The owning attack execution controls its lifetime.
/// </summary>
[RequireComponent(typeof(PooledObject))]
public sealed class BossAreaTelegraphVisual : MonoBehaviour
{
    [SerializeField] private Transform visualRoot;
    [SerializeField, Min(0f)] private float pulseAmplitude = 0.04f;
    [SerializeField, Min(0f)] private float pulseFrequency = 3f;
    [SerializeField] private Transform rotatingSymbol;
    [SerializeField] private float symbolRotationSpeed = 45f;

    private Vector3 initialVisualScale;
    private float baseDiameter;
    private float elapsed;
    private bool initialized;

    private void Awake()
    {
        Transform target = visualRoot != null ? visualRoot : transform;
        initialVisualScale = target.localScale;
    }

    private void OnEnable()
    {
        elapsed = 0f;
        initialized = false;
    }

    public bool Initialize(float radius)
    {
        if (radius <= 0f)
            return false;

        Transform target = visualRoot != null ? visualRoot : transform;

        if (initialVisualScale == Vector3.zero)
            initialVisualScale = target.localScale;

        baseDiameter = radius * 2f;
        target.localScale = new Vector3(
            initialVisualScale.x * baseDiameter,
            initialVisualScale.y,
            initialVisualScale.z * baseDiameter);

        elapsed = 0f;
        initialized = true;
        return true;
    }

    private void Update()
    {
        if (!initialized)
            return;

        elapsed += Time.deltaTime;

        if (rotatingSymbol != null && Mathf.Abs(symbolRotationSpeed) > 0.001f)
        {
            rotatingSymbol.Rotate(
                Vector3.up,
                symbolRotationSpeed * Time.deltaTime,
                Space.World);
        }

        if (visualRoot != null && pulseAmplitude > 0f && pulseFrequency > 0f)
        {
            float pulse = 1f +
                Mathf.Sin(elapsed * pulseFrequency * Mathf.PI * 2f) * pulseAmplitude;

            visualRoot.localScale = new Vector3(
                initialVisualScale.x * baseDiameter * pulse,
                initialVisualScale.y,
                initialVisualScale.z * baseDiameter * pulse);
        }
    }
}
