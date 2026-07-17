using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class SolarOrbitOrb : PooledBehaviour, IOrbital
{
    [Header("Orbit Height")]
    [SerializeField] private float orbitHeight = 1.2f;

    [Header("Orbit Settings")]
    [SerializeField] private float baseAngularSpeed = 180f;

    [Header("Chaotic Self Rotation")]
    [SerializeField] private float chaosBaseSpeed = 360f;
    [SerializeField] private float chaosVariation = 120f;
    [SerializeField] private float wobbleAmount = 15f;

    private RuntimeSkill skill;
    private Transform owner;
    private System.Action onFinished;
    private Rigidbody rb;

    private Vector3 chaosAxis;
    private float chaosSpeed;

    private float currentAngle;
    private float lifetime;
    private float duration;
    private float radius;
    private float angularSpeed;
    private bool finished;
    private Vector3 smoothedCenter;

    private float damage;
    private DamageType damageType;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void Initialize(
        RuntimeSkill runtimeSkill,
        Transform orbitOwner,
        float startAngle,
        System.Action onOrbitFinished)
    {
        ResetPooledState();

        if (runtimeSkill == null)
        {
            Debug.LogError("SolarOrbitOrb initialized with null RuntimeSkill.");
            ReturnToPool();
            return;
        }

        if (orbitOwner == null)
        {
            Debug.LogError("SolarOrbitOrb initialized with null owner.");
            ReturnToPool();
            return;
        }

        skill = runtimeSkill;
        owner = orbitOwner;
        onFinished = onOrbitFinished;

        finished = false;

        currentAngle = startAngle;

        radius = Mathf.Max(0.1f, skill.Stats.FinalRange);
        duration = Mathf.Max(0.01f, skill.Stats.FinalDuration);
        damage = Mathf.Max(0f, skill.Stats.FinalDamage);
        damageType = skill.Definition.DamageType;

        chaosAxis = Random.onUnitSphere;
        chaosSpeed = chaosBaseSpeed + Random.Range(-chaosVariation, chaosVariation);

        smoothedCenter = owner.position;

        float attackSpeedMultiplier = 1f;

        if (skill.Definition.ScalesWithAttackSpeed)
        {
            PlayerModifierSystem modifierSystem =
                owner.GetComponent<PlayerModifierSystem>();

            if (modifierSystem != null)
            {
                attackSpeedMultiplier = Mathf.Max(
                    0.05f,
                    modifierSystem.GetStat(StatType.AttackSpeed));
            }
        }

        angularSpeed = baseAngularSpeed * attackSpeedMultiplier;
        lifetime = 0f;
    }

    private void Update()
    {
        if (owner == null)
        {
            FinishOrbit();
            return;
        }

        lifetime += Time.deltaTime;

        if (lifetime >= duration)
        {
            FinishOrbit();
            return;
        }
    }

    private void LateUpdate()
    {
        if (owner == null)
            return;

        smoothedCenter = Vector3.Lerp(
            smoothedCenter,
            owner.position,
            15f * Time.deltaTime
        );

        RotateAroundOwner(smoothedCenter);
        RotateChaotic();
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

        Vector3 orbitPosition = center + offset;
        orbitPosition.y += orbitHeight;

        transform.position = orbitPosition;
    }

    private void RotateChaotic()
    {
        transform.Rotate(
            chaosAxis,
            chaosSpeed * Time.deltaTime,
            Space.Self
        );

        float wobbleX = Mathf.Sin(Time.time * 7f) * wobbleAmount;
        float wobbleZ = Mathf.Cos(Time.time * 5f) * wobbleAmount;

        transform.localRotation *= Quaternion.Euler(
            wobbleX * Time.deltaTime,
            0f,
            wobbleZ * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (skill == null || owner == null)
            return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable == null || damageable.IsDead)
            return;

        damageable.TakeDamage(new DamageData(
            damage,
            damageType
        ));
    }

    private void FinishOrbit()
    {
        if (finished)
            return;

        finished = true;

        onFinished?.Invoke();
        onFinished = null;

        ReturnToPool();
    }

    protected override void ResetPooledState()
    {
        skill = null;
        owner = null;
        onFinished = null;

        finished = false;

        chaosAxis = Vector3.up;
        chaosSpeed = 0f;

        currentAngle = 0f;
        lifetime = 0f;
        duration = 0f;
        radius = 0f;
        angularSpeed = 0f;
        smoothedCenter = Vector3.zero;

        transform.localRotation = Quaternion.identity;

        damage = 0f;
    }
}