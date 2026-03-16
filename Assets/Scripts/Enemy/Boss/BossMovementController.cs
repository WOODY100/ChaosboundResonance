using System.Collections;
using UnityEngine;

public class BossMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float maxMoveSpeed = 4f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 8f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Charge")]
    [SerializeField] private float chargeSpeed = 15f;
    [SerializeField] private float chargeDuration = 1.2f;
    [SerializeField] private float chargeRecoveryDuration = 0.6f;
    [SerializeField] private float chargeInertiaDeceleration = 25f;

    [Header("Steering")]
    [SerializeField] private float steeringCheckDistance = 3.5f;
    [SerializeField] private float obstacleMemoryTime = 0.6f;

    [Header("Anti Stuck")]
    [SerializeField] private float stuckCheckTime = 0.4f;
    [SerializeField] private float stuckDistance = 0.15f;

    [Header("Wall Repulsion")]
    [SerializeField] private float wallRepulsionRadius = 2f;
    [SerializeField] private float wallRepulsionStrength = 0.4f;

    [SerializeField] private LayerMask obstacleLayer;

    private Collider[] repulsionBuffer = new Collider[16];

    private CapsuleCollider capsule;

    private Vector3 lastPosition;
    private Vector3 lastTangent;
    private Vector3 chargeDirection;

    private float stuckTimer;
    private float obstacleMemoryTimer;
    private float currentSpeed;

    private bool isCharging;
    private bool isChargeRecovering;
    private bool isForcedMovement;
    private bool canMove = true;
    private bool isDead;

    private float chargeTimer;
    private float chargeRecoveryTimer;

    private bool rotationLocked;

    public bool RotationLocked
    {
        get => rotationLocked;
        set => rotationLocked = value;
    }

    public System.Action OnChargeStart;
    public System.Action OnChargeEnd;
    public System.Action OnChargeTick;

    public bool IsCharging => isCharging;

    void Awake()
    {
        capsule = GetComponent<CapsuleCollider>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (isDead || player == null)
            return;

        if (!isCharging && !rotationLocked)
            RotateToPlayer();

        HandleMovement();
        CheckIfStuck();
    }

    // --------------------------------------------------------
    // MOVEMENT
    // --------------------------------------------------------

    void HandleMovement()
    {
        if (isChargeRecovering)
        {
            HandleChargeRecovery();
            return;
        }

        if (isForcedMovement)
            return;

        if (isCharging)
        {
            HandleCharge();
            return;
        }

        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        float distance = dir.magnitude;

        if (!canMove || distance < 1.5f)
        {
            Decelerate();
            return;
        }

        dir.Normalize();

        dir = GetSteeredDirection(dir);
        dir += GetWallRepulsion();

        dir.Normalize();

        Accelerate();

        MoveWithCollision(dir);
        RotateTowards(dir);

        animator.SetFloat("Speed", currentSpeed > 0.1f ? 1f : 0f);
    }

    public void ForceMove(Vector3 direction, float speed, float duration, Vector3 target)
    {
        StartCoroutine(ForceMoveRoutine(direction, speed, duration, target));
    }

    IEnumerator ForceMoveRoutine(Vector3 direction, float speed, float duration, Vector3 target)
    {
        isForcedMovement = true;

        float timer = 0f;

        Vector3 start = transform.position;
        start.y = 0;
        target.y = 0;

        float jumpHeight = 2.5f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);

            Vector3 horizontal = Vector3.Lerp(start, target, t);

            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;

            transform.position = new Vector3(
                horizontal.x,
                start.y + height,
                horizontal.z
            );

            yield return null;
        }

        transform.position = new Vector3(target.x, start.y, target.z);

        currentSpeed = 0;
        isForcedMovement = false;
    }

    // --------------------------------------------------------
    // COLLISION MOVE
    // --------------------------------------------------------

    void MoveWithCollision(Vector3 direction)
    {
        float moveDistance = currentSpeed * Time.deltaTime;

        float radius = capsule.radius * 0.9f;

        Vector3 point1 = transform.position + Vector3.up * radius;
        Vector3 point2 = transform.position + Vector3.up * (capsule.height - radius);

        if (Physics.CapsuleCast(point1, point2, radius, direction,
            out RaycastHit hit, moveDistance, obstacleLayer))
        {
            float safeDistance = hit.distance - 0.02f;

            if (safeDistance > 0)
                transform.position += direction * safeDistance;

            Vector3 slide = Vector3.ProjectOnPlane(direction, hit.normal);

            if (slide.sqrMagnitude > 0.001f)
                transform.position += slide.normalized * moveDistance * 0.5f;
        }
        else
        {
            transform.position += direction * moveDistance;
        }
    }

    // --------------------------------------------------------
    // STEERING
    // --------------------------------------------------------

    Vector3 GetSteeredDirection(Vector3 desiredDir)
    {
        Vector3 origin = transform.position + Vector3.up * capsule.height * 0.5f;

        if (Physics.Raycast(origin, desiredDir, out RaycastHit hit, steeringCheckDistance, obstacleLayer))
        {
            Vector3 tangent = Vector3.Cross(hit.normal, Vector3.up);

            Vector3 toPlayer = (player.position - transform.position).normalized;

            if (Vector3.Dot(tangent, toPlayer) < 0)
                tangent = -tangent;

            lastTangent = tangent;
            obstacleMemoryTimer = obstacleMemoryTime;

            return tangent;
        }

        if (obstacleMemoryTimer > 0)
        {
            obstacleMemoryTimer -= Time.deltaTime;
            return lastTangent;
        }

        return desiredDir;
    }

    // --------------------------------------------------------
    // WALL REPULSION
    // --------------------------------------------------------

    Vector3 GetWallRepulsion()
    {
        Vector3 repulsion = Vector3.zero;

        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            wallRepulsionRadius,
            repulsionBuffer,
            obstacleLayer
        );

        for (int i = 0; i < count; i++)
        {
            Collider hit = repulsionBuffer[i];

            Vector3 closest = hit.ClosestPoint(transform.position);
            Vector3 dir = transform.position - closest;

            float dist = dir.magnitude;

            if (dist > 0.001f)
                repulsion += dir.normalized / dist;
        }

        return repulsion * wallRepulsionStrength;
    }

    // --------------------------------------------------------
    // ANTI STUCK
    // --------------------------------------------------------

    void CheckIfStuck()
    {
        if (isForcedMovement || isCharging || !canMove)
            return;

        stuckTimer += Time.deltaTime;

        if (stuckTimer < stuckCheckTime)
            return;

        float movedDistance = Vector3.Distance(transform.position, lastPosition);

        if (movedDistance < stuckDistance)
        {
            EscapeCorner();
        }

        lastPosition = transform.position;
        stuckTimer = 0f;
    }

    void EscapeCorner()
    {
        Vector3 randomDir = Random.insideUnitSphere;
        randomDir.y = 0;

        randomDir.Normalize();

        transform.position += randomDir * 0.8f;
    }

    // --------------------------------------------------------
    // CHARGE
    // --------------------------------------------------------

    public void StartCharge()
    {
        if (player == null) return;

        isCharging = true;
        chargeTimer = chargeDuration;

        chargeDirection = (player.position - transform.position).normalized;
        chargeDirection.y = 0;

        transform.rotation = Quaternion.LookRotation(chargeDirection);

        OnChargeStart?.Invoke();
    }

    void HandleCharge()
    {
        if (chargeTimer <= 0)
        {
            StopCharge();
            return;
        }

        currentSpeed = chargeSpeed;

        Vector3 dir = GetSteeredDirection(chargeDirection);
        MoveWithCollision(dir);

        OnChargeTick?.Invoke();

        chargeTimer -= Time.deltaTime;
    }

    public void StopCharge()
    {
        isCharging = false;
        isChargeRecovering = true;
        chargeRecoveryTimer = chargeRecoveryDuration;

        OnChargeEnd?.Invoke();
    }

    void HandleChargeRecovery()
    {
        if (chargeRecoveryTimer <= 0)
        {
            isChargeRecovering = false;
            rotationLocked = false;
            currentSpeed = 0;
            return;
        }

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            0,
            chargeInertiaDeceleration * Time.deltaTime
        );

        MoveWithCollision(chargeDirection);

        chargeRecoveryTimer -= Time.deltaTime;
    }

    // --------------------------------------------------------
    // HELPERS
    // --------------------------------------------------------

    void Accelerate()
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxMoveSpeed, acceleration * Time.deltaTime);
    }

    void Decelerate()
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        animator.SetFloat("Speed", 0);
    }

    void RotateTowards(Vector3 dir)
    {
        Quaternion target = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            target,
            rotationSpeed * Time.deltaTime
        );
    }

    void RotateToPlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f)
            return;

        Quaternion target = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            target,
            rotationSpeed * Time.deltaTime
        );
    }

    // --------------------------------------------------------
    // STATE
    // --------------------------------------------------------

    public void StopImmediately()
    {
        StopAllCoroutines();
        isForcedMovement = false;
        isCharging = false;
        currentSpeed = 0;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    public void SetPlayer(Transform target)
    {
        player = target;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        maxMoveSpeed *= multiplier;
    }

    public void OnBossDeath()
    {
        isDead = true;

        StopAllCoroutines();

        currentSpeed = 0;
        animator.SetFloat("Speed", 0);

        GetComponent<Collider>().enabled = false;
    }
}