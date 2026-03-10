using UnityEngine;

public class DungeonStartCombatTest : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject startRoom;
    public GameObject combatRoom;

    public GameObject corridorNS;
    public GameObject corridorEW;

    void Start()
    {
        // -------------------------
        // SPAWN START ROOM
        // -------------------------

        GameObject start = Instantiate(startRoom, Vector3.zero, Quaternion.identity);
        start.name = "StartRoom";

        // -------------------------
        // SPAWN CORRIDOR (NORTH)
        // -------------------------

        GameObject corridor = Instantiate(corridorNS);
        corridor.name = "Corridor_North";

        AlignPieces(
            FindEntry(start, "EntryNorth"),
            FindEntry(corridor, "EntrySouth")
        );

        // -------------------------
        // SPAWN COMBAT ROOM
        // -------------------------

        GameObject combat = Instantiate(combatRoom);
        combat.name = "CombatRoom";

        AlignPieces(
            FindEntry(corridor, "EntryNorth"),
            FindEntry(combat, "EntrySouth")
        );
    }

    // ----------------------------------
    // FIND ENTRY
    // ----------------------------------

    Transform FindEntry(GameObject obj, string entryName)
    {
        Transform entries = obj.transform.Find("Entries");

        if (entries == null)
        {
            Debug.LogError($"Entries not found in {obj.name}");
            return null;
        }

        Transform entry = entries.Find(entryName);

        if (entry == null)
        {
            Debug.LogError($"{entryName} not found in {obj.name}");
        }

        return entry;
    }

    // ----------------------------------
    // ALIGN PIECES
    // ----------------------------------

    void AlignPieces(Transform entryA, Transform entryB)
    {
        if (entryA == null || entryB == null)
            return;

        Transform pieceRoot = entryB.root;

        Vector3 localOffset = entryB.position - pieceRoot.position;

        pieceRoot.position = entryA.position - localOffset;
    }
}