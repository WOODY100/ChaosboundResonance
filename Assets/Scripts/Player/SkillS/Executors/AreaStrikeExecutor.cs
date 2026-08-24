using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaStrikeExecutor :
    MonoBehaviour,
    ISkillExecutor
{
    private RuntimeSkill skill;
    private Transform owner;

    private bool isExecuting = false;

    private readonly List<GameObject> activeStrikes =
        new();

    public void Initialize(
        RuntimeSkill runtimeSkill,
        Transform skillOwner)
    {
        ResetExecutor();

        skill = runtimeSkill;
        owner = skillOwner;
    }

    public void Tick(float deltaTime)
    {
        if (skill == null || owner == null)
            return;

        CleanupInactiveStrikes();

        skill.TickCooldown(deltaTime);

        if (isExecuting)
            return;

        if (skill.IsOnCooldown)
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
        isExecuting = true;

        StartCoroutine(
            SpawnStrikesWithDelay());
    }

    private IEnumerator SpawnStrikesWithDelay()
    {
        int count =
            skill.Stats.FinalCount;

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPoint =
                GetRandomPointAroundOwner(
                    skill.Stats.FinalSpawnRadius);

            SpawnStrike(randomPoint);

            if (count > 1)
                yield return new WaitForSeconds(
                    0.12f);
        }

        isExecuting = false;

        if (skill.Definition.CooldownStartsAfterDuration)
        {
            skill.StartCooldown(
                skill.Stats.FinalCooldown);
        }
    }

    private void SpawnStrike(Vector3 position)
    {
        GameObject strikeObj =
            PoolManager.Instance.Get(
                skill.Definition.ExecutionPrefab,
                position,
                Quaternion.identity
            );

        if (strikeObj == null)
            return;

        activeStrikes.Add(strikeObj);

        IAreaStrike strike =
            strikeObj.GetComponent<IAreaStrike>();

        if (strike != null)
            strike.Initialize(skill);
    }

    private void CleanupStrikes()
    {
        for (int i = activeStrikes.Count - 1; i >= 0; i--)
        {
            GameObject strike =
                activeStrikes[i];

            if (strike == null)
            {
                activeStrikes.RemoveAt(i);
                continue;
            }

            if (!strike.activeInHierarchy)
            {
                activeStrikes.RemoveAt(i);
                continue;
            }

            PooledBehaviour pooledBehaviour =
                strike.GetComponent<PooledBehaviour>();

            if (pooledBehaviour != null)
            {
                pooledBehaviour.ReturnToPool();
            }

            activeStrikes.RemoveAt(i);
        }
    }

    private Vector3 GetRandomPointAroundOwner(
        float radius)
    {
        Vector2 randomCircle =
            Random.insideUnitCircle * radius;

        return owner.position +
               new Vector3(
                   randomCircle.x,
                   0f,
                   randomCircle.y);
    }

    private void CleanupInactiveStrikes()
    {
        activeStrikes.RemoveAll(
            strike =>
                strike == null ||
                !strike.activeInHierarchy);
    }

    public void Cleanup()
    {
        StopAllCoroutines();

        foreach (GameObject strike
            in activeStrikes)
        {
            if (strike == null)
                continue;

            PooledObject pooledObject =
                strike.GetComponent<PooledObject>();

            if (pooledObject != null)
            {
                pooledObject.ReturnToPool();
            }
        }

        activeStrikes.Clear();

        isExecuting = false;
    }

    public void ResetExecutor()
    {
        StopAllCoroutines();

        CleanupStrikes();

        skill = null;
        owner = null;

        isExecuting = false;
    }
}