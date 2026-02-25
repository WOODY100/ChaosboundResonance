using UnityEngine;
using System;

public class PlayerExperienceSystem : MonoBehaviour
{
    [Header("XP Curve")]
    [SerializeField] private float baseXP = 10f;
    [SerializeField] private float growthFactor = 0.35f;

    public int CurrentLevel { get; private set; } = 1;
    public float CurrentXP { get; private set; }
    public float RequiredXP { get; private set; }

    public event Action<float, float> OnXPChanged;
    public event Action<int> OnLevelUp;

    void Awake()
    {
        RecalculateRequiredXP();
    }

    // ===============================
    // PUBLIC API
    // ===============================

    public void AddXP(float amount)
    {
        CurrentXP += amount;

        CheckLevelUp();

        OnXPChanged?.Invoke(CurrentXP, RequiredXP);
    }

    public void ResetProgression()
    {
        CurrentLevel = 1;
        CurrentXP = 0f;
        RecalculateRequiredXP();
    }

    // ===============================
    // INTERNAL LOGIC
    // ===============================

    private void CheckLevelUp()
    {
        //bool leveledUp = false;

        while (CurrentXP >= RequiredXP)
        {
            CurrentXP -= RequiredXP;
            CurrentLevel++;

            RecalculateRequiredXP();

            OnLevelUp?.Invoke(CurrentLevel);
            //leveledUp = true;
        }

        OnXPChanged?.Invoke(CurrentXP, RequiredXP);
    }

    private void RecalculateRequiredXP()
    {
        RequiredXP = baseXP * (1f + (CurrentLevel - 1) * growthFactor);
    }
}