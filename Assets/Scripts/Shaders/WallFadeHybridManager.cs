using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WallFadeHybridManager : MonoBehaviour
{
    public string playerTag = "Player";
    public LayerMask wallLayer;

    public float holeRadius = 2f;
    public float holeSoftness = 0.5f;

    private Transform player;
    private Camera mainCamera;

    private HashSet<FadeWall> activeWalls = new HashSet<FadeWall>();

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (player == null || mainCamera == null)
            return;

        Vector3 camPos = mainCamera.transform.position;
        Vector3 playerPos = player.position;
        Vector3 dir = playerPos - camPos;
        float distance = dir.magnitude;

        float detectionRadius = holeRadius * 0.6f;

        RaycastHit[] hits = Physics.SphereCastAll(
            camPos,
            detectionRadius,
            dir.normalized,
            distance,
            wallLayer
        );

        hits = hits.OrderBy(h => h.distance).ToArray();

        HashSet<FadeWall> newActiveWalls = new HashSet<FadeWall>();

        foreach (var hit in hits)
        {
            FadeWall wall = hit.collider.GetComponentInParent<FadeWall>();
            if (wall == null)
                wall = hit.collider.GetComponentInChildren<FadeWall>();

            if (wall == null)
                continue;

            newActiveWalls.Add(wall);

            wall.SetActive(true);
            wall.UpdateFade(playerPos, camPos, holeRadius, holeSoftness);
        }

        // Desactivar las que ya no bloquean
        foreach (var wall in activeWalls)
        {
            if (!newActiveWalls.Contains(wall))
                wall.SetActive(false);
        }

        activeWalls = newActiveWalls;
    }
}