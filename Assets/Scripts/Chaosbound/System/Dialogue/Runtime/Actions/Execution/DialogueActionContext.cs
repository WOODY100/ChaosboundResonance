using UnityEngine;

namespace Chaosbound.Systems.Dialogue.Runtime.Actions.Execution
{
    public sealed class DialogueActionContext
    {
        public DialogueSessionController SessionController
        {
            get;
        }

        public Transform SourceNpc
        {
            get;
        }

        public IProgressionWindowService ProgressionWindowService
        {
            get;
        }

        public DialogueActionContext(
            DialogueSessionController sessionController,
            Transform sourceNpc,
            IProgressionWindowService progressionWindowService)
        {
            SessionController = sessionController;
            SourceNpc = sourceNpc;
            ProgressionWindowService = progressionWindowService;
        }
    }
}