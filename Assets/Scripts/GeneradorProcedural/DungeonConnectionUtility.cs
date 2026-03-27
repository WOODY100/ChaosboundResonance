using UnityEngine;

public static class DungeonConnectionUtility
{
    // -------------------------
    // OPPOSITE DIRECTION
    // -------------------------
    public static EntryDirection Opposite(EntryDirection dir)
    {
        switch (dir)
        {
            case EntryDirection.North: return EntryDirection.South;
            case EntryDirection.South: return EntryDirection.North;
            case EntryDirection.East: return EntryDirection.West;
            case EntryDirection.West: return EntryDirection.East;
        }

        return dir;
    }

    // -------------------------
    // CONNECT USING SPECIFIC ENTRY
    // -------------------------
    public static bool TryConnectSpecific(
        DungeonEntry entryA,
        GameObject roomB,
        out DungeonEntry entryB)
    {
        entryB = null;

        var entriesB = roomB.GetComponentsInChildren<DungeonEntry>();
        EntryDirection needed = Opposite(entryA.direction);

        foreach (var b in entriesB)
        {
            if (!b.occupied && b.direction == needed)
            {
                ConnectAndAlign(entryA, b);
                entryB = b;
                return true;
            }
        }

        return false;
    }

    // -------------------------
    // ALIGN WITHOUT ROTATION
    // -------------------------
    public static void ConnectAndAlign(DungeonEntry a, DungeonEntry b)
    {
        Transform rootB = b.transform.root;

        // ⚠️ NO ROTAMOS (como necesitas)
        Vector3 offset = b.transform.position - rootB.position;
        rootB.position = a.transform.position - offset;

        a.occupied = true;
        b.occupied = true;
    }
}