using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FadeWall : MonoBehaviour
{
    private Renderer rend;
    private MaterialPropertyBlock mpb;

    private bool isActive;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void SetActive(bool active)
    {
        isActive = active;

        rend.GetPropertyBlock(mpb);

        if (!active)
        {
            // Reset completo
            mpb.SetFloat("_HoleRadius", -1f);
        }

        rend.SetPropertyBlock(mpb);
    }

    public void UpdateFade(Vector3 playerPos, Vector3 cameraPos, float radius, float softness)
    {
        if (!isActive)
            return;

        rend.GetPropertyBlock(mpb);

        mpb.SetVector("_PlayerPos", playerPos);
        mpb.SetVector("_CameraPos", cameraPos);
        mpb.SetFloat("_HoleRadius", radius);
        mpb.SetFloat("_HoleSoftness", softness);

        rend.SetPropertyBlock(mpb);
    }
}