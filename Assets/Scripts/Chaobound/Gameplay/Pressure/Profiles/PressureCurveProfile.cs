using UnityEngine;

namespace Chaosbound.Gameplay.Pressure.Profiles
{
    /// <summary>
    /// Describes how pressure evolves during an expedition.
    /// This asset is interpreted by the PressureEvaluator.
    /// </summary>
    [CreateAssetMenu(
        fileName = "Pressure Curve Profile",
        menuName = "Chaosbound/Pressure/Curve Profile")]
    public sealed class PressureCurveProfile : ScriptableObject
    {
    }
}