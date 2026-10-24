
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Skill Audio Definition")]
public class SkillAudioDefinition : ScriptableObject
{
    [Header("Impact")]
    [SerializeField] private AudioClip impactClip;

    [SerializeField, Range(0f, 1f)]
    private float impactVolume = 1f;

    public AudioClip ImpactClip => impactClip;
    public float ImpactVolume => impactVolume;

    private void OnValidate()
    {
        impactVolume = Mathf.Clamp01(impactVolume);
    }
}
