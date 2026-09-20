using Chaosbound.Content.Items;
using Chaosbound.Content.Materials;
using Chaosbound.Gameplay.Items.Runtime;
using System;
using UnityEngine;

namespace Chaosbound.Core.Composition
{
    /// <summary>
    /// Provides the configurable infrastructure dependencies required
    /// to compose the Expedition Runtime.
    /// </summary>
    public sealed class GameContentContext
        : MonoBehaviour
    {
        public static GameContentContext Current
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

        private ItemContentResolver itemContentResolver;

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

        public ItemContentResolver
            ItemContentResolver =>
                itemContentResolver;

        //==========================================================
        // Unity
        //==========================================================

        private void Awake()
        {
            RegisterCurrentContext();

            itemContentResolver =
                new ItemContentResolver(
                    itemDatabase,
                    expeditionRewardItemDatabase);
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
                    "Multiple GameContentContext " +
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
                    $"{nameof(GameContentContext)}: " +
                    $"'{nameof(materializableContentDatabase)}' " +
                    "is not assigned.",
                    this);
            }

            if (itemDatabase == null)
            {
                Debug.LogWarning(
                    $"{nameof(GameContentContext)}: " +
                    $"'{nameof(itemDatabase)}' " +
                    "is not assigned.",
                    this);
            }

            if (expeditionRewardItemDatabase == null)
            {
                Debug.LogWarning(
                    $"{nameof(GameContentContext)}: " +
                    $"'{nameof(expeditionRewardItemDatabase)}' " +
                    "is not assigned.",
                    this);
            }

            if (materialDatabase == null)
            {
                Debug.LogWarning(
                    $"{nameof(GameContentContext)}: " +
                    $"'{nameof(materialDatabase)}' " +
                    "is not assigned.",
                    this);
            }
        }

#endif
    }
}