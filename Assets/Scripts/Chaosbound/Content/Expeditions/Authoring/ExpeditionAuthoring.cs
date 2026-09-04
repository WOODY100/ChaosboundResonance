using Chaosbound.Content.Expeditions.Authoring.Bosses;
using Chaosbound.Content.Expeditions.Authoring.Combat;
using Chaosbound.Content.Expeditions.Authoring.SkillProgression;
using Chaosbound.Content.Expeditions.Authoring.Completion;
using Chaosbound.Content.Expeditions.Authoring.Enemy;
using Chaosbound.Content.Expeditions.Authoring.ExpeditionEvents;
using Chaosbound.Content.Expeditions.Authoring.Identity;
using Chaosbound.Content.Expeditions.Authoring.MiniBosses;
using Chaosbound.Content.Expeditions.Authoring.Minimap;
using Chaosbound.Content.Expeditions.Authoring.Presentation;
using Chaosbound.Content.Expeditions.Authoring.Rewards;
using Chaosbound.Content.Expeditions.Authoring.Scene;
using Chaosbound.Content.Expeditions.Authoring.Spawn;
using Chaosbound.Content.Expeditions.Authoring.Timeline;
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

        [Header("Skill Progression")]

        [SerializeField]
        private SkillProgressionAuthoring m_skillProgression = new();

        [Header("Timeline")]

        [SerializeField]
        private TimelineAuthoring m_Timeline = new();

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

        [Header("Completion")]

        [SerializeField]

        private CompletionAuthoring m_completion = new();

        [Header("Minimap")]

        [SerializeField]
        private MinimapAuthoring m_minimap = new();

        public SceneAuthoring Scene => m_Scene;

        public IdentityAuthoring Identity => m_identity;

        public PresentationAuthoring Presentation => m_presentation;

        public WorldAuthoring World => m_world;

        public EnemyAuthoring Enemy => m_enemy;

        public SpawnAuthoring Spawn => m_Spawn;

        public CombatAuthoring Combat => m_combat;

        public SkillProgressionAuthoring SkillProgression => m_skillProgression;

        public TimelineAuthoring Timeline => m_Timeline;

        public ExpeditionEventsAuthoring ExpeditionEvents => m_expeditionEvents;

        public MiniBossesAuthoring MiniBosses => m_miniBosses;

        public BossesAuthoring Bosses => m_bosses;

        public RewardsAuthoring Rewards => m_rewards;

        public CompletionAuthoring Completion => m_completion;

        public MinimapAuthoring Minimap => m_minimap;
    }
}