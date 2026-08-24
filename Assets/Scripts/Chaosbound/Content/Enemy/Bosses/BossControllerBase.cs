using UnityEngine;

public abstract class BossControllerBase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected Transform player;
    [SerializeField] protected BossMovementController movement;

    [Header("Ranges")]
    [SerializeField] protected float meleeRange = 3f;
    [SerializeField] protected float midRange = 8f;

    [Header("Cooldowns")]
    [SerializeField] protected float attackCooldown = 2f;
    [SerializeField] protected float stompCooldown = 5f;
    [SerializeField] protected float chargeCooldown = 6f;
    [SerializeField] protected float jumpCooldown = 8f;
    [SerializeField] protected float furyCooldown = 20f;

    protected BossHealth health;

    protected float attackTimer;
    protected float stompTimer;
    protected float chargeTimer;
    protected float jumpTimer;
    protected float furyTimer;

    [SerializeField]
    protected int currentPhase = 1;

    protected bool isPerformingAction = false;
    protected bool isDead = false;
    protected bool furyUsed = false;

    protected virtual void Awake()
    {
        if (movement == null)
            movement =
                GetComponent<BossMovementController>();
    }

    protected virtual void Start()
    {
        health =
            GetComponent<BossHealth>();
    }

    protected virtual void OnEnable()
    {
        ResetRuntimeState();
    }

    protected virtual void ResetRuntimeState()
    {
        // =========================
        // STATE
        // =========================

        isDead = false;
        isPerformingAction = false;
        furyUsed = false;

        // =========================
        // PHASE
        // =========================

        currentPhase = 1;

        // =========================
        // COOLDOWNS
        // =========================

        attackTimer = 0f;
        stompTimer = 0f;
        chargeTimer = 0f;
        jumpTimer = 0f;
        furyTimer = 0f;

        // =========================
        // PLAYER
        // =========================

        player = null;

        ResolvePlayer();

        // =========================
        // MOVEMENT
        // =========================

        if (movement != null)
        {
            movement.SetCanMove(true);

            if (player != null)
                movement.SetPlayer(player);
        }

        // =========================
        // ANIMATION
        // =========================

        if (animator != null)
        {
            animator.speed = 1f;
            animator.SetFloat("Speed", 0f);
        }
    }

    private void ResolvePlayer()
    {
        player =
            GameObject.FindGameObjectWithTag(
                "Player")?.transform;
    }

    protected virtual void Update()
    {
        if (isDead)
            return;

        if (player == null)
        {
            ResolvePlayer();

            if (player != null &&
                movement != null)
            {
                movement.SetPlayer(player);
            }
        }

        if (isPerformingAction || player == null)
            return;

        UpdateCooldowns();
        EvaluateCombat();
    }

    #region Cooldowns

    protected void UpdateCooldowns()
    {
        attackTimer -= Time.deltaTime;
        stompTimer -= Time.deltaTime;
        chargeTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;
        furyTimer -= Time.deltaTime;
    }

    protected bool AttackReady() =>
        attackTimer <= 0f;

    protected bool StompReady() =>
        stompTimer <= 0f;

    protected bool ChargeReady() =>
        chargeTimer <= 0f;

    protected bool JumpReady() =>
        jumpTimer <= 0f;

    protected bool FuryReady() =>
        furyTimer <= 0f;

    #endregion

    #region Abstract Combat Logic

    protected abstract void EvaluateCombat();

    #endregion

    #region Abilities

    protected void TriggerAction(
        string triggerName)
    {
        isPerformingAction = true;

        if (movement != null)
            movement.SetCanMove(false);

        if (animator != null)
            animator.SetTrigger(triggerName);
    }

    protected void ResetAction()
    {
        isPerformingAction = false;

        if (movement != null)
            movement.SetCanMove(true);
    }

    #endregion

    #region Health Callbacks

    public void OnHealthChanged(
        float healthPercent)
    {
        UpdatePhaseFromPercent(
            healthPercent);
    }

    protected virtual void UpdatePhaseFromPercent(
        float healthPercent)
    {
        if (healthPercent <= 0.4f)
            currentPhase = 3;
        else if (healthPercent <= 0.7f)
            currentPhase = 2;
        else
            currentPhase = 1;
    }

    public void OnDeath()
    {
        if (isDead)
            return;

        isDead = true;
        isPerformingAction = false;

        if (movement != null)
            movement.OnBossDeath();

        if (animator != null)
            animator.SetTrigger("Die");
    }

    #endregion
}