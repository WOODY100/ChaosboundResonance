using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentZoneExecutor : MonoBehaviour, ISkillExecutor
{
    private RuntimeSkill skill;
    private Transform player;
    private bool isExecuting;

    private readonly List<GameObject> activeZones =
        new List<GameObject>();

    // Stores the player's relative offset for each following zone.
    private readonly Dictionary<GameObject, Vector3> followOffsets =
        new Dictionary<GameObject, Vector3>();

    private static readonly Collider[] enemyBuffer =
        new Collider[32];

    [SerializeField] private LayerMask enemyLayer;

    private PersistentZonePatternDefinition activePattern;
    private bool followCurrentFormation;
    private Vector3 formationOrigin;

    public void Initialize(RuntimeSkill runtimeSkill, Transform playerTransform)
    {
        ResetExecutor();

        skill = runtimeSkill;
        player = playerTransform;
    }

    public void Tick(float deltaTime)
    {
        if (skill == null || player == null)
            return;

        CleanupInactiveZones();
        UpdateFollowingZones();

        skill.TickCooldown(deltaTime);

        if (isExecuting || skill.IsOnCooldown)
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

        PersistentZonePatternDefinition pattern =
            skill.Definition.ExecutionPattern
                as PersistentZonePatternDefinition;

        int count = Mathf.Max(0, skill.Stats.FinalCount);

        if (pattern != null &&
            pattern.Pattern == PersistentZonePatternType.SingleZone)
        {
            count = 1;
        }

        if (count == 0)
        {
            OnAllZonesFinished();
            yield break;
        }

        int remainingZones = count;
        bool sequenceFinished = false;

        float radius = skill.Stats.FinalSpawnRadius;

        Vector3 origin = player.position;
        Vector3 forward = player.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.0001f)
            forward = Vector3.forward;
        else
            forward.Normalize();

        bool shouldFollow =
            pattern != null &&
            pattern.FollowPlayer &&
            (pattern.Pattern == PersistentZonePatternType.SingleZone ||
             pattern.Pattern == PersistentZonePatternType.Ring);

        float spawnDelay = pattern != null
            ? pattern.SpawnDelay
            : 0.08f;

        activePattern = pattern;
        followCurrentFormation = shouldFollow;
        formationOrigin = origin;

        for (int i = 0; i < count; i++)
        {
            Vector3 position = GetPatternPosition(
                pattern,
                i,
                count,
                origin,
                forward,
                radius);

            PersistentZone zone = SpawnZoneAt(position);

            if (zone != null)
            {
                if (shouldFollow)
                {
                    followOffsets[zone.gameObject] =
                        position - origin;
                }

                zone.OnZoneEnded += (endedZone) =>
                {
                    remainingZones--;

                    activeZones.Remove(endedZone.gameObject);
                    followOffsets.Remove(endedZone.gameObject);

                    if (remainingZones <= 0 && sequenceFinished)
                        OnAllZonesFinished();
                };
            }
            else
            {
                remainingZones--;
            }

            if (spawnDelay > 0f && i < count - 1)
                yield return new WaitForSeconds(spawnDelay);
        }

        sequenceFinished = true;

        if (remainingZones <= 0)
            OnAllZonesFinished();
    }

    private void UpdateFollowingZones()
    {
        if (!followCurrentFormation || player == null)
            return;

        Vector3 playerDelta = player.position - formationOrigin;

        for (int i = 0; i < activeZones.Count; i++)
        {
            GameObject zoneObject = activeZones[i];

            if (zoneObject == null ||
                !zoneObject.activeInHierarchy)
                continue;

            if (!followOffsets.TryGetValue(
                    zoneObject, out Vector3 offset))
                continue;

            Vector3 targetPosition = player.position + offset;

            // Preserve the original vertical position of each zone.
            targetPosition.y = zoneObject.transform.position.y;

            zoneObject.transform.position = targetPosition;
        }

        // Update the reference origin so the next frame uses only
        // the player's movement since the previous frame.
        formationOrigin = player.position;
    }

    private Vector3 GetPatternPosition(
        PersistentZonePatternDefinition pattern,
        int index,
        int count,
        Vector3 origin,
        Vector3 forward,
        float spawnRadius)
    {
        if (pattern == null ||
            pattern.Pattern == PersistentZonePatternType.MultiZone)
        {
            return GetSmartPosition(origin, spawnRadius);
        }

        if (pattern.Pattern == PersistentZonePatternType.SingleZone)
            return origin;

        if (pattern.Pattern == PersistentZonePatternType.Ring)
        {
            float angle = 360f * index / count;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(radians),
                0f,
                Mathf.Sin(radians));

            return origin + offset * pattern.RingRadius;
        }

        if (pattern.Pattern == PersistentZonePatternType.Line)
        {
            float distance = pattern.ForwardOffset;

            if (count > 1)
                distance += pattern.LineLength * index / (count - 1);

            return origin + forward * distance;
        }

        return origin;
    }

    private void OnAllZonesFinished()
    {
        if (!isExecuting)
            return;

        isExecuting = false;
        followCurrentFormation = false;
        followOffsets.Clear();

        if (skill != null)
            skill.StartCooldown(skill.Stats.FinalCooldown);
    }

    private Vector3 GetSmartPosition(Vector3 origin, float radius)
    {
        int count = Physics.OverlapSphereNonAlloc(
            origin,
            radius,
            enemyBuffer,
            enemyLayer);

        if (count > 0 && Random.value < 0.7f)
        {
            Collider target = enemyBuffer[Random.Range(0, count)];

            if (target != null)
            {
                Vector3 basePosition = target.transform.position;

                Vector2 offset = Random.insideUnitCircle *
                    Random.Range(0.8f, 1.8f);

                return basePosition +
                    new Vector3(offset.x, 0f, offset.y);
            }
        }

        Vector2 randomCircle =
            Random.insideUnitCircle * radius;

        return origin + new Vector3(
            randomCircle.x,
            0f,
            randomCircle.y);
    }

    private int GetMaxZones()
    {
        return skill.Stats.FinalCount + 2;
    }

    private PersistentZone SpawnZoneAt(Vector3 position)
    {
        CleanupInactiveZones();

        if (activeZones.Count >= GetMaxZones())
            RemoveOldestZone();

        Quaternion rotation = Quaternion.Euler(
            90f,
            player.eulerAngles.y,
            0f);

        GameObject zoneObject = PoolManager.Instance.Get(
            skill.Definition.ExecutionPrefab,
            position,
            rotation);

        if (zoneObject == null)
            return null;

        PersistentZone zone = zoneObject.GetComponent<PersistentZone>();

        if (zone == null)
        {
            ReturnZoneToPool(zoneObject);
            return null;
        }

        activeZones.Add(zoneObject);
        zone.Initialize(skill);

        return zone;
    }

    private void CleanupInactiveZones()
    {
        activeZones.RemoveAll(zone =>
        {
            bool inactive = zone == null || !zone.activeInHierarchy;

            if (inactive && zone != null)
                followOffsets.Remove(zone);

            return inactive;
        });
    }

    private void ReturnZoneToPool(GameObject zoneObject)
    {
        if (zoneObject == null)
            return;

        followOffsets.Remove(zoneObject);

        PooledBehaviour pooledBehaviour =
            zoneObject.GetComponent<PooledBehaviour>();

        if (pooledBehaviour != null)
        {
            pooledBehaviour.ReturnToPool();
            return;
        }

        PooledObject pooledObject =
            zoneObject.GetComponent<PooledObject>();

        if (pooledObject != null)
            pooledObject.ReturnToPool();
        else
            Destroy(zoneObject);
    }

    private void RemoveOldestZone()
    {
        if (activeZones.Count == 0)
            return;

        GameObject oldest = activeZones[0];
        activeZones.RemoveAt(0);

        if (oldest == null)
            return;

        followOffsets.Remove(oldest);

        PersistentZone zone = oldest.GetComponent<PersistentZone>();

        if (zone != null)
        {
            zone.ForceEnd();
            return;
        }

        ReturnZoneToPool(oldest);
    }

    public void Cleanup()
    {
        StopAllCoroutines();

        List<GameObject> zonesToCleanup =
            new List<GameObject>(activeZones);

        foreach (GameObject zoneObject in zonesToCleanup)
        {
            if (zoneObject == null)
                continue;

            PersistentZone zone =
                zoneObject.GetComponent<PersistentZone>();

            if (zone != null)
                zone.ForceEnd();
            else
                ReturnZoneToPool(zoneObject);
        }

        activeZones.Clear();
        followOffsets.Clear();

        isExecuting = false;
        followCurrentFormation = false;
    }

    public void ResetExecutor()
    {
        StopAllCoroutines();

        List<GameObject> zonesToCleanup =
            new List<GameObject>(activeZones);

        foreach (GameObject zoneObject in zonesToCleanup)
        {
            if (zoneObject == null)
                continue;

            PersistentZone zone =
                zoneObject.GetComponent<PersistentZone>();

            if (zone != null)
                zone.ForceEnd();
            else
                ReturnZoneToPool(zoneObject);
        }

        activeZones.Clear();
        followOffsets.Clear();

        skill = null;
        player = null;
        activePattern = null;

        isExecuting = false;
        followCurrentFormation = false;
    }
}