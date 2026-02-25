using UnityEngine;
using System.Collections.Generic;

public class PlayerSkillExecutorSystem : MonoBehaviour
{
    private PlayerSkillLoadout loadout;

    private List<ISkillExecutor> activeExecutors =
        new List<ISkillExecutor>();

    void Awake()
    {
        loadout = GetComponent<PlayerSkillLoadout>();
    }

    void OnEnable()
    {
        if (loadout != null)
            loadout.OnLoadoutChanged += RebuildExecutors;
    }

    void OnDisable()
    {
        if (loadout != null)
            loadout.OnLoadoutChanged -= RebuildExecutors;
    }

    void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Playing)
            return;

        float delta = Time.deltaTime;

        foreach (var executor in activeExecutors)
        {
            executor.Tick(delta);
        }
    }

    private void RebuildExecutors()
    {
        // Destroy old executor components
        foreach (var executor in activeExecutors)
        {
            if (executor is MonoBehaviour mb)
                Destroy(mb);
        }

        activeExecutors.Clear();

        RuntimeSkill[] skills = loadout.GetAllSkills();

        foreach (var skill in skills)
        {
            if (skill == null)
                continue;

            CreateExecutor(skill);
        }
    }

    private void CreateExecutor(RuntimeSkill skill)
    {
        if (skill.Definition.ExecutorPrefab == null)
        {
            Debug.LogWarning(
                $"Skill {skill.Definition.DisplayName} has no ExecutorPrefab assigned."
            );
            return;
        }

        GameObject executorObj = Instantiate(
            skill.Definition.ExecutorPrefab,
            transform
        );

        ISkillExecutor executor =
            executorObj.GetComponent<ISkillExecutor>();

        if (executor == null)
        {
            Debug.LogError(
                $"ExecutorPrefab for {skill.Definition.DisplayName} does not implement ISkillExecutor."
            );
            return;
        }

        executor.Initialize(skill, transform);
        activeExecutors.Add(executor);
    }
}