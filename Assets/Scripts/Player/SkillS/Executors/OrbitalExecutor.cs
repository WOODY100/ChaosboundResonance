using UnityEngine;

public class OrbitalExecutor : MonoBehaviour, ISkillExecutor
{
    private RuntimeSkill skill;
    private Transform owner;

    private int activeOrbs = 0;
    private bool isActive = false;

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

        // 🔥 Si el cooldown NO depende de duración, iniciarlo aquí
        if (!skill.Definition.CooldownStartsAfterDuration)
        {
            skill.StartCooldown(skill.Stats.FinalCooldown);
        }
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

            GameObject orbObj = Instantiate(
                skill.Definition.ExecutionPrefab,
                owner.position,
                Quaternion.identity
            );

            IOrbital orb = orbObj.GetComponent<IOrbital>();

            if (orb != null)
            {
                orb.Initialize(skill, owner, startAngle, OnSingleOrbFinished);
            }
        }
    }

    private void OnSingleOrbFinished()
    {
        activeOrbs--;

        if (activeOrbs <= 0)
        {
            isActive = false;

            // 🔥 Si depende de duración, iniciarlo aquí
            if (skill.Definition.CooldownStartsAfterDuration)
            {
                skill.StartCooldown(skill.Stats.FinalCooldown);
            }
        }
    }
}