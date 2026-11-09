using Chaosbound.Systems.Dialogue.Runtime.Actions.Definitions;
using Chaosbound.Systems.Dialogue.Runtime.Actions.Execution;
using Chaosbound.Systems.Dialogue.Runtime.Actions.Registry;

namespace Chaosbound.Systems.Dialogue.Runtime.Actions
{
    public sealed class DialogueActionRuntime
    {
        private readonly DialogueActionRegistry registry;
        private readonly DialogueActionExecutor executor;

        public DialogueActionRuntime()
        {
            registry = new DialogueActionRegistry();

            registry.Register<OpenProgressionWindowDefinition>(
                new OpenProgressionWindowAction());

            executor = new DialogueActionExecutor(registry);
        }

        public System.Threading.Tasks.Task<DialogueActionResult>
            ExecuteAsync(
                DialogueActionDefinition definition,
                DialogueActionContext context,
                System.Threading.CancellationToken cancellationToken)
        {
            return executor.ExecuteAsync(
                definition,
                context,
                cancellationToken);
        }
    }
}