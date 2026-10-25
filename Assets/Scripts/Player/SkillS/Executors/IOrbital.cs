using System;
using UnityEngine;

public interface IOrbital
{
    void Initialize(
        RuntimeSkill skill,
        Transform owner,
        OrbitalPatternDefinition pattern,
        float startAngle,
        Action onFinished
    );
}