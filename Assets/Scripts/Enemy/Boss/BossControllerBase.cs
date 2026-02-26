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

    [SerializeField] protected int currentPhase = 1;
    protected bool isPerformingAction = false;
    protected bool isDead = false;
    protected bool furyUsed = false;

    protected virtual void Start()
    {
        health = GetComponent<BossHealth>();
    }

    protected virtual void Update()
    {
        if (isDead || isPerformingAction || player == null)
            return;

        UpdateCooldowns();
        EvaluateCombat();
    }

    protected virtual void Awake()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (movement != null && player != null)
            movement.SetPlayer(player);
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

    protected bool AttackReady() => attackTimer <= 0f;
    protected bool StompReady() => stompTimer <= 0f;
    protected bool ChargeReady() => chargeTimer <= 0f;
    protected bool JumpReady() => jumpTimer <= 0f;
    protected bool FuryReady() => furyTimer <= 0f;

    #endregion

    #region Abstract Combat Logic

    protected abstract void EvaluateCombat();

    #endregion

    #region Abilities

    protected void TriggerAction(string triggerName)
    {
        isPerformingAction = true;

        if (movement != null)
            movement.SetCanMove(false);

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
    public void OnHealthChanged(float healthPercent)
    {
        UpdatePhaseFromPercent(healthPercent);
    }

    protected virtual void UpdatePhaseFromPercent(float healthPercent)
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
        isDead = true;
        animator.SetTrigger("Die");
    }
    #endregion
}