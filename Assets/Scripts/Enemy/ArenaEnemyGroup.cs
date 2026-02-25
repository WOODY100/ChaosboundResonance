using UnityEngine;

[System.Serializable]
public class ArenaEnemyGroup
{
    public EnemyPool pool;
    [Range(0.1f, 10f)]
    public float spawnWeight = 1f;
}
