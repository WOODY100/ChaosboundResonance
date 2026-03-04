using UnityEngine;
using System.Collections.Generic;

public class DungeonVisibilityManager : MonoBehaviour
{
    public static DungeonVisibilityManager Instance;

    private Room currentRoom;
    private Room previousRoom;

    void Awake()
    {
        Instance = this;
    }

    public void EnterRoom(Room room)
    {
        if (room == currentRoom)
            return;

        previousRoom = currentRoom;
        currentRoom = room;

        if (previousRoom != null)
            previousRoom.SetWallsHidden(true);

        currentRoom.SetWallsHidden(true);
    }
}