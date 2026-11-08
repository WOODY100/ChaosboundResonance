using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;
using NovaVertexCode.DialogGraphEngine;
using Chaosbound.Systems.Dialogue.Runtime.Actions;
using Chaosbound.Systems.Dialogue.Runtime.Actions.Definitions;
using Chaosbound.Systems.Dialogue.Runtime.Actions.Execution;
using Chaosbound.Systems.Dialogue.Integration.DGE.Nodes;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Chaosbound.Systems.Dialogue
{
    [RequireComponent(typeof(DGEDialogSelection))]
    public sealed class DialogueSessionController : MonoBehaviour
    {
        [Header("DGE")]
        [SerializeField]
        private DGEDialogSelection dialogSelection;

        [Header("Events")]
        [SerializeField]
        private UnityEvent<DGEBaseNode> onNodePresented =
            new UnityEvent<DGEBaseNode>();

        [Header("NPC Presentation")]
        [SerializeField]
        private Sprite dialoguePortrait;

        [SerializeField]
        private UnityEvent<Sprite> onPortraitChanged =
            new UnityEvent<Sprite>();

        public UnityEvent<Sprite> OnPortraitChanged =>
            onPortraitChanged;

        [SerializeField]
        private UnityEvent onDialogueClosed =
            new UnityEvent();

        [Header("Dialogue Actions")]
        [SerializeField]
        private MonoBehaviour progressionWindowServiceComponent;

        private GameFlow gameFlow;

        private bool sessionActive;

        private CancellationTokenSource sessionCancellation;

        private IProgressionWindowService progressionWindowService;
        private DialogueActionRuntime actionRuntime;

        private bool commandExecutionInProgress;

        public CancellationToken SessionCancellationToken =>
            sessionCancellation != null
                ? sessionCancellation.Token
                : CancellationToken.None;

        public bool IsSessionActive => sessionActive;

        public UnityEvent<DGEBaseNode> OnNodePresented =>
            onNodePresented;

        public UnityEvent OnDialogueClosed =>
            onDialogueClosed;

        private void Awake()
        {
            if (dialogSelection == null)
            {
                dialogSelection =
                    GetComponent<DGEDialogSelection>();
            }

            if (onPortraitChanged == null)
            {
                onPortraitChanged = new UnityEvent<Sprite>();
            }

            if (onNodePresented == null)
            {
                onNodePresented =
                    new UnityEvent<DGEBaseNode>();
            }

            if (onDialogueClosed == null)
            {
                onDialogueClosed =
                    new UnityEvent();
            }

            if (progressionWindowServiceComponent != null)
            {
                progressionWindowService =
                    progressionWindowServiceComponent as IProgressionWindowService;
            }

            if (progressionWindowService == null)
            {
                Debug.LogError(
                    "[Dialogue] Progression Window Service is missing " +
                    "or does not implement IProgressionWindowService.",
                    this);
            }
            else
            {
                actionRuntime = new DialogueActionRuntime(
                    progressionWindowService);
            }
        }

        private void OnEnable()
        {
            if (dialogSelection == null)
                return;

            dialogSelection.onUpdateData.AddListener(
                HandleNodeUpdated);

            dialogSelection.onDialogEnd.AddListener(
                HandleDialogueEnded);
        }

        private void OnDisable()
        {
            if (dialogSelection != null)
            {
                dialogSelection.onUpdateData.RemoveListener(
                    HandleNodeUpdated);

                dialogSelection.onDialogEnd.RemoveListener(
                    HandleDialogueEnded);
            }

            if (sessionActive)
            {
                CloseSession();
            }
        }

        public bool BeginDialogue()
        {
            if (sessionActive)
                return false;

            BootstrapContext bootstrap =
                BootstrapContext.Current;

            if (bootstrap == null)
            {
                Debug.LogError(
                    "[Dialogue] BootstrapContext not available.",
                    this);

                return false;
            }

            gameFlow = bootstrap.GameFlow;

            if (gameFlow == null || !gameFlow.IsInitialized)
            {
                Debug.LogError(
                    "[Dialogue] GameFlow is not initialized.",
                    this);

                return false;
            }

            if (gameFlow.CurrentContext != GameFlowContext.Playing)
                return false;

            if (!gameFlow.Request(GameFlowContext.Dialogue))
                return false;

            sessionActive = true;

            sessionCancellation = new CancellationTokenSource();

            onPortraitChanged.Invoke(dialoguePortrait);

            if (!dialogSelection.LoadStartNode())
            {
                CloseSession();
                return false;
            }

            sessionActive = true;

            dialogSelection.StartDialog();

            return true;
        }

        public async Task<DialogueActionResult> ExecuteCommandAsync(
            DialogueActionDefinition definition)
        {
            if (!sessionActive || actionRuntime == null)
                return DialogueActionResult.Failure;

            CancellationToken token = SessionCancellationToken;

            token.ThrowIfCancellationRequested();

            DialogueActionContext context = new DialogueActionContext(
                this,
                transform,
                progressionWindowService);

            return await actionRuntime.ExecuteAsync(
                definition,
                context,
                token);
        }

        public void SelectAction(DGEBaseNode node)
        {
            if (!sessionActive || node == null)
                return;

            dialogSelection.SelectAction(node);
        }

        public void CloseDialogue()
        {
            if (!sessionActive)
                return;

            dialogSelection.EndDialog();
        }

        private void HandleNodeUpdated(DGEBaseNode node)
        {
            if (!sessionActive || node == null)
                return;

            if (node is CBCommandNode commandNode)
            {
                if (commandExecutionInProgress)
                {
                    Debug.LogWarning(
                        "[Dialogue] A command is already executing.",
                        this);

                    return;
                }

                ExecuteCommandNodeAsync(commandNode);
                return;
            }

            onNodePresented.Invoke(node);
        }

        private async void ExecuteCommandNodeAsync(
            CBCommandNode commandNode)
        {
            commandExecutionInProgress = true;

            DialogueActionResult result;

            try
            {
                result = await ExecuteCommandAsync(
                    commandNode.ActionDefinition);
            }
            catch (OperationCanceledException)
            {
                commandExecutionInProgress = false;

                // La sesión fue cancelada técnicamente.
                // No debe confundirse con el resultado Cancelled.
                return;
            }
            catch (Exception exception)
            {
                commandExecutionInProgress = false;

                Debug.LogException(exception, this);

                if (!sessionActive)
                    return;

                result = DialogueActionResult.Failure;
            }

            commandExecutionInProgress = false;

            if (!sessionActive)
                return;

            int outputIndex = (int)result;

            bool continued =
                dialogSelection.ContinueFromCurrentNode(
                    commandNode,
                    outputIndex);

            if (!continued)
            {
                Debug.LogError(
                    "[Dialogue] Failed to continue from CBCommandNode.",
                    commandNode);
            }
        }

        private void HandleDialogueEnded()
        {
            if (!sessionActive)
                return;

            CloseSession();
        }

        private void CloseSession()
        {
            if (!sessionActive)
                return;

            sessionActive = false;

            // Cancel pending dialogue actions.
            if (sessionCancellation != null)
            {
                sessionCancellation.Cancel();
                sessionCancellation.Dispose();
                sessionCancellation = null;
            }

            if (gameFlow != null &&
                gameFlow.IsInitialized &&
                gameFlow.CurrentContext == GameFlowContext.Dialogue)
            {
                gameFlow.Pop(GameFlowContext.Dialogue);
            }

            onDialogueClosed.Invoke();
        }
    }
}