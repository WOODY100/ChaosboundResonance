using Chaosbound.Runtime.Bootstrap;
using Chaosbound.Content.Expeditions.Assets;
using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Chaosbound.Gameplay.Expeditions.UI
{
    public sealed class ExpeditionSelectionWindowController
        : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private GameObject windowRoot;

        [SerializeField]
        private ExpeditionBootstrap expeditionBootstrap;

        [Header("Catalog")]
        [SerializeField]
        private ExpeditionCatalog expeditionCatalog;

        [Header("Expedition Card")]

        [SerializeField]
        private TMP_Text expeditionNameText;

        [SerializeField]
        private TMP_Text expeditionDescriptionText;

        [SerializeField]
        private TMP_Text selectedExpeditionText;

        [Header("Carousel")]
        [SerializeField]
        private ExpeditionCarouselController carouselController;

        [SerializeField]
        private GameObject expeditionDetailsRoot;

        [Header("Navigation")]
        [SerializeField]
        private Button previousButton;

        [SerializeField]
        private Button nextButton;

        [Header("Start Expedition")]
        [SerializeField]
        private Button startExpeditionButton;

        private int selectedExpeditionIndex;

        private GameFlow gameFlow;

        public ExpeditionAsset SelectedExpedition =>
            carouselController != null
                ? carouselController.SelectedExpedition
                : expeditionCatalog != null
                    ? expeditionCatalog.GetExpedition(selectedExpeditionIndex)
                    : null;

        private void Awake()
        {
            ResolveGameFlow();

            if (windowRoot == null)
            {
                Debug.LogError(
                    "[ExpeditionSelectionWindowController] " +
                    "Window root reference is missing.",
                    this);
            }

            if (gameFlow == null)
            {
                Debug.LogError(
                    "[ExpeditionSelectionWindowController] " +
                    "GameFlow reference could not be resolved.",
                    this);
            }

            if (windowRoot != null)
                windowRoot.SetActive(false);
        }

        public bool Open()
        {
            if (windowRoot == null)
            {
                Debug.LogError(
                    "[ExpeditionSelectionWindowController] " +
                    "Cannot open: window root is missing.",
                    this);

                return false;
            }

            if (expeditionDetailsRoot != null)
                expeditionDetailsRoot.SetActive(true);

            if (gameFlow == null)
                ResolveGameFlow();

            if (gameFlow == null)
                return false;

            if (gameFlow.CurrentEnvironment !=
                GameFlowEnvironment.Sanctuary)
            {
                return false;
            }

            if (gameFlow.IsActive(
                GameFlowContext.ExpeditionSelection))
            {
                return true;
            }

            bool opened = gameFlow.Request(
                GameFlowContext.ExpeditionSelection);

            if (!opened)
                return false;

            windowRoot.SetActive(true);

            selectedExpeditionIndex = 0;

            if (expeditionCatalog != null &&
                expeditionCatalog.Count > 0)
            {
                if (carouselController != null)
                    carouselController.SelectIndex(0);

                ShowExpedition(0);
            }
            else
            {
                UpdateNavigationButtons();

                Debug.LogWarning(
                    "[ExpeditionSelectionWindowController] " +
                    "No expeditions are available in the catalog.",
                    this);
            }

            return true;
        }

        public void Close()
        {
            if (windowRoot == null || gameFlow == null)
                return;

            if (!gameFlow.IsActive(
                GameFlowContext.ExpeditionSelection))
            {
                return;
            }

            if (gameFlow.CurrentContext !=
                GameFlowContext.ExpeditionSelection)
            {
                return;
            }

            bool closed = gameFlow.Pop(
                GameFlowContext.ExpeditionSelection);

            if (!closed)
                return;

            windowRoot.SetActive(false);
        }

        private bool ShowExpedition(int index)
        {
            if (expeditionCatalog == null)
            {
                Debug.LogError(
                    "[ExpeditionSelectionWindowController] " +
                    "Expedition catalog is missing.",
                    this);

                return false;
            }

            if (index < 0 || index >= expeditionCatalog.Count)
                return false;

            ExpeditionAsset asset =
                expeditionCatalog.GetExpedition(index);

            if (asset == null || asset.Expedition == null)
            {
                Debug.LogError(
                    "[ExpeditionSelectionWindowController] " +
                    "Expedition asset or authoring data is missing at index "
                    + index + ".",
                    this);

                return false;
            }

            var presentation = asset.Expedition.Presentation;

            if (presentation == null)
            {
                Debug.LogError(
                    "[ExpeditionSelectionWindowController] " +
                    "Expedition presentation is missing.",
                    this);

                return false;
            }

            selectedExpeditionIndex = index;

            if (expeditionNameText != null)
                expeditionNameText.text = presentation.DisplayName;

            if (expeditionDescriptionText != null)
            {
                expeditionDescriptionText.text =
                    presentation.Description;
            }

            if (selectedExpeditionText != null)
            {
                selectedExpeditionText.text =
                    "Selected: " + presentation.DisplayName;
            }

            UpdateNavigationButtons();

            return true;
        }

        private void OnCarouselTransitionStarted()
        {
            if (expeditionDetailsRoot != null)
                expeditionDetailsRoot.SetActive(false);
        }

        private void OnCarouselSelectionChanged(int index)
        {
            if (!ShowExpedition(index))
                return;

            if (expeditionDetailsRoot != null)
                expeditionDetailsRoot.SetActive(true);
        }

        private void ShowPreviousExpedition()
        {
            if (carouselController != null)
            {
                carouselController.ShowPrevious();
                return;
            }

            ShowExpedition(selectedExpeditionIndex - 1);
        }

        private void ShowNextExpedition()
        {
            if (carouselController != null)
            {
                carouselController.ShowNext();
                return;
            }

            ShowExpedition(selectedExpeditionIndex + 1);
        }

        private void StartSelectedExpedition()
        {
            ExpeditionAsset selectedExpedition = SelectedExpedition;

            if (selectedExpedition == null)
            {
                Debug.LogError(
                    "[ExpeditionSelectionWindowController] " +
                    "No valid expedition is selected.",
                    this);

                return;
            }

            if (expeditionBootstrap == null)
            {
                Debug.LogError(
                    "[ExpeditionSelectionWindowController] " +
                    "ExpeditionBootstrap reference is missing.",
                    this);

                return;
            }

            // Close the selection window visually.
            if (windowRoot != null)
                windowRoot.SetActive(false);

            // Start the expedition and transition to its scene.
            expeditionBootstrap.StartExpedition(selectedExpedition);
        }

        private void UpdateNavigationButtons()
        {
            int count = expeditionCatalog != null
                ? expeditionCatalog.Count
                : 0;

            bool canGoPrevious =
                count > 0 && selectedExpeditionIndex > 0;

            bool canGoNext =
                count > 0 &&
                selectedExpeditionIndex < count - 1;

            if (previousButton != null)
            {
                previousButton.gameObject.SetActive(
                    canGoPrevious);
            }

            if (nextButton != null)
            {
                nextButton.gameObject.SetActive(
                    canGoNext);
            }
        }

        private void OnEnable()
        {
            if (startExpeditionButton != null)
            {
                startExpeditionButton.onClick.AddListener(
                    StartSelectedExpedition);
            }

            if (previousButton != null)
            {
                previousButton.onClick.AddListener(
                    ShowPreviousExpedition);
            }

            if (nextButton != null)
            {
                nextButton.onClick.AddListener(
                    ShowNextExpedition);
            }

            if (carouselController != null)
            {
                carouselController.SelectionChanged +=
                    OnCarouselSelectionChanged;

                carouselController.TransitionStarted +=
                    OnCarouselTransitionStarted;
            }
        }

        private void OnDisable()
        {
            if (startExpeditionButton != null)
            {
                startExpeditionButton.onClick.RemoveListener(
                    StartSelectedExpedition);
            }

            if (previousButton != null)
            {
                previousButton.onClick.RemoveListener(
                    ShowPreviousExpedition);
            }

            if (nextButton != null)
            {
                nextButton.onClick.RemoveListener(
                    ShowNextExpedition);
            }

            if (carouselController != null)
            {
                carouselController.SelectionChanged -=
                    OnCarouselSelectionChanged;

                carouselController.TransitionStarted -=
                    OnCarouselTransitionStarted;
            }
        }

        private void ResolveGameFlow()
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            gameFlow = bootstrapContext.GameFlow;
        }
    }
}
