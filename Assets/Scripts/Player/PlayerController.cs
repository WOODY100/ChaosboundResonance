using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Vector3 Velocity { get; private set; }
    public bool IsMoving => moveInput.magnitude > 0.1f;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 15f;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashDuration = 0.18f;
    [SerializeField] private float dashCooldown = 1f;

    public bool IsDashing => isDashing;

    private bool isDashing;
    private float dashCooldownTimer;
    private bool dashPressed;

    private PlayerDamageReceiver damageReceiver;
    private CharacterController controller;
    private PlayerInputActions inputActions;
    private Animator animator;
    private PlayerCombat combat;
    private PlayerStats stats;
    private Transform cameraTransform;

    private Vector2 moveInput;
    private Vector3 moveDirection;

    private void Start()
    {
        EnemyManager.Instance.SetPlayer(transform);
        cameraTransform = Camera.main.transform;
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        combat = GetComponent<PlayerCombat>();
        stats = GetComponent<PlayerStats>();
        damageReceiver = GetComponent<PlayerDamageReceiver>();

        inputActions = new PlayerInputActions();
        inputActions.Player.Dash.performed += ctx => dashPressed = true;
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Update()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (dashPressed && dashCooldownTimer <= 0f && !isDashing)
        {
            StartCoroutine(Dash());
        }

        dashPressed = false;

        if (!isDashing)
            HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        moveDirection = camForward * moveInput.y + camRight * moveInput.x;

        // 🔥 Obtener velocidad dinámica
        float dynamicSpeed = moveSpeed;

        if (stats != null)
        {
            var modifierSystem = stats.GetComponent<PlayerModifierSystem>();
            if (modifierSystem != null)
                dynamicSpeed = modifierSystem.GetStat(StatType.MovementSpeed);
        }

        if (moveDirection.magnitude > 0.1f)
        {
            moveDirection.Normalize();

            if (combat.CurrentTarget == null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }

            controller.Move(moveDirection * dynamicSpeed * Time.deltaTime);
        }

        float currentSpeed = moveInput.magnitude * dynamicSpeed;
        animator.SetFloat("Speed", currentSpeed);
        Velocity = controller.velocity;
    }

    private IEnumerator Dash()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        Physics.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        isDashing = true;
        dashCooldownTimer = dashCooldown;

        if (combat != null)
        {
            combat.CancelAttack();
        }

        animator.SetTrigger("Dash");

        // 🔥 Invulnerabilidad
        if (damageReceiver != null)
            damageReceiver.IsInvulnerable = true;

        // Dirección actual
        Vector3 dashDir = moveDirection;

        if (dashDir.magnitude < 0.1f)
            dashDir = transform.forward;

        dashDir.Normalize();

        float timer = 0f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + dashDir * dashDistance;

        while (timer < dashDuration)
        {
            timer += Time.deltaTime;
            float t = timer / dashDuration;

            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
            Vector3 delta = newPos - transform.position;

            controller.Move(delta);

            yield return null;
        }

        // 🔥 Restaurar vulnerabilidad
        if (damageReceiver != null)
            damageReceiver.IsInvulnerable = false;

        Physics.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        isDashing = false;
    }
}