using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Skill Database")]
public class SkillDatabase : ScriptableObject
{
    [SerializeField]
    private List<SkillDefinition> allSkills = new();

    public IReadOnlyList<SkillDefinition> AllSkills => allSkills;

    public int Count => allSkills.Count;

    public SkillDefinition GetSkill(int index)
    {
        if (index < 0 || index >= allSkills.Count)
            return null;

        return allSkills[index];
    }

    private void OnValidate()
    {
        allSkills.RemoveAll(skill => skill == null);
    }
}