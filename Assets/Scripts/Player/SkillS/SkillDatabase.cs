using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Skills/Skill Database")]
public class SkillDatabase : ScriptableObject
{
    public List<SkillDefinition> AllSkills;
}