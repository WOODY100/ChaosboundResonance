using Chaosbound.Core.Composition;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillExecutorSystem : MonoBehaviour
{
    private PlayerSkillLoadout loadout;

    private readonly List<ISkillExecutor> activeExecutors = new();

    private bool isCleaningUp;

    public int ExecutorCount => activeExecutors.Count;

    void Awake()
    {
        loadout = GetComponent<PlayerSkillLoadout>();
    }

    private void Start()
    {
        RebuildExecutors();
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
        BootstrapContext context =
            BootstrapContext.Current;

        if (context == null ||
            context.GameFlow == null)
        {
            return;
        }

        if (!context.GameFlow.CanRunGameplay)
            return;

        float delta = Time.deltaTime;

        for (int i = 0; i < activeExecutors.Count; i++)
        {
            activeExecutors[i]?.Tick(delta);
        }
    }

    private void RebuildExecutors()
    {
        if (isCleaningUp)
            return;

        DestroyExecutors();

        if (loadout == null)
            return;

        RuntimeSkill[] skills = loadout.GetAllSkills();

        foreach (RuntimeSkill skill in skills)
        {
            if (skill == null)
                continue;

            CreateExecutor(skill);
        }
    }

    private void DestroyExecutors()
    {
        foreach (ISkillExecutor executor in activeExecutors)
        {
            if (executor == null)
                continue;

            executor.Cleanup();
            executor.ResetExecutor();

            if (executor is MonoBehaviour mb)
            {
                Destroy(mb);
            }
        }

        activeExecutors.Clear();
    }

    private void OnDestroy()
    {
        DestroyExecutors();
    }

    private void CreateExecutor(RuntimeSkill skill)
    {
        if (skill.Definition == null)
        {
            Debug.LogError("RuntimeSkill has no SkillDefinition.");
            return;
        }

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

        if (executorObj == null)
        {
            Debug.LogError(
                $"Failed to instantiate ExecutorPrefab for {skill.Definition.DisplayName}."
            );
            return;
        }

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

    public void Cleanup()
    {
        if (isCleaningUp)
            return;

        isCleaningUp = true;

        DestroyExecutors();

        isCleaningUp = false;
    }
}