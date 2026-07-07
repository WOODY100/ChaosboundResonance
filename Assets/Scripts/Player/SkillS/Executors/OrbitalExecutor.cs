using UnityEngine;

public class OrbitalExecutor : MonoBehaviour, ISkillExecutor
{
    private RuntimeSkill skill;
    private Transform owner;

    private int activeOrbs;
    private bool isActive;

    public void Initialize(RuntimeSkill runtimeSkill, Transform skillOwner)
    {
        skill = runtimeSkill;
        owner = skillOwner;
    }

    public void Tick(float deltaTime)
    {
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
        activeOrbs--;

        if (activeOrbs <= 0)
        {
            isActive = false;

            if (skill.Definition.CooldownStartsAfterDuration)
                skill.StartCooldown(skill.Stats.FinalCooldown);
        }
    }
}