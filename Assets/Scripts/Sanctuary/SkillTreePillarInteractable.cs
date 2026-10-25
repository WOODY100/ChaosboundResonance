
using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;
using UnityEngine;

public sealed class SkillTreePillarInteractable :
    MonoBehaviour,
    IInteractable
{
    [Header("References")]
    [SerializeField]
    private SkillTreeVisualController skillTreeVisualController;

    private GameFlow gameFlow;

    private void Awake()
    {
        ResolveGameFlow();

        if (skillTreeVisualController == null)
        {
            Debug.LogError(
                "[SkillTreePillarInteractable] " +
                "SkillTreeVisualController reference is missing.",
                this);
        }

        if (gameFlow == null)
        {
            Debug.LogError(
                "[SkillTreePillarInteractable] " +
                "GameFlow reference could not be resolved.",
                this);
        }
    }

    public void Interact(PlayerInteractor interactor)
    {
        Debug.Log("[SkillTreePillar] Interact received.", this);

        if (skillTreeVisualController == null)
        {
            Debug.LogError(
                "[SkillTreePillar] Visual Controller is null.",
                this);
            return;
        }

        Debug.Log(
            $"[SkillTreePillar] GameFlow context: " +
            $"{(gameFlow != null ? gameFlow.CurrentContext.ToString() : "NULL")}",
            this);

        if (gameFlow == null)
        {
            ResolveGameFlow();

            if (gameFlow == null)
            {
                Debug.LogError(
                    "[SkillTreePillar] GameFlow could not be resolved.",
                    this);
                return;
            }
        }

        if (gameFlow.CurrentContext != GameFlowContext.Playing)
        {
            Debug.LogWarning(
                $"[SkillTreePillar] Interaction rejected. " +
                $"Current context: {gameFlow.CurrentContext}",
                this);
            return;
        }

        gameFlow.Request(GameFlowContext.Stats);

        Debug.Log(
            $"[SkillTreePillar] Context after request: " +
            $"{gameFlow.CurrentContext}",
            this);

        if (gameFlow.CurrentContext != GameFlowContext.Stats)
        {
            Debug.LogWarning(
                "[SkillTreePillar] GameFlow did not enter Stats.",
                this);
            return;
        }

        Debug.Log(
            "[SkillTreePillar] Opening Skill Tree.",
            this);

        skillTreeVisualController.OpenSkillTree();
    }

    private void ResolveGameFlow()
    {
        BootstrapContext bootstrapContext =
            BootstrapContext.Current;

        if (bootstrapContext == null)
            return;

        gameFlow = bootstrapContext.GameFlow;
    }
}
