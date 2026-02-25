using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;

    private EnemyCore core;
    private EnemyStats stats;
    private EnemyHealth health;
    private Animator animator;
    private float nextAttackTime;
    private bool isAttacking;
    public bool IsAttacking => isAttacking;

    void Awake()
    {
        core = GetComponent<EnemyCore>();
        stats = GetComponent<EnemyStats>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<EnemyHealth>();
    }

    public void Tick()
    {
        if (health != null && health.IsDead)
            return;

        if (core.Player == null)
            return;

        if (isAttacking)
            return;

        float sqrAttackRange = attackRange * attackRange;

        if (core.DistanceToPlayer > sqrAttackRange)
            return;

        if (Time.time < nextAttackTime)
        {
            return;
        }

        ExecuteAttack();
    }

    void ExecuteAttack()
    {
        nextAttackTime = Time.time + attackCooldown;
        isAttacking = true;

        if (animator != null)
            animator.SetTrigger("Attack");
    }

    public void DealDamage()
    {
        if (health != null && health.IsDead)
            return;

        if (core.Player == null)
            return;

        float sqrAttackRange = attackRange * attackRange;

        float currentDistance =
            (core.Player.position - transform.position).sqrMagnitude;

        if (currentDistance > sqrAttackRange)
            return;

        PlayerDamageReceiver receiver =
            core.Player.GetComponent<PlayerDamageReceiver>();

        if (receiver == null)
            return;

        DamageData damage = new DamageData
        {
            amount = stats.Damage,
            type = DamageType.Physical,
            source = gameObject
        };

        receiver.ReceiveDamage(damage);
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void CancelAttack()
    {
        isAttacking = false;

        if (animator != null)
            animator.ResetTrigger("Attack");
    }
}