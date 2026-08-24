using Chaosbound.Content.Portal.Exit;
using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Completion
{
    /// <summary>
    /// Unity authoring data for expedition completion.
    /// </summary>
    [Serializable]
    public sealed class CompletionAuthoring
    {
        [SerializeField]
        private string m_DomainId;

        [SerializeField]
        private string m_EventId;

        [SerializeField]
        private ExitPortalData m_ExitPortal;

        public string DomainId =>
            m_DomainId;

        public string EventId =>
            m_EventId;

        public ExitPortalData ExitPortal =>
            m_ExitPortal;
    }
}