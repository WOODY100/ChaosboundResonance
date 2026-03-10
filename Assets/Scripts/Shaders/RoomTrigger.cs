using UnityEngine;

public enum RoomEntryDirection
{
    North,
    South,
    East,
    West
}

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private RoomEntryDirection entryDirection;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || triggered)
            return;

        triggered = true;

        RoomDoors doors = GetComponentInParent<RoomDoors>();

        if (doors != null)
        {
            doors.CloseDoorsExcept(entryDirection);
            Debug.Log("Closing doors");
        }

        ArenaSpawnDirector director = Object.FindAnyObjectByType<ArenaSpawnDirector>();

        if (director != null)
        {
            director.SetRoomDoors(doors);
            director.ActivateSpawning();
        }
    }
}