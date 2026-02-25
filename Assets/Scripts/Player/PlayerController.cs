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

    private CharacterController controller;
    private PlayerInputActions inputActions;
    private Animator animator;
    private PlayerCombat combat;
    private PlayerStats stats;

    private Vector2 moveInput;
    private Vector3 moveDirection;

    private void Start()
    {
        EnemyManager.Instance.SetPlayer(transform);
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        combat = GetComponent<PlayerCombat>();
        stats = GetComponent<PlayerStats>();

        inputActions = new PlayerInputActions();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

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
}