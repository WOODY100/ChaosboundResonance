using UnityEngine;

public class EnemyBrain : MonoBehaviour, IEnemyTickable
{
    private EnemyCore core;
    private EnemyAttack attack;
    private EnemyMovementArena arenaMovement;

    [Header("Behavior")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int maxAttackers = 6;
    [SerializeField] private bool isBoss = false;

    private static int activeAttackers = 0;
    private bool isRegisteredAttacker = false;

    void Awake()
    {
        core = GetComponent<EnemyCore>();
        attack = GetComponent<EnemyAttack>();
        arenaMovement = GetComponent<EnemyMovementArena>();
    }

    void OnEnable()
    {
        if (EnemyTickManager.Instance != null)
            EnemyTickManager.Instance.Register(this);
    }

    void OnDisable()
    {
        if (isRegisteredAttacker)
        {
            activeAttackers--;
            isRegisteredAttacker = false;
        }

        if (EnemyTickManager.Instance != null)
            EnemyTickManager.Instance.Unregister(this);
    }

    public void Tick()
    {
        if (core.Player == null)
            return;

        core.UpdateDistance();

        float distance = core.DistanceToPlayer;
        float attackRangeSqr = attackRange * attackRange;
        bool canAttack = distance <= attackRangeSqr;

        // 👑 BOSS (ignora slots)
        if (isBoss)
        {
            if (attack != null)
                attack.Tick();

            if (canAttack)
                arenaMovement.SetSpeed(0f);
            else
                arenaMovement.TickMovement(core.Player);

            return;
        }

        // 👇 ENEMIGOS NORMALES
        if (canAttack && activeAttackers < maxAttackers)
        {
            if (!isRegisteredAttacker)
            {
                activeAttackers++;
                isRegisteredAttacker = true;
            }

            if (attack != null)
                attack.Tick();
        }
        else
        {
            if (isRegisteredAttacker)
            {
                activeAttackers--;
                isRegisteredAttacker = false;
            }

            arenaMovement.TickMovement(core.Player);
        }
    }

    public static void ResetAttackSlots()
    {
        activeAttackers = 0;
    }
}