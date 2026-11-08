using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using Chaosbound.Systems.Dialogue.Runtime.Actions.Definitions;
using Chaosbound.Systems.Dialogue.Runtime.Actions.Registry;

namespace Chaosbound.Systems.Dialogue.Runtime.Actions.Execution
{
    public sealed class DialogueActionExecutor
    {
        private readonly DialogueActionRegistry registry;

        public DialogueActionExecutor(DialogueActionRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            this.registry = registry;
        }

        public async Task<DialogueActionResult> ExecuteAsync(
            DialogueActionDefinition definition,
            DialogueActionContext context,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (definition == null)
            {
                Debug.LogError(
                    "[DialogueActionExecutor] Definition is null.");

                return DialogueActionResult.Failure;
            }

            if (context == null)
            {
                Debug.LogError(
                    "[DialogueActionExecutor] Context is null.");

                return DialogueActionResult.Failure;
            }

            if (!registry.TryResolve(definition, out IDialogueAction action))
            {
                return DialogueActionResult.Failure;
            }

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                DialogueActionResult result = await action.ExecuteAsync(
                    definition,
                    context,
                    cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                return result;
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);

                return DialogueActionResult.Failure;
            }
        }
    }
}