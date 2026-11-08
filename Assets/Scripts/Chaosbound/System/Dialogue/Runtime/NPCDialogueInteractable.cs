using UnityEngine;

namespace Chaosbound.Systems.Dialogue
{
    [RequireComponent(typeof(DialogueSessionController))]
    public sealed class NPCDialogueInteractable :
        MonoBehaviour,
        IInteractable
    {
        [Header("References")]
        [SerializeField]
        private DialogueSessionController sessionController;

        private void Awake()
        {
            if (sessionController == null)
            {
                sessionController =
                    GetComponent<DialogueSessionController>();
            }

            if (sessionController == null)
            {
                Debug.LogError(
                    "[NPCDialogueInteractable] " +
                    "DialogueSessionController reference is missing.",
                    this);
            }
        }

        public void Interact(PlayerInteractor interactor)
        {
            if (sessionController == null)
                return;

            if (sessionController.IsSessionActive)
                return;

            sessionController.BeginDialogue();
        }
    }
}