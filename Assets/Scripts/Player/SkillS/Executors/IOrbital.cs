using UnityEngine;

public interface IOrbital
{
    void Initialize(
        RuntimeSkill skill,
        Transform owner,
        float startAngle,
        System.Action onFinished
    );
}