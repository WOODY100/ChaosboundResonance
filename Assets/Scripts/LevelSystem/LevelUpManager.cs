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
        foreach (var effect in option.Effects)
        {
            ApplyEffect(effect, option.SkillDefinition);
        }

        ExitLevelUpState();
    }

    private void ApplyEffect(UpgradeEffect effect, SkillDefinition newSkill)
    {
        switch (effect.EffectType)
        {
            case UpgradeEffectType.AddNewSkill:

                if (loadout.HasFreeSlot())
                {
                    loadout.AssignSkill(newSkill);
                }
                else
                {
                    pendingReplaceSkill = newSkill;
                    OnReplaceRequested?.Invoke(pendingReplaceSkill);
                }

                break;

            case UpgradeEffectType.SkillModifier:

                RuntimeSkill skill =
                    loadout.GetSkill(effect.TargetSlotIndex);

                skill?.ApplyModifier(effect.SkillModifier);
                break;

            case UpgradeEffectType.SkillEvolution:

                RuntimeSkill evoSkill =
                    loadout.GetSkill(effect.TargetSlotIndex);

                evoSkill?.ApplyEvolution(effect.SkillEvolution);
                break;

            case UpgradeEffectType.GlobalModifier:

                ApplyGlobalModifier(effect);
                break;
        }
    }

    private void ApplyGlobalModifier(UpgradeEffect effect)
    {
        Debug.Log($"GLOBAL APPLY: {effect.TargetStat} {effect.ModifierType} {effect.Value}");

        ModifierSource source =
            new ModifierSource("RunUpgrade_" + System.Guid.NewGuid());

        source.Modifiers.Add(new StatModifier
        {
            StatType = effect.TargetStat,
            ModifierType = effect.ModifierType,
            Value = effect.Value
        });

        playerStats.GetComponent<PlayerModifierSystem>()
            .AddSource(ModifierLayer.Run, source);
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