using UnityEngine;

public class SkillBarUI : MonoBehaviour
{
    [SerializeField] private SkillSlotUI[] slots;

    private PlayerSkillLoadout loadout;
    private LevelUpManager levelUpManager;

    public void Bind(PlayerSkillLoadout playerLoadout, LevelUpManager lvlManager)
    {
        loadout = playerLoadout;
        levelUpManager = lvlManager;

        if (loadout != null)
            loadout.OnLoadoutChanged += RefreshAll;

        if (levelUpManager != null)
        {
            levelUpManager.OnReplaceRequested += EnterReplaceMode;
            levelUpManager.OnReplaceFinished += ExitReplaceMode;
            levelUpManager.OnReplaceCancelled += ExitReplaceMode;
        }

        RefreshAll();
    }

    private void OnDestroy()
    {
        if (loadout != null)
            loadout.OnLoadoutChanged -= RefreshAll;

        if (levelUpManager != null)
        {
            levelUpManager.OnReplaceRequested -= EnterReplaceMode;
            levelUpManager.OnReplaceFinished -= ExitReplaceMode;
            levelUpManager.OnReplaceCancelled -= ExitReplaceMode;
        }
    }

    public void RefreshAll()
    {
        if (loadout == null) return; // protección extra

        var skills = loadout.GetAllSkills();

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetIndex(i);
            slots[i].SetSkill(skills[i]);
        }
    }

    private void EnterReplaceMode(SkillDefinition newSkill)
    {
        foreach (var slot in slots)
            slot.EnableReplaceMode();
    }

    public void ExitReplaceMode()
    {
        foreach (var slot in slots)
            slot.DisableReplaceMode();
    }
}