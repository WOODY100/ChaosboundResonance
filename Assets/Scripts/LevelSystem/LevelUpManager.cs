using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerExperienceSystem experience;
    [SerializeField] private PlayerSkillLoadout loadout;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private UpgradeGenerator upgradeGenerator;

    public event System.Action<List<UpgradeOption>> OnLevelUpOptionsGenerated;
    public event System.Action<SkillDefinition> OnReplaceRequested;
    public event System.Action OnReplaceFinished;
    public event System.Action OnReplaceCancelled;
    public event System.Action OnLevelUpFinished;

    private List<UpgradeOption> currentOptions;
    private SkillDefinition pendingReplaceSkill;

    public void BindPlayer(PlayerExperienceSystem exp,
                           PlayerSkillLoadout skillLoadout,
                           PlayerStats stats)
    {
        if (experience != null)
            experience.OnLevelUp -= HandleLevelUp;

        experience = exp;
        loadout = skillLoadout;
        playerStats = stats;

        if (experience != null)
            experience.OnLevelUp += HandleLevelUp;
    }

    private void HandleLevelUp(int newLevel)
    {
        EnterLevelUpState();

        currentOptions = upgradeGenerator.GenerateOptions(loadout);

        OnLevelUpOptionsGenerated?.Invoke(currentOptions);
    }

    public void SelectUpgrade(UpgradeOption option)
    {
        ApplyUpgrade(option);
    }

    private void ApplyUpgrade(UpgradeOption option)
    {
        switch (option.Type)
        {
            case UpgradeType.NewSkill:

                if (loadout.HasFreeSlot())
                {
                    loadout.AssignSkill(option.SkillDefinition);
                    ExitLevelUpState();
                }
                else
                {
                    pendingReplaceSkill = option.SkillDefinition;
                    OnReplaceRequested?.Invoke(pendingReplaceSkill);
                }

                break;

            case UpgradeType.SkillModifier:

                RuntimeSkill skill =
                    loadout.GetSkill(option.TargetSlotIndex);

                skill?.ApplyModifier(option.ModifierDefinition);

                ExitLevelUpState();
                break;

            case UpgradeType.GlobalBuff:

                playerStats.AddPercentDamage(option.Value);

                ExitLevelUpState();
                break;

            case UpgradeType.Passive:
                ExitLevelUpState();
                break;
        }
    }

    public void ReplaceSkillAt(int index)
    {
        if (pendingReplaceSkill == null)
            return;

        loadout.ReplaceSkill(index, pendingReplaceSkill);

        pendingReplaceSkill = null;

        OnReplaceFinished?.Invoke();

        ExitLevelUpState();
    }

    public void CancelReplaceMode()
    {
        pendingReplaceSkill = null;
        OnReplaceCancelled?.Invoke();
    }

    private void EnterLevelUpState()
    {
        GameStateManager.Instance.SetState(GameState.LevelUp);
    }

    private void ExitLevelUpState()
    {
        OnLevelUpFinished?.Invoke();
        GameStateManager.Instance.SetState(GameState.Playing);
    }
}