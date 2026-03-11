using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class ArenaSpawnDirector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<ArenaEnemyGroup> enemyGroups;
    [SerializeField] private EnemyPool bossPool;
    [SerializeField] private Transform player;

    [Header("Run Duration")]
    [SerializeField] private float arenaDuration = 1500f; // 25 minutos
    [SerializeField] private float finalBossTime = 1450f;

    [Header("Scaling")]
    [SerializeField] private AnimationCurve spawnRateOverTime;
    [SerializeField] private AnimationCurve maxEnemiesOverTime;
    [SerializeField] private AnimationCurve speedMultiplierOverTime;

    [Header("Spawn Area")]
    [SerializeField] private float minSpawnRadius = 12f;
    [SerializeField] private float maxSpawnRadius = 18f;
    [SerializeField] private float spawnStartDelay = 1.5f;

    public float CurrentTime => arenaTimer;
    public float Duration => arenaDuration;
    private float arenaTimer;
    private float spawnTimer;
    private bool finalBossSpawned;
    private int activeEnemies;
    private bool spawnActive;
    private RoomDoors currentRoomDoors;
    private bool spawnPending;
    private float delayTimer;

    private List<Transform> spawnPoints = new List<Transform>();

    public void RegisterSpawnPoints(List<Transform> points)
    {
        spawnPoints = points;
    }

    public int ActiveEnemies => activeEnemies;

    private void Start()
    {
        EnemyBrain.ResetAttackSlots();
    }

    void Update()
    {
        if (player == null)
            return;

        // Manejar delay inicial
        if (spawnPending)
        {
            delayTimer -= Time.deltaTime;

            if (delayTimer <= 0f)
            {
                spawnPending = false;
                spawnActive = true;
            }

            return;
        }

        if (!spawnActive)
            return;

        arenaTimer += Time.deltaTime;

        float normalizedTime = Mathf.Clamp01(arenaTimer / arenaDuration);

        HandleScaling(normalizedTime);
        HandleFinalBoss();
    }

    public void ActivateSpawning()
    {
        spawnPending = true;
        delayTimer = spawnStartDelay;
    }

    public void SetRoomDoors(RoomDoors doors)
    {
        currentRoomDoors = doors;
    }

    void HandleScaling(float normalizedTime)
    {
        if (finalBossSpawned)
            return;

        int maxEnemies = Mathf.RoundToInt(maxEnemiesOverTime.Evaluate(normalizedTime));
        float spawnRate = spawnRateOverTime.Evaluate(normalizedTime);
        float speedMultiplier = speedMultiplierOverTime.Evaluate(normalizedTime);

        spawnTimer -= Time.deltaTime;

        if (spawnTimer > 0f)
            return;

        spawnTimer = 1f / Mathf.Max(spawnRate, 0.01f);

        if (activeEnemies >= maxEnemies)
            return;

        SpawnBasicEnemy(speedMultiplier);
    }

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

    void HandleFinalBoss()
    {
        if (finalBossSpawned || bossPool == null)
            return;

        if (arenaTimer >= finalBossTime)
        {
            finalBossSpawned = true;

            Vector3 spawnPos = GetSpawnPoint();

            if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                bossPool.Get(hit.position, Quaternion.identity);
            }
        }
    }

    Vector3 GetSpawnPosition()
    {
        float angle = Random.Range(0f, 360f);
        float radius = Random.Range(minSpawnRadius, maxSpawnRadius);

        Vector3 direction = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            0f,
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        return player.position + direction * radius;
    }

    Vector3 GetSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
            return player.position;

        int index = Random.Range(0, spawnPoints.Count);
        return spawnPoints[index].position;
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

    void HandleEnemyDeath(EnemyHealth enemy)
    {
        enemy.OnDeath -= HandleEnemyDeath;
        activeEnemies--;

        if (activeEnemies <= 0 && currentRoomDoors != null)
        {
            currentRoomDoors.OpenDoors();
        }
    }
}
