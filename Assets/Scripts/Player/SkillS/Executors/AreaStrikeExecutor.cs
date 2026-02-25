using UnityEngine;
using System.Collections;

public class AreaStrikeExecutor : MonoBehaviour, ISkillExecutor
{
    private RuntimeSkill skill;
    private Transform owner;

    private bool isExecuting = false;

    private static readonly Collider[] hitBuffer = new Collider[32];

    public void Initialize(RuntimeSkill runtimeSkill, Transform skillOwner)
    {
        skill = runtimeSkill;
        owner = skillOwner;
    }

    public void Tick(float deltaTime)
    {
        skill.TickCooldown(deltaTime);

        if (isExecuting)
            return;

        if (skill.IsOnCooldown)
            return;

        Execute();

        // 🔥 Si el cooldown NO depende de duración
        if (!skill.Definition.CooldownStartsAfterDuration)
        {
            skill.StartCooldown(skill.Stats.FinalCooldown);
        }
    }

    private void Execute()
    {
        isExecuting = true;
        StartCoroutine(SpawnStrikesWithDelay());
    }

    private IEnumerator SpawnStrikesWithDelay()
    {
        int count = skill.Stats.FinalCount;

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPoint =
                GetRandomPointAroundOwner(skill.Stats.FinalSpawnRadius);

            SpawnStrike(randomPoint);

            if (count > 1)
                yield return new WaitForSeconds(0.12f);
        }

        isExecuting = false;

        // 🔥 Si el cooldown depende de duración
        if (skill.Definition.CooldownStartsAfterDuration)
        {
            skill.StartCooldown(skill.Stats.FinalCooldown);
        }
    }

    private void SpawnStrike(Vector3 position)
    {
        GameObject strikeObj =
            Instantiate(skill.Definition.ExecutionPrefab,
                        position,
                        Quaternion.identity);

        IAreaStrike strike =
            strikeObj.GetComponent<IAreaStrike>();

        if (strike != null)
            strike.Initialize(skill);
    }

    private Vector3 GetRandomPointAroundOwner(float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;

        return owner.position +
               new Vector3(randomCircle.x, 0f, randomCircle.y);
    }
}