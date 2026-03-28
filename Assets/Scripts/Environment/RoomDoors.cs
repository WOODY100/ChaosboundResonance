using UnityEngine;

public class RoomDoors : MonoBehaviour
{
    [SerializeField] private GameObject north;
    [SerializeField] private GameObject south;
    [SerializeField] private GameObject east;
    [SerializeField] private GameObject west;

    public void CloseDoorsExcept(RoomEntryDirection entry)
    {
        if (entry != RoomEntryDirection.North && north != null)
            north.SetActive(true);

        if (entry != RoomEntryDirection.South && south != null)
            south.SetActive(true);

        if (entry != RoomEntryDirection.East && east != null)
            east.SetActive(true);

        if (entry != RoomEntryDirection.West && west != null)
            west.SetActive(true);
    }

    public void OpenDoors()
    {
        if (north != null) north.SetActive(false);
        if (south != null) south.SetActive(false);
        if (east != null) east.SetActive(false);
        if (west != null) west.SetActive(false);
    }

    void SetDoor(GameObject door, bool closed)
    {
        if (door != null)
            door.SetActive(closed);
    }

    public void UpdateDoorsFromEntries()
    {
        var entries = GetComponentsInChildren<DungeonEntry>();

        bool northConnected = false;
        bool southConnected = false;
        bool eastConnected = false;
        bool westConnected = false;

        foreach (var entry in entries)
        {
            if (!entry.occupied) continue;

            switch (entry.direction)
            {
                case EntryDirection.North: northConnected = true; break;
                case EntryDirection.South: southConnected = true; break;
                case EntryDirection.East: eastConnected = true; break;
                case EntryDirection.West: westConnected = true; break;
            }
        }

        SetDoor(north, !northConnected);
        SetDoor(south, !southConnected);
        SetDoor(east, !eastConnected);
        SetDoor(west, !westConnected);
    }
}