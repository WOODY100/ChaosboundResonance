using UnityEngine;

public class RoomCombatController : MonoBehaviour
{
    public RoomState State { get; private set; } = RoomState.Idle;

    [Header("Room Config")]
    [SerializeField] private RoomType roomType;

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

        var spawnPoints = GetComponent<RoomSpawnPoints>();
        var director = Object.FindAnyObjectByType<ArenaSpawnDirector>();

        if (director != null && spawnPoints != null)
        {
            var context = new ArenaSpawnDirector.SpawnContext
            {
                encounterType = GetEncounterType(roomType),
                dungeonTier = GetDungeonTier() // 🔥 importante
            };

            director.StartEncounter(spawnPoints, doors, context);
        }
    }

    public void EndCombat()
    {
        if (State != RoomState.Combat)
            return;

        State = RoomState.Cleared;

        if (doors != null)
            doors.UpdateDoorsFromEntries();
    }

    // ----------------------------------
    // HELPERS
    // ----------------------------------

    ArenaSpawnDirector.EncounterType GetEncounterType(RoomType type)
    {
        switch (type)
        {
            case RoomType.Combat:
                return ArenaSpawnDirector.EncounterType.Combat;

            case RoomType.MiniBoss:
                return ArenaSpawnDirector.EncounterType.MiniBoss;

            case RoomType.Boss:
                return ArenaSpawnDirector.EncounterType.Boss;

            default:
                return ArenaSpawnDirector.EncounterType.None;
        }
    }

    int GetDungeonTier()
    {
        // 🔥 Placeholder (después lo conectas a tu meta progresión real)
        int metaLevel = 1;

        return Mathf.Clamp(metaLevel / 10 + 1, 1, 10);
    }
}