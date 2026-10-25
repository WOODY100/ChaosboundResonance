using UnityEngine;

public enum ProjectilePatternType
{
    TargetNearest,
    Fan,
    Radial,
    RandomSpread,
    FixedDirection
}

[CreateAssetMenu(
    menuName = "Skills/Patterns/Projectile Pattern",
    fileName = "ProjectilePattern")]
public class ProjectilePatternDefinition
    : SkillExecutionPatternDefinition
{
    [Header("Pattern")]
    public ProjectilePatternType Pattern =
        ProjectilePatternType.TargetNearest;

    [Header("Aiming")]
    [Tooltip(
        "When enabled, aims toward the nearest enemy. " +
        "Otherwise uses the owner's forward direction.")]
    public bool AimAtNearestTarget = true;

    [Header("Fan")]
    [Min(0f)]
    [Tooltip("Total angle covered by the fan, in degrees.")]
    public float SpreadAngle = 45f;

    [Header("Random Spread")]
    [Min(0f)]
    [Tooltip("Maximum angular deviation in each direction.")]
    public float RandomSpreadAngle = 15f;

    [Header("Fixed Direction")]
    [Tooltip("Direction relative to the owner's local rotation.")]
    public Vector3 LocalDirection = Vector3.forward;

    private void OnValidate()
    {
        SpreadAngle = Mathf.Clamp(
            SpreadAngle, 0f, 360f);

        RandomSpreadAngle = Mathf.Clamp(
            RandomSpreadAngle, 0f, 180f);

        if (LocalDirection.sqrMagnitude < 0.0001f)
            LocalDirection = Vector3.forward;
    }
}