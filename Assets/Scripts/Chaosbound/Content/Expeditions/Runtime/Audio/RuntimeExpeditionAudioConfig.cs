using Chaosbound.Content.Expeditions.Definitions.Audio;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Audio
{
    public sealed class RuntimeMusicTrackConfig
    {
        public string TrackId { get; }

        public UnityEngine.AudioClip Clip { get; }

        public float Volume { get; }

        public RuntimeMusicTrackConfig(
            string trackId,
            UnityEngine.AudioClip clip,
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
            Volume = UnityEngine.Mathf.Clamp01(volume);
        }
    }

    public sealed class RuntimeExpeditionAudioConfig
    {
        public IReadOnlyList<RuntimeMusicTrackConfig> MusicTracks
        {
            get;
        }

        public UnityEngine.AudioClip AmbienceClip { get; }

        public float AmbienceVolume { get; }

        public RuntimeExpeditionAudioConfig(
            ExpeditionAudioDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            List<RuntimeMusicTrackConfig> tracks =
                new List<RuntimeMusicTrackConfig>();

            for (int i = 0; i < definition.MusicTracks.Count; i++)
            {
                ExpeditionMusicTrackDefinition track =
                    definition.MusicTracks[i];

                if (track == null)
                    continue;

                tracks.Add(
                    new RuntimeMusicTrackConfig(
                        track.TrackId,
                        track.Clip,
                        track.Volume));
            }

            MusicTracks = tracks.AsReadOnly();
            AmbienceClip = definition.AmbienceClip;
            AmbienceVolume = UnityEngine.Mathf.Clamp01(
                definition.AmbienceVolume);
        }
    }
}