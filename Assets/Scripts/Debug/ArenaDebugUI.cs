using UnityEngine;

public class ArenaDebugUI : MonoBehaviour
{
    [SerializeField] private ArenaSpawnDirector director;

    void OnGUI()
    {
        if (director == null)
            return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(10, 40, 300, 30),
            "Active Enemies: " + director.ActiveEnemies,
            style);
    }
}
