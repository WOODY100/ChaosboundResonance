using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentZoneExecutor : MonoBehaviour, ISkillExecutor
{
    private RuntimeSkill skill;
    private Transform player;

    private bool isExecuting = false;

    private List<Vector3> usedPositions = new List<Vector3>();

    private List<GameObject> activeZones = new List<GameObject>();

    private static readonly Collider[] enemyBuffer = new Collider[32];
    [SerializeField] private LayerMask enemyLayer;

    public void Initialize(RuntimeSkill runtimeSkill, Transform playerTransform)
    {
        skill = runtimeSkill;
        player = playerTransform;
    }

    public void Tick(float deltaTime)
    {
        skill.TickCooldown(deltaTime);

        if (isExecuting)
            return;

        if (skill.IsOnCooldown)
            return;

        Execute();
    }

    private void Execute()
    {
        StartCoroutine(ExecuteSequence());
    }

    private void OnAllZonesFinished()
    {
        isExecuting = false;

        skill.StartCooldown(skill.Stats.FinalCooldown);
    }

    private IEnumerator ExecuteSequence()
    {
        isExecuting = true;

        int count = skill.Stats.FinalCount;
        int remainingZones = count;

        float radius = skill.Stats.FinalSpawnRadius;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = GetSmartPosition(radius);

            GameObject zone = SpawnZoneAt(pos);

            var zoneComponent = zone.GetComponent<PersistentZone>();

            zoneComponent.OnZoneEnded += (z) =>
            {
                remainingZones--;

                if (remainingZones <= 0)
                {
                    OnAllZonesFinished();
                }
            };

            yield return new WaitForSeconds(0.08f); // 🔥 delay sexy
        }
    }

    private Vector3 GetSmartPosition(float radius)
    {
        int count = Physics.OverlapSphereNonAlloc(
            player.position,
            radius,
            enemyBuffer,
            enemyLayer
        );

        // 🔥 70% de probabilidad de usar enemigos
        if (count > 0 && Random.value < 0.7f)
        {
            Collider target = enemyBuffer[Random.Range(0, count)];

            Vector3 basePos = target.transform.position;

            // 🔥 pequeño offset para evitar stacking exacto
            Vector2 offset = Random.insideUnitCircle * Random.Range(0.8f, 1.8f);

            return basePos + new Vector3(offset.x, 0f, offset.y);
        }

        // 🔥 fallback random (tipo Stormfall)
        Vector2 randomCircle = Random.insideUnitCircle * radius;

        return player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }

    private int GetMaxZones()
    {
        return skill.Stats.FinalCount + 2;
    }

    private GameObject SpawnZoneAt(Vector3 position)
    {
        CleanupNulls();

        if (activeZones.Count >= GetMaxZones())
        {
            RemoveOldestZone();
        }

        Quaternion rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);

        GameObject zone = Instantiate(
            skill.Definition.ExecutionPrefab,
            position,
            rotation
        );

        var zoneComponent = zone.GetComponent<PersistentZone>();
        zoneComponent.Initialize(skill);

        activeZones.Add(zone);

        return zone; // 🔥 importante
    }

    private void CleanupNulls()
    {
        activeZones.RemoveAll(z => z == null);
    }

    private void RemoveOldestZone()
    {
        if (activeZones.Count == 0)
            return;

        GameObject oldest = activeZones[0];

        if (oldest != null)
            Destroy(oldest);

        activeZones.RemoveAt(0);
    }
}