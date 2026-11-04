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

#if UNITY_EDITOR
    private string currentAnalyticsOptionId;
#endif

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

#if UNITY_EDITOR
        currentAnalyticsOptionId =
            Chaosbound.Development.Analytics.ExpeditionTelemetryCollector
                .RegisterLevelUpOptionsGenerated(
                    currentOptions,
                    experience != null ? experience.CurrentLevel : 1);
#endif

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

#if UNITY_EDITOR
        Chaosbound.Development.Analytics.ExpeditionTelemetryCollector
            .RegisterLevelUpOptionSelected(
                option,
                experience != null ? experience.CurrentLevel : 1);
        currentAnalyticsOptionId = option.AnalyticsOptionId;
#endif

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
                    int assignedSlot = loadout.GetFirstFreeSlotIndex();
                    bool assigned = loadout.AssignSkill(newSkill);

#if UNITY_EDITOR
                    Chaosbound.Development.Analytics.ExpeditionTelemetryCollector
                        .RegisterLevelUpEffect(
                            currentAnalyticsOptionId,
                            "AddNewSkill",
                            newSkill != null ? newSkill.name : string.Empty,
                            string.Empty,
                            assignedSlot,
                            string.Empty,
                            string.Empty,
                            0f,
                            0f,
                            0f,
                            assigned,
                            assigned ? "Applied" : "Failed");
#endif
                }
                else
                {
                    pendingReplaceSkill = newSkill;

#if UNITY_EDITOR
                    Chaosbound.Development.Analytics.ExpeditionTelemetryCollector
                        .RegisterLevelUpEffect(
                            currentAnalyticsOptionId,
                            "AddNewSkill",
                            newSkill != null ? newSkill.name : string.Empty,
                            string.Empty,
                            -1,
                            string.Empty,
                            string.Empty,
                            0f,
                            0f,
                            0f,
                            false,
                            "AwaitingReplacement");
#endif

                    phase = LevelUpPhase.SelectingReplacement;
                    OnReplaceRequested?.Invoke(pendingReplaceSkill);
                }

                break;

            case UpgradeEffectType.SkillModifier:

                RuntimeSkill skill =
                    loadout.GetSkill(effect.TargetSlotIndex);

                int previousSkillLevel = skill != null ? skill.Level : 0;
                bool modifierApplied =
                    skill != null && skill.ApplyModifier(effect.SkillModifier);

#if UNITY_EDITOR
                Chaosbound.Development.Analytics.ExpeditionTelemetryCollector
                    .RegisterLevelUpEffect(
                        currentAnalyticsOptionId,
                        "SkillModifier",
                        effect.SkillModifier != null ? effect.SkillModifier.name : string.Empty,
                        skill != null && skill.Definition != null ? skill.Definition.name : string.Empty,
                        effect.TargetSlotIndex,
                        string.Empty,
                        string.Empty,
                        0f,
                        previousSkillLevel,
                        skill != null ? skill.Level : 0,
                        modifierApplied,
                        modifierApplied ? "Applied" : "Failed");
#endif

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
        float previousValue = modifierSystem != null
            ? modifierSystem.GetStat(effect.TargetStat)
            : 0f;

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

#if UNITY_EDITOR
        float newValue = modifierSystem != null
            ? modifierSystem.GetStat(effect.TargetStat)
            : previousValue;

        Chaosbound.Development.Analytics.ExpeditionTelemetryCollector
            .RegisterLevelUpEffect(
                currentAnalyticsOptionId,
                "GlobalModifier",
                effect.GlobalDefinition != null
                    ? effect.GlobalDefinition.name : "Fallback",
                string.Empty,
                -1,
                effect.TargetStat.ToString(),
                effect.ModifierType.ToString(),
                effect.Value,
                previousValue,
                newValue,
                modifierSystem != null,
                modifierSystem != null ? "Applied" : "Failed");
#endif
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

        SkillDefinition replacedSkill =
            loadout.GetSkill(index) != null
                ? loadout.GetSkill(index).Definition
                : null;

        SkillDefinition replacementSkill = pendingReplaceSkill;
        bool replacementValid =
            index >= 0 &&
            index < PlayerSkillLoadout.MaxSlots &&
            replacedSkill != null &&
            replacementSkill != null;

        loadout.ReplaceSkill(index, replacementSkill);

#if UNITY_EDITOR
        Chaosbound.Development.Analytics.ExpeditionTelemetryCollector
            .RegisterLevelUpEffect(
                currentAnalyticsOptionId,
                "SkillReplacement",
                replacementSkill != null ? replacementSkill.name : string.Empty,
                replacedSkill != null ? replacedSkill.name : string.Empty,
                index,
                string.Empty,
                string.Empty,
                0f,
                0f,
                0f,
                replacementValid,
                replacementValid ? "Applied" : "Failed");

        Chaosbound.Development.Analytics.ExpeditionTelemetryCollector
            .UpdateLevelUpSelectionStatus(
                currentAnalyticsOptionId,
                replacementValid ? "AppliedWithReplacement" : "ReplacementFailed");
#endif

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

#if UNITY_EDITOR
        Chaosbound.Development.Analytics.ExpeditionTelemetryCollector
            .UpdateLevelUpSelectionStatus(
                currentAnalyticsOptionId,
                "ReplacementCancelled");
#endif

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

#if UNITY_EDITOR
        Chaosbound.Development.Analytics.ExpeditionTelemetryCollector
            .CompleteLevelUpSelection(currentAnalyticsOptionId);
#endif

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