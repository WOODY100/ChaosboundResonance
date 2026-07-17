using UnityEngine;

public class OrbitalExecutor : MonoBehaviour, ISkillExecutor
{
    private RuntimeSkill skill;
    private Transform owner;

    private int activeOrbs;
    private bool isActive;

    public void Initialize(RuntimeSkill runtimeSkill, Transform skillOwner)
    {
        ResetExecutor();

        skill = runtimeSkill;
        owner = skillOwner;
    }

    public void Tick(float deltaTime)
    {
        if (skill == null || owner == null)
            return;

        skill.TickCooldown(deltaTime);

        if (isActive)
            return;

        if (skill.IsOnCooldown)
            return;

        ActivateOrbit();

        if (!skill.Definition.CooldownStartsAfterDuration)
            skill.StartCooldown(skill.Stats.FinalCooldown);
    }

    private void ActivateOrbit()
    {
        if (skill.Stats.FinalCount <= 0)
            return;

        if (PoolManager.Instance == null)
        {
            Debug.LogError("PoolManager not found.");
            return;
        }

        if (skill.Definition.ExecutionPrefab == null)
        {
            Debug.LogError(
                $"{skill.Definition.name} has no ExecutionPrefab assigned.");
            return;
        }

        isActive = true;

        int count = skill.Stats.FinalCount;
        activeOrbs = count;

        float angleStep = 360f / count;
        float randomOffset = Random.Range(0f, 360f);

        for (int i = 0; i < count; i++)
        {
            float startAngle = randomOffset + (i * angleStep);

            GameObject orbObj = PoolManager.Instance.Get(
                skill.Definition.ExecutionPrefab,
                owner.position,
                Quaternion.identity
            );

            IOrbital orb = orbObj.GetComponent<IOrbital>();

            if (orb != null)
            {
                orb.Initialize(skill, owner, startAngle, OnSingleOrbFinished);
            }
            else
            {
                OnSingleOrbFinished();
            }
        }
    }

    private void OnSingleOrbFinished()
    {
        if (!isActive)
            return;

        activeOrbs--;

        if (activeOrbs > 0)
            return;

        activeOrbs = 0;
        isActive = false;

        if (skill != null &&
            skill.Definition.CooldownStartsAfterDuration)
        {
            skill.StartCooldown(skill.Stats.FinalCooldown);
        }
    }

    public void ResetExecutor()
    {
        skill = null;
        owner = null;

        activeOrbs = 0;
        isActive = false;
    }
}