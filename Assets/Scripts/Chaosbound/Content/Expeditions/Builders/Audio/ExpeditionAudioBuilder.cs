using Chaosbound.Content.Expeditions.Authoring.Audio;
using Chaosbound.Content.Expeditions.Definitions.Audio;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Builders.Audio
{
    public static class ExpeditionAudioBuilder
    {
        public static ExpeditionAudioDefinition Build(
            ExpeditionAudioAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            List<ExpeditionMusicTrackDefinition> tracks =
                new List<ExpeditionMusicTrackDefinition>();

            IReadOnlyList<ExpeditionMusicTrackAuthoring> sourceTracks =
                authoring.MusicTracks;

            if (sourceTracks != null)
            {
                for (int i = 0; i < sourceTracks.Count; i++)
                {
                    ExpeditionMusicTrackAuthoring track =
                        sourceTracks[i];

                    if (track == null || track.Clip == null)
                        continue;

                    tracks.Add(
                        new ExpeditionMusicTrackDefinition(
                            track.TrackId,
                            track.Clip,
                            track.Volume));
                }
            }

            return new ExpeditionAudioDefinition(
                tracks,
                authoring.AmbienceClip,
                authoring.AmbienceVolume);
        }
    }
}