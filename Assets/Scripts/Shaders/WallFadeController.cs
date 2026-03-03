using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class WallFadeController : MonoBehaviour
{
    public string playerTag = "Player";
    public LayerMask fadeLayer;

    public float holeSize = 0.1f;
    public float checkRadius = 5f;

    private Camera mainCamera;
    private Transform player;

    private MaterialPropertyBlock mpb;

    void Awake()
    {
        mainCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
        mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (player == null || mainCamera == null)
            return;

        Collider[] hitColliders = Physics.OverlapSphere(player.position, checkRadius, fadeLayer);

        float playerCamDistance =
            Vector3.Distance(player.position, mainCamera.transform.position);

        foreach (var hitCollider in hitColliders)
        {
            Renderer rend = hitCollider.GetComponent<Renderer>();
            if (rend == null) continue;

            float colliderCamDistance =
                Vector3.Distance(hitCollider.transform.position, mainCamera.transform.position);

            float stepValue = 0f;

            if (colliderCamDistance < playerCamDistance)
                stepValue = holeSize;

            rend.GetPropertyBlock(mpb);
            mpb.SetFloat("_Step", stepValue);
            rend.SetPropertyBlock(mpb);
        }
    }
}