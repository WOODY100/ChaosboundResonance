using System;
using UnityEngine;

namespace Chaosbound.UI.Timeline
{
    [Serializable]
    public sealed class TimelineIconEntry
    {
        [SerializeField]
        private string iconId;

        [SerializeField]
        private Sprite sprite;

        public string IconId =>
            iconId;

        public Sprite Sprite =>
            sprite;
    }
}