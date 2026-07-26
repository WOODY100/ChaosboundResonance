using Chaosbound.Content.Expeditions.Runtime.Configs;
using UnityEngine;

public sealed class RunSession : MonoBehaviour
{
    private RuntimeExpeditionConfig currentRun;

    public bool HasRun => currentRun != null;

    public RuntimeExpeditionConfig CurrentRun => currentRun;

    public void SetRun(RuntimeExpeditionConfig run)
    {
        currentRun = run;
    }

    public void ClearRun()
    {
        currentRun = null;
    }
}