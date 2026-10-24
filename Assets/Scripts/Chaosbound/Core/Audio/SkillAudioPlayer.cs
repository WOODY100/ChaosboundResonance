using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class SkillAudioPlayer : MonoBehaviour
{
    public static SkillAudioPlayer Instance { get; private set; }

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "Duplicate SkillAudioPlayer detected. Destroying duplicate.",
                this);

            Destroy(gameObject);
            return;
        }

        Instance = this;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.dopplerLevel = 0f;
    }

    public void PlaySkillEvent(SkillAudioDefinition audioDefinition)
    {
        if (audioDefinition == null || audioDefinition.ImpactClip == null)
            return;

        if (audioSource == null)
            return;

        audioSource.PlayOneShot(
            audioDefinition.ImpactClip,
            audioDefinition.ImpactVolume);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}