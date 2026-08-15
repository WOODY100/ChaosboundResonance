using System;
using System.Collections;
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

    private EnemyBrain brain;
    private EnemyAttack attack;
    private EnemyMovementArena movement;
    private EnemyStats stats;
    private Animator animator;
    private PooledObject pooledObject;

    private Coroutine deathRoutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCollider = GetComponent<Collider>();

        brain = GetComponent<EnemyBrain>();
        attack = GetComponent<EnemyAttack>();
        movement = GetComponent<EnemyMovementArena>();
        stats = GetComponent<EnemyStats>();
        animator = GetComponentInChildren<Animator>();
        pooledObject = GetComponent<PooledObject>();

        Initialize(maxHealth);
    }

    private void OnEnable()
    {
        if (deathRoutine != null)
        {
            StopCoroutine(deathRoutine);
            deathRoutine = null;
        }

        Initialize(maxHealth);

        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.Play("Idle", 0, 0f);
        }
    }

    public void Initialize(float healthValue)
    {
        IsDead = false;

        if (stats != null)
            CurrentHealth = stats.CurrentHealth;
        else
            CurrentHealth = healthValue;

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

        float finalDamage = DamageProcessor.CalculateDamage(this, damageData);

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
            Die();
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        if (brain != null)
            brain.enabled = false;

        if (attack != null)
        {
            attack.CancelAttack();
            attack.enabled = false;
        }

        if (agent != null)
            agent.enabled = false;

        if (mainCollider != null)
            mainCollider.enabled = false;

        if (movement != null)
            movement.enabled = false;

        if (animator != null)
            animator.SetTrigger("Die");

        OnDeath?.Invoke(this);

        deathRoutine = StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(3f);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (pooledObject == null)
            pooledObject = GetComponent<PooledObject>();

        if (pooledObject != null)
            pooledObject.ReturnToPool();
        else
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (deathRoutine != null)
        {
            StopCoroutine(deathRoutine);
            deathRoutine = null;
        }
    }
}