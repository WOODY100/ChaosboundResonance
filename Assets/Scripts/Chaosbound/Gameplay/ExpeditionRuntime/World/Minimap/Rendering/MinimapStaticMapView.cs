using System;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Rendering
{
    /// <summary>
    /// Unity UI view responsible for displaying the
    /// generated static minimap texture.
    ///
    /// This component does not generate cartographic data.
    /// It only presents a Texture2D produced by the
    /// MinimapStaticMapRenderer.
    /// </summary>
    public sealed class MinimapStaticMapView :
        MonoBehaviour
    {
        [Header("View")]
        [SerializeField]
        private RawImage targetImage;

        private Texture2D currentTexture;

        /// <summary>
        /// Displays the supplied static minimap texture.
        /// </summary>
        public void SetTexture(
            Texture2D texture)
        {
            if (texture == null)
            {
                Clear();
                return;
            }

            ReleaseCurrentTexture();

            currentTexture =
                texture;

            targetImage.texture =
                currentTexture;
        }

        /// <summary>
        /// Clears the current minimap texture.
        /// </summary>
        public void Clear()
        {
            if (targetImage != null)
            {
                targetImage.texture =
                    null;
            }

            ReleaseCurrentTexture();
        }

        private void Awake()
        {
            if (targetImage == null)
            {
                throw new InvalidOperationException(
                    "MinimapStaticMapView requires a RawImage target.");
            }
        }

        private void OnDestroy()
        {
            ReleaseCurrentTexture();
        }

        private void ReleaseCurrentTexture()
        {
            if (currentTexture == null)
                return;

            DestroyTexture(
                currentTexture);

            currentTexture =
                null;
        }

        private void DestroyTexture(
            Texture2D texture)
        {
            if (texture == null)
                return;

            if (Application.isPlaying)
            {
                Destroy(texture);
            }
            else
            {
                DestroyImmediate(texture);
            }
        }
    }
}