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
    [SerializeField] private float chargeRecoveryDuration = 0.6f;
    [SerializeField] private float chargeInertiaDeceleration = 25f;

    private bool isChargeRecovering = false;
    private float chargeRecoveryTimer = 0f;
    private bool isCharging = false;
    private float chargeTimer;
    private Vector3 chargeDirection;
    private float currentSpeed = 0f;
    private bool canMove = true;
    private bool isForcedMovement = false;
    private bool isDead = false;
    private bool rotationLocked = false;

    public bool RotationLocked
    {
        get => rotationLocked;
        set => rotationLocked = value;
    }

    public System.Action OnChargeStart;
    public System.Action OnChargeEnd;

    public bool IsCharging => isCharging;
    public System.Action OnChargeTick;
    private void Update()
    {
        if (isDead || player == null)
            return;

        if (!isCharging && !rotationLocked)
            RotateToPlayer();

        HandleMovement();
    }

    private void Awake()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isCharging)
            return;

        // 🔥 Si golpea player o entorno sólido
        if (collision.gameObject.CompareTag("Player") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            CancelChargeOnImpact();
        }
    }

    private void CancelChargeOnImpact()
    {
        if (!isCharging)
            return;

        isCharging = false;

        // 🔥 Forzar inercia inmediatamente
        isChargeRecovering = true;
        chargeRecoveryTimer = chargeRecoveryDuration;

        OnChargeEnd?.Invoke();
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    public void StopImmediately()
    {
        StopAllCoroutines();          // 🔥 mata cualquier ForceMove activo
        isForcedMovement = false;
        isCharging = false;
        currentSpeed = 0f;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        maxMoveSpeed *= multiplier;
    }

    public void SetPlayer(Transform target)
    {
        player = target;
    }

    private void HandleMovement()
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

    public void ForceMove(Vector3 direction, float speed, float duration, Vector3 finalTargetPosition)
    {
        StartCoroutine(ForceMoveRoutine(direction, speed, duration, finalTargetPosition));
    }

    private IEnumerator ForceMoveRoutine(
    Vector3 direction,
    float speed,
    float duration,
    Vector3 finalTargetPosition)
    {
        isForcedMovement = true;

        float timer = 0f;

        Vector3 startPosition = transform.position;
        startPosition.y = 0f;

        Vector3 endPosition = finalTargetPosition;
        endPosition.y = 0f;

        float jumpHeight = 2.5f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            progress = Mathf.Clamp01(progress);

            // 🔥 Interpolación horizontal PERFECTA
            Vector3 horizontal = Vector3.Lerp(startPosition, endPosition, progress);

            // 🔥 Altura parabólica alineada
            float height = Mathf.Sin(progress * Mathf.PI) * jumpHeight;

            transform.position = new Vector3(
                horizontal.x,
                startPosition.y + height,
                horizontal.z
            );

            yield return null;
        }

        // 🔥 Posición final EXACTA
        transform.position = new Vector3(
            endPosition.x,
            startPosition.y,
            endPosition.z
        );

        currentSpeed = 0f;
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

    private void HandleChargeRecovery()
    {
        if (chargeRecoveryTimer <= 0f)
        {
            isChargeRecovering = false;
            rotationLocked = false;
            currentSpeed = 0f;
            return;
        }

        // 🔥 Desaceleración fuerte pero no instantánea
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            0f,
            chargeInertiaDeceleration * Time.deltaTime
        );

        transform.position += chargeDirection * currentSpeed * Time.deltaTime;

        chargeRecoveryTimer -= Time.deltaTime;
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

    private void RotateToPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void OnBossDeath()
    {
        isDead = true;

        isCharging = false;
        isForcedMovement = false;
        currentSpeed = 0f;
        GetComponent<Collider>().enabled = false;

        StopAllCoroutines();

        animator.SetFloat("Speed", 0f);
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

        // 🔥 Alinear inmediatamente
        transform.rotation = Quaternion.LookRotation(chargeDirection);

        OnChargeStart?.Invoke();
    }

    public void StopCharge()
    {
        isCharging = false;

        // 🔥 Iniciar fase de inercia
        isChargeRecovering = true;
        chargeRecoveryTimer = chargeRecoveryDuration;

        OnChargeEnd?.Invoke();
    }

    private void HandleCharge()
    {
        if (chargeTimer <= 0f)
        {
            StopCharge();
            return;
        }

        currentSpeed = chargeSpeed;
        transform.position += chargeDirection * currentSpeed * Time.deltaTime;

        OnChargeTick?.Invoke();

        chargeTimer -= Time.deltaTime;
    }
}