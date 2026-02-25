using UnityEngine;

[System.Serializable]
public class CooldownComponent
{
    [SerializeField] private float baseCooldown = 1f;

    private float timer = 0f;

    public float CooldownMultiplier { get; set; } = 1f;

    public bool IsReady => timer <= 0f;

    public float RemainingTime => timer;

    public float NormalizedTime =>
        baseCooldown <= 0f ? 0f : timer / GetFinalCooldown();

    public void Tick(float deltaTime)
    {
        if (timer > 0f)
            timer -= deltaTime;
    }

    public void Trigger()
    {
        timer = GetFinalCooldown();
    }

    public float GetFinalCooldown()
    {
        return baseCooldown * CooldownMultiplier;
    }

    public void SetBaseCooldown(float value)
    {
        baseCooldown = value;
    }

    public void ForceReady()
    {
        timer = 0f;
    }
}