using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class ArenaSpawnDirector : MonoBehaviour
{
    // ----------------------------------
    // ENUMS / CONTEXT
    // ----------------------------------

    public enum EncounterType
    {
        None,
        Combat,
        MiniBoss,
        Boss
    }

    public class SpawnContext
    {
        public EncounterType encounterType;
        public int dungeonTier;
    }

    // ----------------------------------
    // REFERENCES
    // ----------------------------------

    [Header("Enemy Pools")]
    [SerializeField] private List<ArenaEnemyGroup> enemyGroups;
    [SerializeField] private EnemyPool miniBossPool;
    [SerializeField] private EnemyPool bossPool;

    [Header("References")]
    [SerializeField] private Transform player;

    // ----------------------------------
    // SPAWN AREA
    // ----------------------------------

    [Header("Spawn Area")]
    [SerializeField] private float spawnStartDelay = 1.5f;

    private List<Transform> currentSpawnPoints;
    private RoomDoors currentRoomDoors;

    // ----------------------------------
    // STATE
    // ----------------------------------

    private SpawnContext currentContext;

    private int activeEnemies;
    private bool spawnActive;
    private bool spawnPending;
    private float delayTimer;

    public int ActiveEnemies => activeEnemies;

    // ----------------------------------
    // ENTRY POINT
    // ----------------------------------

    public void StartEncounter(RoomSpawnPoints room, RoomDoors doors, SpawnContext context)
    {
        ResetArena();

        currentSpawnPoints = room.spawnPoints;
        currentRoomDoors = doors;
        currentContext = context;

        spawnPending = true;
        delayTimer = spawnStartDelay;
    }

    // ----------------------------------
    // UPDATE
    // ----------------------------------

    void Update()
    {
        if (player == null || currentContext == null)
            return;

        // Delay inicial
        if (spawnPending)
        {
            delayTimer -= Time.deltaTime;

            if (delayTimer <= 0f)
            {
                spawnPending = false;
                spawnActive = true;

                ExecuteEncounter();
            }

            return;
        }

        if (!spawnActive)
            return;

        // Abrir puertas cuando no hay enemigos
        if (activeEnemies <= 0)
        {
            if (currentRoomDoors != null)
            {
                currentRoomDoors.OpenDoors();
                spawnActive = false;
            }
        }
    }

    // ----------------------------------
    // ENCOUNTER EXECUTION
    // ----------------------------------

    void ExecuteEncounter()
    {
        switch (currentContext.encounterType)
        {
            case EncounterType.Combat:
                SpawnCombat();
                break;

            case EncounterType.MiniBoss:
                SpawnMiniBoss();
                break;

            case EncounterType.Boss:
                SpawnBossEncounter();
                break;
        }
    }

    // ----------------------------------
    // SPAWN TYPES
    // ----------------------------------

    void SpawnCombat()
    {
        int count = 3 + currentContext.dungeonTier * 2;

        for (int i = 0; i < count; i++)
        {
            SpawnBasicEnemy(1f);
        }
    }

    void SpawnMiniBoss()
    {
        SpawnFromPool(miniBossPool);

        int adds = currentContext.dungeonTier;

        for (int i = 0; i < adds; i++)
        {
            SpawnBasicEnemy(1f);
        }
    }

    void SpawnBossEncounter()
    {
        SpawnFromPool(bossPool);

        int adds = currentContext.dungeonTier * 2;

        for (int i = 0; i < adds; i++)
        {
            SpawnBasicEnemy(1f);
        }
    }

    // ----------------------------------
    // SPAWN HELPERS
    // ----------------------------------

    void SpawnBasicEnemy(float speedMultiplier)
    {
        if (enemyGroups == null || enemyGroups.Count == 0)
            return;

        EnemyPool selectedPool = GetRandomWeightedPool();

        Vector3 spawnPos = GetSpawnPoint();

        if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            GameObject enemy = selectedPool.Get(hit.position, Quaternion.identity);

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            health.OnDeath += HandleEnemyDeath;

            EnemyMovementArena movement = enemy.GetComponent<EnemyMovementArena>();
            if (movement != null)
                movement.SetDifficultyMultiplier(speedMultiplier);

            activeEnemies++;
        }
    }

    void SpawnFromPool(EnemyPool pool)
    {
        if (pool == null)
            return;

        Vector3 spawnPos = GetSpawnPoint();

        if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            GameObject enemy = pool.Get(hit.position, Quaternion.identity);

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            health.OnDeath += HandleEnemyDeath;

            activeEnemies++;
        }
    }

    // ----------------------------------
    // SPAWN POINTS
    // ----------------------------------

    Vector3 GetSpawnPoint()
    {
        if (currentSpawnPoints == null || currentSpawnPoints.Count == 0)
            return player.position;

        int index = Random.Range(0, currentSpawnPoints.Count);
        return currentSpawnPoints[index].position;
    }

    EnemyPool GetRandomWeightedPool()
    {
        float totalWeight = 0f;

        foreach (var group in enemyGroups)
            totalWeight += group.spawnWeight;

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var group in enemyGroups)
        {
            cumulative += group.spawnWeight;

            if (randomValue <= cumulative)
                return group.pool;
        }

        return enemyGroups[0].pool;
    }

    // ----------------------------------
    // EVENTS
    // ----------------------------------

    void HandleEnemyDeath(EnemyHealth enemy)
    {
        enemy.OnDeath -= HandleEnemyDeath;
        activeEnemies--;

        if (activeEnemies <= 0 && currentRoomDoors != null)
        {
            currentRoomDoors.OpenDoors();
        }
    }

    // ----------------------------------
    // RESET
    // ----------------------------------

    public void ResetArena()
    {
        spawnActive = false;
        spawnPending = false;

        activeEnemies = 0;
        currentContext = null;
    }
}