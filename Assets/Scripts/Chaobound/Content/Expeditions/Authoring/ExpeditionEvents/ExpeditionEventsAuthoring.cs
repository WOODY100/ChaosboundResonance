using UnityEngine;
using Chaosbound.Shared.Authoring;

namespace Chaosbound.Content.Expeditions.Authoring.ExpeditionEvents
{
    [System.Serializable]
    public sealed class ExpeditionEventsAuthoring
    {
        [SerializeField]
        private ContentReferenceAuthoring[] content;

        public ContentReferenceAuthoring[] Content => content;
    }
}