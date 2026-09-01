using Chaosbound.Core.Composition;
using UnityEngine;

namespace Chaosbound.Core.GameFlow
{
    public sealed class GameplayInputBinding : MonoBehaviour
    {
        private IGameplayInputTarget target;
        private bool hasStarted;

        private void Awake()
        {
            target =
                GetComponent<IGameplayInputTarget>();

            if (target == null)
            {
                Debug.LogError(
                    $"{nameof(GameplayInputBinding)} " +
                    $"requires a component implementing " +
                    $"{nameof(IGameplayInputTarget)}.",
                    this);
            }
        }

        private void OnEnable()
        {
            if (!hasStarted)
                return;

            Bind();
        }

        private void Start()
        {
            hasStarted = true;

            Bind();
        }

        private void OnDisable()
        {
            if (!hasStarted)
                return;

            Unbind();
        }

        private void Bind()
        {
            if (target == null)
                return;

            BootstrapContext context =
                BootstrapContext.Current;

            if (context == null)
            {
                return;
            }

            GameFlow gameFlow =
                context.GameFlow;

            if (gameFlow == null)
            {
                return;
            }

            gameFlow.BindGameplayInputTarget(target);
        }

        private void Unbind()
        {
            if (target == null)
                return;

            BootstrapContext context =
                BootstrapContext.Current;

            if (context == null)
                return;

            GameFlow gameFlow =
                context.GameFlow;

            if (gameFlow == null)
                return;

            gameFlow.UnbindGameplayInputTarget(target);
        }
    }
}