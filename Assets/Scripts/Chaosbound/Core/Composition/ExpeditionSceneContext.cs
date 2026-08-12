using System;
using UnityEngine;

namespace Chaosbound.Core.Composition
{
    public sealed class ExpeditionSceneContext : MonoBehaviour
    {
        public static ExpeditionSceneContext Current { get; private set; }

        //==========================================================
        // Player
        //==========================================================

        [Header("Player")]

        [SerializeField] private PlayerHealth player;
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private PlayerExperienceSystem playerExperienceSystem;
        [SerializeField] private PlayerSkillLoadout playerSkillLoadout;

        //==========================================================
        // World
        //==========================================================

        [Header("World")]

        [SerializeField]
        private OpenWorldMapGenerator mapGenerator;

        [SerializeField]
        private OpenWorldDecorationGenerator decorationGenerator;

        //==========================================================
        // Public Properties
        //==========================================================

        public PlayerHealth Player => player;
        public PlayerStats PlayerStats => playerStats;
        public PlayerExperienceSystem PlayerExperienceSystem => playerExperienceSystem;
        public PlayerSkillLoadout PlayerSkillLoadout => playerSkillLoadout;

        public OpenWorldMapGenerator MapGenerator => mapGenerator;
        public OpenWorldDecorationGenerator DecorationGenerator => decorationGenerator;

        //==========================================================
        // Unity
        //==========================================================

        private void Awake()
        {
            RegisterCurrentContext();
        }

        private void OnDestroy()
        {
            if (Current == this)
            {
                Current = null;
            }
        }

        //==========================================================
        // Initialization
        //==========================================================

        private void RegisterCurrentContext()
        {
            if (Current != null && Current != this)
            {
                throw new InvalidOperationException(
                    "Multiple ExpeditionSceneContext instances were detected.");
            }

            Current = this;
        }

#if UNITY_EDITOR

        //==========================================================
        // Validation
        //==========================================================

        private void OnValidate()
        {
            ValidateReference(player, nameof(player));
            ValidateReference(playerStats, nameof(playerStats));
            ValidateReference(playerExperienceSystem, nameof(playerExperienceSystem));
            ValidateReference(playerSkillLoadout, nameof(playerSkillLoadout));
            
            ValidateReference(mapGenerator, nameof(mapGenerator));
            ValidateReference(decorationGenerator, nameof(decorationGenerator));
        }

        private void ValidateReference(
            UnityEngine.Object reference,
            string fieldName)
        {
            if (reference == null)
            {
                Debug.LogWarning(
                    $"{nameof(ExpeditionSceneContext)}: '{fieldName}' is not assigned.",
                    this);
            }
        }

#endif
    }
}