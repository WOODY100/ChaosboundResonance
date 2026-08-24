using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerController : MonoBehaviour
{
    public Vector3 Velocity { get; private set; }

    public bool IsMoving =>
        moveInput.sqrMagnitude > 0.01f;

    public bool IsDashing =>
        isDashing;

    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashDuration = 0.18f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;

    private CharacterController controller;
    private PlayerInputActions inputActions;

    private Animator animator;
    private PlayerCombat combat;
    private PlayerModifierSystem modifierSystem;
    private PlayerDamageReceiver damageReceiver;
    private PlayerInteractor interactor;

    private Transform cameraTransform;

    private Vector2 moveInput;
    private Vector3 moveDirection;

    private float verticalVelocity;
    private float dashCooldownTimer;

    private bool isDashing;
    private bool dashPressed;

    private Coroutine dashRoutine;

    private int playerLayer = -1;
    private int enemyLayer = -1;

    private void Awake()
    {
        controller =
            GetComponent<CharacterController>();

        animator =
            GetComponentInChildren<Animator>();

        combat =
            GetComponent<PlayerCombat>();

        modifierSystem =
            GetComponent<PlayerModifierSystem>();

        damageReceiver =
            GetComponent<PlayerDamageReceiver>();

        interactor =
            GetComponentInChildren<PlayerInteractor>();

        inputActions =
            new PlayerInputActions();

        playerLayer =
            LayerMask.NameToLayer("Player");

        enemyLayer =
            LayerMask.NameToLayer("Enemy");

        inputActions.Player.Move.performed +=
            OnMovePerformed;

        inputActions.Player.Move.canceled +=
            OnMoveCanceled;

        inputActions.Player.Dash.performed +=
            OnDashPerformed;

        inputActions.Player.Interact.performed +=
            OnInteractPerformed;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();

        inputActions.Player.Move.performed -=
            OnMovePerformed;

        inputActions.Player.Move.canceled -=
            OnMoveCanceled;

        inputActions.Player.Dash.performed -=
            OnDashPerformed;

        inputActions.Player.Interact.performed -=
            OnInteractPerformed;

        StopDash();

        moveInput = Vector2.zero;
        moveDirection = Vector3.zero;
        dashPressed = false;
    }

    private void Start()
    {
        ResolveCamera();

        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.SetPlayer(transform);
        }
    }

    private void Update()
    {
        UpdateDashCooldown();
        HandleGravity();

        if (dashPressed)
        {
            dashPressed = false;

            TryStartDash();
        }

        if (!isDashing)
        {
            HandleMovement();
        }
        else
        {
            Velocity = controller.velocity;
        }
    }

    // =========================================================
    // INPUT
    // =========================================================

    private void OnMovePerformed(
        InputAction.CallbackContext context)
    {
        moveInput =
            context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(
        InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnDashPerformed(
        InputAction.CallbackContext context)
    {
        dashPressed = true;
    }

    private void OnInteractPerformed(
        InputAction.CallbackContext context)
    {
        interactor?.TryInteract();
    }

    // =========================================================
    // MOVEMENT
    // =========================================================

    private void HandleMovement()
    {
        if (cameraTransform == null)
        {
            ResolveCamera();

            if (cameraTransform == null)
                return;
        }

        Vector3 cameraForward =
            cameraTransform.forward;

        Vector3 cameraRight =
            cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection =
            cameraForward * moveInput.y +
            cameraRight * moveInput.x;

        float movementSpeed =
            GetMovementSpeed();

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            moveDirection.Normalize();

            if (combat == null ||
                combat.CurrentTarget == null)
            {
                RotateTowardsMovement();
            }

            controller.Move(
                moveDirection *
                movementSpeed *
                Time.deltaTime);
        }

        float animationSpeed =
            moveInput.magnitude *
            movementSpeed;

        if (animator != null)
        {
            animator.SetFloat(
                "Speed",
                animationSpeed);
        }

        Velocity =
            controller.velocity;
    }

    private float GetMovementSpeed()
    {
        if (modifierSystem == null)
            return 0f;

        return Mathf.Max(
            0f,
            modifierSystem.GetStat(
                StatType.MovementSpeed));
    }

    private void RotateTowardsMovement()
    {
        Quaternion targetRotation =
            Quaternion.LookRotation(
                moveDirection);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed *
                Time.deltaTime);
    }

    // =========================================================
    // GRAVITY
    // =========================================================

    private void HandleGravity()
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
        }
        else
        {
            verticalVelocity +=
                gravity *
                Time.deltaTime;
        }

        controller.Move(
            Vector3.up *
            verticalVelocity *
            Time.deltaTime);
    }

    // =========================================================
    // DASH
    // =========================================================

    private void UpdateDashCooldown()
    {
        if (dashCooldownTimer <= 0f)
            return;

        dashCooldownTimer -=
            Time.deltaTime;

        if (dashCooldownTimer < 0f)
        {
            dashCooldownTimer = 0f;
        }
    }

    private void TryStartDash()
    {
        if (isDashing)
            return;

        if (dashCooldownTimer > 0f)
            return;

        dashRoutine =
            StartCoroutine(
                DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        IgnoreEnemyCollision(true);

        if (combat != null)
        {
            combat.CancelAttack();
        }

        if (animator != null)
        {
            animator.SetTrigger("Dash");
        }

        if (damageReceiver != null)
        {
            damageReceiver.IsInvulnerable = true;
        }

        Vector3 dashDirection =
            moveDirection;

        if (dashDirection.sqrMagnitude < 0.01f)
        {
            dashDirection =
                transform.forward;
        }

        dashDirection.y = 0f;

        if (dashDirection.sqrMagnitude < 0.01f)
        {
            dashDirection = Vector3.forward;
        }

        dashDirection.Normalize();

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed +=
                Time.deltaTime;

            float normalizedTime =
                Mathf.Clamp01(
                    elapsed /
                    dashDuration);

            Vector3 delta =
                dashDirection *
                (dashDistance /
                 Mathf.Max(
                     0.0001f,
                     dashDuration)) *
                Time.deltaTime;

            controller.Move(delta);

            yield return null;
        }

        EndDash();
    }

    private void EndDash()
    {
        if (!isDashing)
            return;

        if (damageReceiver != null)
        {
            damageReceiver.IsInvulnerable = false;
        }

        IgnoreEnemyCollision(false);

        isDashing = false;
        dashRoutine = null;

        Velocity =
            controller.velocity;
    }

    private void StopDash()
    {
        if (dashRoutine != null)
        {
            StopCoroutine(dashRoutine);
            dashRoutine = null;
        }

        if (isDashing)
        {
            EndDash();
        }
        else
        {
            IgnoreEnemyCollision(false);

            if (damageReceiver != null)
            {
                damageReceiver.IsInvulnerable = false;
            }
        }

        isDashing = false;
    }

    private void IgnoreEnemyCollision(
        bool ignore)
    {
        if (playerLayer < 0 ||
            enemyLayer < 0)
        {
            return;
        }

        Physics.IgnoreLayerCollision(
            playerLayer,
            enemyLayer,
            ignore);
    }

    // =========================================================
    // CAMERA
    // =========================================================

    private void ResolveCamera()
    {
        Camera mainCamera =
            Camera.main;

        if (mainCamera == null)
        {
            cameraTransform = null;

            Debug.LogError(
                "PlayerController could not find the Main Camera.",
                this);

            return;
        }

        cameraTransform =
            mainCamera.transform;
    }
}