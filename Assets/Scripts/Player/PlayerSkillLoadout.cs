using Chaosbound.Content.Expeditions.Runtime.Configs;
using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerSkillLoadout : MonoBehaviour
{
    public const int MaxSlots = 4;

    private RuntimeSkill[] skillSlots = new RuntimeSkill[MaxSlots];

    private RuntimeSkillProgressionConfig skillProgressionConfig;

    private bool isInitialized;

    private readonly List<ModifierEntry> globalModifiers =
        new List<ModifierEntry>();

    public IReadOnlyList<ModifierEntry> GlobalModifiers =>
        globalModifiers;

    public event Action OnLoadoutChanged;
    public event Action OnGlobalModifiersChanged;

    // ===============================
    // INICIALIZACIÓN
    // ===============================

    public void Initialize(
        RuntimeSkillProgressionConfig progressionConfig)
    {
        if (progressionConfig == null)
            throw new ArgumentNullException(
                nameof(progressionConfig));

        if (isInitialized)
            throw new InvalidOperationException(
                "PlayerSkillLoadout has already been initialized.");

        skillProgressionConfig = progressionConfig;
        isInitialized = true;
    }

    // ===============================
    // GLOBAL META MODIFIERS
    // ===============================

    /// <summary>
    /// Replaces the global modifier set and reapplies it to every owned skill.
    /// Can be called before Initialize; newly assigned skills receive the set.
    /// </summary>
    public void SetGlobalModifiers(IReadOnlyList<ModifierEntry> modifiers)
    {
        globalModifiers.Clear();

        if (modifiers != null)
        {
            for (int i = 0; i < modifiers.Count; i++)
                globalModifiers.Add(modifiers[i]);
        }

        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (skillSlots[i] != null)
                skillSlots[i].SetGlobalModifiers(globalModifiers);
        }

        OnGlobalModifiersChanged?.Invoke();
    }

    private void ApplyGlobalModifiers(RuntimeSkill skill)
    {
        if (skill != null)
            skill.SetGlobalModifiers(globalModifiers);
    }

    // ===============================
    // CONSULTAS
    // ===============================

    public bool HasFreeSlot()
    {
        for (int i = 0; i < MaxSlots; i++)
        {
            if (skillSlots[i] == null)
                return true;
        }

        return false;
    }

    public int GetFirstFreeSlotIndex()
    {
        for (int i = 0; i < MaxSlots; i++)
        {
            if (skillSlots[i] == null)
                return i;
        }

        return -1;
    }

    public RuntimeSkill GetSkill(int index)
    {
        if (index < 0 || index >= MaxSlots)
            return null;

        return skillSlots[index];
    }

    public RuntimeSkill[] GetAllSkills()
    {
        return skillSlots;
    }

    // ===============================
    // ASIGNAR NUEVA HABILIDAD
    // ===============================

    public bool AssignSkill(SkillDefinition definition)
    {
        if (!isInitialized)
            throw new InvalidOperationException(
                "PlayerSkillLoadout must be initialized before assigning skills.");

        int index = GetFirstFreeSlotIndex();

        if (index == -1)
            return false;

        RuntimeSkill runtimeSkill =
            new RuntimeSkill(
                definition,
                skillProgressionConfig);

        ApplyGlobalModifiers(runtimeSkill);
        skillSlots[index] = runtimeSkill;

        OnLoadoutChanged?.Invoke();
        return true;
    }

    // ===============================
    // REEMPLAZAR HABILIDAD
    // ===============================

    public void ReplaceSkill(int slotIndex, SkillDefinition newDefinition)
    {
        if (!isInitialized)
            throw new InvalidOperationException(
                "PlayerSkillLoadout must be initialized before replacing skills.");

        if (slotIndex < 0 || slotIndex >= MaxSlots)
            return;

        RuntimeSkill runtimeSkill =
            new RuntimeSkill(
                newDefinition,
                skillProgressionConfig);

        ApplyGlobalModifiers(runtimeSkill);
        skillSlots[slotIndex] = runtimeSkill;

        OnLoadoutChanged?.Invoke();
    }

    public bool ReplaceSkill(
    int slotIndex,
    RuntimeSkill runtimeSkill)
    {
        if (!isInitialized)
            return false;

        if (runtimeSkill == null)
            return false;

        if (slotIndex < 0 || slotIndex >= MaxSlots)
            return false;

        if (skillSlots[slotIndex] == null)
            return false;

        // Evolution creates a new RuntimeSkill instance. Reapply the
        // current global set so evolved skills cannot lose meta effects.
        ApplyGlobalModifiers(runtimeSkill);
        skillSlots[slotIndex] = runtimeSkill;

        OnLoadoutChanged?.Invoke();

        return true;
    }

    // ===============================
    // REMOVER (por si en el futuro)
    // ===============================

    public void RemoveSkill(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MaxSlots)
            return;

        skillSlots[slotIndex] = null;

        OnLoadoutChanged?.Invoke();
    }

    // ===============================
    // UTILIDADES
    // ===============================

    public int GetOccupiedSlotCount()
    {
        int count = 0;

        for (int i = 0; i < MaxSlots; i++)
        {
            if (skillSlots[i] != null)
                count++;
        }

        return count;
    }

    public void ClearAllSkills()
    {
        bool changed = false;

        for (int i = 0; i < MaxSlots; i++)
        {
            if (skillSlots[i] != null)
            {
                skillSlots[i] = null;
                changed = true;
            }
        }

        if (changed)
        {
            OnLoadoutChanged?.Invoke();
        }
    }
}