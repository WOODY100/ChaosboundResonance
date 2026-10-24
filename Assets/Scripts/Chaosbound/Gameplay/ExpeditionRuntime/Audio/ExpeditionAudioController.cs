using Chaosbound.Content.Expeditions.Runtime.Audio;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Audio
{
    public sealed class ExpeditionAudioController : MonoBehaviour
    {
        [Header("Audio Mixer")]
        [SerializeField]
        private AudioMixerGroup musicGroup;

        [SerializeField]
        private AudioMixerGroup ambienceGroup;

        [Header("Audio Sources")]
        [SerializeField]
        private AudioSource musicSource;

        [SerializeField]
        private AudioSource ambienceSource;

        private RuntimeExpeditionAudioConfig configuration;
        private bool isInitialized;

        public void Initialize(
            RuntimeExpeditionAudioConfig audioConfig)
        {
            if (audioConfig == null)
                throw new ArgumentNullException(nameof(audioConfig));

            if (isInitialized)
                StopAudio();

            ValidateReferences();

            configuration = audioConfig;

            ConfigureSource(musicSource, musicGroup);
            ConfigureSource(ambienceSource, ambienceGroup);

            PlayMusic();
            PlayAmbience();

            isInitialized = true;
        }

        public void StopAudio()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
                musicSource.clip = null;
            }

            if (ambienceSource != null)
            {
                ambienceSource.Stop();
                ambienceSource.clip = null;
            }

            configuration = null;
            isInitialized = false;
        }

        private void PlayMusic()
        {
            if (configuration.MusicTracks == null ||
                configuration.MusicTracks.Count == 0)
                return;

            RuntimeMusicTrackConfig track =
                configuration.MusicTracks[0];

            if (track == null || track.Clip == null)
                return;

            musicSource.clip = track.Clip;
            musicSource.volume = track.Volume;
            musicSource.loop = true;
            musicSource.Play();
        }

        private void PlayAmbience()
        {
            if (configuration.AmbienceClip == null)
                return;

            ambienceSource.clip = configuration.AmbienceClip;
            ambienceSource.volume = configuration.AmbienceVolume;
            ambienceSource.loop = true;
            ambienceSource.Play();
        }

        private void ConfigureSource(
            AudioSource source,
            AudioMixerGroup outputGroup)
        {
            source.Stop();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = outputGroup;
            source.spatialBlend = 0f;
            source.loop = true;
        }

        private void ValidateReferences()
        {
            if (musicSource == null)
                throw new InvalidOperationException(
                    "Music AudioSource is not assigned.");

            if (ambienceSource == null)
                throw new InvalidOperationException(
                    "Ambience AudioSource is not assigned.");

            if (musicGroup == null)
                throw new InvalidOperationException(
                    "Music AudioMixerGroup is not assigned.");

            if (ambienceGroup == null)
                throw new InvalidOperationException(
                    "Ambience AudioMixerGroup is not assigned.");
        }

        private void OnDestroy()
        {
            StopAudio();
        }
    }
}