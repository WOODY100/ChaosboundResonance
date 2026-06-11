using UnityEngine;
using System.Collections.Generic;

public class CameraWallOcclusion : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float sphereRadius = 1f;

    private readonly List<WallOccluder> hiddenWalls = new List<WallOccluder>(32);
    private readonly RaycastHit[] hitBuffer = new RaycastHit[32];

    void LateUpdate()
    {
        if (player == null) return;

        ShowHiddenWalls();

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        int hitCount = Physics.SphereCastNonAlloc(
            transform.position,
            sphereRadius,
            direction.normalized,
            hitBuffer,
            distance,
            wallLayer,
            QueryTriggerInteraction.Ignore
        );

        Debug.DrawLine(transform.position, player.position, Color.red);

        for (int i = 0; i < hitCount; i++)
        {
            WallOccluder wall = hitBuffer[i].collider.GetComponent<WallOccluder>();

            if (wall != null && !hiddenWalls.Contains(wall))
            {
                wall.HideWall();
                hiddenWalls.Add(wall);
            }
        }
    }

    private void ShowHiddenWalls()
    {
        foreach (WallOccluder wall in hiddenWalls)
        {
            if (wall != null)
                wall.ShowWall();
        }

        hiddenWalls.Clear();
    }
}