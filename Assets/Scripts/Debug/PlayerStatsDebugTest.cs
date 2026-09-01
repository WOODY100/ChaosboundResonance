using UnityEngine;
using UnityEngine.InputSystem;
using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;

public class PlayerStatsDebugTest : MonoBehaviour
{
    private PlayerExperienceSystem experience;

    private void Awake()
    {
        experience =
            GetComponent<PlayerExperienceSystem>();
    }

    private void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            experience.AddXP(50f);
        }

        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            BootstrapContext context =
                BootstrapContext.Current;

            if (context != null &&
                context.GameFlow != null)
            {
                context.GameFlow.Request(
                    GameFlowContext.Confirmation);
            }
        }

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            BootstrapContext context =
                BootstrapContext.Current;

            if (context != null &&
                context.GameFlow != null)
            {
                context.GameFlow.Request(
                    GameFlowContext.Pause);
            }
        }

        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            BootstrapContext context =
                BootstrapContext.Current;

            if (context != null &&
                context.GameFlow != null)
            {
                context.GameFlow.Pop(
                    GameFlowContext.Pause);
            }
        }
    }
}