using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public sealed class EnemyProjectile : PooledBehaviour
{
    private Rigidbody rb;

    private Transform owner;
    private Vector3 direction;

    private float speed;
    private float damage;
    private DamageType damageType;

    private float lifetimeRemaining;

    private bool initialized;
    private bool hasImpacted;

    private int playerLayer;
    private int obstacleLayer;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();

        playerLayer = LayerMask.NameToLayer("Player");
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
    }

    protected override void ResetPooledState()
    {
        owner = null;

        direction = Vector3.zero;

        speed = 0f;
        damage = 0f;
        damageType = default;

        lifetimeRemaining = 0f;

        initialized = false;
        hasImpacted = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void Initialize(
        Transform projectileOwner,
        Vector3 projectileDirection,
        float projectileSpeed,
        float lifetime,
        float projectileDamage,
        DamageType projectileDamageType)
    {
        ResetPooledState();

        if (projectileOwner == null)
        {
            Debug.LogError(
                "EnemyProjectile initialized with a null owner.");

            ReturnToPool();
            return;
        }

        if (projectileDirection.sqrMagnitude < 0.0001f)
        {
            Debug.LogError(
                "EnemyProjectile initialized with an invalid direction.");

            ReturnToPool();
            return;
        }

        if (projectileSpeed <= 0f)
        {
            Debug.LogError(
                "EnemyProjectile initialized with an invalid speed.");

            ReturnToPool();
            return;
        }

        if (lifetime <= 0f)
        {
            Debug.LogError(
                "EnemyProjectile initialized with an invalid lifetime.");

            ReturnToPool();
            return;
        }

        if (rb == null)
        {
            Debug.LogError(
                "EnemyProjectile requires a Rigidbody.");

            ReturnToPool();
            return;
        }

        owner = projectileOwner;

        direction = projectileDirection.normalized;

        speed = projectileSpeed;
        lifetimeRemaining = lifetime;

        damage = projectileDamage;
        damageType = projectileDamageType;

        transform.forward = direction;

        rb.linearVelocity =
            direction * speed;

        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
            return;

        lifetimeRemaining -= Time.deltaTime;

        if (lifetimeRemaining <= 0f)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!initialized || hasImpacted)
            return;

        if (other == null)
            return;

        if (owner != null &&
            other.transform.IsChildOf(owner))
        {
            return;
        }


        if (other.gameObject.layer == obstacleLayer)
        {

            hasImpacted = true;
            ReturnToPool();
            return;
        }

        if (other.gameObject.layer != playerLayer)
        {
            return;
        }

        IDamageable damageable =
            other.GetComponentInParent<IDamageable>();

        if (damageable == null)
            return;

        if (damageable.IsDead)
            return;

        hasImpacted = true;

        DamageData damageData =
            new DamageData(
                damage,
                damageType);

        damageData.source =
            owner != null
                ? owner.gameObject
                : null;

        damageable.TakeDamage(damageData);

        ReturnToPool();
    }
}