using UnityEngine;
using System.Collections.Generic;

public class CameraWallOcclusion : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float sphereRadius = 1f;

    private List<WallOccluder> hiddenWalls = new List<WallOccluder>();

    void LateUpdate()
    {
        ShowHiddenWalls();

        Vector3 dir = player.position - transform.position;
        float dist = dir.magnitude;

        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            sphereRadius,
            dir.normalized,
            dist,
            wallLayer,
            QueryTriggerInteraction.Ignore
        );

        Debug.DrawLine(transform.position, player.position, Color.red);

        foreach (var hit in hits)
        {
            Debug.Log("Hit: " + hit.collider.name);

            WallOccluder wall = hit.collider.GetComponent<WallOccluder>();

            if (wall != null)
            {
                wall.HideWall();
                hiddenWalls.Add(wall);
            }
        }
    }

    void ShowHiddenWalls()
    {
        foreach (var wall in hiddenWalls)
        {
            if (wall != null)
                wall.ShowWall();
        }

        hiddenWalls.Clear();
    }
}