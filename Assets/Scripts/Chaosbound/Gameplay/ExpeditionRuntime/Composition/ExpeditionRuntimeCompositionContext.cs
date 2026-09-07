using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Composition
{
    /// <summary>
    /// Provides the configurable infrastructure dependencies required
    /// to compose the Expedition Runtime.
    /// </summary>
    public sealed class ExpeditionRuntimeCompositionContext
        : MonoBehaviour
    {
        public static ExpeditionRuntimeCompositionContext Current
        {
            get;
            private set;
        }

        //==========================================================
        // Reward / Materializable Content
        //==========================================================

        [Header("Reward / Materializable Content")]

        [SerializeField]
        private MaterializableContentDatabase
            materializableContentDatabase;

        //==========================================================
        // Public Properties
        //==========================================================

        public MaterializableContentDatabase
            MaterializableContentDatabase =>
                materializableContentDatabase;

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
                    "Multiple ExpeditionRuntimeCompositionContext " +
                    "instances were detected.");
            }

            Current = this;
        }

#if UNITY_EDITOR

        //==========================================================
        // Validation
        //==========================================================

        private void OnValidate()
        {
            if (materializableContentDatabase == null)
            {
                Debug.LogWarning(
                    $"{nameof(ExpeditionRuntimeCompositionContext)}: " +
                    $"'{nameof(materializableContentDatabase)}' " +
                    "is not assigned.",
                    this);
            }
        }

#endif
    }
}