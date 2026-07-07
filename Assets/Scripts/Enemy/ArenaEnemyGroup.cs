using UnityEngine;

[System.Serializable]
public class ArenaEnemyGroup
{
    public GameObject enemyPrefab;

    [Min(0f)]
    public float spawnWeight = 1f;
}