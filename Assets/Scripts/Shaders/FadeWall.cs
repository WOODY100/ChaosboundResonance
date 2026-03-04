using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FadeWall : MonoBehaviour
{
    private MeshRenderer rend;

    void Awake()
    {
        rend = GetComponent<MeshRenderer>();
    }

    public void Hide()
    {
        rend.enabled = false;
    }

    public void Show()
    {
        rend.enabled = true;
    }
}