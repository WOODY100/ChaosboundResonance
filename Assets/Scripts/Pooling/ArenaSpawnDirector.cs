using UnityEngine;
using System.Collections.Generic;

public class ArenaSpawnDirector : MonoBehaviour
{
    public enum EncounterType
    {
        None,
        Combat,
        MiniBoss,
        Boss
    }

    [Header("Debug")]
    [SerializeField] private bool autoStartOpenWorld = true;
    [SerializeField] private int debugDungeonTier = 1;

    [Header("Enemy Prefabs")]
    [SerializeField] private List<ArenaEnemyGroup> enemyGroups;
    [SerializeField] private GameObject miniBossPrefab;
    [SerializeField] private GameObject bossPrefab;

    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Open World Spawn")]
    [SerializeField] private float spawnStartDelay = 1.5f;
    [SerializeField] private float minSpawnDistance = 14f;
    [SerializeField] private float maxSpawnDistance = 22f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int spawnPositionAttempts = 12;

    private EncounterType currentEncounterType = EncounterType.None;
    private int currentDungeonTier;

    private int activeEnemies;
    private bool spawnActive;
    private bool spawnPending;
    private float delayTimer;

    public int ActiveEnemies => activeEnemies;

    private void Start()
    {
        if (player == null && EnemyManager.Instance != null)
            player = EnemyManager.Instance.Player;

        if (autoStartOpenWorld)
        {
            StartOpenWorldEncounter(EncounterType.Boss, debugDungeonTier);
        }
    }

    public void StartOpenWorldEncounter(EncounterType encounterType, int dungeonTier)
    {
        ResetArena();

        currentEncounterType = encounterType;
        currentDungeonTier = dungeonTier;

        spawnPending = true;
        delayTimer = spawnStartDelay;
    }

    private void Update()
    {
        if (player == null && EnemyManager.Instance != null)
            player = EnemyManager.Instance.Player;

        if (player == null || currentEncounterType == EncounterType.None)
            return;

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

        if (activeEnemies <= 0)
            spawnActive = false;
    }

    private void ExecuteEncounter()
    {
        switch (currentEncounterType)
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

    private void SpawnCombat()
    {
        int count = 120 + currentDungeonTier * 4;

        for (int i = 0; i < count; i++)
            SpawnBasicEnemy(1f);
    }

    private void SpawnMiniBoss()
    {
        SpawnFromPrefab(miniBossPrefab);

        int adds = currentDungeonTier;

        for (int i = 0; i < adds; i++)
            SpawnBasicEnemy(1f);
    }

    private void SpawnBossEncounter()
    {
        SpawnFromPrefab(bossPrefab);

        int adds = currentDungeonTier * 2;

        for (int i = 0; i < adds; i++)
            SpawnBasicEnemy(1f);
    }

    private void SpawnBasicEnemy(float speedMultiplier)
    {
        GameObject prefab = GetRandomWeightedPrefab();

        if (prefab == null)
            return;

        GameObject enemy = SpawnFromPrefab(prefab);

        if (enemy == null)
            return;

        EnemyMovementArena movement = enemy.GetComponent<EnemyMovementArena>();

        if (movement != null)
            movement.SetDifficultyMultiplier(speedMultiplier);
    }

    private GameObject SpawnFromPrefab(GameObject prefab)
    {
        if (prefab == null)
            return null;

        Vector3 spawnPos = GetOpenWorldSpawnPosition();

        GameObject enemy = PoolManager.Instance.Get(
            prefab,
            spawnPos,
            Quaternion.identity
        );

        EnemyHealth health = enemy.GetComponent<EnemyHealth>();

        if (health != null)
        {
            health.OnDeath -= HandleEnemyDeath;
            health.OnDeath += HandleEnemyDeath;
        }

        activeEnemies++;

        return enemy;
    }

    private Vector3 GetOpenWorldSpawnPosition()
    {
        for (int i = 0; i < spawnPositionAttempts; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

            Vector3 candidate = player.position + new Vector3(
                dir.x * distance,
                0f,
                dir.y * distance
            );

            Vector3 rayOrigin = candidate + Vector3.up * 20f;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 50f, groundLayer))
            {
                return hit.point;
            }
        }

        Vector2 fallbackDir = Random.insideUnitCircle.normalized;

        return player.position + new Vector3(
            fallbackDir.x * minSpawnDistance,
            0f,
            fallbackDir.y * minSpawnDistance
        );
    }

    private GameObject GetRandomWeightedPrefab()
    {
        if (enemyGroups == null || enemyGroups.Count == 0)
            return null;

        float totalWeight = 0f;

        foreach (ArenaEnemyGroup group in enemyGroups)
        {
            if (group.enemyPrefab != null)
                totalWeight += group.spawnWeight;
        }

        if (totalWeight <= 0f)
            return null;

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (ArenaEnemyGroup group in enemyGroups)
        {
            if (group.enemyPrefab == null)
                continue;

            cumulative += group.spawnWeight;

            if (randomValue <= cumulative)
                return group.enemyPrefab;
        }

        return enemyGroups[0].enemyPrefab;
    }

    private void HandleEnemyDeath(EnemyHealth enemy)
    {
        enemy.OnDeath -= HandleEnemyDeath;
        activeEnemies = Mathf.Max(0, activeEnemies - 1);
    }

    public void ResetArena()
    {
        spawnActive = false;
        spawnPending = false;
        activeEnemies = 0;
        currentEncounterType = EncounterType.None;
        currentDungeonTier = 0;
    }
}