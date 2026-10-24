
using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Audio
{
    [Serializable]
    public sealed class ExpeditionMusicTrackAuthoring
    {
        [SerializeField]
        private string m_trackId = "expedition_main";

        [SerializeField]
        private AudioClip m_clip;

        [SerializeField, Range(0f, 1f)]
        private float m_volume = 1f;

        public string TrackId => m_trackId;

        public AudioClip Clip => m_clip;

        public float Volume => m_volume;

        private void OnValidate()
        {
            m_volume = Mathf.Clamp01(m_volume);

            if (string.IsNullOrWhiteSpace(m_trackId))
            {
                m_trackId = "expedition_main";
            }
        }
    }
}
