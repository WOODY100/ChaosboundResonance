using Chaosbound.Content.Expeditions.Runtime.Configs;
using UnityEngine;
using System;

public class PlayerSkillLoadout : MonoBehaviour
{
    public const int MaxSlots = 4;

    private RuntimeSkill[] skillSlots = new RuntimeSkill[MaxSlots];

    private RuntimeSkillProgressionConfig skillProgressionConfig;

    private bool isInitialized;

    public event Action OnLoadoutChanged;

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

        skillSlots[index] =
            new RuntimeSkill(
                definition,
                skillProgressionConfig);

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

        skillSlots[slotIndex] =
            new RuntimeSkill(
                newDefinition,
                skillProgressionConfig);

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