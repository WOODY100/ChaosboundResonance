using UnityEngine;

public sealed class HealingPickup : AutoPickupBehaviour
{
    public const string ContentId =
        "healing_pickup";

    [Header("Healing")]
    [SerializeField]
    private float healAmount = 25f;

    [Header("Pickup")]
    [SerializeField]
    private float pickupRadius = 1f;

    protected override float GetPickupRadius()
    {
        return pickupRadius;
    }

    protected override void OnAutoPickupTriggered()
    {
        if (Player == null)
            return;

        PlayerHealth playerHealth =
            Player.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return;

        if (playerHealth.IsDead)
            return;

        float previousHealth =
            playerHealth.CurrentHealth;

        float maxHealth =
            playerHealth.MaxHealth;

        if (previousHealth >= maxHealth)
            return;

        playerHealth.Heal(
            healAmount);

        if (playerHealth.CurrentHealth > previousHealth)
        {
            ReturnToPool();
        }
    }

    private void OnValidate()
    {
        healAmount =
            Mathf.Max(
                0f,
                healAmount);

        pickupRadius =
            Mathf.Max(
                0f,
                pickupRadius);
    }
}