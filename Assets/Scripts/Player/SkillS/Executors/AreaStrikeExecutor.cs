using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaStrikeExecutor :
    MonoBehaviour,
    ISkillExecutor
{
    private const float DefaultSpawnDelay = 0.12f;

    private RuntimeSkill skill;
    private Transform owner;

    private bool isExecuting;

    private readonly List<GameObject> activeStrikes =
        new List<GameObject>();

    private AreaStrikePatternDefinition Pattern
    {
        get
        {
            if (skill == null || skill.Definition == null)
                return null;

            return skill.Definition.ExecutionPattern
                as AreaStrikePatternDefinition;
        }
    }

    public void Initialize(
        RuntimeSkill runtimeSkill,
        Transform skillOwner)
    {
        ResetExecutor();

        if (runtimeSkill == null || skillOwner == null)
        {
            Debug.LogError(
                "AreaStrikeExecutor requires a RuntimeSkill and an owner.",
                this);
            return;
        }

        skill = runtimeSkill;
        owner = skillOwner;
    }

    public void Tick(float deltaTime)
    {
        if (skill == null || owner == null)
            return;

        CleanupInactiveStrikes();

        skill.TickCooldown(deltaTime);

        if (isExecuting || skill.IsOnCooldown)
            return;

        Execute();

        if (!skill.Definition.CooldownStartsAfterDuration)
        {
            skill.StartCooldown(
                skill.Stats.FinalCooldown);
        }
    }

    private void Execute()
    {
        AreaStrikePatternDefinition pattern = Pattern;

        // Capture the origin and facing direction at execution time.
        Vector3 origin = owner.position;
        Vector3 forward = GetHorizontalForward(owner);

        int count = Mathf.Max(0, skill.Stats.FinalCount);

        float spawnDelay = pattern != null
            ? pattern.SpawnDelay
            : DefaultSpawnDelay;

        isExecuting = true;

        StartCoroutine(
            SpawnStrikesWithDelay(
                pattern,
                origin,
                forward,
                count,
                spawnDelay));
    }

    private IEnumerator SpawnStrikesWithDelay(
        AreaStrikePatternDefinition pattern,
        Vector3 origin,
        Vector3 forward,
        int count,
        float spawnDelay)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 position = GetStrikePosition(
                pattern,
                origin,
                forward,
                i,
                count);

            SpawnStrike(position);

            if (i < count - 1 && spawnDelay > 0f)
            {
                yield return new WaitForSeconds(spawnDelay);
            }
        }

        isExecuting = false;

        if (skill != null &&
            skill.Definition.CooldownStartsAfterDuration)
        {
            skill.StartCooldown(
                skill.Stats.FinalCooldown);
        }
    }

    private Vector3 GetStrikePosition(
        AreaStrikePatternDefinition pattern,
        Vector3 origin,
        Vector3 forward,
        int index,
        int count)
    {
        // No assigned pattern preserves the original random placement.
        if (pattern == null)
        {
            return GetRandomPointAroundOwner(
                origin,
                skill.Stats.FinalSpawnRadius);
        }

        switch (pattern.Pattern)
        {
            case AreaStrikePatternType.SinglePoint:
                {
                    return origin +
                        forward * pattern.ForwardOffset;
                }

            case AreaStrikePatternType.MultiPoint:
                {
                    return GetRandomPointAroundOwner(
                        origin,
                        skill.Stats.FinalSpawnRadius);
                }

            case AreaStrikePatternType.Ring:
                {
                    Vector3 center =
                        origin + forward * pattern.ForwardOffset;

                    Vector2 randomOffset =
                        Random.insideUnitCircle * pattern.AreaRadius;

                    return center + new Vector3(
                        randomOffset.x,
                        0f,
                        randomOffset.y);
                }

            case AreaStrikePatternType.Line:
                {
                    Vector3 start =
                        origin + forward * pattern.ForwardOffset;

                    if (count <= 1)
                        return start;

                    float t = (float)index / (count - 1);

                    return start +
                        forward * (pattern.LineLength * t);
                }

            default:
                {
                    return GetRandomPointAroundOwner(
                        origin,
                        skill.Stats.FinalSpawnRadius);
                }
        }
    }

    private Vector3 GetRandomPointAroundOwner(
        Vector3 origin,
        float radius)
    {
        Vector2 randomCircle =
            Random.insideUnitCircle * Mathf.Max(0f, radius);

        return origin + new Vector3(
            randomCircle.x,
            0f,
            randomCircle.y);
    }

    private Vector3 GetHorizontalForward(Transform target)
    {
        Vector3 forward = target.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.0001f)
            return Vector3.forward;

        return forward.normalized;
    }

    private void SpawnStrike(Vector3 position)
    {
        if (skill == null ||
            skill.Definition.ExecutionPrefab == null)
        {
            return;
        }

        if (PoolManager.Instance == null)
        {
            Debug.LogError(
                "AreaStrikeExecutor could not find PoolManager.",
                this);
            return;
        }

        GameObject strikeObj = PoolManager.Instance.Get(
            skill.Definition.ExecutionPrefab,
            position,
            Quaternion.identity);

        if (strikeObj == null)
            return;

        IAreaStrike strike =
            strikeObj.GetComponent<IAreaStrike>();

        if (strike == null)
        {
            Debug.LogError(
                "Area strike prefab '" + strikeObj.name +
                "' does not implement IAreaStrike.",
                strikeObj);

            ReturnStrikeToPool(strikeObj);
            return;
        }

        activeStrikes.Add(strikeObj);
        strike.Initialize(skill);
    }

    private void CleanupInactiveStrikes()
    {
        activeStrikes.RemoveAll(
            strike =>
                strike == null ||
                !strike.activeInHierarchy);
    }

    private void ReturnStrikeToPool(GameObject strike)
    {
        if (strike == null)
            return;

        PooledBehaviour pooledBehaviour =
            strike.GetComponent<PooledBehaviour>();

        if (pooledBehaviour != null)
        {
            pooledBehaviour.ReturnToPool();
            return;
        }

        PooledObject pooledObject =
            strike.GetComponent<PooledObject>();

        if (pooledObject != null)
        {
            pooledObject.ReturnToPool();
            return;
        }

        Destroy(strike);
    }

    public void Cleanup()
    {
        StopAllCoroutines();

        for (int i = activeStrikes.Count - 1; i >= 0; i--)
        {
            ReturnStrikeToPool(activeStrikes[i]);
        }

        activeStrikes.Clear();
        isExecuting = false;
    }

    public void ResetExecutor()
    {
        Cleanup();

        skill = null;
        owner = null;
    }
}