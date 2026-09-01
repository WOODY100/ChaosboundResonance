using UnityEngine;

namespace Chaosbound.Core.GameFlow
{
    [CreateAssetMenu(
        fileName = "GameFlowConfiguration",
        menuName = "Chaosbound/Game Flow/Game Flow Configuration")]
    public sealed class GameFlowConfiguration : ScriptableObject
    {
        //==========================================================
        // Confirmation
        //==========================================================

        [Header("Confirmation")]

        [SerializeField]
        private bool confirmationSimulation = true;

        //==========================================================
        // Dialogue
        //==========================================================

        [Header("Dialogue")]

        [SerializeField]
        private bool dialogueSimulation = false;

        //==========================================================
        // Public Properties
        //==========================================================

        public bool ConfirmationSimulation =>
            confirmationSimulation;

        public bool DialogueSimulation =>
            dialogueSimulation;
    }
}