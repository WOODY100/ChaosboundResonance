using UnityEngine;
using UnityEngine.Audio;

namespace Chaosbound.Core.Audio
{
    public sealed class SanctuaryMusicPlayer : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip musicClip;

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixerGroup musicMixerGroup;

        [Header("Sanctuary Volume")]
        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 0.7f;

        private void Start()
        {
            if (audioSource == null)
            {
                Debug.LogError(
                    "[SanctuaryMusicPlayer] AudioSource is missing.",
                    this);
                return;
            }

            if (musicClip == null)
            {
                Debug.LogError(
                    "[SanctuaryMusicPlayer] Music clip is missing.",
                    this);
                return;
            }

            if (musicMixerGroup == null)
            {
                Debug.LogError(
                    "[SanctuaryMusicPlayer] Music Mixer Group is missing.",
                    this);
                return;
            }

            audioSource.Stop();
            audioSource.clip = musicClip;
            audioSource.outputAudioMixerGroup = musicMixerGroup;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = musicVolume;

            audioSource.Play();
        }

        private void OnDisable()
        {
            if (audioSource != null)
                audioSource.Stop();
        }
    }
}