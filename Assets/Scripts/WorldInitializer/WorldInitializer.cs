using UnityEngine;

public class WorldInitializer : MonoBehaviour
{
    private HUDController hud;

    void Start()
    {
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        ArenaSpawnDirector arena = FindFirstObjectByType<ArenaSpawnDirector>();
        HUDController hud = FindFirstObjectByType<HUDController>();
        RunManager runManager = FindFirstObjectByType<RunManager>();
        PlayerSkillLoadout loadout = FindFirstObjectByType<PlayerSkillLoadout>();
        PlayerStats stats = FindFirstObjectByType<PlayerStats>();
        LevelUpManager levelUpManager = FindFirstObjectByType<LevelUpManager>();
        PlayerExperienceSystem xpSystem = player.GetComponent<PlayerExperienceSystem>();
        HUDXPBarUI xpUI = FindFirstObjectByType<HUDXPBarUI>();
        HUDLevelUI levelUI = FindFirstObjectByType<HUDLevelUI>();
        SkillBarUI skillBar = FindFirstObjectByType<SkillBarUI>();

        if (hud != null)
            hud.gameObject.SetActive(true);

        if (player != null && hud != null)
            hud.BindPlayer(player);

        if (arena != null && hud != null)
            hud.BindArena(arena);

        if (runManager != null && player != null)
            runManager.BindPlayer(player);
        
        if (xpUI != null)
            xpUI.Bind(xpSystem);

        if (levelUI != null)
            levelUI.Bind(xpSystem);


        if (EnemyManager.Instance != null && player != null)
            EnemyManager.Instance.SetPlayer(player.transform);

        if (levelUpManager != null &&
            xpSystem != null &&
            loadout != null &&
            stats != null)
            {
                levelUpManager.BindPlayer(xpSystem, loadout, stats);
        }

        if (skillBar != null && loadout != null && levelUpManager != null)
        {
            skillBar.Bind(loadout, levelUpManager);
        }

        EnemyBrain.ResetAttackSlots();
    }

    void OnDestroy()
    {
        if (hud != null)
            hud.gameObject.SetActive(false);
    }
}