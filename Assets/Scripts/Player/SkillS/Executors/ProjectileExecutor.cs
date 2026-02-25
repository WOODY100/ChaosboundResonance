using UnityEngine;
using System.Collections.Generic;

public class ProjectileExecutor : MonoBehaviour, ISkillExecutor
{
    private RuntimeSkill skill;
    private Transform owner;
    private PlayerStats playerStats;

    private bool isExecuting = false;

    private static readonly Collider[] hitBuffer = new Collider[32];

    public void Initialize(RuntimeSkill runtimeSkill, Transform skillOwner)
    {
        skill = runtimeSkill;
        owner = skillOwner;
        playerStats = owner.GetComponent<PlayerStats>();
    }

    public void Tick(float deltaTime)
    {
        skill.TickCooldown(deltaTime);

        if (isExecuting)
            return;

        if (skill.IsOnCooldown)
            return;

        Execute();

        // 🔥 Si el cooldown NO depende de duración
        if (!skill.Definition.CooldownStartsAfterDuration)
        {
            skill.StartCooldown(skill.Stats.FinalCooldown);
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

        List<Transform> availableTargets = new List<Transform>();

        for (int i = 0; i < hits; i++)
        {
            if (hitBuffer[i] != null)
                availableTargets.Add(hitBuffer[i].transform);
        }

        if (availableTargets.Count == 0)
        {
            isExecuting = false;
            return;
        }

        int projectilesToFire = Mathf.Min(
            skill.Stats.FinalCount,
            availableTargets.Count
        );

        for (int i = 0; i < projectilesToFire; i++)
        {
            Transform target = GetClosestTarget(availableTargets);

            if (target == null)
                break;

            FireProjectile(target);
            availableTargets.Remove(target);
        }

        isExecuting = false;

        // 🔥 Si el cooldown depende de duración (poco común en proyectiles)
        if (skill.Definition.CooldownStartsAfterDuration)
        {
            skill.StartCooldown(skill.Stats.FinalCooldown);
        }
    }

    private Transform GetClosestTarget(List<Transform> targets)
    {
        Transform closest = null;
        float closestDist = float.MaxValue;

        foreach (var t in targets)
        {
            float dist = (t.position - owner.position).sqrMagnitude;

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = t;
            }
        }

        return closest;
    }

    private void FireProjectile(Transform target)
    {
        Vector3 direction =
            (target.position - owner.position).normalized;

        GameObject projectileObj = Instantiate(
            skill.Definition.ExecutionPrefab,
            owner.position,
            Quaternion.LookRotation(direction)
        );

        IProjectile projectile =
            projectileObj.GetComponent<IProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(skill, direction, playerStats);
        }
    }
}