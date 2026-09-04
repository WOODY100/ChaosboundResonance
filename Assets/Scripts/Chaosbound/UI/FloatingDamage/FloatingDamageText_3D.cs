using UnityEngine;
using TMPro;

public class FloatingDamageText : PooledBehaviour
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
    private Transform cameraTransform;

    private float timer;
    private Vector3 initialScale;
    private Color currentColor;

    protected override void Awake()
    {
        base.Awake();

        textMesh = GetComponent<TextMeshPro>();
        initialScale = transform.localScale;

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    protected override void ResetPooledState()
    {
        timer = 0f;
        transform.localScale = initialScale;

        if (textMesh != null)
        {
            Color c = textMesh.color;
            c.a = 1f;
            textMesh.color = c;
        }

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    public void Initialize(float damage, bool isCritical)
    {
        ResetPooledState();

        textMesh.text = Mathf.RoundToInt(damage).ToString();

        currentColor = isCritical ? criticalColor : normalColor;
        currentColor.a = 1f;
        textMesh.color = currentColor;

        transform.localScale = initialScale * appearScaleMultiplier;

        transform.position += new Vector3(
            Random.Range(-0.25f, 0.25f),
            0f,
            Random.Range(-0.25f, 0.25f)
        );
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (cameraTransform != null)
            transform.forward = cameraTransform.forward;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            initialScale,
            scaleRecoverSpeed * Time.deltaTime
        );

        timer += Time.deltaTime;

        if (timer >= fadeStartTime)
        {
            float fadeProgress = (timer - fadeStartTime) / (lifetime - fadeStartTime);

            Color c = currentColor;
            c.a = Mathf.Lerp(1f, 0f, fadeProgress);
            textMesh.color = c;
        }

        if (timer >= lifetime)
            ReturnToPool();
    }
}