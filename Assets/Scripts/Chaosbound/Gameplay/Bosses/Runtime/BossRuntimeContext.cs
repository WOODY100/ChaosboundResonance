using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Stores the runtime context associated with
    /// a materialized Boss.
    /// </summary>
    [RequireComponent(typeof(BossHealth))]
    public sealed class BossRuntimeContext :
        MonoBehaviour
    {
        public BossData Boss
        {
            get;
            private set;
        }

        public ExpeditionRuntimeState
            ExpeditionRuntime
        {
            get;
            private set;
        }

        public bool IsInitialized
        {
            get;
            private set;
        }

        public void Initialize(
            BossData boss,
            ExpeditionRuntimeState expeditionRuntime)
        {
            Boss =
                boss
                ?? throw new ArgumentNullException(
                    nameof(boss));

            ExpeditionRuntime =
                expeditionRuntime
                ?? throw new ArgumentNullException(
                    nameof(expeditionRuntime));

            IsInitialized = true;
        }

        private void OnDisable()
        {
            Boss = null;
            ExpeditionRuntime = null;
            IsInitialized = false;
        }
    }
}