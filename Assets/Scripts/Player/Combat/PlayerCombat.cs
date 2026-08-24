using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerCombat : MonoBehaviour
{
    public Transform CurrentTarget { get; private set; }
    public bool IsAttacking { get; private set; }

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackAngle = 60f;
    [SerializeField] private float attackRotationSpeed = 720f;

    [Header("Cooldown")]
    [SerializeField] private CooldownComponent autoAttackCooldown;
    [SerializeField] private float baseAttackCooldown = 0.9f;

    [Header("Targeting")]
    [SerializeField] private LayerMask enemyLayer;

    [Header("Presentation")]
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private Transform attackSpawnPoint;

    private Animator animator;
    private PlayerStats playerStats;
    private PlayerModifierSystem modifierSystem;
    private PlayerController controller;

    private static readonly Collider[] attackHits =
        new Collider[64];

    private void Awake()
    {
        animator =
            GetComponentInChildren<Animator>();

        playerStats =
            GetComponent<PlayerStats>();

        modifierSystem =
            GetComponent<PlayerModifierSystem>();

        controller =
            GetComponent<PlayerController>();

        ValidateReferences();

        if (autoAttackCooldown != null)
        {
            autoAttackCooldown.SetBaseCooldown(
                baseAttackCooldown);
        }
    }

    private void Update()
    {
        if (!CanProcessCombat())
            return;

        UpdateAttackSpeed();

        autoAttackCooldown.Tick(
            Time.deltaTime);

        IDamageable target =
            FindBestTarget();

        UpdateCurrentTarget(target);

        if (CurrentTarget != null)
        {
            RotateTowards(CurrentTarget);
        }

        if (target != null)
        {
            TryExecuteAttack(target);
        }
    }

    // =========================================================
    // COMBAT STATE
    // =========================================================

    private bool CanProcessCombat()
    {
        if (GameStateManager.Instance == null)
            return false;

        if (GameStateManager.Instance.CurrentState !=
            GameState.Playing)
        {
            return false;
        }

        if (controller != null &&
            controller.IsDashing)
        {
            return false;
        }

        return true;
    }

    private void UpdateAttackSpeed()
    {
        if (modifierSystem == null ||
            autoAttackCooldown == null)
        {
            return;
        }

        float attackSpeed =
            Mathf.Max(
                0.01f,
                modifierSystem.GetStat(
                    StatType.AttackSpeed));

        autoAttackCooldown.CooldownMultiplier =
            1f / attackSpeed;

        if (animator != null)
        {
            animator.SetFloat(
                "AttackSpeed",
                attackSpeed);
        }
    }

    private void UpdateCurrentTarget(
        IDamageable target)
    {
        if (target == null)
        {
            CurrentTarget = null;
            return;
        }

        MonoBehaviour targetBehaviour =
            target as MonoBehaviour;

        if (targetBehaviour == null)
        {
            CurrentTarget = null;
            return;
        }

        CurrentTarget =
            targetBehaviour.transform;
    }

    // =========================================================
    // TARGETING
    // =========================================================

    private IDamageable FindBestTarget()
    {
        int hitCount =
            Physics.OverlapSphereNonAlloc(
                transform.position,
                attackRange,
                attackHits,
                enemyLayer);

        if (hitCount <= 0)
            return null;

        IDamageable bestTarget = null;
        float bestScore = float.MinValue;

        bool surrounded =
            hitCount > 4;

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit =
                attackHits[i];

            if (hit == null)
                continue;

            IDamageable damageable =
                hit.GetComponentInParent<IDamageable>();

            if (damageable == null ||
                damageable.IsDead)
            {
                continue;
            }

            MonoBehaviour targetBehaviour =
                damageable as MonoBehaviour;

            if (targetBehaviour == null)
                continue;

            Vector3 toTarget =
                targetBehaviour.transform.position -
                transform.position;

            toTarget.y = 0f;

            float distanceSqr =
                toTarget.sqrMagnitude;

            if (distanceSqr <= 0.0001f)
                continue;

            float distance =
                Mathf.Sqrt(distanceSqr);

            Vector3 direction =
                toTarget / distance;

            float dot =
                Vector3.Dot(
                    transform.forward,
                    direction);

            float score = surrounded
                ? 1f / distance
                : dot * 2f + (1f / distance);

            if (score <= bestScore)
                continue;

            bestScore = score;
            bestTarget = damageable;
        }

        return bestTarget;
    }

    // =========================================================
    // ATTACK
    // =========================================================

    private void TryExecuteAttack(
        IDamageable target)
    {
        if (autoAttackCooldown == null)
            return;

        if (!autoAttackCooldown.IsReady)
            return;

        if (IsAttacking)
            return;

        ExecuteAttack(target);
    }

    private void ExecuteAttack(
        IDamageable target)
    {
        if (target == null)
            return;

        MonoBehaviour targetBehaviour =
            target as MonoBehaviour;

        if (targetBehaviour == null)
            return;

        autoAttackCooldown.Trigger();

        IsAttacking = true;

        CurrentTarget =
            targetBehaviour.transform;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    // =========================================================
    // ROTATION
    // =========================================================

    private void RotateTowards(
        Transform target)
    {
        if (target == null)
            return;

        Vector3 direction =
            target.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction);

        transform.rotation =
            Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                attackRotationSpeed *
                Time.deltaTime);
    }

    // =========================================================
    // DAMAGE
    // =========================================================

    // Animation Event
    public void DealDamageInCone()
    {
        if (!IsAttacking)
            return;

        int hitCount =
            Physics.OverlapSphereNonAlloc(
                transform.position,
                attackRange,
                attackHits,
                enemyLayer);

        if (hitCount <= 0)
            return;

        HashSet<IDamageable> damaged =
            new HashSet<IDamageable>();

        float cosHalfAngle =
            Mathf.Cos(
                attackAngle *
                0.5f *
                Mathf.Deg2Rad);

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit =
                attackHits[i];

            if (hit == null)
                continue;

            IDamageable damageable =
                hit.GetComponentInParent<IDamageable>();

            if (damageable == null ||
                damageable.IsDead ||
                damaged.Contains(damageable))
            {
                continue;
            }

            MonoBehaviour targetBehaviour =
                damageable as MonoBehaviour;

            if (targetBehaviour == null)
                continue;

            Vector3 toTarget =
                targetBehaviour.transform.position -
                transform.position;

            toTarget.y = 0f;

            if (toTarget.sqrMagnitude < 0.04f)
            {
                ApplyDamage(damageable);
                damaged.Add(damageable);
                continue;
            }

            toTarget.Normalize();

            float dot =
                Vector3.Dot(
                    transform.forward,
                    toTarget);

            if (dot >= cosHalfAngle)
            {
                ApplyDamage(damageable);
                damaged.Add(damageable);
            }
        }
    }

    private void ApplyDamage(
        IDamageable target)
    {
        if (target == null)
            return;

        if (modifierSystem == null ||
            playerStats == null)
        {
            return;
        }

        float damageAmount =
            Mathf.Max(
                0f,
                modifierSystem.GetStat(
                    StatType.Damage));

        DamageData damage =
            new DamageData(
                damageAmount,
                playerStats.CurrentDamageType);

        target.TakeDamage(damage);
    }

    // =========================================================
    // PRESENTATION
    // =========================================================

    // Animation Event
    public void SpawnSlash()
    {
        if (slashPrefab == null ||
            attackSpawnPoint == null)
        {
            return;
        }

        if (PoolManager.Instance == null)
            return;

        GameObject slash =
            PoolManager.Instance.Get(
                slashPrefab,
                attackSpawnPoint.position,
                attackSpawnPoint.rotation);

        if (slash == null)
            return;

        slash.transform.SetParent(transform);

        SlashVFX vfx =
            slash.GetComponent<SlashVFX>();

        if (vfx != null &&
            playerStats != null)
        {
            vfx.SetColor(
                DamageVisuals.GetColor(
                    playerStats.CurrentDamageType));
        }
    }

    // =========================================================
    // ATTACK END / CANCELLATION
    // =========================================================

    // Animation Event
    public void EndAttack()
    {
        IsAttacking = false;
        CurrentTarget = null;
    }

    public void CancelAttack()
    {
        IsAttacking = false;
        CurrentTarget = null;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
        }
    }

    // =========================================================
    // VALIDATION
    // =========================================================

    private void ValidateReferences()
    {
        if (animator == null)
        {
            Debug.LogError(
                $"{name}: PlayerCombat requires an Animator.",
                this);
        }

        if (playerStats == null)
        {
            Debug.LogError(
                $"{name}: PlayerCombat requires PlayerStats.",
                this);
        }

        if (modifierSystem == null)
        {
            Debug.LogError(
                $"{name}: PlayerCombat requires PlayerModifierSystem.",
                this);
        }

        if (controller == null)
        {
            Debug.LogError(
                $"{name}: PlayerCombat requires PlayerController.",
                this);
        }

        if (autoAttackCooldown == null)
        {
            Debug.LogError(
                $"{name}: Auto Attack Cooldown is not assigned.",
                this);
        }

        if (slashPrefab == null)
        {
            Debug.LogWarning(
                $"{name}: Slash Prefab is not assigned.",
                this);
        }

        if (attackSpawnPoint == null)
        {
            Debug.LogWarning(
                $"{name}: Attack Spawn Point is not assigned.",
                this);
        }

        if (enemyLayer.value == 0)
        {
            Debug.LogWarning(
                $"{name}: Enemy LayerMask is empty.",
                this);
        }
    }

    // =========================================================
    // GIZMOS
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange);

        Vector3 forward =
            transform.forward;

        Quaternion leftRotation =
            Quaternion.AngleAxis(
                -attackAngle * 0.5f,
                Vector3.up);

        Quaternion rightRotation =
            Quaternion.AngleAxis(
                attackAngle * 0.5f,
                Vector3.up);

        Vector3 leftDirection =
            leftRotation * forward;

        Vector3 rightDirection =
            rightRotation * forward;

        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(
            transform.position,
            leftDirection * attackRange);

        Gizmos.DrawRay(
            transform.position,
            rightDirection * attackRange);
    }

    // =========================================================
    // DAMAGE VISUALS
    // =========================================================

    public static class DamageVisuals
    {
        public static Color GetColor(
            DamageType type)
        {
            switch (type)
            {
                case DamageType.Fire:
                    return new Color(
                        1f,
                        0.3f,
                        0f);

                case DamageType.Poison:
                    return new Color(
                        0.2f,
                        1f,
                        0.2f);

                case DamageType.Chaos:
                    return new Color(
                        0.6f,
                        0f,
                        1f);

                default:
                    return Color.white;
            }
        }
    }
}