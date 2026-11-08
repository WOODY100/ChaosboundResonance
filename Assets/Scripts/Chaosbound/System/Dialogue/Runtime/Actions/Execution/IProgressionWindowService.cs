using System.Threading;
using System.Threading.Tasks;

namespace Chaosbound.Systems.Dialogue.Runtime.Actions.Execution
{
    public interface IProgressionWindowService
    {
        Task<DialogueActionResult> OpenAndWaitAsync(
            CancellationToken cancellationToken);
    }
}