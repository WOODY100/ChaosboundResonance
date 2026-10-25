using UnityEngine;

public enum AreaStrikePatternType
{
    SinglePoint,
    MultiPoint,
    Ring,
    Line
}

[CreateAssetMenu(
    menuName = "Skills/Patterns/Area Strike Pattern",
    fileName = "AreaStrikePattern")]
public class AreaStrikePatternDefinition :
    SkillExecutionPatternDefinition
{
    [Header("Pattern")]
    public AreaStrikePatternType Pattern =
        AreaStrikePatternType.MultiPoint;

    [Header("Forward Placement")]
    [Min(0f)]
    [Tooltip("Distance in front of the player where the pattern begins or is centered.")]
    public float ForwardOffset = 3f;

    [Header("Area")]
    [Min(0f)]
    [Tooltip("Radius of the random circular area used by Ring.")]
    public float AreaRadius = 2.5f;

    [Header("Line")]
    [Min(0f)]
    [Tooltip("Length of the line extending forward from its starting position.")]
    public float LineLength = 6f;

    [Header("Timing")]
    [Min(0f)]
    [Tooltip("Delay between spawning consecutive strikes, in seconds.")]
    public float SpawnDelay = 0.12f;

    private void OnValidate()
    {
        ForwardOffset = Mathf.Max(0f, ForwardOffset);
        AreaRadius = Mathf.Max(0f, AreaRadius);
        LineLength = Mathf.Max(0f, LineLength);
        SpawnDelay = Mathf.Max(0f, SpawnDelay);
    }
}