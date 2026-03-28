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

    private GameObject currentRoom;

    List<RoomType> dungeonPlan;
    List<GameObject> spawnedObjects = new List<GameObject>();

    // 🔥 GRID DE OCUPACIÓN
    Dictionary<Vector2Int, GameObject> occupied = new Dictionary<Vector2Int, GameObject>();
    Vector2Int currentGridPos = Vector2Int.zero;

    void Start()
    {
        StartCoroutine(GenerateWithRetry());
    }

    IEnumerator GenerateWithRetry()
    {
        int maxAttempts = 5;

        for (int i = 0; i < maxAttempts; i++)
        {
            ClearDungeon();

            bool success = GenerateDungeon();

            if (success)
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
            if (obj != null)
                Destroy(obj);
        }

        spawnedObjects.Clear();
        occupied.Clear();
    }

    bool GenerateDungeon()
    {
        dungeonPlan = DungeonPlanGenerator.GeneratePlan();

        GameObject start = Instantiate(startRoom, Vector3.zero, Quaternion.identity);
        spawnedObjects.Add(start);
        start.name = "Start";
        ResetEntries(start);

        currentRoom = start;
        currentGridPos = Vector2Int.zero;
        occupied[currentGridPos] = start;

        int placedRooms = 1; // start

        for (int i = 1; i < dungeonPlan.Count; i++)
        {
            bool roomPlaced = false; // 🔥 RESET POR ITERACIÓN

            RoomType roomType = dungeonPlan[i];

            List<DungeonEntry> validEntries = GetAllValidEntries(currentRoom, currentGridPos);
            Shuffle(validEntries);

            foreach (var entry in validEntries)
            {
                Vector2Int nextGridPos = currentGridPos + GetOffset(entry.direction);

                if (occupied.ContainsKey(nextGridPos))
                    continue;

                GameObject corridor = GetCorridor(entry.direction);
                spawnedObjects.Add(corridor);

                if (!DungeonConnectionUtility.TryConnectSpecific(entry, corridor, out var corridorEntry))
                {
                    Destroy(corridor);
                    continue;
                }

                DungeonEntry exitEntry = GetOtherEntry(corridor, corridorEntry);

                if (exitEntry == null)
                {
                    Destroy(corridor);
                    continue;
                }

                GameObject nextRoomPrefab = roomSelector.GetRoom(roomType, exitEntry.direction);

                if (nextRoomPrefab == null)
                {
                    Destroy(corridor);
                    continue;
                }

                GameObject nextRoom = Instantiate(nextRoomPrefab);
                spawnedObjects.Add(nextRoom);
                ResetEntries(nextRoom);

                if (!DungeonConnectionUtility.TryConnectSpecific(exitEntry, nextRoom, out _))
                {
                    Destroy(nextRoom);
                    Destroy(corridor);
                    continue;
                }

                // ✅ SUCCESS
                occupied[nextGridPos] = nextRoom;
                currentGridPos = nextGridPos;
                currentRoom = nextRoom;

                TrySpawnBranch(nextRoom, currentGridPos, roomType);

                placedRooms++;
                roomPlaced = true;
                break;
            }

            if (!roomPlaced)
            {
                Debug.LogWarning($"No se pudo colocar room tipo {roomType}");
            }
        }

        // 🔥 FINALIZAR PUERTAS
        FinalizeDoors();
        Debug.Log("FinalizeDoors ejecutado");

        // 🔥🔥🔥 AQUÍ VA LA VALIDACIÓN FINAL
        if (placedRooms < dungeonPlan.Count)
        {
            Debug.LogWarning($"Dungeon incompleta: {placedRooms}/{dungeonPlan.Count}");
            return false;
        }

        return true;
    }

    void FinalizeDoors()
    {
        foreach (var room in occupied.Values)
        {
            if (room == null) continue;

            RoomDoors doors = room.GetComponent<RoomDoors>();

            if (doors != null)
            {
                doors.UpdateDoorsFromEntries();
            }
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    List<DungeonEntry> GetAllValidEntries(GameObject room, Vector2Int currentPos)
    {
        var entries = room.GetComponentsInChildren<DungeonEntry>();

        List<DungeonEntry> valid = new List<DungeonEntry>();

        foreach (var e in entries)
        {
            if (e.occupied) continue;

            Vector2Int nextPos = currentPos + GetOffset(e.direction);

            if (!occupied.ContainsKey(nextPos))
                valid.Add(e);
        }

        return valid;
    }

    // ----------------------------------
    // BRANCH
    // ----------------------------------

    void TrySpawnBranch(GameObject room, Vector2Int roomPos, RoomType roomType)
    {
        if (Random.value > 0.5f) return;

        // ❌ No branching en bosses
        if (roomType == RoomType.Boss || roomType == RoomType.MiniBoss)
            return;

        var entries = room.GetComponentsInChildren<DungeonEntry>();

        foreach (var entry in entries)
        {
            if (entry.occupied) continue;

            // Solo laterales
            if (entry.direction != EntryDirection.East && entry.direction != EntryDirection.West)
                continue;

            Vector2Int branchPos = roomPos + GetOffset(entry.direction);

            if (occupied.ContainsKey(branchPos))
                continue;

            GameObject corridor = Instantiate(corridorEW);
            spawnedObjects.Add(corridor);

            if (!DungeonConnectionUtility.TryConnectSpecific(entry, corridor, out var corridorEntry))
            {
                Destroy(corridor);
                continue;
            }

            DungeonEntry exitEntry = GetOtherEntry(corridor, corridorEntry);

            if (exitEntry == null)
            {
                Destroy(corridor);
                continue;
            }

            GameObject combatPrefab = roomSelector.GetRoom(RoomType.Combat, exitEntry.direction);

            if (combatPrefab == null)
            {
                Destroy(corridor);
                continue;
            }

            GameObject combatRoom = Instantiate(combatPrefab);
            spawnedObjects.Add(combatRoom);
            ResetEntries(combatRoom);

            if (!DungeonConnectionUtility.TryConnectSpecific(exitEntry, combatRoom, out _))
            {
                Destroy(combatRoom);
                Destroy(corridor);
                continue;
            }

            occupied[branchPos] = combatRoom;
        }
    }

    // ----------------------------------
    // GRID UTILS
    // ----------------------------------

    void ResetEntries(GameObject room)
    {
        var entries = room.GetComponentsInChildren<DungeonEntry>();

        foreach (var entry in entries)
        {
            entry.occupied = false;
        }
    }

    Vector2Int GetOffset(EntryDirection dir)
    {
        switch (dir)
        {
            case EntryDirection.North: return new Vector2Int(0, 1);
            case EntryDirection.South: return new Vector2Int(0, -1);
            case EntryDirection.East: return new Vector2Int(1, 0);
            case EntryDirection.West: return new Vector2Int(-1, 0);
            default: return Vector2Int.zero;
        }
    }

    DungeonEntry GetValidEntry(GameObject room, Vector2Int currentPos)
    {
        var entries = room.GetComponentsInChildren<DungeonEntry>();

        List<DungeonEntry> valid = new List<DungeonEntry>();

        foreach (var e in entries)
        {
            if (e.occupied) continue;

            Vector2Int nextPos = currentPos + GetOffset(e.direction);

            if (!occupied.ContainsKey(nextPos))
                valid.Add(e);
        }

        if (valid.Count == 0) return null;

        return valid[Random.Range(0, valid.Count)];
    }

    DungeonEntry GetOtherEntry(GameObject room, DungeonEntry usedEntry)
    {
        var entries = room.GetComponentsInChildren<DungeonEntry>();

        foreach (var e in entries)
        {
            if (e != usedEntry)
                return e;
        }

        return null;
    }

    GameObject GetCorridor(EntryDirection dir)
    {
        if (dir == EntryDirection.North || dir == EntryDirection.South)
            return Instantiate(corridorNS);
        else
            return Instantiate(corridorEW);
    }
}