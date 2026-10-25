
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExecutor : MonoBehaviour, ISkillExecutor
{
    private RuntimeSkill skill;
    private Transform owner;
    private PlayerModifierSystem modifierSystem;

    private bool isExecuting;

    private static readonly Collider[] hitBuffer =
        new Collider[32];

    private readonly List<Transform> availableTargets =
        new List<Transform>();

    private readonly List<GameObject> activeProjectiles =
        new List<GameObject>();

    private ProjectilePatternDefinition Pattern
    {
        get
        {
            if (skill == null || skill.Definition == null)
                return null;

            return skill.Definition.ExecutionPattern
                as ProjectilePatternDefinition;
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
                "ProjectileExecutor requires a RuntimeSkill and an owner.",
                this);
            return;
        }

        skill = runtimeSkill;
        owner = skillOwner;

        modifierSystem =
            owner.GetComponent<PlayerModifierSystem>();

        if (modifierSystem == null)
        {
            Debug.LogError(
                "ProjectileExecutor could not find PlayerModifierSystem " +
                "on '" + owner.name + "'.",
                this);
        }
    }

    public void Tick(float deltaTime)
    {
        if (skill == null || owner == null)
            return;

        CleanupInactiveProjectiles();

        skill.TickCooldown(deltaTime);

        if (isExecuting || skill.IsOnCooldown)
            return;

        Execute();

        // Preserve existing immediate-cooldown behavior.
        if (!skill.Definition.CooldownStartsAfterDuration)
        {
            skill.StartCooldown(
                skill.Stats.FinalCooldown);
        }
    }

    private void Execute()
    {
        ProjectilePatternDefinition pattern = Pattern;

        // A missing pattern preserves the original behavior.
        ProjectilePatternType patternType =
            pattern != null
                ? pattern.Pattern
                : ProjectilePatternType.TargetNearest;

        switch (patternType)
        {
            case ProjectilePatternType.TargetNearest:
                ExecuteTargetNearest();
                break;

            case ProjectilePatternType.Fan:
                ExecuteDirectionalPattern(
                    pattern,
                    ProjectilePatternType.Fan);
                break;

            case ProjectilePatternType.Radial:
                ExecuteDirectionalPattern(
                    pattern,
                    ProjectilePatternType.Radial);
                break;

            case ProjectilePatternType.RandomSpread:
                ExecuteDirectionalPattern(
                    pattern,
                    ProjectilePatternType.RandomSpread);
                break;

            case ProjectilePatternType.FixedDirection:
                ExecuteDirectionalPattern(
                    pattern,
                    ProjectilePatternType.FixedDirection);
                break;

            default:
                ExecuteTargetNearest();
                break;
        }
    }

    // =========================================================
    // TARGET NEAREST
    // Preserves the original target-based behavior.
    // =========================================================

    private void ExecuteTargetNearest()
    {
        isExecuting = true;

        FindAvailableTargets();

        if (availableTargets.Count == 0)
        {
            isExecuting = false;
            return;
        }

        int projectileCount = Mathf.Min(
            Mathf.Max(0, skill.Stats.FinalCount),
            availableTargets.Count);

        for (int i = 0; i < projectileCount; i++)
        {
            Transform target =
                GetClosestTarget(availableTargets);

            if (target == null)
                break;

            Vector3 direction =
                target.position - owner.position;

            if (direction.sqrMagnitude > 0.0001f)
                FireProjectile(direction.normalized);

            availableTargets.Remove(target);
        }

        isExecuting = false;

        StartDeferredCooldownIfNeeded();
    }

    private void FindAvailableTargets()
    {
        availableTargets.Clear();

        int hits = Physics.OverlapSphereNonAlloc(
            owner.position,
            skill.Stats.FinalRange,
            hitBuffer,
            LayerMask.GetMask("Enemy"));

        for (int i = 0; i < hits; i++)
        {
            Collider hit = hitBuffer[i];

            if (hit == null)
                continue;

            IDamageable damageable =
                hit.GetComponentInParent<IDamageable>();

            if (damageable == null || damageable.IsDead)
                continue;

            // Use the damageable component's transform so that
            // multiple colliders on one enemy do not duplicate it.
            Component targetComponent =
                damageable as Component;

            if (targetComponent == null)
                continue;

            Transform target = targetComponent.transform;

            if (!availableTargets.Contains(target))
                availableTargets.Add(target);
        }
    }

    private Transform GetClosestTarget(
        List<Transform> targets)
    {
        Transform closest = null;
        float closestDistance = float.MaxValue;

        foreach (Transform target in targets)
        {
            if (target == null)
                continue;

            float distance =
                (target.position - owner.position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = target;
            }
        }

        return closest;
    }

    // =========================================================
    // CONFIGURABLE DIRECTIONAL PATTERNS
    // These patterns can fire without nearby enemies.
    // =========================================================

    private void ExecuteDirectionalPattern(
        ProjectilePatternDefinition pattern,
        ProjectilePatternType patternType)
    {
        if (pattern == null)
        {
            ExecuteTargetNearest();
            return;
        }

        int count = Mathf.Max(0, skill.Stats.FinalCount);

        if (count == 0)
            return;

        isExecuting = true;

        Vector3 baseDirection = GetBaseDirection(pattern);

        Debug.Log(
            $"[ProjectilePattern] Pattern={patternType}, " +
            $"OwnerForward={owner.forward}, " +
            $"BaseDirection={baseDirection}, " +
            $"Count={count}, " +
            $"Spread={pattern.SpreadAngle}");

        for (int i = 0; i < count; i++)
        {
            Vector3 direction = GetPatternDirection(
                pattern,
                patternType,
                baseDirection,
                i,
                count);

            if (direction.sqrMagnitude > 0.0001f)
                FireProjectile(direction.normalized);

            Debug.Log(
            $"[ProjectilePattern] Projectile {i + 1}/{count}: " +
            $"Direction={direction}");
        }

        isExecuting = false;

        StartDeferredCooldownIfNeeded();
    }

    private Vector3 GetBaseDirection(
        ProjectilePatternDefinition pattern)
    {
        if (pattern.AimAtNearestTarget)
        {
            FindAvailableTargets();

            Transform target =
                GetClosestTarget(availableTargets);

            if (target != null)
            {
                Vector3 toTarget =
                    target.position - owner.position;

                // Directional patterns operate on the horizontal plane.
                toTarget.y = 0f;

                if (toTarget.sqrMagnitude > 0.0001f)
                    return toTarget.normalized;
            }
        }

        Vector3 forward = owner.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.0001f)
            forward = Vector3.forward;

        return forward.normalized;
    }

    private Vector3 GetPatternDirection(
        ProjectilePatternDefinition pattern,
        ProjectilePatternType patternType,
        Vector3 baseDirection,
        int index,
        int count)
    {
        switch (patternType)
        {
            case ProjectilePatternType.Fan:
                {
                    float angle = 0f;

                    if (count > 1)
                    {
                        float step =
                            pattern.SpreadAngle / (count - 1);

                        angle =
                            -pattern.SpreadAngle * 0.5f +
                            step * index;
                    }

                    return Quaternion.AngleAxis(
                        angle,
                        Vector3.up) * baseDirection;
                }

            case ProjectilePatternType.Radial:
                {
                    float angle = 360f * index / count;

                    return Quaternion.AngleAxis(
                        angle,
                        Vector3.up) * baseDirection;
                }

            case ProjectilePatternType.RandomSpread:
                {
                    float angle = Random.Range(
                        -pattern.RandomSpreadAngle,
                        pattern.RandomSpreadAngle);

                    return Quaternion.AngleAxis(
                        angle,
                        Vector3.up) * baseDirection;
                }

            case ProjectilePatternType.FixedDirection:
                {
                    Vector3 direction =
                        owner.TransformDirection(
                            pattern.LocalDirection);

                    direction.y = 0f;

                    if (direction.sqrMagnitude < 0.0001f)
                        return baseDirection;

                    return direction.normalized;
                }

            default:
                return baseDirection;
        }
    }

    // =========================================================
    // PROJECTILE SPAWNING
    // =========================================================

    private void FireProjectile(Vector3 direction)
    {
        if (skill == null ||
            owner == null ||
            skill.Definition.ExecutionPrefab == null)
        {
            return;
        }

        if (PoolManager.Instance == null)
        {
            Debug.LogError(
                "ProjectileExecutor could not find PoolManager.",
                this);
            return;
        }

        Quaternion rotation =
            Quaternion.LookRotation(direction);

        GameObject projectileObject =
            PoolManager.Instance.Get(
                skill.Definition.ExecutionPrefab,
                owner.position,
                rotation);

        if (projectileObject == null)
            return;

        IProjectile projectile =
            projectileObject.GetComponent<IProjectile>();

        if (projectile == null)
        {
            Debug.LogError(
                "Projectile prefab '" +
                projectileObject.name +
                "' does not implement IProjectile.",
                projectileObject);

            ReturnProjectileToPool(projectileObject);
            return;
        }

        activeProjectiles.Add(projectileObject);

        projectile.Initialize(
            skill,
            direction,
            modifierSystem);
    }

    private void StartDeferredCooldownIfNeeded()
    {
        if (skill != null &&
            skill.Definition.CooldownStartsAfterDuration)
        {
            skill.StartCooldown(
                skill.Stats.FinalCooldown);
        }
    }

    // =========================================================
    // POOLING / LIFECYCLE
    // =========================================================

    private void CleanupInactiveProjectiles()
    {
        activeProjectiles.RemoveAll(
            projectile =>
                projectile == null ||
                !projectile.activeInHierarchy);
    }

    private void ReturnProjectileToPool(
        GameObject projectile)
    {
        if (projectile == null)
            return;

        PooledBehaviour pooledBehaviour =
            projectile.GetComponent<PooledBehaviour>();

        if (pooledBehaviour != null)
        {
            pooledBehaviour.ReturnToPool();
            return;
        }

        PooledObject pooledObject =
            projectile.GetComponent<PooledObject>();

        if (pooledObject != null)
        {
            pooledObject.ReturnToPool();
            return;
        }

        Destroy(projectile);
    }

    public void Cleanup()
    {
        CleanupProjectiles();
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

    private void CleanupProjectiles()
    {
        for (int i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            GameObject projectile =
                activeProjectiles[i];

            if (projectile == null)
                continue;

            ReturnProjectileToPool(projectile);
        }

        activeProjectiles.Clear();
    }
}
