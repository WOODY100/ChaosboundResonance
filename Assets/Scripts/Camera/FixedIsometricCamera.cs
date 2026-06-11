using UnityEngine;

public class FixedNorthCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public float height = 16f;
    public float distance = 11f;
    public float verticalAngle = 55f;

    [Header("Follow")]
    public float smoothTime = 0.12f;

    private Vector3 velocity;

    void LateUpdate()
    {
        if (target == null) return;

        // Rotación fija: mira hacia el norte Z+
        transform.rotation = Quaternion.Euler(verticalAngle, 0f, 0f);

        Vector3 desiredPosition =
            target.position +
            Vector3.up * height +
            Vector3.back * distance;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothTime
        );
    }
}