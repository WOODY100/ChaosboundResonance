using System.Collections;
using UnityEngine;

public class MinotaurBossController : BossControllerBase
{
    [SerializeField] private GameObject jumpWarningPrefab;
    [SerializeField] private float jumpWarningDuration = 0.8f;

    private Vector3 jumpTargetPosition;

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
            StartCoroutine(JumpWithWarning());
            jumpTimer = jumpCooldown;
        }
        else if (distance > meleeRange && ChargeReady())
        {
            TriggerAction("Charge");
            chargeTimer = chargeCooldown;
        }
        else if (distance <= meleeRange && StompReady())
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

    // Animation Event
    public void OnActionFinished()
    {
        ResetAction();
    }

    public void StartCharge()
    {
        if (movement != null)
            movement.StartCharge();
    }

    public void StopCharge()
    {
        if (movement != null)
            movement.StopCharge();
    }

    public void StartJumpMovement()
    {
        if (movement == null)
            return;

        Vector3 startPos = transform.position;
        Vector3 targetPos = jumpTargetPosition;

        startPos.y = 0f;
        targetPos.y = 0f;

        Vector3 dir = (targetPos - startPos).normalized;

        float distance = Vector3.Distance(startPos, targetPos);

        float duration = 0.45f;
        float speed = distance / duration;

        movement.ForceMove(dir, speed, duration);
    }

    private IEnumerator JumpWithWarning()
    {
        isPerformingAction = true;

        jumpTargetPosition = player.position;
        jumpTargetPosition.y = 0f;

        GameObject warning = Instantiate(
            jumpWarningPrefab,
            jumpTargetPosition + Vector3.up * 0.05f,
            Quaternion.Euler(90f, 0f, 0f)
        );

        // Ajustar tamaño según radio real
        float radius = 3f; // o usa skill.Stats.FinalImpactRadius
        warning.transform.localScale = Vector3.one * radius;
        
        TriggerAction("JumpAttack");

        yield return new WaitForSeconds(jumpWarningDuration);

        Destroy(warning);
    }
}