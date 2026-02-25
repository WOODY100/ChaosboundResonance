using UnityEngine;

public interface ISkillExecutor
{
    void Initialize(RuntimeSkill skill, Transform owner);
    void Tick(float deltaTime);
}