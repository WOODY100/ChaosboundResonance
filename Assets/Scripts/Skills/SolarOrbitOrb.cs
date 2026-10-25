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
    private OrbitalPatternDefinition pattern;
    private System.Action onFinished;
    private Rigidbody rb;

    private Vector3 chaosAxis;
    private float chaosSpeed;

    private float currentAngle;
    private float lifetime;
    private float duration;
    private float radius;
    private float angularSpeed;
    private float activeOrbitHeight;
    private bool finished;
    private Vector3 smoothedCenter;

    private float damage;

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
        OrbitalPatternDefinition orbitalPattern,
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
        pattern = orbitalPattern;
        onFinished = onOrbitFinished;

        finished = false;
        currentAngle = startAngle;

        duration = Mathf.Max(0.01f, skill.Stats.FinalDuration);
        damage = Mathf.Max(0f, skill.Stats.FinalDamage);

        // Without a pattern, preserve the original orbital behavior.
        if (pattern == null)
        {
            radius = Mathf.Max(0.1f, skill.Stats.FinalRange);
            activeOrbitHeight = orbitHeight;
            angularSpeed = CalculateAttackSpeed(baseAngularSpeed);
        }
        else
        {
            radius = Mathf.Max(0.1f, pattern.OrbitRadius);
            activeOrbitHeight = pattern.OrbitHeight;

            if (pattern.Pattern == OrbitalPatternType.Orbit)
            {
                angularSpeed = pattern.RotateWithAttackSpeed
                    ? CalculateAttackSpeed(pattern.AngularSpeed)
                    : pattern.AngularSpeed;
            }
            else
            {
                angularSpeed = 0f;
            }
        }

        chaosAxis = Random.onUnitSphere;
        chaosSpeed = chaosBaseSpeed
            + Random.Range(-chaosVariation, chaosVariation);

        smoothedCenter = owner.position;
        lifetime = 0f;
    }

    private float CalculateAttackSpeed(float baseSpeed)
    {
        float multiplier = 1f;

        if (skill != null &&
            skill.Definition != null &&
            skill.Definition.ScalesWithAttackSpeed)
        {
            PlayerModifierSystem modifierSystem =
                owner.GetComponent<PlayerModifierSystem>();

            if (modifierSystem != null)
            {
                multiplier = Mathf.Max(
                    0.05f,
                    modifierSystem.GetStat(StatType.AttackSpeed));
            }
        }

        return baseSpeed * multiplier;
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
        }
    }

    private void LateUpdate()
    {
        if (owner == null || finished)
            return;

        smoothedCenter = Vector3.Lerp(
            smoothedCenter,
            owner.position,
            15f * Time.deltaTime
        );

        UpdateFormation(smoothedCenter);
        RotateChaotic();
    }

    private void UpdateFormation(Vector3 center)
    {
        // Orbit rotates continuously. FixedRing and ForwardArc
        // preserve their assigned angle relative to the world.
        if (pattern == null ||
            pattern.Pattern == OrbitalPatternType.Orbit)
        {
            currentAngle += angularSpeed * Time.deltaTime;
        }

        float radians = currentAngle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(radians),
            0f,
            Mathf.Sin(radians)
        ) * radius;

        Vector3 position = center + offset;
        position.y += activeOrbitHeight;

        transform.position = position;
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

        IDamageable damageable =
            other.GetComponentInParent<IDamageable>();

        if (damageable == null || damageable.IsDead)
            return;

        damageable.TakeDamage(new DamageData(damage));
    }

    private void FinishOrbit()
    {
        if (finished)
            return;

        finished = true;

        System.Action callback = onFinished;
        onFinished = null;

        callback?.Invoke();

        ReturnToPool();
    }

    protected override void ResetPooledState()
    {
        skill = null;
        owner = null;
        pattern = null;
        onFinished = null;

        finished = false;

        chaosAxis = Vector3.up;
        chaosSpeed = 0f;

        currentAngle = 0f;
        lifetime = 0f;
        duration = 0f;
        radius = 0f;
        angularSpeed = 0f;
        activeOrbitHeight = 0f;
        smoothedCenter = Vector3.zero;

        transform.localRotation = Quaternion.identity;

        damage = 0f;
    }
}