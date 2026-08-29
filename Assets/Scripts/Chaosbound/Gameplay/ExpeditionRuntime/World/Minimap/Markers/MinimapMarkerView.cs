using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Markers
{
    /// <summary>
    /// Visual representation of a single minimap marker.
    ///
    /// This component owns only the UI representation.
    /// It does not know about world coordinates or gameplay systems.
    /// </summary>
    public sealed class MinimapMarkerView :
        MonoBehaviour
    {
        [Header("View")]

        [SerializeField]
        private Image image;

        [SerializeField]
        private Image offscreenImage;

        public RectTransform RectTransform =>
            transform as RectTransform;

        public void SetSprite(
            Sprite sprite)
        {
            if (image == null)
                return;

            image.sprite =
                sprite;
        }

        public void SetOffscreenSprite(
            Sprite sprite)
        {
            if (offscreenImage == null)
                return;

            offscreenImage.sprite =
                sprite;
        }

        public void SetUseOffscreenImage(
            bool useOffscreenImage)
        {
            if (image != null)
            {
                image.enabled =
                    !useOffscreenImage;
            }

            if (offscreenImage != null)
            {
                offscreenImage.enabled =
                    useOffscreenImage;
            }
        }

        public void SetVisible(
            bool visible)
        {
            gameObject.SetActive(
                visible);
        }

        public void SetPosition(
            Vector2 position)
        {
            RectTransform rectTransform =
                RectTransform;

            if (rectTransform == null)
                return;

            rectTransform.anchoredPosition =
                position;
        }

        public void SetRotation(
            float angleDegrees)
        {
            RectTransform rectTransform =
                RectTransform;

            if (rectTransform == null)
                return;

            rectTransform.localRotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    angleDegrees);
        }
    }
}