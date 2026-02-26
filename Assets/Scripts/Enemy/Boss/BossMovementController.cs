using System.Collections;
using UnityEngine;

public class BossMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    [Header("Movement Settings")]
    [SerializeField] private float maxMoveSpeed = 4f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 8f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float chargeSpeed = 15f;
    [SerializeField] private float chargeDuration = 1.2f;

    private bool isCharging = false;
    private float chargeTimer;
    private Vector3 chargeDirection;
    private float currentSpeed = 0f;
    private bool canMove = true;
    private bool isForcedMovement = false;

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        maxMoveSpeed *= multiplier;
    }

    private void Update()
    {
        if (player == null)
            return;

        HandleMovement();
    }

    private void Awake()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void SetPlayer(Transform target)
    {
        player = target;
    }

    private void HandleMovement()
    {
        if (isForcedMovement)
            return;

        if (isCharging)
        {
            HandleCharge();
            return;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (!canMove || distance < 1.5f)
        {
            Decelerate();
            return;
        }

        direction.Normalize();

        Accelerate();

        transform.position += direction * currentSpeed * Time.deltaTime;

        RotateTowards(direction);

        animator.SetFloat("Speed", currentSpeed > 0.1f ? 1f : 0f);
    }

    public void ForceMove(Vector3 direction, float speed, float duration)
    {
        StartCoroutine(ForceMoveRoutine(direction, speed, duration));
    }

    private IEnumerator ForceMoveRoutine(Vector3 direction, float speed, float duration)
    {
        isForcedMovement = true;

        float timer = 0f;

        Vector3 startPosition = transform.position;
        Vector3 horizontalStart = startPosition;
        horizontalStart.y = 0f;

        float jumpHeight = 2.5f; // altura máxima del salto

        while (timer < duration)
        {
            float delta = Time.deltaTime;
            timer += delta;

            float progress = timer / duration; // 0 → 1

            // Movimiento horizontal
            Vector3 horizontalMove = direction * speed * delta;

            // Altura parabólica (0 → altura → 0)
            float height = Mathf.Sin(progress * Mathf.PI) * jumpHeight;

            Vector3 newPosition = transform.position + horizontalMove;
            newPosition.y = startPosition.y + height;

            transform.position = newPosition;

            yield return null;
        }

        // Asegurar que termina en el suelo exacto
        Vector3 finalPos = transform.position;
        finalPos.y = startPosition.y;
        transform.position = finalPos;

        isForcedMovement = false;
    }

    private void Accelerate()
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxMoveSpeed, acceleration * Time.deltaTime);
    }

    private void Decelerate()
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        animator.SetFloat("Speed", 0f);
    }

    private void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void StartCharge()
    {
        if (player == null)
            return;

        isCharging = true;
        chargeTimer = chargeDuration;

        chargeDirection = (player.position - transform.position);
        chargeDirection.y = 0f;
        chargeDirection.Normalize();
    }

    public void StopCharge()
    {
        isCharging = false;
        currentSpeed = 0f;
    }

    private void HandleCharge()
    {
        if (chargeTimer <= 0f)
        {
            StopCharge();
            return;
        }

        transform.position += chargeDirection * chargeSpeed * Time.deltaTime;

        chargeTimer -= Time.deltaTime;
    }
}