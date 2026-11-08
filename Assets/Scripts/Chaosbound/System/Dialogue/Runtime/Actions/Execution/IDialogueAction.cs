using System.Threading;
using System.Threading.Tasks;

using Chaosbound.Systems.Dialogue.Runtime.Actions.Definitions;

namespace Chaosbound.Systems.Dialogue.Runtime.Actions.Execution
{
    public interface IDialogueAction
    {
        Task<DialogueActionResult> ExecuteAsync(
            DialogueActionDefinition definition,
            DialogueActionContext context,
            CancellationToken cancellationToken);
    }
}