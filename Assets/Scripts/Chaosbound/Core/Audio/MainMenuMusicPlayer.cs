using UnityEngine;
using UnityEngine.Audio;

namespace Chaosbound.Core.Audio
{
    public sealed class MainMenuMusicPlayer : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip musicClip;

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixerGroup musicMixerGroup;

        private void Start()
        {
            if (audioSource == null)
            {
                Debug.LogError(
                    "[MainMenuMusicPlayer] AudioSource is not assigned.",
                    this);
                return;
            }

            if (musicClip == null)
            {
                Debug.LogError(
                    "[MainMenuMusicPlayer] Music clip is not assigned.",
                    this);
                return;
            }

            if (musicMixerGroup == null)
            {
                Debug.LogError(
                    "[MainMenuMusicPlayer] Music Mixer Group is not assigned.",
                    this);
                return;
            }

            audioSource.Stop();
            audioSource.clip = musicClip;
            audioSource.outputAudioMixerGroup = musicMixerGroup;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.6f;

            audioSource.Play();
        }

        private void OnDisable()
        {
            if (audioSource != null)
                audioSource.Stop();
        }
    }
}