#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Chaosbound.Gameplay.Debugging.SkillGizmos;
using Chaosbound.Content.Enemy.Bosses;

namespace Chaosbound.Content.Enemy.Bosses.Editor
{
    internal static class BossAuthoringPreview
    {
        private const string ShowGeometryKey = "Chaosbound.BossAuthoringPreview.ShowGeometry";
        private const string ShowLabelsKey = "Chaosbound.BossAuthoringPreview.ShowLabels";
        private const string ShowPrefabColliderKey = "Chaosbound.BossAuthoringPreview.ShowPrefabCollider";

        // Shared semantic palette. Keep these meanings aligned with Runtime Skill Gizmos.
        private static readonly Color PreferredColor = new Color(0.2f, 0.85f, 1f, 0.95f);
        private static readonly Color MinimumColor = new Color(1f, 0.68f, 0.18f, 0.95f);
        private static readonly Color RangeColor = new Color(0.95f, 0.95f, 0.95f, 0.85f);
        private static readonly Color ActiveColor = new Color(1f, 0.2f, 0.75f, 1f);
        private static readonly Color AreaColor = new Color(1f, 0.25f, 0.2f, 0.95f);
        private static readonly Color ProjectileColor = new Color(0.25f, 0.9f, 1f, 0.95f);

        public static bool ShowGeometry
        {
            get => SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Boss) &&
                   SkillGizmoUtility.IsLayerEnabled(SkillGizmoUtility.VisualLayer.Geometry) &&
                   EditorPrefs.GetBool(ShowGeometryKey, true);
            set => EditorPrefs.SetBool(ShowGeometryKey, value);
        }

