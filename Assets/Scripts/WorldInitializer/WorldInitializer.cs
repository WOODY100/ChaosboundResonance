using Chaosbound.Core.Composition;
using UnityEngine;

public class WorldInitializer : MonoBehaviour
{
    private void Start()
    {
        BootstrapContext bootstrap = BootstrapContext.Current;
        ExpeditionSceneContext scene = ExpeditionSceneContext.Current;

        PlayerHealth player = scene.Player;
        PlayerSkillLoadout loadout = scene.PlayerSkillLoadout;
        PlayerStats stats = scene.PlayerStats;
        PlayerExperienceSystem xpSystem = scene.PlayerExperienceSystem;

        SkillBarUI skillBar = bootstrap.SkillBarUI;

        RunManager runManager = bootstrap.RunManager;
        LevelUpManager levelUpManager = bootstrap.LevelUpManager;
        EnemyManager enemyManager = bootstrap.EnemyManager;

        if (runManager != null && player != null)
            runManager.BindPlayer(player);

        if (enemyManager != null && player != null)
            enemyManager.SetPlayer(player.transform);

        if (levelUpManager != null &&
            xpSystem != null &&
            loadout != null &&
            stats != null)
        {
            levelUpManager.Initialize(
            xpSystem,
            loadout,
            stats);
        }

        if (skillBar != null &&
            loadout != null &&
            levelUpManager != null)
        {
            skillBar.Initialize(
            loadout,
            levelUpManager);
        }

        EnemyBrain.ResetAttackSlots();
    }
}