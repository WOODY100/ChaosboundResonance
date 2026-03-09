using UnityEngine;

public class WallOccluder : MonoBehaviour
{
    [SerializeField] private FadeWall wall;

    public void HideWall()
    {
        if (wall != null)
            wall.Hide();
    }

    public void ShowWall()
    {
        if (wall != null)
            wall.Show();
    }
}