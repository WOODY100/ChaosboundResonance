using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using Chaosbound.Gameplay.Inventory.UI;

namespace Chaosbound.Systems.Dialogue.Runtime.Actions.Execution
{
    public sealed class BlacksmithProgressionWindowService :
        MonoBehaviour,
        IProgressionWindowService
    {
        [Header("Blacksmith Window")]
        [SerializeField]
        private BlacksmithWindowController windowController;

        private bool isExecuting;

        public async Task<DialogueActionResult> OpenAndWaitAsync(
            CancellationToken cancellationToken)
        {
            if (windowController == null)
            {
                Debug.LogError(
                    "[BlacksmithProgressionWindowService] " +
                    "BlacksmithWindowController reference is missing.",
                    this);

                return DialogueActionResult.Failure;
            }

            if (isExecuting || windowController.gameObject.activeSelf)
            {
                Debug.LogWarning(
                    "[BlacksmithProgressionWindowService] " +
                    "The Blacksmith window is already open or in use.",
                    this);

                return DialogueActionResult.Failure;
            }

            cancellationToken.ThrowIfCancellationRequested();

            isExecuting = true;

            TaskCompletionSource<DialogueActionResult> completion =
                new TaskCompletionSource<DialogueActionResult>(
                    TaskCreationOptions.RunContinuationsAsynchronously);

            TaskCompletionSource<bool> cancellationSignal =
                new TaskCompletionSource<bool>(
                    TaskCreationOptions.RunContinuationsAsynchronously);

            void HandleWindowClosed()
            {
                completion.TrySetResult(
                    DialogueActionResult.Success);
            }

            windowController.OnWindowClosed += HandleWindowClosed;

            using (cancellationToken.Register(
                () => cancellationSignal.TrySetResult(true)))
            {
                try
                {
                    windowController.OpenWindow();

                    await Task.WhenAny(
                        completion.Task,
                        cancellationSignal.Task);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        windowController.CloseWindow();

                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    return await completion.Task;
                }
                finally
                {
                    windowController.OnWindowClosed -=
                        HandleWindowClosed;

                    isExecuting = false;
                }
            }
        }

        private void OnValidate()
        {
            if (windowController == null)
            {
                windowController =
                    GetComponent<BlacksmithWindowController>();
            }
        }
    }
}