using System.Collections;
using UnityEngine;

public class MinotaurBossController : BossControllerBase
{
    [Header("Attack Settings")]
    [SerializeField] private GameObject chargeGhostPrefab;
    [SerializeField] private float ghostSpawnInterval = 0.05f;
    [SerializeField] private AnimationClip jumpClip;
    [SerializeField] private GameObject jumpWarningPrefab;
    [SerializeField] private GameObject jumpImpactVFX;
    [SerializeField] private float jumpImpactRadius = 3f;
    //[SerializeField] private float jumpRecoveryDuration = 1.2f;
    [SerializeField] private float meleeRadius = 2f;
    [SerializeField] private float stompRadius = 3f;
    [SerializeField] private float chargeRadius = 1.5f;
    [SerializeField] private float chargeVulnerabilityDuration = 0.8f;

    [Header("Damage Settings")]
    [SerializeField] private float jumpDamage = 50f;
    [SerializeField] private float meleeDamage = 25f;
    [SerializeField] private float stompDamage = 35f;
    [SerializeField] private float chargeDamage = 30f;
    [SerializeField] private float chargeDamageInterval = 0.3f;

    private float chargeDamageTimer = 0f;
    private bool isChargeDamageActive = false;
    private float ghostTimer;

    private GameObject currentJumpWarning;
    private JumpWarning_Bestial currentWarningScript;
    private BossMovementController movementController;

    // Impacto
    private const float totalFrames = 114f;
    private const float takeoffFrame = 17f;
    private const float impactFrame = 50f;
    private Vector3 jumpTargetPosition;

    protected override void Start()
    {
        base.Start();

        if (movement != null)
        {
            movement.OnChargeTick += HandleChargeDamage;
            movement.OnChargeStart += ActivateChargeDamage;
            movement.OnChargeEnd += DeactivateChargeDamage;
            movement.OnChargeEnd += OnChargeFinished;
        }
    }

    protected override void Update()
    {
        if (isDead)
            return;

        base.Update();

        HandleChargeDamage();
    }

