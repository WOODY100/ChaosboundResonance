using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentZoneExecutor : MonoBehaviour, ISkillExecutor
{
    private RuntimeSkill skill;
    private Transform player;

    private bool isExecuting;

    private readonly List<GameObject> activeZones = new();

    private static readonly Collider[] enemyBuffer = new Collider[32];

    [SerializeField] private LayerMask enemyLayer;

    public void Initialize(RuntimeSkill runtimeSkill, Transform playerTransform)
    {
        ResetExecutor();

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

    private IEnumerator ExecuteSequence()
    {
        isExecuting = true;

        int count = skill.Stats.FinalCount;
        int remainingZones = count;

        float radius = skill.Stats.FinalSpawnRadius;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = GetSmartPosition(radius);

            PersistentZone zoneComponent = SpawnZoneAt(pos);

            if (zoneComponent != null)
            {
                zoneComponent.OnZoneEnded += (z) =>
                {
                    remainingZones--;

                    activeZones.Remove(z.gameObject);

                    if (remainingZones <= 0)
                    {
                        OnAllZonesFinished();
                    }
                };
            }
            else
            {
                remainingZones--;
            }

            yield return new WaitForSeconds(0.08f);
        }

        if (remainingZones <= 0)
        {
            OnAllZonesFinished();
        }
    }

    private void OnAllZonesFinished()
    {
        if (!isExecuting)
            return;

        isExecuting = false;
        skill.StartCooldown(skill.Stats.FinalCooldown);
    }

    private Vector3 GetSmartPosition(float radius)
    {
        int count = Physics.OverlapSphereNonAlloc(
            player.position,
            radius,
            enemyBuffer,
            enemyLayer
        );

        if (count > 0 && Random.value < 0.7f)
        {
            Collider target = enemyBuffer[Random.Range(0, count)];

            Vector3 basePos = target.transform.position;
            Vector2 offset = Random.insideUnitCircle * Random.Range(0.8f, 1.8f);

            return basePos + new Vector3(offset.x, 0f, offset.y);
        }

        Vector2 randomCircle = Random.insideUnitCircle * radius;

        return player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }

    private int GetMaxZones()
    {
        return skill.Stats.FinalCount + 2;
    }

    private PersistentZone SpawnZoneAt(Vector3 position)
    {
        CleanupInactiveZones();

        if (activeZones.Count >= GetMaxZones())
        {
            RemoveOldestZone();
        }

        Quaternion rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);

        GameObject zoneObj = PoolManager.Instance.Get(
            skill.Definition.ExecutionPrefab,
            position,
            rotation
        );

        if (zoneObj == null)
            return null;

        PersistentZone zoneComponent = zoneObj.GetComponent<PersistentZone>();

        if (zoneComponent == null)
            return null;

        zoneComponent.Initialize(skill);

        activeZones.Add(zoneObj);

        return zoneComponent;
    }

    private void CleanupInactiveZones()
    {
        activeZones.RemoveAll(z => z == null || !z.activeInHierarchy);
    }

    private void RemoveOldestZone()
    {
        if (activeZones.Count == 0)
            return;

        GameObject oldest = activeZones[0];
        activeZones.RemoveAt(0);

        if (oldest == null)
            return;

        PersistentZone zone = oldest.GetComponent<PersistentZone>();

        if (zone != null)
        {
            zone.ForceEnd();
            return;
        }

        PooledObject pooledObject = oldest.GetComponent<PooledObject>();

        if (pooledObject != null)
            pooledObject.ReturnToPool();
        else
            Destroy(oldest);
    }

    public void ResetExecutor()
    {
        StopAllCoroutines();

        skill = null;
        player = null;

        isExecuting = false;

        activeZones.Clear();
    }
}