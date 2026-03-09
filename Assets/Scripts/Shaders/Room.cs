using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Room : MonoBehaviour
{
    [Header("Walls To Hide")]
    [SerializeField] private GameObject southWallVisual;
    [SerializeField] private GameObject westWallVisual;

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

        HideWalls();

        Debug.Log("Room Activated: " + name);
    }

    public void HideWalls()
    {
        if (southWallVisual != null)
            southWallVisual.SetActive(false);

        if (westWallVisual != null)
            westWallVisual.SetActive(false);
    }

    public void ShowWalls()
    {
        if (southWallVisual != null)
            southWallVisual.SetActive(true);

        if (westWallVisual != null)
            westWallVisual.SetActive(true);
    }
}