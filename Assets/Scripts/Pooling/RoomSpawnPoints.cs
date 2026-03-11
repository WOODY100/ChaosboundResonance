using UnityEngine;
using System.Collections.Generic;

public class RoomSpawnPoints : MonoBehaviour
{
    public List<Transform> spawnPoints = new List<Transform>();

    void Awake()
    {
        Transform container = transform.Find("SpawnPoints");

        if (container == null)
            return;

        spawnPoints.Clear();

        foreach (Transform child in container)
        {
            spawnPoints.Add(child);
        }

        // REGISTRAR EN EL DIRECTOR
        ArenaSpawnDirector director = Object.FindAnyObjectByType<ArenaSpawnDirector>();

        if (director != null)
        {
            director.RegisterSpawnPoints(spawnPoints);
        }
    }
}