    protected override void EvaluateCombat()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (currentPhase == 1)
            PhaseOne(distance);
        else if (currentPhase == 2)
            PhaseTwo(distance);
        else
            PhaseThree(distance);
    }

    private void PhaseOne(float distance)
    {
        if (distance <= meleeRange)
        {
            if (StompReady())
            {
                TriggerAction("Stomp");
                stompTimer = stompCooldown;
            }
            else if (AttackReady())
            {
                TriggerAction("Attack");
                attackTimer = attackCooldown;
            }
        }
    }

    private void PhaseTwo(float distance)
    {
        if (distance > meleeRange && distance <= midRange && ChargeReady())
        {
            TriggerAction("Charge");
            chargeTimer = chargeCooldown;
        }
        else if (distance <= meleeRange)
        {
            if (StompReady())
            {
                TriggerAction("Stomp");
                stompTimer = stompCooldown;
            }
            else if (AttackReady())
            {
                TriggerAction("Attack");
                attackTimer = attackCooldown;
            }
        }
    }

    private void PhaseThree(float distance)
    {
        if (!furyUsed && FuryReady())
        {
            TriggerAction("Fury");
            furyTimer = furyCooldown;
            furyUsed = true;
            return;
        }

        if (distance > midRange && JumpReady())
        {
            jumpTimer = jumpCooldown;

            Vector3 target = player.position;
            target.y = 0f;
            jumpTargetPosition = target;

            TriggerAction("JumpAttack");
            SpawnJumpWarning();
        }
        else if (distance > meleeRange && ChargeReady())
        {
            TriggerAction("Charge");
            chargeTimer = chargeCooldown;
        }
        else if (distance <= meleeRange)
        {
            if (StompReady())
            {
                TriggerAction("Stomp");
                stompTimer = stompCooldown;
            }
            else if (AttackReady())
            {
                TriggerAction("Attack");
                attackTimer = attackCooldown;
            }
        }
    }

    // Damage Application - Animation Events
    public void OnMeleeHit()
    {
        DealRadialDamage(meleeRadius, meleeDamage);
    }

    public void OnStompHit()
    {
        DealRadialDamage(stompRadius, stompDamage);
    }

    // Animation Event
    public void OnActionFinished()
    {
        ResetAction();
    }

    public void StartCharge()
    {
        movement?.StartCharge();
    }

    public void StopCharge()
    {
        movement?.StopCharge();
    }

    public void StartJumpMovement()
    {
        if (movement == null || jumpClip == null)
            return;

        if (currentWarningScript != null)
        {
            currentWarningScript.SetIntensity(2f);
        }

        Vector3 startPos = transform.position;
        Vector3 targetPos = jumpTargetPosition;

        startPos.y = 0f;
        targetPos.y = 0f;

        Vector3 dir = (targetPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, targetPos);

        float airFrames = impactFrame - takeoffFrame;

        float clipLength = jumpClip.length;
        float airClipDuration = (airFrames / totalFrames) * clipLength;

        float desiredAirDuration = Mathf.Clamp(distance / 8f, 0.4f, 1.2f);

        animator.speed = airClipDuration / desiredAirDuration;

        float speed = distance / desiredAirDuration;

        movement.ForceMove(dir, speed, desiredAirDuration, jumpTargetPosition);
    }

    private void SpawnJumpWarning()
    {
        if (jumpWarningPrefab == null)
            return;

        currentJumpWarning = Instantiate(
            jumpWarningPrefab,
            jumpTargetPosition + Vector3.up * 0.05f,
            Quaternion.identity
        );

        currentWarningScript = currentJumpWarning.GetComponent<JumpWarning_Bestial>();

        // 🔴 Escalar por diámetro (Quad usa tamaño completo)
        float diameter = jumpImpactRadius * 2f;

        currentJumpWarning.transform.localScale =
            new Vector3(diameter, 1f, diameter);
    }

    public void OnJumpImpact()
    {
        movement.StopImmediately();

        Vector3 impactPosition = transform.position;
        impactPosition.y = 0f;

        if (currentWarningScript != null)
        {
            currentWarningScript.SetIntensity(3f);
        }

        if (currentJumpWarning != null)
        {
            Destroy(currentJumpWarning);
            currentJumpWarning = null;
        }

        // 🔹 1. Spawn VFX
        if (jumpImpactVFX != null)
            Instantiate(jumpImpactVFX, impactPosition, Quaternion.identity);

        // 🔹 2. Daño plano radial
        Collider[] hits = Physics.OverlapSphere(impactPosition, jumpImpactRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    DamageData dmg = new DamageData(
                        jumpDamage,
                        DamageType.Physical
                    );
                    damageable.TakeDamage(dmg);
                }
            }
        }

        // 🔹 3. Sensación de peso
        StartCoroutine(ImpactFreeze());

        // 🔹 4. Vulnerabilidad
        StartCoroutine(JumpRecoveryWindow());
    }

    private IEnumerator ImpactFreeze()
    {
        float originalTimeScale = Time.timeScale;

        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(0.05f);

        Time.timeScale = originalTimeScale;
    }

    private IEnumerator JumpRecoveryWindow()
    {
        movement.StopImmediately();
        movement.SetCanMove(false);
        isPerformingAction = true;

        // Esperar hasta que la animación termine realmente
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
        );

        animator.speed = 1f;
        movement.SetCanMove(true);
        ResetAction();
    }

    private void DealRadialDamage(float radius, float damage)
    {
        Vector3 center = transform.position;
        center.y = 0f;

        Collider[] hits = Physics.OverlapSphere(center, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerDamageReceiver receiver = hit.GetComponent<PlayerDamageReceiver>();

                if (receiver != null)
                {
                    DamageData dmg = new DamageData
                    {
                        amount = damage,
                        type = DamageType.Physical,
                        source = gameObject
                    };

                    receiver.ReceiveDamage(dmg);
                }
            }
        }
    }

    private void ActivateChargeDamage()
    {
        isChargeDamageActive = true;
    }

    private void HandleChargeDamage()
    {
        if (!isChargeDamageActive)
            return;  // 🔥 ESTA LÍNEA FALTABA

        ghostTimer -= Time.deltaTime;

        if (ghostTimer <= 0f)
        {
            SpawnGhost();
            ghostTimer = ghostSpawnInterval;
        }

        chargeDamageTimer -= Time.deltaTime;

        if (chargeDamageTimer > 0f)
            return;

        chargeDamageTimer = chargeDamageInterval;

        Vector3 center = transform.position;
        center.y = 0f;

        Collider[] hits = Physics.OverlapSphere(center, chargeRadius);

        foreach (var hit in hits)
        {
            PlayerDamageReceiver receiver =
                hit.GetComponent<PlayerDamageReceiver>();

            if (receiver != null)
            {
                DamageData dmg = new DamageData
                {
                    amount = chargeDamage,
                    type = DamageType.Physical,
                    source = gameObject
                };

                receiver.ReceiveDamage(dmg);
            }
        }
    }

    private void DeactivateChargeDamage()
    {
        isChargeDamageActive = false;
    }

    private void SpawnGhost()
    {
        SkinnedMeshRenderer original =
            GetComponentInChildren<SkinnedMeshRenderer>();

        if (original == null)
            return;

        Mesh bakedMesh = new Mesh();
        original.BakeMesh(bakedMesh);

        GameObject ghost = Instantiate(
            chargeGhostPrefab,
            original.transform.position,
            original.transform.rotation
        );

        MeshFilter mf = ghost.GetComponent<MeshFilter>();
        mf.mesh = bakedMesh;

        ghost.transform.localScale = Vector3.one;
    }

    private void OnChargeFinished()
    {
        StartCoroutine(ChargeVulnerabilityWindow());
    }

    private IEnumerator ChargeVulnerabilityWindow()
    {
        movement.SetCanMove(false);
        isPerformingAction = true;

        yield return new WaitForSeconds(chargeVulnerabilityDuration);

        movement.SetCanMove(true);
        isPerformingAction = false;
    }
}