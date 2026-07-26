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
    public bool IsPlayerBound =>
    experience != null &&
    loadout != null &&
    playerStats != null &&
    modifierSystem != null;

    private List<UpgradeOption> currentOptions;
    private SkillDefinition pendingReplaceSkill;
    private PlayerModifierSystem modifierSystem;

    private void OnEnable()
    {
        if (experience != null)
            experience.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        if (experience != null)
            experience.OnLevelUp -= HandleLevelUp;
    }

    public void Initialize(PlayerExperienceSystem exp,
                           PlayerSkillLoadout skillLoadout,
                           PlayerStats stats)
    {
        BindPlayer(
            exp,
            skillLoadout,
            stats);
    }

    public void BindPlayer(PlayerExperienceSystem exp,
                           PlayerSkillLoadout skillLoadout,
                           PlayerStats stats)
    {
        modifierSystem = stats != null
            ? stats.GetComponent<PlayerModifierSystem>()
            : null;

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

        if (!IsPlayerBound)
        {
            Debug.LogError("LevelUpManager has no Player bound.");
            ExitLevelUpState();
            return;
        }

        if (upgradeGenerator == null)
        {
            Debug.LogError("LevelUpManager is missing UpgradeGenerator.");
            ExitLevelUpState();
            return;
        }

        currentOptions =
            upgradeGenerator.GenerateOptions(loadout);

        OnLevelUpOptionsGenerated?.Invoke(currentOptions);
    }

    public void SelectUpgrade(UpgradeOption option)
    {
        if (option == null)
            return;

        ApplyUpgrade(option);
    }

    private void ApplyUpgrade(UpgradeOption option)
    {
        if (option.Effects == null || option.Effects.Count == 0)
        {
            ExitLevelUpState();
            return;
        }

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
        ModifierSource source =
            new ModifierSource("RunUpgrade_" + System.Guid.NewGuid());

        source.Modifiers.Add(new StatModifier
        {
            StatType = effect.TargetStat,
            ModifierType = effect.ModifierType,
            Value = effect.Value
        });

        modifierSystem?.AddSource(
            ModifierLayer.Run,
            source);
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
        if (GameStateManager.Instance == null)
            return;

        GameStateManager.Instance.SetState(GameState.LevelUp);
    }

    private void ExitLevelUpState()
    {
        OnLevelUpFinished?.Invoke();

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.SetState(GameState.Playing);
    }

    private void OnValidate()
    {
        if (upgradeGenerator == null)
        {
            Debug.LogWarning(
                $"{name}: UpgradeGenerator is not assigned.",
                this);
        }
    }
}