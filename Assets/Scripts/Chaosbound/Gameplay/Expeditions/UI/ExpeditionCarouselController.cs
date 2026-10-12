using System;
using System.Collections;
using System.Collections.Generic;
using Chaosbound.Content.Expeditions.Assets;
using UnityEngine;

namespace Chaosbound.Gameplay.Expeditions.UI
{
    [DisallowMultipleComponent]
    public sealed class ExpeditionCarouselController : MonoBehaviour
    {
        [Header("Catalog")]
        [SerializeField] private ExpeditionCatalog expeditionCatalog;

        [Header("Card Prefab")]
        [SerializeField] private ExpeditionCarouselCardView cardPrefab;

        [Header("Container")]
        [SerializeField] private RectTransform cardContainer;

        [Header("Layout")]
        [SerializeField] private float horizontalSpacing = 360f;
        [SerializeField] private float sideCardAngle = 180f;
        [SerializeField] private float sideCardScale = 0.78f;
        [SerializeField] private float transitionDuration = 0.4f;

        private readonly List<CardEntry> cards =
            new List<CardEntry>();

        private int selectedIndex;
        private int previousSelectedIndex;
        private bool initialized;
        private Coroutine movementCoroutine;

        public int SelectedIndex => selectedIndex;

        public ExpeditionAsset SelectedExpedition
        {
            get
            {
                if (selectedIndex < 0 || selectedIndex >= cards.Count)
                    return null;

                return cards[selectedIndex].Asset;
            }
        }

        public int ExpeditionCount =>
            expeditionCatalog != null
                ? expeditionCatalog.Count
                : 0;

        public event Action<int> SelectionChanged;

        public event Action TransitionStarted;

        private sealed class CardEntry
        {
            public ExpeditionAsset Asset;
            public ExpeditionCarouselCardView View;
        }

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (initialized)
                return;

            if (expeditionCatalog == null)
            {
                Debug.LogError(
                    "[ExpeditionCarouselController] Expedition catalog is missing.",
                    this);
                return;
            }

            if (cardPrefab == null)
            {
                Debug.LogError(
                    "[ExpeditionCarouselController] Card prefab is missing.",
                    this);
                return;
            }

            if (cardContainer == null)
            {
                Debug.LogError(
                    "[ExpeditionCarouselController] Card container is missing.",
                    this);
                return;
            }

            if (expeditionCatalog.Count == 0)
            {
                Debug.LogWarning(
                    "[ExpeditionCarouselController] Catalog contains no expeditions.",
                    this);
                return;
            }

            for (int i = 0; i < expeditionCatalog.Count; i++)
            {
                ExpeditionAsset asset =
                    expeditionCatalog.GetExpedition(i);

                if (asset == null ||
                    asset.Expedition == null ||
                    asset.Expedition.Presentation == null)
                {
                    Debug.LogWarning(
                        "[ExpeditionCarouselController] Skipping invalid expedition at index "
                        + i + ".",
                        this);
                    continue;
                }

                ExpeditionCarouselCardView view =
                    Instantiate(cardPrefab, cardContainer, false);

                RectTransform rect = view.RectTransform;
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(400f, 440f);

                view.SetExpeditionImage(
                    asset.Expedition.Presentation.Image);

                cards.Add(new CardEntry
                {
                    Asset = asset,
                    View = view
                });
            }

            if (cards.Count == 0)
            {
                Debug.LogWarning(
                    "[ExpeditionCarouselController] No valid expedition cards were created.",
                    this);
                return;
            }

            selectedIndex = 0;
            previousSelectedIndex = selectedIndex;
            initialized = true;

            ApplyLayout(false);

            // Sincronizar el orden visual inicial.
            if (cards[selectedIndex].View != null)
            {
                cards[selectedIndex].View.transform.SetAsLastSibling();
            }

            previousSelectedIndex = selectedIndex;
        }

        public void ShowPrevious()
        {
            SelectIndex(selectedIndex - 1);
        }

        public void ShowNext()
        {
            SelectIndex(selectedIndex + 1);
        }

        public bool SelectIndex(int index)
        {
            if (!initialized)
                return false;

            if (index < 0 || index >= cards.Count)
                return false;

            if (index == selectedIndex)
                return true;

            previousSelectedIndex = selectedIndex;
            selectedIndex = index;

            TransitionStarted?.Invoke();

            ApplyLayout(true);

            return true;
        }

        public ExpeditionAsset GetSelectedExpedition()
        {
            if (!initialized ||
                selectedIndex < 0 ||
                selectedIndex >= cards.Count)
            {
                return null;
            }

            return cards[selectedIndex].Asset;
        }

        private void ApplyLayout(bool animate)
        {
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
                movementCoroutine = null;
            }

