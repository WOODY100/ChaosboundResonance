using UnityEngine;

public class ChargeGhostFade : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.1f;

    private Renderer rend;
    private Material materialInstance;
    private Color baseColor;
    private float timer;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        materialInstance = rend.material;
        baseColor = materialInstance.GetColor("_BaseColor");
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float t = timer / lifetime;
        float intensity = Mathf.Lerp(0.3f, 0f, t * t);

        Color newColor = baseColor * intensity;
        materialInstance.color = newColor;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}