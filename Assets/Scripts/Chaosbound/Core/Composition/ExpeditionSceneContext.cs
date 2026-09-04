using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Config;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Markers;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Rendering;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Runtime;
using Chaosbound.Gameplay.Navigation;
using Chaosbound.UI.ExpeditionResult;
using Chaosbound.UI.Timeline;
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

        [SerializeField]
        private PlayerHealth player;

        [SerializeField]
        private PlayerStats playerStats;

        [SerializeField]
        private PlayerExperienceSystem playerExperienceSystem;

        [SerializeField]
        private PlayerSkillLoadout playerSkillLoadout;

        //==========================================================
        // World
        //==========================================================

        [Header("World")]

        [SerializeField]
        private OpenWorldMapGenerator mapGenerator;

        [SerializeField]
        private OpenWorldDecorationGenerator decorationGenerator;

        [SerializeField]
        private ExpeditionNavigation navigation;

        //==========================================================
        // UI
        //==========================================================

        [Header("UI")]

        [SerializeField]
        private HUDController hudController;

        [SerializeField]
        private HUDXPBarUI hudXPBarUI;

        [SerializeField]
        private HUDLevelUI hudLevelUI;

        [SerializeField]
        private SkillBarUI skillBarUI;

        [SerializeField]
        private TimelineUI timelineUI;

        [SerializeField]
        private ExpeditionResultPanel expeditionResultPanel;

        [SerializeField] 
        private SkillEvolutionPanel skillEvolutionPanel;


        //==========================================================
        // Minimap
        //==========================================================

        [Header("Minimap")]

        [SerializeField]
        private MinimapStaticMapView minimapStaticMapView;

        [SerializeField]
        private MinimapConfig minimapConfig;

        [SerializeField]
        private MinimapRuntimeUpdater minimapRuntimeUpdater;

        [SerializeField]
        private MinimapMarkerView minimapPlayerMarkerView;

        [SerializeField]
        private MinimapMarkerView minimapBossMarkerView;

        [SerializeField]
        private MinimapMarkerView minimapExitPortalMarkerView;

        [SerializeField]
        private MinimapMarkerView minimapModifierStructureMarkerView;

        [SerializeField]
        private RectTransform minimapMapViewport;

        [SerializeField]
        private RectTransform minimapMapContent;

        //==========================================================
        // Public Properties
        //==========================================================

        // Player

        public PlayerHealth Player =>
            player;

        public PlayerStats PlayerStats =>
            playerStats;

        public PlayerExperienceSystem PlayerExperienceSystem =>
            playerExperienceSystem;

        public PlayerSkillLoadout PlayerSkillLoadout =>
            playerSkillLoadout;

        // World

        public OpenWorldMapGenerator MapGenerator =>
            mapGenerator;

        public OpenWorldDecorationGenerator DecorationGenerator =>
            decorationGenerator;

        public ExpeditionNavigation Navigation =>
            navigation;

        // UI

        public HUDController HUDController =>
            hudController;

        public HUDXPBarUI HUDXPBarUI =>
            hudXPBarUI;

        public HUDLevelUI HUDLevelUI =>
            hudLevelUI;

        public SkillBarUI SkillBarUI =>
            skillBarUI;

        public TimelineUI TimelineUI => 
            timelineUI;

        public ExpeditionResultPanel ExpeditionResultPanel =>
            expeditionResultPanel;

        public SkillEvolutionPanel SkillEvolutionPanel =>
            skillEvolutionPanel;

        // Minimap

        public MinimapStaticMapView MinimapStaticMapView =>
            minimapStaticMapView;

        public MinimapConfig MinimapConfig =>
            minimapConfig;

        public MinimapRuntimeUpdater MinimapRuntimeUpdater =>
            minimapRuntimeUpdater;

        public MinimapMarkerView MinimapPlayerMarkerView =>
            minimapPlayerMarkerView;

        public MinimapMarkerView MinimapBossMarkerView =>
            minimapBossMarkerView;

        public MinimapMarkerView MinimapExitPortalMarkerView =>
            minimapExitPortalMarkerView;

        public MinimapMarkerView MinimapModifierStructureMarkerView =>
            minimapModifierStructureMarkerView;

        public RectTransform MinimapMapViewport =>
            minimapMapViewport;

        public RectTransform MinimapMapContent =>
            minimapMapContent;

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

        private void ValidateMinimapConfig()
        {
            if (minimapConfig == null)
            {
                Debug.LogWarning(
                    $"{nameof(ExpeditionSceneContext)}: '{nameof(minimapConfig)}' is not assigned.",
                    this);
            }
        }

        private void OnValidate()
        {
            // Player
            ValidateReference(
                player,
                nameof(player));

            ValidateReference(
                playerStats,
                nameof(playerStats));

            ValidateReference(
                playerExperienceSystem,
                nameof(playerExperienceSystem));

            ValidateReference(
                playerSkillLoadout,
                nameof(playerSkillLoadout));

            // World
            ValidateReference(
                mapGenerator,
                nameof(mapGenerator));

            ValidateReference(
                decorationGenerator,
                nameof(decorationGenerator));

            ValidateReference(
                navigation,
                nameof(navigation));

            // UI
            ValidateReference(
                hudController,
                nameof(hudController));

            ValidateReference(
                hudXPBarUI,
                nameof(hudXPBarUI));

            ValidateReference(
                hudLevelUI,
                nameof(hudLevelUI));

            ValidateReference(
                skillBarUI,
                nameof(skillBarUI));

            ValidateReference(
                timelineUI,
                nameof(timelineUI));

            ValidateReference(
                expeditionResultPanel,
                nameof(expeditionResultPanel));

            // Minimap
            ValidateReference(
                minimapStaticMapView,
                nameof(minimapStaticMapView));

            ValidateMinimapConfig();

            ValidateReference(
                minimapRuntimeUpdater,
                nameof(minimapRuntimeUpdater));

            ValidateReference(
                minimapPlayerMarkerView,
                nameof(minimapPlayerMarkerView));

            ValidateReference(
                minimapBossMarkerView,
                nameof(minimapBossMarkerView));

            ValidateReference(
                minimapExitPortalMarkerView,
                nameof(minimapExitPortalMarkerView));

            ValidateReference(
                minimapModifierStructureMarkerView,
                nameof(minimapModifierStructureMarkerView));

            ValidateReference(
                minimapMapViewport,
                nameof(minimapMapViewport));

            ValidateReference(
                minimapMapContent,
                nameof(minimapMapContent));
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