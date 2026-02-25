using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;

    private ArenaSpawnDirector arena;

    void Start()
    {
        arena = FindFirstObjectByType<ArenaSpawnDirector>();
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width;
        int h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(10, 10, w, h * 5 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.white;

        float fps = 1.0f / deltaTime;

        string timeText = "00:00";
        float percent = (arena.CurrentTime / arena.Duration) * 100f;
        string enemiesText = "";

        if (arena != null)
        {
            float current = arena.CurrentTime;
            float duration = arena.Duration;

            int minutes = Mathf.FloorToInt(current / 60f);
            int seconds = Mathf.FloorToInt(current % 60f);

            timeText = string.Format("{0:00}:{1:00}", minutes, seconds);

            enemiesText = " | Active: " + arena.ActiveEnemies;
        }

        string text = $"FPS: {fps:0} | Time: {timeText}{enemiesText}";

        GUI.Label(rect, text, style);
    }
}