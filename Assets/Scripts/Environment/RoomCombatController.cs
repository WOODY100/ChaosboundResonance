using UnityEngine;

public class RoomCombatController : MonoBehaviour
{
    public RoomState State { get; private set; } = RoomState.Idle;

    private RoomDoors doors;

    private void Awake()
    {
        doors = GetComponent<RoomDoors>();
    }

    public void StartCombat(RoomEntryDirection entryDirection)
    {
        if (State != RoomState.Idle)
            return;

        State = RoomState.Combat;

        if (doors != null)
            doors.CloseDoorsExcept(entryDirection);

        // 🔥 NUEVO
        var spawnPoints = GetComponent<RoomSpawnPoints>();
        var director = Object.FindAnyObjectByType<ArenaSpawnDirector>();

        if (director != null && spawnPoints != null)
        {
            director.StartArena(spawnPoints, doors);
        }
    }

    public void EndCombat()
    {
        if (State != RoomState.Combat)
            return;

        State = RoomState.Cleared;

        if (doors != null)
            doors.OpenDoors();
    }
}