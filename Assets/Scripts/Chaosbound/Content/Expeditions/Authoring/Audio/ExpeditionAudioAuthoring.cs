
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Audio
{
    [Serializable]
    public sealed class ExpeditionAudioAuthoring
    {
        [Header("Music Tracks")]

        [SerializeField]
        private List<ExpeditionMusicTrackAuthoring> m_musicTracks =
            new List<ExpeditionMusicTrackAuthoring>();

        [Header("Ambience")]

        [SerializeField]
        private AudioClip m_ambienceClip;

        [SerializeField, Range(0f, 1f)]
        private float m_ambienceVolume = 1f;

        public IReadOnlyList<ExpeditionMusicTrackAuthoring> MusicTracks =>
            m_musicTracks;

        public AudioClip AmbienceClip => m_ambienceClip;

        public float AmbienceVolume => m_ambienceVolume;

        private void OnValidate()
        {
            m_ambienceVolume = Mathf.Clamp01(m_ambienceVolume);

            if (m_musicTracks == null)
            {
                m_musicTracks =
                    new List<ExpeditionMusicTrackAuthoring>();
            }
        }
    }
}
