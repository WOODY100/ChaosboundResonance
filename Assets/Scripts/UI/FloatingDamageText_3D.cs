using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    [Header("Motion")]
    [SerializeField] private float floatSpeed = 2.5f;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float fadeStartTime = 0.5f;

    [Header("Scale Punch")]
    [SerializeField] private float appearScaleMultiplier = 1.4f;
    [SerializeField] private float scaleRecoverSpeed = 10f;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(1f, 0.9f, 0.4f);
    [SerializeField] private Color criticalColor = new Color(1f, 0.3f, 0.1f);

    private TextMeshPro textMesh;
    private float timer;
    private Transform cameraTransform;

    private Vector3 initialScale;
    private Color currentColor;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        cameraTransform = Camera.main.transform;
        initialScale = transform.localScale;
    }

    public void Initialize(float damage, bool isCritical)
    {
        textMesh.text = Mathf.RoundToInt(damage).ToString();

        // Color
        currentColor = isCritical ? criticalColor : normalColor;
        textMesh.color = currentColor;

        // Scale punch
        transform.localScale = initialScale * appearScaleMultiplier;

        // Pequeña variación lateral
        transform.position += new Vector3(
            Random.Range(-0.25f, 0.25f),
            0f,
            Random.Range(-0.25f, 0.25f)
        );
    }

    private void Update()
    {
        // Movimiento vertical
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Siempre mirar a cámara
        if (cameraTransform != null)
            transform.forward = cameraTransform.forward;

        // Recuperar escala suavemente
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            initialScale,
            scaleRecoverSpeed * Time.deltaTime
        );

        timer += Time.deltaTime;

        // Fade
        if (timer >= fadeStartTime)
        {
            float fadeProgress =
                (timer - fadeStartTime) / (lifetime - fadeStartTime);

            Color c = currentColor;
            c.a = Mathf.Lerp(1f, 0f, fadeProgress);
            textMesh.color = c;
        }

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}