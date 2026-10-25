using UnityEngine;

public enum PersistentZonePatternType
{
    SingleZone,
    MultiZone,
    Ring,
    Line
}

[CreateAssetMenu(
    menuName = "Skills/Patterns/Persistent Zone Pattern",
    fileName = "PersistentZonePattern")]
public class PersistentZonePatternDefinition
    : SkillExecutionPatternDefinition
{
    [Header("Pattern")]
    public PersistentZonePatternType Pattern =
        PersistentZonePatternType.MultiZone;

    [Header("Follow Player")]
    [Tooltip(
        "When enabled, SingleZone and Ring formations follow the player.")]
    public bool FollowPlayer = false;

    [Header("Forward Placement")]
    [Min(0f)]
    [Tooltip(
        "Distance forward from the player where the line begins.")]
    public float ForwardOffset = 1f;

    [Header("Ring")]
    [Min(0.1f)]
    [Tooltip(
        "Radius of the ring formed by the persistent zones.")]
    public float RingRadius = 3f;

    [Header("Line")]
    [Min(0f)]
    [Tooltip(
        "Length of the line extending forward from its starting position.")]
    public float LineLength = 6f;

    [Header("Timing")]
    [Min(0f)]
    [Tooltip(
        "Delay between spawning consecutive zones, in seconds.")]
    public float SpawnDelay = 0.08f;

    private void OnValidate()
    {
        ForwardOffset = Mathf.Max(0f, ForwardOffset);
        RingRadius = Mathf.Max(0.1f, RingRadius);
        LineLength = Mathf.Max(0f, LineLength);
        SpawnDelay = Mathf.Max(0f, SpawnDelay);
    }
}