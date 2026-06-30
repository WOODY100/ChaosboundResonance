using UnityEngine;

public class FixedNorthCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public float height = 13f;
    public float distance = 10f;
    public float verticalAngle = 58f;

    [Tooltip("0 = Norte, 45 = Noreste, 90 = Este, 135 = Sureste")]
    public float horizontalAngle = 45f;

    [Header("Screen Framing")]
    public float forwardOffset = -2.5f;
    public float sideOffset = 0f;

    [Header("Follow")]
    public float smoothTime = 0.12f;

    private Vector3 velocity;

    void LateUpdate()
    {
        if (target == null) return;

        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
        transform.rotation = rotation;

        Vector3 forwardFlat = rotation * Vector3.forward;
        forwardFlat.y = 0f;
        forwardFlat.Normalize();

        Vector3 rightFlat = rotation * Vector3.right;
        rightFlat.y = 0f;
        rightFlat.Normalize();

        Vector3 focusPoint =
            target.position
            + forwardFlat * forwardOffset
            + rightFlat * sideOffset;

        Vector3 desiredPosition =
            focusPoint
            - forwardFlat * distance
            + Vector3.up * height;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothTime
        );
    }
}