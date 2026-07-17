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

    [Header("Gravity")]
    [SerializeField] float gravity = -20f;
    [SerializeField] float groundCheckDistance = 1.5f;
    [SerializeField] LayerMask groundLayer;

    float verticalVelocity;

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
    private PlayerInteractor interactor;
    private Transform cameraTransform;

    private Vector2 moveInput;
    private Vector3 moveDirection;

    private void Start()
    {
        EnemyManager.Instance.SetPlayer(transform);
        cameraTransform = Camera.main.transform;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        combat = GetComponent<PlayerCombat>();
        stats = GetComponent<PlayerStats>();
        damageReceiver = GetComponent<PlayerDamageReceiver>();
        interactor = GetComponentInChildren<PlayerInteractor>();

        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Dash.performed += OnDashPerformed;
        inputActions.Player.Interact.performed += OnInteractPerformed;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();

        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Dash.performed -= OnDashPerformed;
        inputActions.Player.Interact.performed -= OnInteractPerformed;
    }

    private void Update()
    {
        ApplyGravity();

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

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        dashPressed = true;
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        interactor?.TryInteract();
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            float targetY = hit.point.y;

            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * 15f);

            transform.position = pos;
        }

        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private void HandleMovement()
    {
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        moveDirection = camForward * moveInput.y + camRight * moveInput.x;

        float dynamicSpeed = moveSpeed;

        if (stats != null)
        {
            var modifierSystem = stats.GetComponent<PlayerModifierSystem>();

            if (modifierSystem != null)
            {
                dynamicSpeed = modifierSystem.GetStat(StatType.MovementSpeed);
            }
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

        if (damageReceiver != null)
        {
            damageReceiver.IsInvulnerable = true;
        }

        Vector3 dashDir = moveDirection;

        if (dashDir.magnitude < 0.1f)
        {
            dashDir = transform.forward;
        }

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

        if (damageReceiver != null)
        {
            damageReceiver.IsInvulnerable = false;
        }

        Physics.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        isDashing = false;
    }
}