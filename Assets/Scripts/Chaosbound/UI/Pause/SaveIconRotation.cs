using UnityEngine;

public sealed class SaveIconRotation : MonoBehaviour
{
    [SerializeField, Min(0f)]
    private float rotationSpeed = 180f;

    private void Update()
    {
        transform.Rotate(
            0f,
            0f,
            -rotationSpeed * Time.unscaledDeltaTime,
            Space.Self
        );
    }
}