using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStatsDebugTest : MonoBehaviour
{
    private PlayerStats stats;
    private PlayerExperienceSystem experience;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        experience = GetComponent<PlayerExperienceSystem>();
    }

    void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            experience.AddXP(10f);
        }

        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            GameStateManager.Instance.SetState(GameState.LevelUp);
        }
    }
}