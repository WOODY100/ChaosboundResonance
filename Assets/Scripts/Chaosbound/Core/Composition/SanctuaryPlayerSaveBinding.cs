using Chaosbound.Gameplay.Save;
using UnityEngine;

namespace Chaosbound.Core.Composition
{
    [DisallowMultipleComponent]
    public sealed class SanctuaryPlayerSaveBinding : MonoBehaviour
    {
        private BootstrapContext bootstrapContext;
        private Vector3 initialPosition;
        private Quaternion initialRotation;

        private void Awake()
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
        }

        private void OnEnable()
        {
            bootstrapContext = BootstrapContext.Current;

            if (bootstrapContext != null)
                bootstrapContext.RegisterSanctuaryPlayer(this);
        }

        private void OnDisable()
        {
            if (bootstrapContext != null)
                bootstrapContext.UnregisterSanctuaryPlayer(this);

            bootstrapContext = null;
        }

        public SaveSanctuaryPlayerData Capture()
        {
            return SaveSanctuaryPlayerData.Capture(transform);
        }

        public void Apply(SaveSanctuaryPlayerData data)
        {
            if (data != null)
                data.Apply(transform);
            else
                ResetToInitialTransform();
        }

        public void ResetToInitialTransform()
        {
            transform.SetPositionAndRotation(
                initialPosition,
                initialRotation);
        }
    }
}
