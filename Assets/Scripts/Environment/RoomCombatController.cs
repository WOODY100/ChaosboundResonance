using UnityEngine;

public class RoomCombatController : MonoBehaviour
{
    public RoomState State { get; private set; } = RoomState.Idle;

    [Header("Room Config")]
    [SerializeField] private RoomType roomType;

    [Header("Auto Start")]
    [SerializeField] private bool autoStartOnPlay = false;
    [SerializeField] private RoomEntryDirection autoEntryDirection = RoomEntryDirection.South;
    [SerializeField] private float autoStartDelay = 0.5f;

    private RoomDoors doors;

    private void Awake()
    {
        doors = GetComponent<RoomDoors>();
    }

    private void Start()
    {
        Debug.Log($"[{name}] Start() ejecutado");

        if (autoStartOnPlay)
        {
            Debug.Log($"[{name}] AutoStart activado");
            Invoke(nameof(AutoStartCombat), autoStartDelay);
        }
    }

    private void AutoStartCombat()
    {
        Debug.Log($"[{name}] AutoStartCombat()");
        StartCombat(autoEntryDirection);
    }

    public void StartCombat(RoomEntryDirection entryDirection)
    {
        Debug.Log($"[{name}] StartCombat()");

        if (State != RoomState.Idle)
            return;

        State = RoomState.Combat;

        if (doors != null)
            doors.CloseDoorsExcept(entryDirection);

        var spawnPoints = GetComponent<RoomSpawnPoints>();
        var director = Object.FindAnyObjectByType<ArenaSpawnDirector>();

        Debug.Log($"[{name}] Director: {director}");
        Debug.Log($"[{name}] SpawnPoints: {spawnPoints}");

        if (director != null && spawnPoints != null)
        {
            Debug.Log($"[{name}] Calling director.StartEncounter()");

            var context = new ArenaSpawnDirector.SpawnContext
            {
                encounterType = GetEncounterType(roomType),
                dungeonTier = GetDungeonTier()
            };

            director.StartEncounter(spawnPoints, doors, context);
        }
        else
        {
            Debug.LogWarning($"[{name}] Missing director or spawn points.");
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
        int metaLevel = 1;
        return Mathf.Clamp(metaLevel / 10 + 1, 1, 10);
    }
}