using Chaosbound.Core.GameFlow;
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class SkillEvolutionManager
{
    private readonly PlayerSkillLoadout loadout;
    private readonly GameFlow gameFlow;

    private readonly RuntimeSkill[] observedSkills =
        new RuntimeSkill[PlayerSkillLoadout.MaxSlots];

    private RuntimeSkill currentEvolutionSkill;
    private int currentEvolutionSlotIndex = -1;

    private bool isEvolutionOpen;
    private bool isInitialized;

    private readonly HashSet<RuntimeSkill> autoOpenedEvolutionSkills =
    new HashSet<RuntimeSkill>();

    public RuntimeSkill CurrentEvolutionSkill =>
        currentEvolutionSkill;

    public int CurrentEvolutionSlotIndex =>
        currentEvolutionSlotIndex;

    public bool IsEvolutionOpen =>
        isEvolutionOpen;

    public event Action<RuntimeSkill, int>
        OnEvolutionAvailable;

    public event Action<SkillEvolutionPresentationData>
        OnEvolutionOpened;

    public event Action<RuntimeSkill, int>
        OnEvolutionClosed;

    public event Action<
        RuntimeSkill,
        int,
        SkillEvolutionDefinition>
        OnEvolutionApplied;

    public SkillEvolutionManager(
        PlayerSkillLoadout loadout,
        GameFlow gameFlow)
    {
        this.loadout =
            loadout
            ?? throw new ArgumentNullException(
                nameof(loadout));

        this.gameFlow =
            gameFlow
            ?? throw new ArgumentNullException(
                nameof(gameFlow));
    }

    //==========================================================
    // Initialization
    //==========================================================

    public void Initialize()
    {
        if (isInitialized)
            return;

        isInitialized = true;

        loadout.OnLoadoutChanged +=
            HandleLoadoutChanged;

        gameFlow.OnContextChanged +=
            HandleGameFlowContextChanged;

        SubscribeToCurrentSkills();

        ProcessPendingEvolutions();
    }

    public void Cleanup()
    {
        if (!isInitialized)
            return;

        loadout.OnLoadoutChanged -=
            HandleLoadoutChanged;

        gameFlow.OnContextChanged -=
            HandleGameFlowContextChanged;

        UnsubscribeFromAllSkills();

        currentEvolutionSkill = null;
        currentEvolutionSlotIndex = -1;
        isEvolutionOpen = false;

        autoOpenedEvolutionSkills.Clear();

        isInitialized = false;
    }

    //==========================================================
    // Loadout Observation
    //==========================================================

    private void HandleLoadoutChanged()
    {
        SubscribeToCurrentSkills();

        ProcessPendingEvolutions();
    }

    private void SubscribeToCurrentSkills()
    {
        RuntimeSkill[] currentSkills =
            loadout.GetAllSkills();

        for (int i = 0;
             i < PlayerSkillLoadout.MaxSlots;
             i++)
        {
            RuntimeSkill currentSkill =
                currentSkills[i];

            if (ReferenceEquals(
                observedSkills[i],
                currentSkill))
            {
                continue;
            }

            if (observedSkills[i] != null)
            {
                observedSkills[i].OnLevelChanged -=
                    HandleSkillLevelChanged;
            }

            observedSkills[i] =
                currentSkill;

            if (currentSkill != null)
            {
                currentSkill.OnLevelChanged +=
                    HandleSkillLevelChanged;
            }
        }
    }

    private void UnsubscribeFromAllSkills()
    {
        for (int i = 0;
             i < observedSkills.Length;
             i++)
        {
            if (observedSkills[i] == null)
                continue;

            observedSkills[i].OnLevelChanged -=
                HandleSkillLevelChanged;

            observedSkills[i] = null;
        }
    }

    //==========================================================
    // Skill Level Observation
    //==========================================================

    private void HandleSkillLevelChanged(
        RuntimeSkill skill)
    {
        if (skill == null)
            return;

        if (!skill.CanEvolve)
            return;

        if (!skill.IsEvolutionPending)
        {
            skill.MarkEvolutionPending();
        }

        ProcessPendingEvolutions();
    }

    //==========================================================
    // GameFlow Observation
    //==========================================================

    private void HandleGameFlowContextChanged(
        GameFlowContext previous,
        GameFlowContext current)
    {
        if (current != GameFlowContext.Playing)
            return;

        ProcessPendingEvolutions();
    }

    //==========================================================
    // Pending Evolutions
    //==========================================================

    private void ProcessPendingEvolutions()
    {
        if (!isInitialized)
            return;

        if (isEvolutionOpen)
            return;

        RuntimeSkill[] skills =
            loadout.GetAllSkills();

        for (int i = 0;
             i < PlayerSkillLoadout.MaxSlots;
             i++)
        {
            RuntimeSkill skill =
                skills[i];

            if (skill == null)
                continue;

            if (!skill.IsEvolutionPending)
                continue;

            OnEvolutionAvailable?.Invoke(
                skill,
                i);

            if (gameFlow.CurrentContext !=
                GameFlowContext.Playing)
            {
                continue;
            }

            if (autoOpenedEvolutionSkills.Contains(skill))
                continue;

            autoOpenedEvolutionSkills.Add(skill);

            RequestOpenEvolution(i);

            return;
        }
    }

    //==========================================================
    // Request Open
    //==========================================================

    public bool RequestOpenEvolution(
        int slotIndex)
    {
        if (!isInitialized)
            return false;

        if (isEvolutionOpen)
            return false;

        if (gameFlow.CurrentContext !=
            GameFlowContext.Playing)
        {
            return false;
        }

        if (slotIndex < 0 ||
            slotIndex >= PlayerSkillLoadout.MaxSlots)
        {
            return false;
        }

        RuntimeSkill skill =
            loadout.GetSkill(slotIndex);

        if (skill == null)
            return false;

        if (!skill.IsEvolutionPending)
            return false;

        if (!skill.CanEvolve)
            return false;

        currentEvolutionSkill =
            skill;

        currentEvolutionSlotIndex =
            slotIndex;

        if (!gameFlow.Request(
                GameFlowContext.Evolution))
        {
            currentEvolutionSkill = null;
            currentEvolutionSlotIndex = -1;

            return false;
        }

        isEvolutionOpen = true;

        SkillEvolutionPresentationData presentationData =
            BuildPresentationData();

        OnEvolutionOpened?.Invoke(
            presentationData);

        return true;
    }

    //==========================================================
    // Cancel / "Ahora no"
    //==========================================================

    public bool CancelEvolution()
    {
        if (!isEvolutionOpen)
            return false;

        RuntimeSkill skill =
            currentEvolutionSkill;

        int slotIndex =
            currentEvolutionSlotIndex;

        if (!gameFlow.Pop(
                GameFlowContext.Evolution))
        {
            return false;
        }

        currentEvolutionSkill = null;
        currentEvolutionSlotIndex = -1;
        isEvolutionOpen = false;

        OnEvolutionClosed?.Invoke(
            skill,
            slotIndex);

        return true;
    }

    //==========================================================
    // Apply Evolution
    //==========================================================

    public bool ApplyEvolution(
        SkillEvolutionDefinition evolution)
    {
        if (!isEvolutionOpen)
            return false;

        if (evolution == null)
            return false;

        if (evolution.ResultingSkill == null)
            return false;

        RuntimeSkill sourceSkill =
            currentEvolutionSkill;

        int slotIndex =
            currentEvolutionSlotIndex;

        if (sourceSkill == null)
            return false;

        if (!sourceSkill.IsEvolutionPending)
            return false;

        if (!sourceSkill.CanEvolve)
            return false;

        if (!sourceSkill.Definition.Evolutions.Contains(
                evolution))
        {
            return false;
        }

        RuntimeSkill evolvedSkill =
            sourceSkill.CreateEvolvedSkill(
                evolution.ResultingSkill);

        if (!loadout.ReplaceSkill(
                slotIndex,
                evolvedSkill))
        {
            return false;
        }

        sourceSkill.ClearEvolutionPending();

        if (!gameFlow.Pop(
                GameFlowContext.Evolution))
        {
            return false;
        }

        isEvolutionOpen = false;

        currentEvolutionSkill = null;
        currentEvolutionSlotIndex = -1;

        OnEvolutionApplied?.Invoke(
            sourceSkill,
            slotIndex,
            evolution);

        ProcessPendingEvolutions();

        return true;
    }

    //==========================================================
    // Presentation Data
    //==========================================================

    private SkillEvolutionPresentationData BuildPresentationData()
    {
        RuntimeSkill skill =
            currentEvolutionSkill;

        List<SkillEvolutionChoice> choices =
            new List<SkillEvolutionChoice>();

        IReadOnlyList<SkillEvolutionDefinition> evolutions =
            skill.Definition.Evolutions;

        for (int i = 0;
             i < evolutions.Count;
             i++)
        {
            SkillEvolutionDefinition evolution =
                evolutions[i];

            if (evolution == null)
                continue;

            if (evolution.ResultingSkill == null)
                continue;

            SkillEvolutionTransferPreview preview =
                skill.BuildEvolutionTransferPreview(
                    evolution.ResultingSkill);

            choices.Add(
                new SkillEvolutionChoice(
                    evolution,
                    preview));
        }

        return new SkillEvolutionPresentationData(
            skill,
            currentEvolutionSlotIndex,
            choices);
    }
}