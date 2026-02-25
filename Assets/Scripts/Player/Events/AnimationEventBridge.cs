using UnityEngine;

public class PlayerAnimationBridge : MonoBehaviour
{
    private PlayerCombat combat;

    void Awake()
    {
        combat = GetComponentInParent<PlayerCombat>();
    }

    // Animation Event
    public void SpawnSlash()
    {
        if (combat != null)
            combat.SpawnSlash();
    }

    // Animation Event
    public void DealDamageInCone()
    {
        if (combat != null)
            combat.DealDamageInCone();
    }

    // Animation Event
    public void EndAttack()
    {
        if (combat != null)
            combat.EndAttack();
    }
}
