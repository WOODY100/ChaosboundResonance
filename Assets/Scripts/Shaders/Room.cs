using UnityEngine;

public class Room : MonoBehaviour
{
    private FadeWall[] walls;

    void Awake()
    {
        walls = GetComponentsInChildren<FadeWall>();
    }

    public void SetWallsHidden(bool hidden)
    {
        foreach (var wall in walls)
        {
            if (hidden)
                wall.Hide();
            else
                wall.Show();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        DungeonVisibilityManager.Instance.EnterRoom(this);
    }
}