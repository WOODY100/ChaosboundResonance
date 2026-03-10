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
}