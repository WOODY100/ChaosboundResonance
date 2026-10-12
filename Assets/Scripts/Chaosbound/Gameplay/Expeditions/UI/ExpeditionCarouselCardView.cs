
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.Expeditions.UI
{
    [DisallowMultipleComponent]
    public sealed class ExpeditionCarouselCardView : MonoBehaviour
    {
        [Header("Card Faces")]
        [SerializeField] private GameObject frontFace;
        [SerializeField] private GameObject backFace;

        [Header("Front Presentation")]
        [SerializeField] private Image expeditionArtwork;

        private RectTransform rectTransform;
        private Coroutine rotationCoroutine;

        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = GetComponent<RectTransform>();

                return rectTransform;
            }
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            SetFrontVisible(true);
        }

        public void SetExpeditionImage(Sprite sprite)
        {
            if (expeditionArtwork == null)
                return;

            expeditionArtwork.sprite = sprite;
            expeditionArtwork.enabled = sprite != null;
        }

        public void SetFrontVisible(bool showFront)
        {
            if (frontFace != null)
                frontFace.SetActive(showFront);

            if (backFace != null)
                backFace.SetActive(!showFront);
        }

        public void SetVisualRotation(float yAngle)
        {
            if (RectTransform == null)
                return;

            Vector3 rotation = RectTransform.localEulerAngles;
            rotation.y = yAngle;
            RectTransform.localEulerAngles = rotation;

            UpdateVisibleFace(yAngle);
        }

        public void AnimateRotation(
            float targetYAngle,
            float duration)
        {
            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
                rotationCoroutine = null;
            }

            rotationCoroutine = StartCoroutine(
                AnimateRotationRoutine(targetYAngle, duration));
        }

        private IEnumerator AnimateRotationRoutine(
            float targetYAngle,
            float duration)
        {
            float startAngle = RectTransform.localEulerAngles.y;
            float elapsed = 0f;

            if (duration <= 0f)
            {
                SetVisualRotation(targetYAngle);
                rotationCoroutine = null;
                yield break;
            }

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = Mathf.SmoothStep(0f, 1f, t);

                SetVisualRotation(
                    Mathf.Lerp(startAngle, targetYAngle, easedT));

                yield return null;
            }

            SetVisualRotation(targetYAngle);
            rotationCoroutine = null;
        }

        private void UpdateVisibleFace(float yAngle)
        {
            float angle = Mathf.Repeat(yAngle, 360f);

            bool showFront = angle < 90f || angle >= 270f;
            SetFrontVisible(showFront);
        }

        private void OnDisable()
        {
            if (rotationCoroutine == null)
                return;

            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }
    }
}
