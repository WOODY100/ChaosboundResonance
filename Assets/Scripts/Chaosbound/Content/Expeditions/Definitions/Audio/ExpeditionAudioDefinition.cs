
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Definitions.Audio
{
    public sealed class ExpeditionMusicTrackDefinition
    {
        public string TrackId { get; }

        public AudioClip Clip { get; }

        public float Volume { get; }

        public ExpeditionMusicTrackDefinition(
            string trackId,
            AudioClip clip,
            float volume)
        {
            if (string.IsNullOrWhiteSpace(trackId))
                throw new ArgumentException(
                    "Track ID cannot be null or empty.",
                    nameof(trackId));

            if (clip == null)
                throw new ArgumentNullException(nameof(clip));

            TrackId = trackId;
            Clip = clip;
            Volume = Mathf.Clamp01(volume);
        }
    }

    public sealed class ExpeditionAudioDefinition
    {
        public IReadOnlyList<ExpeditionMusicTrackDefinition> MusicTracks
        {
            get;
        }

        public AudioClip AmbienceClip { get; }

        public float AmbienceVolume { get; }

        public ExpeditionAudioDefinition(
            IReadOnlyList<ExpeditionMusicTrackDefinition> musicTracks,
            AudioClip ambienceClip,
            float ambienceVolume)
        {
            MusicTracks = musicTracks ??
                throw new ArgumentNullException(nameof(musicTracks));

            AmbienceClip = ambienceClip;
            AmbienceVolume = Mathf.Clamp01(ambienceVolume);
        }
    }
}
