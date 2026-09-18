using UnityEngine;

public abstract class AutoPickupBehaviour : PooledBehaviour
{
    private Transform player;
    private bool pickupTriggered;

    private PlayerModifierSystem modifierSystem;

    protected Transform Player => player;
    protected bool IsPickupTriggered => pickupTriggered;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        pickupTriggered = false;
        ResolvePlayer();
    }

    protected override void OnDisable()
    {
        player = null;
        pickupTriggered = false;

        base.OnDisable();
    }

    private void Update()
    {
        if (player == null)
        {
            ResolvePlayer();

            if (player == null)
                return;
        }

        if (!pickupTriggered)
        {
            float sqrDistance =
                (transform.position - player.position).sqrMagnitude;

            float radius =
                Mathf.Max(0f, GetPickupRadius());

            if (sqrDistance <= radius * radius)
            {
                pickupTriggered = true;
                OnAutoPickupTriggered();
            }
        }

        OnAutoPickupUpdate();
    }

    protected abstract float GetPickupRadius();

    protected abstract void OnAutoPickupTriggered();

    protected virtual void OnAutoPickupUpdate()
    {
    }

    protected void ResolvePlayer()
    {
        if (player != null)
            return;

        if (EnemyManager.Instance == null)
            return;

        player = EnemyManager.Instance.Player;

        if (player == null)
            return;

        modifierSystem =
            player.GetComponent<PlayerModifierSystem>();
    }

    protected float GetPlayerPickupRadius()
    {
        if (modifierSystem == null)
            return 0f;

        return Mathf.Max(
            0f,
            modifierSystem.GetStat(
                StatType.PickupRadius));
    }

    protected override void ResetPooledState()
    {
        base.ResetPooledState();

        player = null;
        pickupTriggered = false;
    }
}