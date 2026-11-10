using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Save
{
    [Serializable]
    public sealed class SaveSanctuaryPlayerData
    {
        public float PositionX;
        public float PositionY;
        public float PositionZ;

        public float RotationX;
        public float RotationY;
        public float RotationZ;
        public float RotationW = 1f;

        public Vector3 Position =>
            new Vector3(PositionX, PositionY, PositionZ);

        public Quaternion Rotation =>
            new Quaternion(RotationX, RotationY, RotationZ, RotationW);

        public static SaveSanctuaryPlayerData Capture(Transform target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            Vector3 position = target.position;
            Quaternion rotation = target.rotation;

            return new SaveSanctuaryPlayerData
            {
                PositionX = position.x,
                PositionY = position.y,
                PositionZ = position.z,
                RotationX = rotation.x,
                RotationY = rotation.y,
                RotationZ = rotation.z,
                RotationW = rotation.w
            };
        }

        public void Apply(Transform target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.SetPositionAndRotation(Position, Rotation);
        }
    }
}
