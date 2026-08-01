using Chaosbound.Gameplay.Pressure.Profiles;
using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Pressure
{
    /// <summary>
    /// Authoring configuration for the expedition pressure settings.
    /// </summary>
    [Serializable]
    public sealed class PressureAuthoring
    {
        [SerializeField]
        private PressureCurveProfile m_curveProfile;

        public PressureCurveProfile CurveProfile
        {
            get { return m_curveProfile; }
        }
    }
}