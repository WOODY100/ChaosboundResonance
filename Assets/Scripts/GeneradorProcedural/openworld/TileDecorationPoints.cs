using UnityEngine;

public class TileDecorationPoints : MonoBehaviour
{
    [Header("Regular Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Special Center Point")]
    [SerializeField] private Transform centerPoint;

    public Transform[] SpawnPoints => spawnPoints;
    public Transform CenterPoint => centerPoint;
}