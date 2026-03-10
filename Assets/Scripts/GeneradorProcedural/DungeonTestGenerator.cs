using UnityEngine;

public class DungeonTestGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject roomPrefab;
    public GameObject corridorEW;
    public GameObject corridorNS;

    void Start()
    {
        // -------------------------
        // SALA CENTRAL
        // -------------------------

        GameObject centerRoom = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);

        // -------------------------
        // WEST
        // -------------------------

        GameObject corridorWest = Instantiate(corridorEW);
        AlignPieces(
            FindEntry(centerRoom, "EntryWest"),
            FindEntry(corridorWest, "EntryEast")
        );

        GameObject roomWest = Instantiate(roomPrefab);
        AlignPieces(
            FindEntry(corridorWest, "EntryWest"),
            FindEntry(roomWest, "EntryEast")
        );

        // -------------------------
        // EAST
        // -------------------------

        GameObject corridorEast = Instantiate(corridorEW);
        AlignPieces(
            FindEntry(centerRoom, "EntryEast"),
            FindEntry(corridorEast, "EntryWest")
        );

        GameObject roomEast = Instantiate(roomPrefab);
        AlignPieces(
            FindEntry(corridorEast, "EntryEast"),
            FindEntry(roomEast, "EntryWest")
        );

        // -------------------------
        // NORTH
        // -------------------------

        GameObject corridorNorth = Instantiate(corridorNS);
        AlignPieces(
            FindEntry(centerRoom, "EntryNorth"),
            FindEntry(corridorNorth, "EntrySouth")
        );

        GameObject roomNorth = Instantiate(roomPrefab);
        AlignPieces(
            FindEntry(corridorNorth, "EntryNorth"),
            FindEntry(roomNorth, "EntrySouth")
        );

        // -------------------------
        // SOUTH
        // -------------------------

        GameObject corridorSouth = Instantiate(corridorNS);
        AlignPieces(
            FindEntry(centerRoom, "EntrySouth"),
            FindEntry(corridorSouth, "EntryNorth")
        );

        GameObject roomSouth = Instantiate(roomPrefab);
        AlignPieces(
            FindEntry(corridorSouth, "EntrySouth"),
            FindEntry(roomSouth, "EntryNorth")
        );
    }

    // ----------------------------------
    // ENCUENTRA ENTRY
    // ----------------------------------

    Transform FindEntry(GameObject obj, string entryName)
    {
        Transform entries = obj.transform.Find("Entries");
        return entries.Find(entryName);
    }

    // ----------------------------------
    // ALINEA PIEZAS SIN ROTAR
    // ----------------------------------

    void AlignPieces(Transform entryA, Transform entryB)
    {
        Transform pieceRoot = entryB.root;

        Vector3 localOffset = entryB.position - pieceRoot.position;

        pieceRoot.position = entryA.position - localOffset;
    }
}