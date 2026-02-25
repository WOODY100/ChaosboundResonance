using UnityEngine;

public class FixedIsometricCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public float height = 20f;
    public float distance = 20f;
    public float verticalAngle = 50f;
    public float horizontalAngle = 45f;

    [Header("Follow")]
    [Range(0f, 1f)]
    public float smoothSpeed = 0.12f;

    private Vector3 velocity;

    void LateUpdate()
    {
        if (target == null)
            return;

        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
        transform.rotation = rotation;

        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);
        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothSpeed
        );
    }
}