        public static bool ShowLabels
        {
            get => SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Boss) &&
                   SkillGizmoUtility.IsLayerEnabled(SkillGizmoUtility.VisualLayer.Labels) &&
                   EditorPrefs.GetBool(ShowLabelsKey, true);
            set => EditorPrefs.SetBool(ShowLabelsKey, value);
        }

        public static bool ShowPrefabCollider
        {
            get => SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Boss) &&
                   SkillGizmoUtility.IsLayerEnabled(SkillGizmoUtility.VisualLayer.Colliders) &&
                   EditorPrefs.GetBool(ShowPrefabColliderKey, true);
            set => EditorPrefs.SetBool(ShowPrefabColliderKey, value);
        }

        public static void DrawPreviewOptions()
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Authoring Preview", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            ShowGeometry = EditorGUILayout.ToggleLeft("Show geometry", ShowGeometry);
            ShowLabels = EditorGUILayout.ToggleLeft("Show labels", ShowLabels);
            ShowPrefabCollider = EditorGUILayout.ToggleLeft("Show projectile prefab collider", ShowPrefabCollider);
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();
        }

        public static void DrawCombat(BossCombatDefinition definition)
        {
            if (definition == null || (!ShowGeometry && !ShowLabels)) return;

            BossMovementSettings movement = definition.MovementSettings;
            if (ShowGeometry)
            {
                DrawCircle(Vector3.zero, movement.MinimumDistance, MinimumColor);
                DrawCircle(Vector3.zero, movement.PreferredDistance, PreferredColor);
            }

            if (definition.Phases == null) return;

            for (int p = 0; p < definition.Phases.Count; p++)
            {
                BossPhaseDefinition phase = definition.Phases[p];
                if (phase == null) continue;

                if (ShowGeometry)
                    DrawPhaseRanges(phase, false);

                if (ShowLabels)
                {
                    float labelRadius = Mathf.Max(
                        movement.PreferredDistance + 1.5f,
                        GetPhaseMaximumRange(phase) + 1.5f);
                    Vector3 labelPosition = GetStackedLabelPosition(p, definition.Phases.Count, labelRadius);
                    Handles.color = Color.white;
                    Handles.Label(labelPosition,
                        $"Phase {p + 1}: {phase.DisplayName} · {phase.HealthThreshold:0.#}% HP");
                }
            }

            if (ShowLabels)
            {
                Handles.color = MinimumColor;
                Handles.Label(new Vector3(movement.MinimumDistance, 0f, -0.75f),
                    $"Minimum distance · {movement.MinimumDistance:0.##}");
                Handles.color = PreferredColor;
                Handles.Label(new Vector3(movement.PreferredDistance, 0f, -1.5f),
                    $"Preferred distance · {movement.PreferredDistance:0.##}");
            }
        }

        public static void DrawPhase(BossPhaseDefinition phase)
        {
            if (phase == null) return;

            if (ShowGeometry)
                DrawPhaseRanges(phase, false);

            if (ShowLabels)
            {
                Handles.color = Color.white;
                Handles.Label(new Vector3(0f, 0f, -1.5f),
                    $"Phase: {phase.DisplayName} · {phase.HealthThreshold:0.#}% HP");

                if (phase.AvailableAttacks != null)
                {
                    for (int i = 0; i < phase.AvailableAttacks.Count; i++)
                    {
                        BossAttackDefinition attack = phase.AvailableAttacks[i];
                        if (attack == null) continue;
                        Vector3 labelPosition = GetStackedLabelPosition(
                            i, phase.AvailableAttacks.Count,
                            Mathf.Max(attack.MaximumRange, 1f) + 0.75f);
                        Handles.color = RangeColor;
                        Handles.Label(labelPosition,
                            $"{i + 1}. {attack.DisplayName} · {attack.MinimumRange:0.##}–{attack.MaximumRange:0.##}");
                    }
                }
            }
        }

        public static void DrawAttack(BossAttackDefinition attack)
        {
            if (attack == null) return;

            if (ShowGeometry)
            {
                DrawCircle(Vector3.zero, attack.MinimumRange, MinimumColor);
                DrawCircle(Vector3.zero, attack.MaximumRange, ActiveColor);
            }

            if (ShowLabels)
            {
                Handles.color = MinimumColor;
                Handles.Label(new Vector3(attack.MinimumRange, 0f, -0.75f),
                    $"Attack minimum · {attack.MinimumRange:0.##}");
                Handles.color = ActiveColor;
                Handles.Label(new Vector3(attack.MaximumRange, 0f, -1.5f),
                    $"Attack maximum · {attack.MaximumRange:0.##}");
            }
        }

        public static void DrawArea(BossAreaEffectDefinition area)
        {
            if (area == null) return;
            Vector3 center = area.PositionOffset;

            if (ShowGeometry)
            {
                DrawCircle(center, area.Radius, AreaColor);
                Handles.color = Color.white;
                Handles.DrawDottedLine(Vector3.zero, center, 4f);
            }

            if (ShowLabels)
            {
                Handles.color = AreaColor;
                Handles.Label(center + Vector3.up * 0.2f + Vector3.forward * (area.Radius + 0.5f),
                    $"Damage radius · {area.Radius:0.##}");
                Handles.color = Color.white;
                Handles.Label(center + Vector3.up * 0.2f, $"Offset · {FormatVector(area.PositionOffset)}");
            }
        }

        public static void DrawProjectile(BossProjectileEffectDefinition projectile)
        {
            if (projectile == null) return;
            Vector3 origin = projectile.LocalSpawnOffset;
            Vector3 direction = Vector3.forward;
            Vector3 end = origin + direction * projectile.MaxTravelDistance;

            if (ShowGeometry)
            {
                Handles.color = ProjectileColor;
                Handles.DrawLine(origin, end);
                Handles.ConeHandleCap(
                    0, end, Quaternion.LookRotation(direction),
                    Mathf.Clamp(projectile.MaxTravelDistance * 0.035f, 0.15f, 0.5f),
                    EventType.Repaint);
                Handles.DrawWireDisc(origin, Vector3.up, 0.18f);

                if (ShowPrefabCollider)
                    DrawPrefabCollider(projectile.ProjectilePrefab, origin);
            }

            if (ShowLabels)
            {
                Handles.color = ProjectileColor;
                Handles.Label(origin + Vector3.up * 0.2f, $"Spawn · {FormatVector(projectile.LocalSpawnOffset)}");
                Handles.Label(end + Vector3.up * 0.2f,
                    $"Max travel · {projectile.MaxTravelDistance:0.##}");
                Handles.Label(end + Vector3.up * 0.8f,
                    $"Speed · {projectile.Speed:0.##}");
            }
        }

        private static void DrawPhaseRanges(BossPhaseDefinition phase, bool active)
        {
            if (phase == null || phase.AvailableAttacks == null) return;
            for (int i = 0; i < phase.AvailableAttacks.Count; i++)
            {
                BossAttackDefinition attack = phase.AvailableAttacks[i];
                if (attack == null) continue;
                Color color = active ? ActiveColor : RangeColor;
                DrawCircle(Vector3.zero, attack.MinimumRange, MinimumColor);
                DrawCircle(Vector3.zero, attack.MaximumRange, color);
            }
        }

        private static float GetPhaseMaximumRange(BossPhaseDefinition phase)
        {
            float maximum = 0f;
            if (phase == null || phase.AvailableAttacks == null) return maximum;
            for (int i = 0; i < phase.AvailableAttacks.Count; i++)
            {
                BossAttackDefinition attack = phase.AvailableAttacks[i];
                if (attack != null)
                    maximum = Mathf.Max(maximum, attack.MaximumRange);
            }
            return maximum;
        }

        private static Vector3 GetStackedLabelPosition(int index, int count, float radius)
        {
            // Labels occupy separate angular lanes around the preview, rather than stacking at origin.
            float angle = count <= 1 ? 35f : Mathf.Lerp(-35f, 35f, (float)index / (count - 1));
            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            return direction * radius + Vector3.up * (0.15f + index * 0.08f);
        }

        private static void DrawCircle(Vector3 center, float radius, Color color)
        {
            if (radius <= 0f) return;
            Handles.color = color;
            Handles.DrawWireDisc(center, Vector3.up, radius);
        }

        private static string FormatVector(Vector3 value)
        {
            return $"({value.x:0.##}, {value.y:0.##}, {value.z:0.##})";
        }

        private static void DrawPrefabCollider(GameObject prefab, Vector3 origin)
        {
            if (prefab == null) return;
            Collider collider = prefab.GetComponent<Collider>();
            if (collider == null) return;

            Matrix4x4 oldMatrix = Handles.matrix;
            Color oldColor = Handles.color;
            Handles.matrix = Matrix4x4.TRS(origin, Quaternion.identity, Vector3.one)
                             * prefab.transform.localToWorldMatrix;
            Handles.color = new Color(1f, 0.85f, 0.25f, 0.9f);

            if (collider is SphereCollider sphere)
            {
                Vector3 scale = prefab.transform.lossyScale;
                float radius = sphere.radius *
                    Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
                Handles.DrawWireDisc(sphere.center, Vector3.up, radius);
                Handles.DrawWireDisc(sphere.center, Vector3.right, radius);
                Handles.DrawWireDisc(sphere.center, Vector3.forward, radius);
            }
            else if (collider is BoxCollider box)
            {
                Handles.DrawWireCube(box.center, box.size);
            }
            else if (collider is CapsuleCollider capsule)
            {
                float radius = capsule.radius;
                float halfHeight = Mathf.Max(capsule.height * 0.5f, radius);
                Vector3 axis = capsule.direction == 0 ? Vector3.right :
                    capsule.direction == 2 ? Vector3.forward : Vector3.up;
                Vector3 top = capsule.center + axis * (halfHeight - radius);
                Vector3 bottom = capsule.center - axis * (halfHeight - radius);
                Handles.DrawWireDisc(top, axis, radius);
                Handles.DrawWireDisc(bottom, axis, radius);
                Handles.DrawLine(top + Vector3.up * radius, bottom + Vector3.up * radius);
                Handles.DrawLine(top - Vector3.up * radius, bottom - Vector3.up * radius);
            }

            Handles.matrix = oldMatrix;
            Handles.color = oldColor;
        }
    }

    internal abstract class BossAuthoringPreviewEditorBase : UnityEditor.Editor
    {
        protected void DrawCommonOptions()
        {
            BossAuthoringPreview.DrawPreviewOptions();
        }

        protected void Subscribe(System.Action<SceneView> callback)
        {
            SceneView.duringSceneGui += callback;
        }

        protected void Unsubscribe(System.Action<SceneView> callback)
        {
            SceneView.duringSceneGui -= callback;
        }
    }

    [CustomEditor(typeof(BossCombatDefinition))]
    internal sealed class BossCombatDefinitionAuthoringEditor : BossAuthoringPreviewEditorBase
    {
        private void OnEnable() => Subscribe(DrawPreview);
        private void OnDisable() => Unsubscribe(DrawPreview);
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawCommonOptions();
            EditorGUILayout.HelpBox(
                "Rings are drawn around the Scene origin. Phase labels are distributed around the outer range to avoid overlap.",
                MessageType.Info);
        }
        private void DrawPreview(SceneView view)
        {
            if (SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Boss) && target != null && Selection.activeObject == target)
                BossAuthoringPreview.DrawCombat((BossCombatDefinition)target);
        }
    }

    [CustomEditor(typeof(BossPhaseDefinition))]
    internal sealed class BossPhaseDefinitionAuthoringEditor : BossAuthoringPreviewEditorBase
    {
        private void OnEnable() => Subscribe(DrawPreview);
        private void OnDisable() => Unsubscribe(DrawPreview);
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawCommonOptions();
            EditorGUILayout.HelpBox(
                "Displays attack eligibility ranges and individually positioned attack labels.",
                MessageType.Info);
        }
        private void DrawPreview(SceneView view)
        {
            if (SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Boss) && target != null && Selection.activeObject == target)
                BossAuthoringPreview.DrawPhase((BossPhaseDefinition)target);
        }
    }

    [CustomEditor(typeof(BossAttackDefinition))]
    internal sealed class BossAttackDefinitionAuthoringEditor : BossAuthoringPreviewEditorBase
    {
        private void OnEnable() => Subscribe(DrawPreview);
        private void OnDisable() => Unsubscribe(DrawPreview);
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawCommonOptions();
            EditorGUILayout.HelpBox(
                "Shows the configured attack eligibility range. This is not a damage area.",
                MessageType.Info);
        }
        private void DrawPreview(SceneView view)
        {
            if (SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Boss) && target != null && Selection.activeObject == target)
                BossAuthoringPreview.DrawAttack((BossAttackDefinition)target);
        }
    }

    [CustomEditor(typeof(BossAreaEffectDefinition))]
    internal sealed class BossAreaEffectDefinitionAuthoringEditor : BossAuthoringPreviewEditorBase
    {
        private void OnEnable() => Subscribe(DrawPreview);
        private void OnDisable() => Unsubscribe(DrawPreview);
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawCommonOptions();
            EditorGUILayout.HelpBox(
                "Draws the configured damage radius and PositionOffset. Telegraph prefab geometry remains a separate presentation asset.",
                MessageType.Info);
        }
        private void DrawPreview(SceneView view)
        {
            if (SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Boss) && target != null && Selection.activeObject == target)
                BossAuthoringPreview.DrawArea((BossAreaEffectDefinition)target);
        }
    }

    [CustomEditor(typeof(BossProjectileEffectDefinition))]
    internal sealed class BossProjectileEffectDefinitionAuthoringEditor : BossAuthoringPreviewEditorBase
    {
        private void OnEnable() => Subscribe(DrawPreview);
        private void OnDisable() => Unsubscribe(DrawPreview);
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawCommonOptions();
            EditorGUILayout.HelpBox(
                "Shows the local spawn offset, forward maximum-travel path, speed label, and optional root prefab collider.",
                MessageType.Info);
        }
        private void DrawPreview(SceneView view)
        {
            if (SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Boss) && target != null && Selection.activeObject == target)
                BossAuthoringPreview.DrawProjectile((BossProjectileEffectDefinition)target);
        }
    }
}
#endif
