using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;

    private EnemyCore core;
    private EnemyStats stats;
    private float nextAttackTime;
    private Animator animator;
    private bool isAttacking;
    public bool IsAttacking => isAttacking;

    void Awake()
    {
        core = GetComponent<EnemyCore>();
        stats = GetComponent<EnemyStats>();
        animator = GetComponentInChildren<Animator>();
    }

    public void Tick()
    {
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
        if (core.Player == null)
            return;

        float sqrAttackRange = attackRange * attackRange;

        // 🔥 Revalidar distancia al momento del impacto
        float currentDistance = (core.Player.position - transform.position).sqrMagnitude;

        if (currentDistance > sqrAttackRange)
            return; // El player se alejó → no hay daño

        PlayerDamageReceiver receiver = core.Player.GetComponent<PlayerDamageReceiver>();

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
}