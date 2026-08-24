using UnityEngine;
using System.Collections.Generic;

public class ProjectileExecutor : MonoBehaviour, ISkillExecutor
{
    private RuntimeSkill skill;
    private Transform owner;
    private PlayerModifierSystem modifierSystem;

    private bool isExecuting = false;

    private static readonly Collider[] hitBuffer =
        new Collider[32];

    private readonly List<Transform> availableTargets =
        new();

    private readonly List<GameObject> activeProjectiles =
        new();

    public void Initialize(
        RuntimeSkill runtimeSkill,
        Transform skillOwner)
    {
        ResetExecutor();

        skill = runtimeSkill;
        owner = skillOwner;

        modifierSystem =
            owner.GetComponent<PlayerModifierSystem>();

        if (modifierSystem == null)
        {
            Debug.LogError(
                $"ProjectileExecutor could not find PlayerModifierSystem on '{owner.name}'.");
        }
    }

    public void Tick(float deltaTime)
    {
        if (skill == null || owner == null)
            return;

        CleanupInactiveProjectiles();

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

        int hits = Physics.OverlapSphereNonAlloc(
            owner.position,
            skill.Stats.FinalRange,
            hitBuffer,
            LayerMask.GetMask("Enemy")
        );

        if (hits == 0)
        {
            isExecuting = false;
            return;
        }

        availableTargets.Clear();

        for (int i = 0; i < hits; i++)
        {
            if (hitBuffer[i] != null)
                availableTargets.Add(
                    hitBuffer[i].transform);
        }

        if (availableTargets.Count == 0)
        {
            isExecuting = false;
            return;
        }

        int projectilesToFire =
            Mathf.Min(
                skill.Stats.FinalCount,
                availableTargets.Count);

        for (int i = 0; i < projectilesToFire; i++)
        {
            Transform target =
                GetClosestTarget(
                    availableTargets);

            if (target == null)
                break;

            FireProjectile(target);

            availableTargets.Remove(target);
        }

        isExecuting = false;

        if (skill.Definition.CooldownStartsAfterDuration)
        {
            skill.StartCooldown(
                skill.Stats.FinalCooldown);
        }
    }

    private Transform GetClosestTarget(
        List<Transform> targets)
    {
        Transform closest = null;
        float closestDist = float.MaxValue;

        foreach (Transform target in targets)
        {
            if (target == null)
                continue;

            float distance =
                (target.position - owner.position)
                .sqrMagnitude;

            if (distance < closestDist)
            {
                closestDist = distance;
                closest = target;
            }
        }

        return closest;
    }

    private void FireProjectile(Transform target)
    {
        Vector3 direction =
            (target.position - owner.position).normalized;

        GameObject projectileObj =
            PoolManager.Instance.Get(
                skill.Definition.ExecutionPrefab,
                owner.position,
                Quaternion.LookRotation(direction)
            );

        if (projectileObj == null)
            return;

        TrackProjectile(projectileObj);

        IProjectile projectile =
            projectileObj.GetComponent<IProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(
                skill,
                direction,
                modifierSystem);
        }
    }

    private void CleanupInactiveProjectiles()
    {
        activeProjectiles.RemoveAll(
            projectile =>
                projectile == null ||
                !projectile.activeInHierarchy);
    }

    public void Cleanup()
    {
        foreach (GameObject projectile
            in activeProjectiles)
        {
            if (projectile == null)
                continue;

            PooledObject pooledObject =
                projectile.GetComponent<PooledObject>();

            if (pooledObject != null)
            {
                pooledObject.ReturnToPool();
            }
        }

        activeProjectiles.Clear();

        isExecuting = false;
    }

    public void ResetExecutor()
    {
        CleanupProjectiles();

        skill = null;
        owner = null;
        modifierSystem = null;

        isExecuting = false;
    }

    private void TrackProjectile(
    GameObject projectile)
    {
        if (projectile == null)
            return;

        activeProjectiles.Add(projectile);
    }

    private void CleanupProjectiles()
    {
        for (int i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            GameObject projectile =
                activeProjectiles[i];

            if (projectile == null)
            {
                activeProjectiles.RemoveAt(i);
                continue;
            }

            if (!projectile.activeInHierarchy)
            {
                activeProjectiles.RemoveAt(i);
                continue;
            }

            PooledBehaviour pooledBehaviour =
                projectile.GetComponent<PooledBehaviour>();

            if (pooledBehaviour != null)
            {
                pooledBehaviour.ReturnToPool();
            }

            activeProjectiles.RemoveAt(i);
        }
    }
}