using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDungeonGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject startRoom;
    public GameObject corridorNS;
    public GameObject corridorEW;

    [Header("References")]
    public RoomSelector roomSelector;
    public RoomPool roomPool;

    [Header("Debug")]
    public bool useFixedSeed = false;
    public int fixedSeed = 12345;

    System.Random rng;
    Vector2Int bossPosition;

    List<(GameObject room, Vector2Int pos, EntryDirection dir, float score)> miniBossCandidates
        = new List<(GameObject, Vector2Int, EntryDirection, float)>();

    List<RoomType> dungeonPlan;
    List<GameObject> spawnedObjects = new List<GameObject>();
    List<RoomDoors> cachedDoors = new List<RoomDoors>();
    List<Vector2Int> frontier = new List<Vector2Int>();

    HashSet<Vector2Int> reservedPositions = new HashSet<Vector2Int>();

    Dictionary<Vector2Int, GameObject> occupied = new Dictionary<Vector2Int, GameObject>();
    Dictionary<GameObject, GameObject> instanceToPrefab = new Dictionary<GameObject, GameObject>();

    void Start()
    {
        StartCoroutine(GenerateWithRetry());
    }

    IEnumerator GenerateWithRetry()
    {
        int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            ClearDungeon();

            if (GenerateDungeon())
            {
                Debug.Log("Dungeon generada correctamente");
                yield break;
            }

            Debug.LogWarning("Reintentando generación...");
            yield return null;
        }

        Debug.LogError("No se pudo generar dungeon válida");
    }

    void ClearDungeon()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null && instanceToPrefab.ContainsKey(obj))
                roomPool.Return(obj, instanceToPrefab[obj]);
        }

        spawnedObjects.Clear();
        instanceToPrefab.Clear();
        occupied.Clear();
        cachedDoors.Clear();
        miniBossCandidates.Clear();
        reservedPositions.Clear();
        frontier.Clear();
    }

    bool GenerateDungeon()
    {
        int seed = useFixedSeed ? fixedSeed : Random.Range(0, 999999);
        rng = new System.Random(seed);

        Debug.Log($"[GEN] Seed: {seed}");

        dungeonPlan = DungeonPlanGenerator.GeneratePlan(rng);

        Debug.Log($"[GEN] Plan: {string.Join(", ", dungeonPlan)}");

        // =========================
        // 🟢 MAIN PATH GARANTIZADO
        // =========================

        Vector2Int currentPos = Vector2Int.zero;
        GameObject currentRoom = Spawn(startRoom, Vector3.zero);

        occupied[currentPos] = currentRoom;

        for (int i = 1; i < dungeonPlan.Count; i++)
        {
            RoomType roomType = dungeonPlan[i];

            bool placed = false;

            var entries = GetAllValidEntries(currentRoom, currentPos);
            Shuffle(entries);

            foreach (var entry in entries)
            {
                Vector2Int nextPos = currentPos + GetOffset(entry.direction);

                if (occupied.ContainsKey(nextPos))
                    continue;

                GameObject corridor = Spawn(GetCorridorPrefab(entry.direction));

                if (!DungeonConnectionUtility.TryConnectSpecific(entry, corridor, out var corridorEntry))
                {
                    Return(corridor);
                    continue;
                }

                var exitEntry = GetOtherEntry(corridor, corridorEntry);
                if (exitEntry == null)
                {
                    Return(corridor);
                    continue;
                }

                var neededDir = DungeonConnectionUtility.Opposite(entry.direction);

                GameObject prefab = roomSelector.GetRoom(roomType, neededDir, 0);

                if (prefab == null || !HasEntry(prefab, neededDir))
                {
                    Return(corridor);
                    continue;
                }

                GameObject nextRoom = Spawn(prefab);

                if (!DungeonConnectionUtility.TryConnectSpecific(exitEntry, nextRoom, out _))
                {
                    Return(nextRoom);
                    Return(corridor);
                    continue;
                }

                // 🔥 CONFIRMAR COLOCACIÓN
                occupied[nextPos] = nextRoom;

                currentPos = nextPos;
                currentRoom = nextRoom;

                if (roomType == RoomType.Boss)
                    bossPosition = nextPos;

                placed = true;
                break;
            }

            // 🔥 SI FALLA (muy raro), fuerza fallback lineal
            if (!placed)
            {
                Debug.LogError($"💀 No se pudo colocar {roomType} en step {i}");
                Debug.LogError($"💀 Seed fallida: {seed}");
                return false;
            }
        }

        // =========================
        // 🟠 BRANCHES (MINIBOSS)
        // =========================

        var snapshot = new List<KeyValuePair<Vector2Int, GameObject>>(occupied);

        foreach (var kvp in snapshot)
        {
            if (kvp.Key == bossPosition)
                continue;

            if (rng.NextDouble() > 0.4f)
                continue;

            var entries = GetAllValidEntries(kvp.Value, kvp.Key);

            foreach (var entry in entries)
            {
                Vector2Int branchPos = kvp.Key + GetOffset(entry.direction);

                if (occupied.ContainsKey(branchPos))
                    continue;

                if (TrySpawnMiniBoss(kvp.Value, kvp.Key, entry.direction))
                    break;
            }
        }

        Debug.Log($"🔥 Rooms generadas: {occupied.Count}");

        FinalizeDoors();

        return true; // 🔥 GARANTIZADO
    }

    void SpawnMiniBoss()
    {
        miniBossCandidates.Sort((a, b) => b.score.CompareTo(a.score));

        foreach (var c in miniBossCandidates)
        {
            if (TrySpawnMiniBoss(c.room, c.pos, c.dir))
                return;
        }

        TryForceMiniBoss();
    }

    bool TrySpawnMiniBoss(GameObject room, Vector2Int pos, EntryDirection dir)
    {
        Vector2Int branchPos = pos + GetOffset(dir);

        if (occupied.ContainsKey(branchPos))
            return false;

        var entry = GetEntryByDirection(room, dir);
        if (entry == null)
            return false;

        var neededDir = DungeonConnectionUtility.Opposite(dir);

        GameObject prefab = roomSelector.GetRoom(RoomType.MiniBoss, neededDir, 0);
        if (prefab == null)
            return false;

        GameObject corridor = Spawn(GetCorridorPrefab(dir));

        if (!DungeonConnectionUtility.TryConnectSpecific(entry, corridor, out var corridorEntry))
        {
            Return(corridor);
            return false;
        }

        var exit = GetOtherEntry(corridor, corridorEntry);
        if (exit == null)
        {
            Return(corridor);
            return false;
        }

        GameObject roomObj = Spawn(prefab);

        if (!DungeonConnectionUtility.TryConnectSpecific(exit, roomObj, out _))
        {
            Return(roomObj);
            Return(corridor);
            return false;
        }

        occupied[branchPos] = roomObj;

        Debug.Log($"🔥 MiniBoss SPAWNED en {branchPos} desde {pos} dir {dir}");

        return true;
    }

    bool TryForceMiniBoss()
    {
        foreach (var kvp in occupied)
        {
            if (kvp.Key == bossPosition)
                continue;

            var entries = GetAllValidEntries(kvp.Value, kvp.Key);

            foreach (var e in entries)
            {
                if (TrySpawnMiniBoss(kvp.Value, kvp.Key, e.direction))
                {
                    Debug.Log($"🔥 MiniBoss SPAWNED (fallback) en {kvp.Key}");
                    return true;
                }
            }
        }

        return false;
    }

    void TrySpawnBranch(GameObject room, Vector2Int pos)
    {
        if (rng.NextDouble() > 0.5f) return;

        var entries = GetAllValidEntries(room, pos);

        foreach (var e in entries)
        {
            Vector2Int branchPos = pos + GetOffset(e.direction);

            if (occupied.ContainsKey(branchPos))
                continue;

            GameObject corridor = Spawn(GetCorridorPrefab(e.direction));

            if (!DungeonConnectionUtility.TryConnectSpecific(e, corridor, out var corridorEntry))
            {
                Return(corridor);
                continue;
            }

            var exit = GetOtherEntry(corridor, corridorEntry);
            if (exit == null)
            {
                Return(corridor);
                continue;
            }

            var neededDir = DungeonConnectionUtility.Opposite(e.direction);

            GameObject prefab = roomSelector.GetRoom(RoomType.Combat, neededDir, 0);
            if (prefab == null)
            {
                Return(corridor);
                continue;
            }

            GameObject newRoom = Spawn(prefab);

            if (!DungeonConnectionUtility.TryConnectSpecific(exit, newRoom, out _))
            {
                Return(newRoom);
                Return(corridor);
                continue;
            }

            occupied[branchPos] = newRoom;
        }
    }

    GameObject Spawn(GameObject prefab, Vector3 pos = default)
    {
        GameObject obj = roomPool.Get(prefab);
        obj.transform.position = pos;

        spawnedObjects.Add(obj);
        instanceToPrefab[obj] = prefab;

        RegisterRoom(obj);
        ResetEntries(obj);

        return obj;
    }

    void Return(GameObject obj)
    {
        if (obj != null && instanceToPrefab.ContainsKey(obj))
        {
            roomPool.Return(obj, instanceToPrefab[obj]);
            instanceToPrefab.Remove(obj);
        }
    }

    bool HasFutureSpace(Vector2Int pos)
    {
        foreach (EntryDirection dir in System.Enum.GetValues(typeof(EntryDirection)))
        {
            if (!occupied.ContainsKey(pos + GetOffset(dir)))
                return true;
        }
        return false;
    }

    void FinalizeDoors()
    {
        foreach (var d in cachedDoors)
            d?.UpdateDoorsFromEntries();
    }

    void RegisterRoom(GameObject room)
    {
        var doors = room.GetComponent<RoomDoors>();
        if (doors != null)
            cachedDoors.Add(doors);
    }

    void ResetEntries(GameObject room)
    {
        foreach (var e in room.GetComponentsInChildren<DungeonEntry>())
            e.occupied = false;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = rng.Next(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    GameObject GetCorridorPrefab(EntryDirection dir)
    {
        return (dir == EntryDirection.North || dir == EntryDirection.South) ? corridorNS : corridorEW;
    }

    Vector2Int GetOffset(EntryDirection dir)
    {
        return dir switch
        {
            EntryDirection.North => new Vector2Int(0, 1),
            EntryDirection.South => new Vector2Int(0, -1),
            EntryDirection.East => new Vector2Int(1, 0),
            EntryDirection.West => new Vector2Int(-1, 0),
            _ => Vector2Int.zero
        };
    }

    DungeonEntry GetOtherEntry(GameObject room, DungeonEntry used)
    {
        foreach (var e in room.GetComponentsInChildren<DungeonEntry>())
            if (e != used)
                return e;

        return null;
    }

    List<DungeonEntry> GetAllValidEntries(GameObject room, Vector2Int pos)
    {
        List<DungeonEntry> list = new();

        foreach (var e in room.GetComponentsInChildren<DungeonEntry>())
        {
            if (e.occupied) continue;

            Vector2Int next = pos + GetOffset(e.direction);
            if (!occupied.ContainsKey(next))
                list.Add(e);
        }

        return list;
    }

    bool HasEntry(GameObject prefab, EntryDirection dir)
    {
        foreach (var e in prefab.GetComponentsInChildren<DungeonEntry>())
            if (e.direction == dir)
                return true;

        return false;
    }

    bool CanPlaceRoomChain(GameObject room, DungeonEntry entry, RoomType type, int remaining)
    {
        var needed = DungeonConnectionUtility.Opposite(entry.direction);
        var prefab = roomSelector.GetRoom(type, needed, remaining);

        return prefab != null && HasEntry(prefab, needed);
    }

    DungeonEntry GetEntryByDirection(GameObject room, EntryDirection dir)
    {
        foreach (var e in room.GetComponentsInChildren<DungeonEntry>())
            if (e.direction == dir)
                return e;

        return null;
    }
}