
using UnityEngine;

[DefaultExecutionOrder(100)]
public sealed class PlayerCloakMotion : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform playerRoot;

    [Tooltip("Bones ordered from top to bottom. Do not include the root Bone.")]
    [SerializeField] private Transform[] cloakBones;

    [Header("Forward / Backward Motion")]
    [SerializeField] private float movementAngle = 12f;
    [SerializeField] private float dashAngle = 20f;

    [Header("Turning")]
    [SerializeField] private float turningAngle = 8f;

    [Header("Response")]
    [SerializeField] private float smoothSpeed = 7f;
    [SerializeField] private float returnSpeed = 5f;

    [Header("Bone Distribution")]
    [SerializeField]
    private AnimationCurve boneInfluence =
        new AnimationCurve(
            new Keyframe(0f, 0.15f),
            new Keyframe(0.35f, 0.4f),
            new Keyframe(0.7f, 0.75f),
            new Keyframe(1f, 1f));

    [Header("Local Rotation Axes")]
    [SerializeField] private Vector3 bendAxis = Vector3.right;
    [SerializeField] private Vector3 swayAxis = Vector3.forward;

    private Quaternion[] initialRotations;
    private float currentMovementAngle;
    private float currentTurningAngle;
    private float previousYaw;
    private bool initialized;

    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponentInParent<PlayerController>();

        if (playerRoot == null && playerController != null)
            playerRoot = playerController.transform;

        if (cloakBones == null || cloakBones.Length == 0)
        {
            Debug.LogWarning(
                "PlayerCloakMotion: Assign the cloak bones in the Inspector.",
                this);
            enabled = false;
            return;
        }

        initialRotations = new Quaternion[cloakBones.Length];

        for (int i = 0; i < cloakBones.Length; i++)
        {
            if (cloakBones[i] != null)
                initialRotations[i] = cloakBones[i].localRotation;
        }

        if (playerRoot != null)
            previousYaw = playerRoot.eulerAngles.y;

        initialized = true;
    }

    private void LateUpdate()
    {
        if (!initialized ||
            playerController == null ||
            playerRoot == null)
            return;

        float deltaTime = Mathf.Max(Time.deltaTime, 0.0001f);

        Vector3 localVelocity =
            playerRoot.InverseTransformDirection(
                playerController.Velocity);

        float forwardSpeed = localVelocity.z;

        float targetMovementAngle =
            -Mathf.Clamp(forwardSpeed / 6f, -1f, 1f)
            * movementAngle;

        if (playerController.IsDashing)
            targetMovementAngle =
                -Mathf.Clamp(forwardSpeed / 6f, -1f, 1f)
                * dashAngle;

        float yaw = playerRoot.eulerAngles.y;

        float yawDelta = Mathf.DeltaAngle(previousYaw, yaw);
        float turnRate = yawDelta / deltaTime;

        previousYaw = yaw;

        float targetTurningAngle =
            -Mathf.Clamp(turnRate / 180f, -1f, 1f)
            * turningAngle;

        float response =
            Mathf.Abs(forwardSpeed) > 0.1f
                ? smoothSpeed
                : returnSpeed;

        currentMovementAngle = Mathf.Lerp(
            currentMovementAngle,
            targetMovementAngle,
            response * deltaTime);

        currentTurningAngle = Mathf.Lerp(
            currentTurningAngle,
            targetTurningAngle,
            response * deltaTime);

        Vector3 safeBendAxis =
            bendAxis.sqrMagnitude > 0.001f
                ? bendAxis.normalized
                : Vector3.right;

        Vector3 safeSwayAxis =
            swayAxis.sqrMagnitude > 0.001f
                ? swayAxis.normalized
                : Vector3.forward;

        for (int i = 0; i < cloakBones.Length; i++)
        {
            Transform bone = cloakBones[i];

            if (bone == null)
                continue;

            float normalizedIndex =
                cloakBones.Length <= 1
                    ? 1f
                    : (float)i / (cloakBones.Length - 1);

            float influence =
                boneInfluence.Evaluate(normalizedIndex);

            Quaternion bend =
                Quaternion.AngleAxis(
                    currentMovementAngle * influence,
                    safeBendAxis);

            Quaternion sway =
                Quaternion.AngleAxis(
                    currentTurningAngle * influence,
                    safeSwayAxis);

            bone.localRotation =
                initialRotations[i] * bend * sway;
        }
    }
}
