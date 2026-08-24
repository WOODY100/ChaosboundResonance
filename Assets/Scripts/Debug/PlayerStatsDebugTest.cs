using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStatsDebugTest : MonoBehaviour
{    private PlayerExperienceSystem experience;

    void Awake()
    {
        experience = GetComponent<PlayerExperienceSystem>();
    }

    void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            experience.AddXP(50f);
        }

        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            GameStateManager.Instance.SetState(GameState.LevelUp);
        }
    }
}