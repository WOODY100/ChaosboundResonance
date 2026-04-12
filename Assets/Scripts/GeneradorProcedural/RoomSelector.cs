using System.Collections.Generic;
using UnityEngine;

public class RoomSelector : MonoBehaviour
{
    [Header("Neutral Rooms")]
    public List<GameObject> neutralRooms;

    [Header("Combat Rooms")]
    public List<GameObject> combatRooms;

    [Header("MiniBoss Rooms")]
    public List<GameObject> miniBossRooms;

    [Header("Boss Rooms")]
    public List<GameObject> bossRooms;

    Dictionary<string, List<GameObject>> roomLookup = new Dictionary<string, List<GameObject>>();

    void Awake()
    {
        BuildLookup(neutralRooms);
        BuildLookup(combatRooms);
    }

    void BuildLookup(List<GameObject> rooms)
    {
        foreach (var room in rooms)
        {
            string key = GetRoomKey(room);

            if (!roomLookup.ContainsKey(key))
                roomLookup[key] = new List<GameObject>();

            roomLookup[key].Add(room);
        }
    }

    // -------------------------
    // GENERATE KEY (Ej: "NS", "SEW")
    // -------------------------
    public GameObject GetRoom(RoomType type, EntryDirection requiredEntrance, int remainingRooms)
    {
        List<GameObject> pool = GetPool(type);

        if (pool == null || pool.Count == 0)
            return null;

        EntryDirection needed = DungeonConnectionUtility.Opposite(requiredEntrance);

        List<GameObject> candidates = new List<GameObject>();

        foreach (var room in pool)
        {
            var entries = room.GetComponentsInChildren<DungeonEntry>();

            bool hasEntrance = false;
            int doorCount = 0;

            foreach (var e in entries)
            {
                doorCount++;

                if (e.direction == needed)
                    hasEntrance = true;
            }

            if (!hasEntrance)
                continue;

            // 🔥 REGLAS POR TIPO
            if (type == RoomType.Neutral)
            {
                // 🔥 NO permitir dead ends hasta el final
                if (remainingRooms > 1 && doorCount < 2)
                    continue;

                // 🔥 NO permitir 2 doors muy temprano
                if (remainingRooms > 3 && doorCount < 3)
                    continue;
            }

            if (type == RoomType.Combat && doorCount < 1)
                continue;

            if (type == RoomType.MiniBoss && doorCount < 1)
                continue;

            // MiniBoss/Boss pueden ser dead end
            candidates.Add(room);
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    List<GameObject> GetPool(RoomType type)
    {
        switch (type)
        {
            case RoomType.Neutral:
                return neutralRooms;

            case RoomType.Combat:
                return combatRooms;

            case RoomType.MiniBoss:
                return miniBossRooms;

            case RoomType.Boss:
                return bossRooms;

            default:
                return null;
        }
    }

    string GetRoomKey(GameObject room)
    {
        var entries = room.GetComponentsInChildren<DungeonEntry>();

        List<string> dirs = new List<string>();

        foreach (var e in entries)
        {
            dirs.Add(e.direction.ToString()[0].ToString()); // N,S,E,W
        }

        dirs.Sort();
        return string.Join("", dirs);
    }

    // -------------------------
    // GET ROOM BY REQUIRED ENTRY
    // -------------------------
    public GameObject GetRoomWithEntrance(EntryDirection requiredEntrance)
    {
        EntryDirection needed = DungeonConnectionUtility.Opposite(requiredEntrance);

        List<GameObject> candidates = new List<GameObject>();

        foreach (var room in neutralRooms)
        {
            var entries = room.GetComponentsInChildren<DungeonEntry>();

            bool hasEntrance = false;
            int doorCount = 0;

            foreach (var e in entries)
            {
                doorCount++;

                if (e.direction == needed)
                    hasEntrance = true;
            }

            // 🔥 CLAVE: evitar dead ends
            if (hasEntrance && doorCount >= 2)
            {
                candidates.Add(room);
            }
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    public GameObject GetCombatRoomWithEntrance(EntryDirection requiredEntrance)
    {
        EntryDirection needed = DungeonConnectionUtility.Opposite(requiredEntrance);

        foreach (var room in combatRooms)
        {
            var entries = room.GetComponentsInChildren<DungeonEntry>();

            foreach (var e in entries)
            {
                if (e.direction == needed)
                {
                    return room;
                }
            }
        }

        return null;
    }
}