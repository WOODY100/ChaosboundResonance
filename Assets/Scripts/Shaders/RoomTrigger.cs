using UnityEngine;

public enum RoomEntryDirection
{
    North,
    South,
    East,
    West
}

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private RoomEntryDirection entryDirection;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || triggered)
            return;

        RoomCombatController combat = GetComponentInParent<RoomCombatController>();

        if (combat == null)
            return;

        // Si ya fue completada no hacemos nada
        if (combat.State == RoomState.Cleared)
            return;

        triggered = true;

        // ✅ SOLO esto
        combat.StartCombat(entryDirection);
    }
}