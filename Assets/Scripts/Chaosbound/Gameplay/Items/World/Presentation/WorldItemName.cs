using TMPro;
using UnityEngine;

namespace Chaosbound.Gameplay.Items.World.Presentation
{
    public sealed class WorldItemName : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text text;

        [Header("Placement")]
        [SerializeField] private float heightOffset = 1.5f;

        [Header("Billboard")]
        [SerializeField] private bool faceCamera = true;

        private Transform cameraTransform;

        private void Awake()
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void LateUpdate()
        {
            if (!faceCamera)
            {
                return;
            }

            if (cameraTransform == null)
            {
                if (Camera.main == null)
                {
                    return;
                }

                cameraTransform = Camera.main.transform;
            }

            transform.rotation = cameraTransform.rotation;
        }

        private void OnEnable()
        {
            transform.localPosition = new Vector3(
                0f,
                heightOffset,
                0f);
        }

        public void SetName(string displayName)
        {
            if (text == null)
            {
                return;
            }

            text.text = displayName ?? string.Empty;
        }

        public void SetColor(Color color)
        {
            if (text == null)
            {
                return;
            }

            text.color = color;
        }

        public void Clear()
        {
            if (text == null)
            {
                return;
            }

            text.text = string.Empty;
        }
    }
}