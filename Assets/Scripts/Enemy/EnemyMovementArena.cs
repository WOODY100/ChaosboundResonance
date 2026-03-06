using UnityEngine;

public class EnemyMovementArena : MonoBehaviour
{
    [SerializeField] private float defaultMoveSpeed = 5f;
    [SerializeField] private float separationStrength = 1.5f;
    [SerializeField] private float separationRadius = 1.2f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float steeringCheckDistance = 1.2f;
    [SerializeField] private float steeringAngle = 35f;
    [SerializeField] private int steeringRays = 3;

    private CapsuleCollider myCollider;
    private float baseSpeed;
    private float difficultyMultiplier = 1f;
    private float currentSpeed;
    private float sqrSeparationRadius;
    private Animator animator;
    private Transform player;
    private EnemyCore core;

    void Awake()
    {
        baseSpeed = defaultMoveSpeed;
        RecalculateSpeed();
        sqrSeparationRadius = separationRadius * separationRadius;
        animator = GetComponent<Animator>();
        core = GetComponent<EnemyCore>();
        myCollider = GetComponent<CapsuleCollider>();
    }

    public void SetPlayer(Transform target)
    {
        player = target;
    }

    void RecalculateSpeed()
    {
        currentSpeed = baseSpeed * difficultyMultiplier;
    }

    public void SetDifficultyMultiplier(float multiplier)
    {
        difficultyMultiplier = multiplier;
        RecalculateSpeed();
    }

    public void SetBaseSpeed(float speed)
    {
        baseSpeed = speed;
        RecalculateSpeed();
    }

    public void TickMovement(Transform player)
    {
        if (player == null)
            return;

        float distance = core.DistanceToPlayer;

        if (distance < 0.1f)
        {
            SetSpeed(0f);
            return;
        }

        // Dirección hacia el jugador
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        Vector3 dirToPlayer = toPlayer.normalized;

        Vector3 separation = CalculateSeparation();

        float dynamicSeparation = separationStrength;

        if (distance < 4f)
            dynamicSeparation *= 0.5f;

        Vector3 desiredDir = (dirToPlayer + separation * dynamicSeparation).normalized;

        Vector3 finalDir = GetSteeredDirection(desiredDir);

        // Movimiento
        MoveWithCollision(finalDir);

        // Rotación
        Quaternion targetRotation = Quaternion.LookRotation(finalDir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            15f * Time.deltaTime
        );

        SetSpeed(1f);
    }

    public void SetSpeed(float value)
    {
        if (animator != null)
            animator.SetFloat("Speed", value);
    }

    Vector3 CalculateSeparation()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, separationRadius, enemyLayer);

        Vector3 separation = Vector3.zero;

        foreach (var hit in hits)
        {
            if (hit.transform == transform)
                continue;

            Vector3 diff = transform.position - hit.transform.position;
            float sqrDist = diff.sqrMagnitude;

            if (sqrDist > 0.001f)
                separation += diff.normalized / sqrDist;
        }

        separation.y = 0f;
        return separation;
    }

    void MoveWithCollision(Vector3 direction)
    {
        float moveDistance = currentSpeed * Time.deltaTime;

        Vector3 point1 = transform.position + Vector3.up * myCollider.radius;
        Vector3 point2 = transform.position + Vector3.up * (myCollider.height - myCollider.radius);

        if (Physics.CapsuleCast(
            point1,
            point2,
            myCollider.radius,
            direction,
            out RaycastHit hit,
            moveDistance,
            obstacleLayer))
        {
            Vector3 slideDir = Vector3.ProjectOnPlane(direction, hit.normal);
            slideDir += transform.right * Random.Range(-0.2f, 0.2f);

            if (slideDir.sqrMagnitude > 0.001f)
            {
                slideDir.Normalize();
                transform.position += slideDir * moveDistance;
            }
        }
        else
        {
            transform.position += direction * moveDistance;
        }
    }

    Vector3 GetSteeredDirection(Vector3 desiredDir)
    {
        if (!IsDirectionBlocked(desiredDir))
            return desiredDir;

        float angleStep = steeringAngle;

        for (int i = 1; i <= steeringRays; i++)
        {
            Vector3 left = Quaternion.Euler(0, -angleStep * i, 0) * desiredDir;
            if (!IsDirectionBlocked(left))
                return left;

            Vector3 right = Quaternion.Euler(0, angleStep * i, 0) * desiredDir;
            if (!IsDirectionBlocked(right))
                return right;
        }

        return -desiredDir;
    }

    bool IsDirectionBlocked(Vector3 dir)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        return Physics.Raycast(
            origin,
            dir,
            steeringCheckDistance,
            obstacleLayer
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 origin = transform.position + Vector3.up * 0.5f;

        Gizmos.DrawRay(origin, transform.forward * steeringCheckDistance);
    }
}
