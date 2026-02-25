using UnityEngine;

public class EnemyMovementArena : MonoBehaviour
{
    [SerializeField] private float defaultMoveSpeed = 5f;
    [SerializeField] private float separationStrength = 1.5f;
    [SerializeField] private float separationRadius = 1.2f;
    [SerializeField] private LayerMask enemyLayer;

    private float baseSpeed;
    private float difficultyMultiplier = 1f;
    private float currentSpeed;
    private float sqrSeparationRadius;
    private Animator animator;
    private Transform player;

    public void SetPlayer(Transform target)
    {
        player = target;
    }

    void Awake()
    {
        baseSpeed = defaultMoveSpeed;
        RecalculateSpeed();
        sqrSeparationRadius = separationRadius * separationRadius;
        animator = GetComponent<Animator>();
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

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        float distance = toPlayer.sqrMagnitude;

        if (distance < 0.1f)
        {
            SetSpeed(0f);
            return;
        }

        Vector3 dirToPlayer = toPlayer.normalized;
        Vector3 separation = CalculateSeparation();

        float dynamicSeparation = separationStrength;

        if (distance < 4f)
            dynamicSeparation *= 0.5f;

        Vector3 finalDir = (dirToPlayer + separation * dynamicSeparation).normalized;

        // 🔥 Movimiento inmediato
        transform.position += finalDir * currentSpeed * Time.deltaTime;

        // 🔥 Rotación suave pero rápida
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
}
