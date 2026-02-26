using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    public Transform CurrentTarget { get; private set; }
    public bool IsAttacking { get; private set; }

    [Header("Attack Settings")]
    public float attackRange = 3f;
    public float attackAngle = 60f;
    public float attackRotationSpeed = 720f;

    [Header("Cooldown")]
    [SerializeField] private CooldownComponent autoAttackCooldown;

    public LayerMask enemyLayer;
    public GameObject slashPrefab;
    public Transform attackSpawnPoint;

    private Animator animator;
    private PlayerStats stats;

    private Coroutine rotationCoroutine;

    private static Collider[] attackHits = new Collider[64];

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        stats = GetComponent<PlayerStats>();

        if (stats == null)
            Debug.LogError("PlayerStats NOT FOUND");

        if (autoAttackCooldown == null)
            Debug.LogError("CooldownComponent NOT ASSIGNED");

        autoAttackCooldown.SetBaseCooldown(0.9f);
    }

    void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Playing)
            return;

        if (CurrentTarget != null)
        {
            RotateTowards(CurrentTarget);
        }

        autoAttackCooldown.CooldownMultiplier =
    1f / Mathf.Max(0.01f, stats.FinalAttackSpeed);

        autoAttackCooldown.Tick(Time.deltaTime);

        animator.SetFloat("AttackSpeed", stats.FinalAttackSpeed);

        UpdateTarget();       // 🔥 nuevo
        HandleAutoAttack();
    }

    void UpdateTarget()
    {
        IDamageable target = FindBestTarget();

        if (target != null)
            CurrentTarget = ((MonoBehaviour)target).transform;
        else
            CurrentTarget = null;
    }

    void HandleAutoAttack()
    {
        if (!autoAttackCooldown.IsReady)
            return;

        IDamageable target = FindBestTarget();
        if (target == null)
            return;

        ExecuteAttack(target);
    }

    void HandleDynamicRetarget()
    {
        if (!IsAttacking)
            return;

        IDamageable newTarget = FindBestTarget();

        if (newTarget == null)
            return;

        Transform newTransform = ((MonoBehaviour)newTarget).transform;

        if (CurrentTarget == null)
        {
            CurrentTarget = newTransform;
            return;
        }

        float currentDist = Vector3.SqrMagnitude(
            CurrentTarget.position - transform.position);

        float newDist = Vector3.SqrMagnitude(
            newTransform.position - transform.position);

        if (newDist + 0.1f < currentDist)
        {
            CurrentTarget = newTransform;
        }
    }

    IDamageable FindBestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            attackRange,
            enemyLayer
        );

        if (hits.Length == 0)
            return null;

        IDamageable bestTarget = null;
        float bestScore = float.MinValue;

        bool surrounded = hits.Length > 4;

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable == null || damageable.IsDead)
                continue;

            Transform enemyTransform = hit.GetComponentInParent<Transform>();

            Vector3 toEnemy = enemyTransform.position - transform.position;
            toEnemy.y = 0f;

            float distance = toEnemy.magnitude;
            if (distance <= 0.01f)
                continue;

            Vector3 dir = toEnemy.normalized;
            float dot = Vector3.Dot(transform.forward, dir);

            float score = surrounded
                ? 1f / distance
                : dot * 2f + (1f / distance);

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = damageable;
            }
        }

        return bestTarget;
    }

    void RotateTowards(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            attackRotationSpeed * Time.deltaTime
        );
    }

    void ExecuteAttack(IDamageable target)
    {
        autoAttackCooldown.Trigger();

        IsAttacking = true;
        CurrentTarget = ((MonoBehaviour)target).transform;

        animator.SetTrigger("Attack");
    }

    // 🔥 Animation Event
    public void SpawnSlash()
    {
        if (slashPrefab == null) return;

        GameObject slash = Instantiate(
            slashPrefab,
            attackSpawnPoint.position,
            attackSpawnPoint.rotation,
            transform
        );

        SlashVFX vfx = slash.GetComponent<SlashVFX>();
        if (vfx != null && stats != null)
            vfx.SetColor(DamageVisuals.GetColor(stats.CurrentDamageType));
    }

    // 🔥 Animation Event
    public void DealDamageInCone()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            attackRange,
            attackHits,
            enemyLayer
        );

        HashSet<IDamageable> damaged = new HashSet<IDamageable>();
        float cosHalfAngle = Mathf.Cos(attackAngle * 0.5f * Mathf.Deg2Rad);

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = attackHits[i];
            if (hit == null) continue;

            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable == null || damageable.IsDead || damaged.Contains(damageable))
                continue;

            Transform targetTransform = hit.GetComponentInParent<Transform>();
            Vector3 toTarget = targetTransform.position - transform.position;
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude < 0.04f)
            {
                ApplyDamage(damageable);
                damaged.Add(damageable);
                continue;
            }

            toTarget.Normalize();
            float dot = Vector3.Dot(transform.forward, toTarget);

            if (dot >= cosHalfAngle)
            {
                ApplyDamage(damageable);
                damaged.Add(damageable);
            }
        }
    }

    void ApplyDamage(IDamageable target)
    {
        if (stats == null) return;

        float damageAmount = stats.FinalDamage;

        DamageData damage = new DamageData(
            damageAmount,
            stats.CurrentDamageType
        );

        target.TakeDamage(damage);
    }

    // 🔥 Animation Event (solo libera animación, NO cooldown)
    public void EndAttack()
    {
        IsAttacking = false;
        CurrentTarget = null;
    }

    public static class DamageVisuals
    {
        public static Color GetColor(DamageType type)
        {
            switch (type)
            {
                case DamageType.Fire: return new Color(1f, 0.3f, 0f);
                case DamageType.Poison: return new Color(0.2f, 1f, 0.2f);
                case DamageType.Chaos: return new Color(0.6f, 0f, 1f);
                default: return Color.white;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 forward = transform.forward;

        Quaternion leftRayRotation = Quaternion.AngleAxis(-attackAngle / 2f, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(attackAngle / 2f, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftRayDirection * attackRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * attackRange);
    }
}