using UnityEngine;

public enum OrbitalPatternType
{
    Orbit,
    FixedRing,
    ForwardArc
}

[CreateAssetMenu(
    menuName = "Skills/Patterns/Orbital Pattern",
    fileName = "OrbitalPattern")]
public class OrbitalPatternDefinition :
    SkillExecutionPatternDefinition
{
    [Header("Pattern")]
    public OrbitalPatternType Pattern =
        OrbitalPatternType.Orbit;

    [Header("Formation")]
    [Min(0.1f)]
    [Tooltip("Distance from the owner to each orb.")]
    public float OrbitRadius = 2f;

    [Tooltip("Vertical offset of the orbs.")]
    public float OrbitHeight = 1.2f;

    [Header("Orbit")]
    [Min(0f)]
    [Tooltip("Angular rotation speed in degrees per second.")]
    public float AngularSpeed = 180f;

    [Tooltip("Whether orbit speed scales with the owner's Attack Speed.")]
    public bool RotateWithAttackSpeed = true;

    [Header("Forward Arc")]
    [Range(0f, 360f)]
    [Tooltip("Total angle covered by the forward arc.")]
    public float ForwardArcAngle = 120f;

    private void OnValidate()
    {
        OrbitRadius = Mathf.Max(0.1f, OrbitRadius);
        OrbitHeight = Mathf.Clamp(OrbitHeight, -10f, 10f);
        AngularSpeed = Mathf.Max(0f, AngularSpeed);
        ForwardArcAngle = Mathf.Clamp(
            ForwardArcAngle,
            0f,
            360f);
    }
}