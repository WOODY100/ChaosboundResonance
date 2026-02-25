using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class SolarOrbitOrb : MonoBehaviour, IOrbital
{
    [Header("Orbit Settings")]
    [SerializeField] private float baseAngularSpeed = 180f;

    private RuntimeSkill skill;
    private Transform owner;
    private System.Action onFinished;

    private float currentAngle;
    private float lifetime;
    private float duration;
    private float radius;
    private float angularSpeed;
    private Vector3 smoothedCenter;

    private float damage;
    private DamageType damageType;

    private void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void Initialize(
    RuntimeSkill runtimeSkill,
    Transform orbitOwner,
    float startAngle,
    System.Action onOrbitFinished)
    {
        skill = runtimeSkill;
        owner = orbitOwner;
        onFinished = onOrbitFinished;

        currentAngle = startAngle;

        radius = skill.Stats.FinalRange;
        duration = skill.Stats.FinalDuration;
        damage = skill.Stats.FinalDamage;
        damageType = skill.Definition.DamageType;

        smoothedCenter = owner.position;

        float attackSpeedMultiplier = 1f;

        if (skill.Definition.ScalesWithAttackSpeed)
        {
            PlayerStats stats = owner.GetComponent<PlayerStats>();
            if (stats != null)
                attackSpeedMultiplier = stats.FinalAttackSpeed;
        }

        angularSpeed = baseAngularSpeed * attackSpeedMultiplier;
        lifetime = 0f;
    }

    private void Update()
    {
        if (owner == null)
        {
            Destroy(gameObject);
            return;
        }

        lifetime += Time.deltaTime;

        if (lifetime >= duration)
        {
            onFinished?.Invoke();
            Destroy(gameObject);
            return;
        }
    }

    private void LateUpdate()
    {
        if (owner == null)
            return;

        // Suavizar centro orbital
        smoothedCenter = Vector3.Lerp(
            smoothedCenter,
            owner.position,
            15f * Time.deltaTime   // 10–20 es buen rango
        );

        RotateAroundOwner(smoothedCenter);
    }

    private void RotateAroundOwner(Vector3 center)
    {
        currentAngle += angularSpeed * Time.deltaTime;

        float rad = currentAngle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(rad),
            0f,
            Mathf.Sin(rad)
        ) * radius;

        transform.position = center + offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy == null || enemy.IsDead)
            return;

        DamageData damageData = new DamageData(
            damage,
            damageType
        );

        enemy.TakeDamage(damageData);
    }
}