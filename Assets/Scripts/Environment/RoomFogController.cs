using UnityEngine;

public class RoomFogController : MonoBehaviour
{
    [SerializeField] private Renderer[] fogRenderers;

    [Header("Fog Color (solo RGB)")]
    public Color fogColor = Color.white;

    private MaterialPropertyBlock mpb;

    void Awake()
    {
        mpb = new MaterialPropertyBlock();
        ApplyFog();
    }

    public void ApplyFog()
    {
        foreach (var rend in fogRenderers)
        {
            if (rend == null) continue;

            rend.GetPropertyBlock(mpb);

            // 🔥 Tomamos el color original del material
            Color baseColor = rend.sharedMaterial.GetColor("_BaseColor");

            // 🔥 Solo reemplazamos RGB, conservamos alpha original
            baseColor.r = fogColor.r;
            baseColor.g = fogColor.g;
            baseColor.b = fogColor.b;

            mpb.SetColor("_BaseColor", baseColor);
            rend.SetPropertyBlock(mpb);
        }
    }

    public void SetColor(Color newColor)
    {
        fogColor = newColor;
        ApplyFog();
    }
}