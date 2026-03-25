using UnityEngine;

public class SimpleDungeonGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject startRoom;
    public GameObject[] combatRooms;
    public GameObject[] neutralRooms;

    public GameObject miniBossRoom;
    public GameObject bossRoom;

    public GameObject corridorNS;
    public GameObject corridorEW;

    [Header("Settings")]
    public int mainPathLength = 4;

    private GameObject lastRoom;

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        // -------------------------
        // START
        // -------------------------
        GameObject start = Instantiate(startRoom, Vector3.zero, Quaternion.identity);
        start.name = "Start";
        lastRoom = start;

        Transform currentExit = FindEntry(start, "EntryNorth");

        // -------------------------
        // MAIN PATH
        // -------------------------
        for (int i = 0; i < mainPathLength; i++)
        {
            // Corridor
            GameObject corridor = Instantiate(corridorNS);
            corridor.name = "Corridor_Main_" + i;

            AlignPieces(
                currentExit,
                FindEntry(corridor, "EntrySouth")
            );

            // Random room (combat o neutral)
            GameObject nextRoomPrefab = Random.value > 0.5f
                ? combatRooms[Random.Range(0, combatRooms.Length)]
                : neutralRooms[Random.Range(0, neutralRooms.Length)];

            GameObject nextRoom = Instantiate(nextRoomPrefab);
            nextRoom.name = "MainRoom_" + i;

            AlignPieces(
                FindEntry(corridor, "EntryNorth"),
                FindEntry(nextRoom, "EntrySouth")
            );

            // OPCIONAL: RAMA LATERAL
            TrySpawnBranch(nextRoom);

            currentExit = FindEntry(nextRoom, "EntryNorth");
            lastRoom = nextRoom;
        }

        // -------------------------
        // MINIBOSS
        // -------------------------
        GameObject corridorToMiniBoss = Instantiate(corridorNS);

        AlignPieces(
            currentExit,
            FindEntry(corridorToMiniBoss, "EntrySouth")
        );

        GameObject miniBoss = Instantiate(miniBossRoom);
        miniBoss.name = "MiniBoss";

        AlignPieces(
            FindEntry(corridorToMiniBoss, "EntryNorth"),
            FindEntry(miniBoss, "EntrySouth")
        );

        // -------------------------
        // BOSS
        // -------------------------
        GameObject corridorToBoss = Instantiate(corridorNS);

        AlignPieces(
            FindEntry(miniBoss, "EntryNorth"),
            FindEntry(corridorToBoss, "EntrySouth")
        );

        GameObject boss = Instantiate(bossRoom);
        boss.name = "Boss";

        AlignPieces(
            FindEntry(corridorToBoss, "EntryNorth"),
            FindEntry(boss, "EntrySouth")
        );
    }

    // ----------------------------------
    // OPTIONAL BRANCH
    // ----------------------------------

    void TrySpawnBranch(GameObject room)
    {
        if (Random.value > 0.5f) return;

        string[] possibleEntries = { "EntryEast", "EntryWest" };

        foreach (var entryName in possibleEntries)
        {
            Transform entry = FindEntry(room, entryName);
            if (entry == null) continue;

            GameObject corridor = Instantiate(corridorEW);
            corridor.name = "BranchCorridor";

            AlignPieces(
                entry,
                FindEntry(corridor, OppositeEntry(entryName))
            );

            GameObject combat = Instantiate(combatRooms[Random.Range(0, combatRooms.Length)]);
            combat.name = "BranchCombat";

            AlignPieces(
                FindEntry(corridor, entryName),
                FindEntry(combat, OppositeEntry(entryName))
            );
        }
    }

    // ----------------------------------
    // FIND ENTRY
    // ----------------------------------

    Transform FindEntry(GameObject obj, string entryName)
    {
        Transform entries = obj.transform.Find("Entries");
        if (entries == null) return null;

        return entries.Find(entryName);
    }

    // ----------------------------------
    // OPPOSITE ENTRY
    // ----------------------------------

    string OppositeEntry(string entry)
    {
        switch (entry)
        {
            case "EntryNorth": return "EntrySouth";
            case "EntrySouth": return "EntryNorth";
            case "EntryEast": return "EntryWest";
            case "EntryWest": return "EntryEast";
        }
        return "";
    }

    // ----------------------------------
    // ALIGN
    // ----------------------------------

    void AlignPieces(Transform entryA, Transform entryB)
    {
        if (entryA == null || entryB == null) return;

        Transform root = entryB.root;
        Vector3 offset = entryB.position - root.position;
        root.position = entryA.position - offset;
    }
}