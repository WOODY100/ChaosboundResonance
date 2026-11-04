using UnityEngine;

public sealed class HealingPickup : AutoPickupBehaviour
{
    public const string ContentId =
        "healing_pickup";

    [Header("Healing")]
    [SerializeField]
    private float healAmount = 25f;

    [Header("Audio")]
    [SerializeField]
    private PowerUpAudioDefinition audioDefinition;

    protected override float GetPickupRadius()
    {
        return GetPlayerPickupRadius();
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

        // Keep the pickup available. OnAutoPickupUpdate will re-arm it
        // once the player has lost health.
        if (previousHealth >= maxHealth)
            return;

        playerHealth.Heal(healAmount, "Pickup");

        if (playerHealth.CurrentHealth > previousHealth)
        {
            PlayPickupAudio();
            ReturnToPool();
        }
    }

    protected override void OnAutoPickupUpdate()
    {
        if (!IsPickupTriggered || Player == null)
            return;

        PlayerHealth playerHealth =
            Player.GetComponent<PlayerHealth>();

        if (playerHealth == null || playerHealth.IsDead)
            return;

        // The pickup previously failed because health was full.
        // Re-arm it only after health drops, avoiding repeated attempts
        // every frame while the player remains at full health.
        if (playerHealth.CurrentHealth < playerHealth.MaxHealth)
        {
            ResetPickupTrigger();
        }
    }

    private void PlayPickupAudio()
    {
        if (audioDefinition == null)
            return;

        if (GameplayAudioPlayer.Instance == null)
            return;

        GameplayAudioPlayer.Instance.PlayOneShot(
            audioDefinition.PickupClip,
            audioDefinition.PickupVolume);
    }
}
