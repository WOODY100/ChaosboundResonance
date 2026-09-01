using System;
using System.Collections.Generic;
using UnityEngine;
using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;

public class LevelUpManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerExperienceSystem experience;
    [SerializeField] private PlayerSkillLoadout loadout;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private UpgradeGenerator upgradeGenerator;

    public event Action<List<UpgradeOption>> OnLevelUpOptionsGenerated;
    public event Action<SkillDefinition> OnReplaceRequested;
    public event Action OnReplaceFinished;
    public event Action OnReplaceCancelled;
    public event Action OnLevelUpFinished;

    public bool IsPlayerBound =>
        experience != null &&
        loadout != null &&
        playerStats != null &&
        modifierSystem != null;

    private enum LevelUpPhase
    {
        None,
        SelectingUpgrade,
        SelectingReplacement
    }

    private List<UpgradeOption> currentOptions;

    private SkillDefinition pendingReplaceSkill;

    private PlayerModifierSystem modifierSystem;

    private int pendingLevelUps;

    private LevelUpPhase phase =
        LevelUpPhase.None;

    private GameFlow gameFlow;

    //==========================================================
    // Unity
    //==========================================================

    private void Start()
    {
        BootstrapContext context =
            BootstrapContext.Current;

        if (context == null)
        {
            Debug.LogError(
                $"{nameof(LevelUpManager)} could not find " +
                $"{nameof(BootstrapContext)}.",
                this);

            return;
        }

        gameFlow =
            context.GameFlow;

        if (gameFlow == null)
        {
            Debug.LogError(
                $"{nameof(LevelUpManager)} could not find " +
                $"{nameof(GameFlow)}.",
                this);

            return;
        }

        gameFlow.OnContextChanged +=
            HandleContextChanged;

        TryPresentPendingLevelUp();
    }

    private void OnDestroy()
    {
        if (gameFlow != null)
        {
            gameFlow.OnContextChanged -=
                HandleContextChanged;
        }
    }

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

    //==========================================================
    // Initialization
    //==========================================================

    public void Initialize(
        PlayerExperienceSystem exp,
        PlayerSkillLoadout skillLoadout,
        PlayerStats stats)
    {
        BindPlayer(
            exp,
            skillLoadout,
            stats);
    }

    public void BindPlayer(
        PlayerExperienceSystem exp,
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

    //==========================================================
    // Level Up
    //==========================================================

    private void HandleLevelUp(int newLevel)
    {
        pendingLevelUps++;

        TryPresentPendingLevelUp();
    }

    private void TryPresentPendingLevelUp()
    {
        if (pendingLevelUps <= 0)
            return;

        if (gameFlow == null)
            return;

        if (phase != LevelUpPhase.None)
            return;

        if (gameFlow.CurrentContext ==
            GameFlowContext.LevelUp)
        {
            PresentNextLevelUp();

            return;
        }

        gameFlow.Request(
            GameFlowContext.LevelUp);
    }

    private void HandleContextChanged(
        GameFlowContext previous,
        GameFlowContext current)
    {
        if (current != GameFlowContext.LevelUp)
            return;

        PresentNextLevelUp();
    }

    private void PresentNextLevelUp()
    {
        if (pendingLevelUps <= 0)
            return;

        if (!IsPlayerBound)
        {
            Debug.LogError(
                "LevelUpManager has no Player bound.");

            return;
        }

        if (upgradeGenerator == null)
        {
            Debug.LogError(
                "LevelUpManager is missing UpgradeGenerator.");

            return;
        }

        phase =
            LevelUpPhase.SelectingUpgrade;

        currentOptions =
            upgradeGenerator.GenerateOptions(
                loadout);

        OnLevelUpOptionsGenerated?.Invoke(
            currentOptions);
    }

    //==========================================================
    // Upgrade Selection
    //==========================================================

    public void SelectUpgrade(
        UpgradeOption option)
    {
        if (option == null)
            return;

        if (phase !=
            LevelUpPhase.SelectingUpgrade)
        {
            return;
        }

        ApplyUpgrade(option);
    }

    private void ApplyUpgrade(
        UpgradeOption option)
    {
        if (option.Effects == null ||
            option.Effects.Count == 0)
        {
            CompleteCurrentLevelUp();

            return;
        }

        foreach (var effect in option.Effects)
        {
            ApplyEffect(
                effect,
                option.SkillDefinition);
        }

        if (phase ==
            LevelUpPhase.SelectingReplacement)
        {
            return;
        }

        CompleteCurrentLevelUp();
    }

    //==========================================================
    // Upgrade Effects
    //==========================================================

    private void ApplyEffect(
        UpgradeEffect effect,
        SkillDefinition newSkill)
    {
        switch (effect.EffectType)
        {
            case UpgradeEffectType.AddNewSkill:

                if (loadout.HasFreeSlot())
                {
                    loadout.AssignSkill(
                        newSkill);
                }
                else
                {
                    pendingReplaceSkill =
                        newSkill;

                    phase =
                        LevelUpPhase.SelectingReplacement;

                    OnReplaceRequested?.Invoke(
                        pendingReplaceSkill);
                }

                break;

            case UpgradeEffectType.SkillModifier:

                RuntimeSkill skill =
                    loadout.GetSkill(
                        effect.TargetSlotIndex);

                skill?.ApplyModifier(
                    effect.SkillModifier);

                break;

            case UpgradeEffectType.SkillEvolution:

                RuntimeSkill evoSkill =
                    loadout.GetSkill(
                        effect.TargetSlotIndex);

                evoSkill?.ApplyEvolution(
                    effect.SkillEvolution);

                break;

            case UpgradeEffectType.GlobalModifier:

                ApplyGlobalModifier(
                    effect);

                break;
        }
    }

    private void ApplyGlobalModifier(
        UpgradeEffect effect)
    {
        ModifierSource source =
            new ModifierSource(
                "RunUpgrade_" +
                Guid.NewGuid());

        source.Modifiers.Add(
            new StatModifier
            {
                StatType =
                    effect.TargetStat,

                ModifierType =
                    effect.ModifierType,

                Value =
                    effect.Value
            });

        modifierSystem?.AddSource(
            ModifierLayer.Run,
            source);
    }

    //==========================================================
    // Replacement
    //==========================================================

    public void ReplaceSkillAt(int index)
    {
        if (phase !=
            LevelUpPhase.SelectingReplacement)
        {
            return;
        }

        if (pendingReplaceSkill == null)
            return;

        loadout.ReplaceSkill(
            index,
            pendingReplaceSkill);

        pendingReplaceSkill = null;

        OnReplaceFinished?.Invoke();

        CompleteCurrentLevelUp();
    }

    public void CancelReplaceMode()
    {
        if (phase !=
            LevelUpPhase.SelectingReplacement)
        {
            return;
        }

        pendingReplaceSkill = null;

        phase =
            LevelUpPhase.SelectingUpgrade;

        OnReplaceCancelled?.Invoke();
    }

    //==========================================================
    // Level Up Completion
    //==========================================================

    private void CompleteCurrentLevelUp()
    {
        if (pendingLevelUps <= 0)
            return;

        pendingLevelUps--;

        phase =
            LevelUpPhase.None;

        currentOptions = null;

        if (pendingLevelUps > 0)
        {
            PresentNextLevelUp();

            return;
        }

        FinishLevelUp();
    }

    private void FinishLevelUp()
    {
        OnLevelUpFinished?.Invoke();

        if (gameFlow == null)
            return;

        if (gameFlow.CurrentContext ==
            GameFlowContext.LevelUp)
        {
            gameFlow.Pop(
                GameFlowContext.LevelUp);
        }
    }

    //==========================================================
    // Validation
    //==========================================================

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