            movementCoroutine = StartCoroutine(
                ApplyLayoutRoutine(animate));
        }


        private IEnumerator ApplyLayoutRoutine(bool animate)
        {
            float duration = animate
                ? Mathf.Max(0f, transitionDuration)
                : 0f;

            // Sin animación: inicializar todas las cartas,
            // incluidas las que empiezan fuera de la zona visible.
            if (!animate || duration <= 0f)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    CardEntry entry = cards[i];

                    if (entry.View == null)
                        continue;

                    int offset = i - selectedIndex;
                    bool visible = Mathf.Abs(offset) <= 1;

                    RectTransform rect = entry.View.RectTransform;

                    float targetX = offset * horizontalSpacing;
                    float targetScale = offset == 0
                        ? 1f
                        : sideCardScale;

                    float targetAngle = offset == 0
                        ? 0f
                        : sideCardAngle;

                    // Inicializar la transformación antes de mostrar
                    // la carta o dejarla oculta.
                    rect.anchoredPosition = new Vector2(targetX, 0f);
                    rect.localScale = Vector3.one * targetScale;
                    entry.View.SetVisualRotation(targetAngle);

                    entry.View.gameObject.SetActive(visible);
                }

                if (selectedIndex >= 0 &&
                    selectedIndex < cards.Count &&
                    cards[selectedIndex].View != null)
                {
                    cards[selectedIndex].View.transform.SetAsLastSibling();
                }

                movementCoroutine = null;
                yield break;
            }

            // La carta que sale comienza delante.
            if (previousSelectedIndex >= 0 &&
                previousSelectedIndex < cards.Count &&
                cards[previousSelectedIndex].View != null)
            {
                cards[previousSelectedIndex].View.transform.SetAsLastSibling();
            }

            // Preparar las cartas para la transición.
            for (int i = 0; i < cards.Count; i++)
            {
                CardEntry entry = cards[i];

                if (entry.View == null)
                    continue;

                int oldOffset = i - previousSelectedIndex;
                int newOffset = i - selectedIndex;

                bool wasVisible = Mathf.Abs(oldOffset) <= 1;
                bool shouldBeVisible = Mathf.Abs(newOffset) <= 1;

                if (!shouldBeVisible)
                {
                    entry.View.gameObject.SetActive(false);
                    continue;
                }

                RectTransform rect = entry.View.RectTransform;

                // Si la carta entra desde fuera, colocarla primero
                // en su posición del diseño anterior.
                if (!wasVisible)
                {
                    float oldX = oldOffset * horizontalSpacing;

                    float oldScale = oldOffset == 0
                        ? 1f
                        : sideCardScale;

                    float oldAngle = oldOffset == 0
                        ? 0f
                        : sideCardAngle;

                    rect.anchoredPosition = new Vector2(oldX, 0f);
                    rect.localScale = Vector3.one * oldScale;
                    entry.View.SetVisualRotation(oldAngle);
                }

                entry.View.gameObject.SetActive(true);

                float targetAngle = newOffset == 0
                    ? 0f
                    : sideCardAngle;

                entry.View.AnimateRotation(targetAngle, duration);
            }

            List<RectTransform> movingRects =
                new List<RectTransform>();

            List<Vector2> startPositions =
                new List<Vector2>();

            List<Vector2> targetPositions =
                new List<Vector2>();

            List<Vector3> startScales =
                new List<Vector3>();

            List<Vector3> targetScales =
                new List<Vector3>();

            for (int i = 0; i < cards.Count; i++)
            {
                CardEntry entry = cards[i];

                if (entry.View == null ||
                    !entry.View.gameObject.activeSelf)
                {
                    continue;
                }

                RectTransform rect = entry.View.RectTransform;
                int offset = i - selectedIndex;

                movingRects.Add(rect);
                startPositions.Add(rect.anchoredPosition);

                targetPositions.Add(
                    new Vector2(offset * horizontalSpacing, 0f));

                startScales.Add(rect.localScale);

                float targetScale = offset == 0
                    ? 1f
                    : sideCardScale;

                targetScales.Add(Vector3.one * targetScale);
            }

            float elapsed = 0f;
            bool orderChanged = false;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = Mathf.SmoothStep(0f, 1f, t);

                // Cambiar el orden una sola vez, a mitad del giro.
                if (!orderChanged && t >= 0.5f)
                {
                    if (selectedIndex >= 0 &&
                        selectedIndex < cards.Count &&
                        cards[selectedIndex].View != null)
                    {
                        cards[selectedIndex].View.transform.SetAsLastSibling();
                    }

                    orderChanged = true;
                }

                for (int i = 0; i < movingRects.Count; i++)
                {
                    if (movingRects[i] == null)
                        continue;

                    movingRects[i].anchoredPosition = Vector2.Lerp(
                        startPositions[i],
                        targetPositions[i],
                        easedT);

                    movingRects[i].localScale = Vector3.Lerp(
                        startScales[i],
                        targetScales[i],
                        easedT);
                }

                yield return null;
            }

            // Asegurar los valores finales exactos.
            for (int i = 0; i < movingRects.Count; i++)
            {
                if (movingRects[i] == null)
                    continue;

                movingRects[i].anchoredPosition = targetPositions[i];
                movingRects[i].localScale = targetScales[i];
            }

            if (selectedIndex >= 0 &&
                selectedIndex < cards.Count &&
                cards[selectedIndex].View != null)
            {
                cards[selectedIndex].View.transform.SetAsLastSibling();
            }

            movementCoroutine = null;

            SelectionChanged?.Invoke(selectedIndex);
        }

        private void OnDestroy()
        {
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
                movementCoroutine = null;
            }
        }
    }
}