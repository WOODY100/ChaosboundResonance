
using UnityEngine;
using System.Collections.Generic;

public class OrbitalExecutor : MonoBehaviour, ISkillExecutor
{
    private RuntimeSkill skill;
    private Transform owner;

    private int activeOrbs;
    private bool isActive;

    private readonly List<GameObject> activeOrbObjects =
        new List<GameObject>();

    public void Initialize(RuntimeSkill runtimeSkill, Transform skillOwner)
    {
        ResetExecutor();

        skill = runtimeSkill;
        owner = skillOwner;
    }

    public void Tick(float deltaTime)
    {
        if (skill == null || owner == null)
            return;

        CleanupInactiveOrbs();

        skill.TickCooldown(deltaTime);

        if (isActive || skill.IsOnCooldown)
            return;

        ActivateOrbit();

        if (!skill.Definition.CooldownStartsAfterDuration)
        {
            skill.StartCooldown(skill.Stats.FinalCooldown);
        }
    }

    private void ActivateOrbit()
    {
        if (skill.Stats.FinalCount <= 0)
            return;

        if (PoolManager.Instance == null)
        {
            Debug.LogError("PoolManager not found.");
            return;
        }

        if (skill.Definition.ExecutionPrefab == null)
        {
            Debug.LogError(
                $"{skill.Definition.name} has no ExecutionPrefab assigned.");
            return;
        }

        OrbitalPatternDefinition pattern =
            skill.Definition.ExecutionPattern as OrbitalPatternDefinition;

        int count = skill.Stats.FinalCount;

        float[] startAngles = CalculateStartAngles(pattern, count);

        isActive = true;
        activeOrbs = count;

        for (int i = 0; i < count; i++)
        {
            GameObject orbObj = PoolManager.Instance.Get(
                skill.Definition.ExecutionPrefab,
                owner.position,
                Quaternion.identity);

            if (orbObj == null)
            {
                OnSingleOrbFinished();
                continue;
            }

            IOrbital orb = orbObj.GetComponent<IOrbital>();

            if (orb == null)
            {
                Debug.LogError(
                    $"{orbObj.name} does not implement IOrbital.");

                ReturnOrbToPool(orbObj);
                OnSingleOrbFinished();
                continue;
            }

            activeOrbObjects.Add(orbObj);

            orb.Initialize(
                skill,
                owner,
                pattern,
                startAngles[i],
                OnSingleOrbFinished);
        }
    }

    private float[] CalculateStartAngles(
        OrbitalPatternDefinition pattern,
        int count)
    {
        float[] angles = new float[count];

        if (count <= 0)
            return angles;

        // No pattern, or Orbit: preserve the original random starting phase.
        if (pattern == null ||
            pattern.Pattern == OrbitalPatternType.Orbit)
        {
            float randomOffset = Random.Range(0f, 360f);
            float angleStep = 360f / count;

            for (int i = 0; i < count; i++)
            {
                angles[i] = randomOffset + i * angleStep;
            }

            return angles;
        }

        // FixedRing: distribute the orbs evenly around the owner.
        if (pattern.Pattern == OrbitalPatternType.FixedRing)
        {
            float randomOffset = Random.Range(0f, 360f);
            float angleStep = 360f / count;

            for (int i = 0; i < count; i++)
            {
                angles[i] = randomOffset + i * angleStep;
            }

            return angles;
        }

        // ForwardArc: center the formation on the owner's horizontal
        // forward direction at activation time.
        Vector3 forward = owner.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.0001f)
            forward = Vector3.forward;
        else
            forward.Normalize();

        float forwardAngle =
            Mathf.Atan2(forward.z, forward.x) * Mathf.Rad2Deg;

        float arcAngle = Mathf.Clamp(
            pattern.ForwardArcAngle, 0f, 360f);

        if (count == 1)
        {
            angles[0] = forwardAngle;
            return angles;
        }

        float arcStep = arcAngle / (count - 1);
        float firstAngle = forwardAngle - arcAngle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            angles[i] = firstAngle + arcStep * i;
        }

        return angles;
    }

    private void OnSingleOrbFinished()
    {
        if (!isActive)
            return;

        activeOrbs--;

        if (activeOrbs > 0)
            return;

        activeOrbs = 0;
        isActive = false;

        if (skill != null &&
            skill.Definition.CooldownStartsAfterDuration)
        {
            skill.StartCooldown(skill.Stats.FinalCooldown);
        }
    }

    private void CleanupInactiveOrbs()
    {
        activeOrbObjects.RemoveAll(
            orb => orb == null || !orb.activeInHierarchy);
    }

    private void ReturnOrbToPool(GameObject orb)
    {
        if (orb == null)
            return;

        PooledBehaviour pooledBehaviour =
            orb.GetComponent<PooledBehaviour>();

        if (pooledBehaviour != null)
        {
            pooledBehaviour.ReturnToPool();
            return;
        }

        PooledObject pooledObject =
            orb.GetComponent<PooledObject>();

        if (pooledObject != null)
            pooledObject.ReturnToPool();
    }

    private void CleanupOrbs()
    {
        for (int i = activeOrbObjects.Count - 1; i >= 0; i--)
        {
            ReturnOrbToPool(activeOrbObjects[i]);
        }

        activeOrbObjects.Clear();
    }

    public void Cleanup()
    {
        CleanupOrbs();

        activeOrbs = 0;
        isActive = false;
    }

    public void ResetExecutor()
    {
        CleanupOrbs();

        skill = null;
        owner = null;

        activeOrbs = 0;
        isActive = false;
    }
}
