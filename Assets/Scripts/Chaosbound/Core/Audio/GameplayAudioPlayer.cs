using UnityEngine;
using UnityEngine.Audio;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public sealed class GameplayAudioPlayer : MonoBehaviour
{
    public static GameplayAudioPlayer Instance { get; private set; }

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioMixerGroup effectsMixerGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "Another GameplayAudioPlayer already exists.",
                this);

            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        if (effectsMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup =
                effectsMixerGroup;
        }
    }

    public void PlayOneShot(
        AudioClip clip,
        float volume = 1f)
    {
        if (clip == null)
            return;

        if (audioSource == null)
            return;

        audioSource.PlayOneShot(
            clip,
            Mathf.Clamp01(volume));
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}