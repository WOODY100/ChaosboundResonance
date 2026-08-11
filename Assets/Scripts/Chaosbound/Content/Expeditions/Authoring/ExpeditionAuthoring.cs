using Chaosbound.Content.Expeditions.Authoring.Combat;
using Chaosbound.Content.Expeditions.Authoring.Bosses;
using Chaosbound.Content.Expeditions.Authoring.Enemy;
using Chaosbound.Content.Expeditions.Authoring.ExpeditionEvents;
using Chaosbound.Content.Expeditions.Authoring.General;
using Chaosbound.Content.Expeditions.Authoring.Identity;
using Chaosbound.Content.Expeditions.Authoring.MiniBosses;
using Chaosbound.Content.Expeditions.Authoring.Presentation;
using Chaosbound.Content.Expeditions.Authoring.Pressure;
using Chaosbound.Content.Expeditions.Authoring.Rewards;
using Chaosbound.Content.Expeditions.Authoring.Scene;
using Chaosbound.Content.Expeditions.Authoring.Spawn;
using Chaosbound.Content.Expeditions.Authoring.World;
using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring
{
    /// <summary>
    /// Root authoring model for an expedition.
    /// Aggregates every editable section that composes an expedition.
    /// </summary>
    [Serializable]
    public sealed class ExpeditionAuthoring
    {
        [Header("Scene")]

        [SerializeField]
        private SceneAuthoring m_Scene;

        [Header("Identity")]

        [SerializeField]
        private IdentityAuthoring m_identity = new();

        [Header("Presentation")]

        [SerializeField]
        private PresentationAuthoring m_presentation = new();

        [Header("General")]

        [SerializeField]
        private GeneralAuthoring m_general = new();
        
        [Header("Pressure")]

        [SerializeField]
        private PressureAuthoring m_pressure = new();

        [Header("World")]

        [SerializeField]
        private WorldAuthoring m_world = new();

        [Header("Enemy")]

        [SerializeField]
        private EnemyAuthoring m_enemy = new();

        [Header("Spawn")]

        [SerializeField]
        private SpawnAuthoring m_Spawn = new();

        [Header("Combat")]

        [SerializeField]
        private CombatAuthoring m_combat = new();

        [Header("Expedition Events")]

        [SerializeField]
        private ExpeditionEventsAuthoring m_expeditionEvents = new();

        [Header("Mini Bosses")]

        [SerializeField]
        private MiniBossesAuthoring m_miniBosses = new();

        [Header("Bosses")]

        [SerializeField]
        private BossesAuthoring m_bosses = new();

        [Header("Rewards")]

        [SerializeField]
        private RewardsAuthoring m_rewards = new();

        public SceneAuthoring Scene => m_Scene;

        public IdentityAuthoring Identity => m_identity;

        public PresentationAuthoring Presentation => m_presentation;

        public GeneralAuthoring General => m_general;

        public PressureAuthoring Pressure => m_pressure;

        public WorldAuthoring World => m_world;

        public EnemyAuthoring Enemy => m_enemy;

        public SpawnAuthoring Spawn => m_Spawn;

        public CombatAuthoring Combat => m_combat;

        public ExpeditionEventsAuthoring ExpeditionEvents => m_expeditionEvents;

        public MiniBossesAuthoring MiniBosses => m_miniBosses;

        public BossesAuthoring Bosses => m_bosses;

        public RewardsAuthoring Rewards => m_rewards;
    }
}