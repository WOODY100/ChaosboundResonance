using System.Threading;
using System.Threading.Tasks;

using Chaosbound.Systems.Dialogue.Runtime.Actions.Definitions;

namespace Chaosbound.Systems.Dialogue.Runtime.Actions.Execution
{
    public sealed class OpenProgressionWindowAction : IDialogueAction
    {
        public async Task<DialogueActionResult> ExecuteAsync(
            DialogueActionDefinition definition,
            DialogueActionContext context,
            CancellationToken cancellationToken)
        {
            if (!(definition is OpenProgressionWindowDefinition))
            {
                return DialogueActionResult.Failure;
            }

            if (context == null ||
                context.ProgressionWindowService == null)
            {
                return DialogueActionResult.Failure;
            }

            cancellationToken.ThrowIfCancellationRequested();

            return await context.ProgressionWindowService.OpenAndWaitAsync(
                cancellationToken);
        }
    }
}