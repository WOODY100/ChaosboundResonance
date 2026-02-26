using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 30f;

    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<EnemyHealth> OnDeath;
    public event Action<float> OnDamageTaken;

    private NavMeshAgent agent;
    private Collider mainCollider;

    private EnemyPool pool;
    private EnemyBrain brain;
    private EnemyAttack attack;
    private EnemyMovementArena movement;
    private EnemyStats stats;
    private Animator animator;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCollider = GetComponent<Collider>();

        brain = GetComponent<EnemyBrain>();
        attack = GetComponent<EnemyAttack>();
        movement = GetComponent<EnemyMovementArena>();
        pool = GetComponentInParent<EnemyPool>();
        stats = GetComponent<EnemyStats>();
        animator = GetComponentInChildren<Animator>();


        Initialize(maxHealth);
    }

    void OnEnable()
    {
        Initialize(maxHealth);

        if (animator != null)
            animator.ResetTrigger("Die");
    }

    // 🔹 Preparado para pooling
    public void Initialize(float healthValue)
    {
        IsDead = false;

        if (stats != null)
            CurrentHealth = stats.CurrentHealth;

        if (agent != null)
            agent.enabled = true;

        if (brain != null)
            brain.enabled = true;

        if (attack != null)
            attack.enabled = true;

        if (movement != null)
            movement.enabled = true;

        if (mainCollider != null)
            mainCollider.enabled = true;
    }

    public void TakeDamage(DamageData damageData)
    {
        if (IsDead)
            return;

        float finalDamage =
            DamageProcessor.CalculateDamage(this, damageData);

        if (finalDamage <= 0f)
            return;

        CurrentHealth -= finalDamage;

        OnDamageTaken?.Invoke(finalDamage);

        FloatingDamageManager.Instance?.ShowDamage(
            transform.position,
            finalDamage,
            false
        );

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        // 🔒 Detener sistemas
        if (brain != null)
            brain.enabled = false;

        if (attack != null)
            attack.enabled = false;

        if (attack != null)
            attack.CancelAttack();

        if (agent != null)
            agent.enabled = false;

        if (mainCollider != null)
            mainCollider.enabled = false;

        if (movement != null)
            movement.enabled = false;

        // 🎬 Animación
        if (animator != null)
            animator.SetTrigger("Die");

        OnDeath?.Invoke(this);

        StartCoroutine(DeathRoutine());
    }

    System.Collections.IEnumerator DeathRoutine()
    {
        // Duración animación (ajústalo si quieres)
        yield return new WaitForSeconds(3f);

        pool.Return(gameObject);
    }
}