using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.MetaProgression.Persistent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class MetaProgressionHUD : MonoBehaviour
{
    [Header("References")]

    private PersistentMetaRuntime metaRuntime;

    [SerializeField]
    private TMP_Text metaLevelText;

    [SerializeField]
    private TMP_Text metaExperienceText;

    [SerializeField]
    private Image metaExperienceFill;

    private void OnEnable()
    {
        BootstrapContext context =
            BootstrapContext.Current;

        if (context == null)
        {
            Debug.LogError(
                "[MetaProgressionHUD] " +
                "BootstrapContext.Current could not be found.",
                this);

            return;
        }

        metaRuntime =
            context.PersistentMetaRuntime;

        if (metaRuntime == null)
        {
            Debug.LogError(
                "[MetaProgressionHUD] " +
                "PersistentMetaRuntime could not be found.",
                this);

            return;
        }

        metaRuntime.MetaProgressionChanged += Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDisable()
    {
        if (metaRuntime == null)
            return;

        metaRuntime.MetaProgressionChanged -= Refresh;
    }

    public void Refresh()
    {
        if (metaRuntime == null)
            return;

        PersistentMetaState state =
            metaRuntime.State;

        if (state == null)
            return;

        int experienceRequired =
            metaRuntime.ExperienceRequiredForNextLevel;

        if (metaLevelText != null)
        {
            metaLevelText.text =
                "Nivel: " +
                state.MetaLevel;
        }

        if (metaExperienceText != null)
        {
            metaExperienceText.text =
                state.Experience +
                " / " +
                experienceRequired;
        }

        if (metaExperienceFill != null)
        {
            metaExperienceFill.fillAmount =
                CalculateExperienceProgress(
                    state);
        }
    }

    private float CalculateExperienceProgress(
        PersistentMetaState state)
    {
        int experienceRequired =
            metaRuntime.ExperienceRequiredForNextLevel;

        if (experienceRequired <= 0)
            return 0f;

        float progress =
            (float)state.Experience /
            experienceRequired;

        return Mathf.Clamp01(progress);
    }
}