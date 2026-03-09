using UnityEngine;

public class DungeonVisibilityManager : MonoBehaviour
{
    public static DungeonVisibilityManager Instance;

    private Room currentRoom;

    void Awake()
    {
        Instance = this;
    }

    public void EnterRoom(Room room)
    {
        if (room == currentRoom)
            return;

        if (currentRoom != null)
        {
            currentRoom.ShowWalls();
        }

        currentRoom = room;

        currentRoom.HideWalls();

        Debug.Log("Entered room: " + room.name);
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }
}