using System;
using UnityEngine;
using Chaosbound.Content.Items;
using Chaosbound.Content.Materials;

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
        // Items
        //==========================================================

        [Header("Items")]

        [SerializeField]
        private ItemDatabase
            itemDatabase;

        [SerializeField]
        private ExpeditionRewardItemDatabase
            expeditionRewardItemDatabase;

        //==========================================================
        // Materials
        //==========================================================

        [Header("Materials")]

        [SerializeField]
        private MaterialDatabase
            materialDatabase;

        //==========================================================
        // Public Properties
        //==========================================================

        public MaterializableContentDatabase
            MaterializableContentDatabase =>
                materializableContentDatabase;

        public ItemDatabase
            ItemDatabase =>
                itemDatabase;

        public ExpeditionRewardItemDatabase
            ExpeditionRewardItemDatabase =>
                expeditionRewardItemDatabase;

        public MaterialDatabase
            MaterialDatabase =>
                materialDatabase;

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

            if (itemDatabase == null)
            {
                Debug.LogWarning(
                    $"{nameof(ExpeditionRuntimeCompositionContext)}: " +
                    $"'{nameof(itemDatabase)}' " +
                    "is not assigned.",
                    this);
            }

            if (expeditionRewardItemDatabase == null)
            {
                Debug.LogWarning(
                    $"{nameof(ExpeditionRuntimeCompositionContext)}: " +
                    $"'{nameof(expeditionRewardItemDatabase)}' " +
                    "is not assigned.",
                    this);
            }

            if (materialDatabase == null)
            {
                Debug.LogWarning(
                    $"{nameof(ExpeditionRuntimeCompositionContext)}: " +
                    $"'{nameof(materialDatabase)}' " +
                    "is not assigned.",
                    this);
            }
        }

#endif
    }
}