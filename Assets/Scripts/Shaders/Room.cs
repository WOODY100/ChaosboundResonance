using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Room : MonoBehaviour
{
    private bool activated = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        DungeonVisibilityManager.Instance.EnterRoom(this);

        if (!activated)
        {
            ActivateRoom();
        }
    }

    void ActivateRoom()
    {
        activated = true;

        Debug.Log("Room Activated: " + name);
    }
}