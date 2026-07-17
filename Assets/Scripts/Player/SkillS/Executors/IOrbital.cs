using System;
using UnityEngine;

public interface IOrbital
{
    void Initialize(
        RuntimeSkill skill,
        Transform owner,
        float startAngle,
        Action onFinished
    );
}