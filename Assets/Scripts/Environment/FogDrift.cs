using UnityEngine;

public class FogDrift : MonoBehaviour
{
    public float speedX = 0.02f;
    public float speedY = 0.01f;

    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        Vector2 offset = mat.mainTextureOffset;
        offset.x += speedX * Time.deltaTime;
        offset.y += speedY * Time.deltaTime;
        mat.mainTextureOffset = offset;
    }
}