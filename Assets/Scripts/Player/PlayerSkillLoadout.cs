using UnityEngine;
using System;

public class PlayerSkillLoadout : MonoBehaviour
{
    public const int MaxSlots = 4;

    private RuntimeSkill[] skillSlots = new RuntimeSkill[MaxSlots];

    public event Action OnLoadoutChanged;

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
        int index = GetFirstFreeSlotIndex();

        if (index == -1)
            return false;

        skillSlots[index] = new RuntimeSkill(definition);

        OnLoadoutChanged?.Invoke();
        return true;
    }

    // ===============================
    // REEMPLAZAR HABILIDAD
    // ===============================

    public void ReplaceSkill(int slotIndex, SkillDefinition newDefinition)
    {
        if (slotIndex < 0 || slotIndex >= MaxSlots)
            return;

        skillSlots[slotIndex] = new RuntimeSkill(newDefinition);

        OnLoadoutChanged?.Invoke();
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
}