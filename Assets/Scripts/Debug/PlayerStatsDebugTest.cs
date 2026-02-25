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
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            stats.AddPercentDamage(0.5f);
            Debug.Log("Added +50% damage");
            Debug.Log("Current Final Damage: " + stats.FinalDamage);
        }

        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            stats.AddFlatDamage(2f);
            Debug.Log("Added +2 flat damage");
            Debug.Log("Current Final Damage: " + stats.FinalDamage);
        }

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