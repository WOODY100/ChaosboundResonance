using UnityEngine;
using UnityEngine.Audio;

namespace Chaosbound.Core.Audio
{
    public sealed class UIAudioPlayer : MonoBehaviour
    {
        [Header("Audio Source")]
        [SerializeField] private AudioSource audioSource;

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixerGroup uiMixerGroup;

        [Header("UI Sound Effects")]
        [SerializeField] private AudioClip confirmClip;
        [SerializeField] private AudioClip hoverClip;
        [SerializeField] private AudioClip tabSwitchClip;
        [SerializeField] private AudioClip popupClip;
        [SerializeField] private AudioClip itemDestroyClip;
        [SerializeField] private AudioClip expeditionCardSwitchClip;

        private void Awake()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                Debug.LogError(
                    "[UIAudioPlayer] AudioSource is missing.",
                    this);
                enabled = false;
                return;
            }

            if (uiMixerGroup == null)
            {
                Debug.LogError(
                    "[UIAudioPlayer] UI Mixer Group is missing.",
                    this);
                enabled = false;
                return;
            }

            audioSource.outputAudioMixerGroup = uiMixerGroup;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        public void PlayConfirm()
        {
            PlayClip(confirmClip);
        }

        public void PlayHover()
        {
            PlayClip(hoverClip);
        }

        public void PlayTabSwitch()
        {
            PlayClip(tabSwitchClip);
        }

        public void PlayPopup()
        {
            PlayClip(popupClip);
        }

        private void PlayClip(AudioClip clip)
        {
            if (!enabled || audioSource == null || clip == null)
                return;

            audioSource.PlayOneShot(clip);
        }

        public void PlayItemDestroy()
        {
            if (itemDestroyClip == null)
                return;

            audioSource.PlayOneShot(itemDestroyClip);
        }

        public void PlayExpeditionCardSwitch()
        {
            if (expeditionCardSwitchClip == null)
                return;

            audioSource.PlayOneShot(expeditionCardSwitchClip);
        }
    }
}