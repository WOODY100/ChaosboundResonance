using UnityEngine.InputSystem;
using NovaVertexCode.DialogGraphEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Systems.Dialogue.UI
{
    public sealed class DialoguePanelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private DialogueSessionController sessionController;

        [SerializeField]
        private GameObject panelRoot;

        [Header("Portrait")]
        [SerializeField]
        private Image portraitImage;

        [Header("Dialogue Text")]
        [SerializeField]
        private TMP_Text speakerNameText;

        [SerializeField]
        private TMP_Text dialogueBodyText;

        [Header("Choices")]
        [SerializeField]
        private Transform choicesContainer;

        [SerializeField]
        private Button choiceButtonPrefab;

        [Header("Controls")]
        [SerializeField]
        private Button closeButton;

        private const int MaxVisibleChoices = 4;

        private void Update()
        {
            if (sessionController == null ||
                !sessionController.IsSessionActive)
                return;

            if (Keyboard.current != null &&
                Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CloseDialogue();
            }
        }

        private void Awake()
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);

            if (sessionController == null)
            {
                Debug.LogError(
                    "[DialoguePanel] Session Controller reference is missing.",
                    this);
            }

            if (speakerNameText == null ||
                dialogueBodyText == null ||
                choicesContainer == null ||
                choiceButtonPrefab == null ||
                closeButton == null)
            {
                Debug.LogError(
                    "[DialoguePanel] One or more UI references are missing.",
                    this);
            }
        }

        private void OnEnable()
        {
            if (sessionController != null)
            {
                sessionController.OnPortraitChanged.AddListener(
                    HandlePortraitChanged);

                sessionController.OnNodePresented.AddListener(
                    HandleNodePresented);

                sessionController.OnDialogueClosed.AddListener(
                    HandleDialogueClosed);
            }

            if (closeButton != null)
                closeButton.onClick.AddListener(CloseDialogue);
        }

        private void OnDisable()
        {
            if (sessionController != null)
            {
                sessionController.OnPortraitChanged.RemoveListener(
                    HandlePortraitChanged);

                sessionController.OnNodePresented.RemoveListener(
                    HandleNodePresented);

                sessionController.OnDialogueClosed.RemoveListener(
                    HandleDialogueClosed);
            }

            if (closeButton != null)
                closeButton.onClick.RemoveListener(CloseDialogue);
        }

        private void HandleNodePresented(DGEBaseNode node)
        {
            if (!(node is DGETextNode textNode))
            {
                Debug.LogWarning(
                    "[DialoguePanel] Unsupported dialogue node type: " +
                    node.GetType().Name,
                    this);

                return;
            }

            if (speakerNameText != null)
                speakerNameText.text = textNode.actorName;

            if (dialogueBodyText != null)
                dialogueBodyText.text = textNode.text;

            BuildChoices(textNode);

            if (panelRoot != null)
                panelRoot.SetActive(true);
        }

        private void BuildChoices(DGETextNode textNode)
        {
            ClearChoices();

            if (textNode.outputs == null)
                return;

            int choiceCount = 0;

            foreach (DGEBaseNode output in textNode.outputs)
            {
                if (!(output is DGEActionNode actionNode))
                    continue;

                if (choiceCount >= MaxVisibleChoices)
                {
                    Debug.LogWarning(
                        "[DialoguePanel] Maximum of four choices reached.",
                        this);

                    break;
                }

                Button choiceButton = Instantiate(
                    choiceButtonPrefab,
                    choicesContainer);

                choiceCount++;

                TMP_Text choiceText =
                    choiceButton.GetComponentInChildren<TMP_Text>(true);

                if (choiceText != null)
                    choiceText.text = actionNode.text;

                choiceButton.onClick.AddListener(
                    () => SelectChoice(actionNode));
            }
        }

        private void SelectChoice(DGEActionNode actionNode)
        {
            if (sessionController == null)
                return;

            sessionController.SelectAction(actionNode);
        }

        private void ClearChoices()
        {
            if (choicesContainer == null)
                return;

            for (int i = choicesContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(choicesContainer.GetChild(i).gameObject);
            }
        }

        public void CloseDialogue()
        {
            if (sessionController == null)
                return;

            sessionController.CloseDialogue();
        }

        private void HandleDialogueClosed()
        {
            ClearChoices();

            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        private void HandlePortraitChanged(Sprite portrait)
        {
            if (portraitImage == null)
                return;

            portraitImage.sprite = portrait;
            portraitImage.enabled = portrait != null;
        }
    }
}