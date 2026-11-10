#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Chaosbound.Gameplay.Debugging.SkillGizmos;

internal static class PlayerSkillAuthoringPreview
{
    private const string GeometryKey = "Chaosbound.PlayerSkillPreview.Geometry";
    private const string LabelsKey = "Chaosbound.PlayerSkillPreview.Labels";
    private const string ColliderKey = "Chaosbound.PlayerSkillPreview.Collider";

    private static readonly Color SpawnColor = new Color(0.2f, 0.85f, 1f, 0.95f);
    private static readonly Color ImpactColor = new Color(1f, 0.28f, 0.2f, 0.95f);
    private static readonly Color PatternColor = new Color(1f, 0.68f, 0.18f, 0.95f);
    private static readonly Color ProjectileColor = new Color(0.25f, 0.9f, 1f, 0.95f);
    private static readonly Color OrbitColor = new Color(0.25f, 1f, 0.5f, 0.95f);

    private static bool ShowGeometry
    {
        get => SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Player) &&
               SkillGizmoUtility.IsLayerEnabled(SkillGizmoUtility.VisualLayer.Geometry) &&
               EditorPrefs.GetBool(GeometryKey, true);
        set => EditorPrefs.SetBool(GeometryKey, value);
    }

    private static bool ShowLabels
    {
        get => SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Player) &&
               SkillGizmoUtility.IsLayerEnabled(SkillGizmoUtility.VisualLayer.Labels) &&
               EditorPrefs.GetBool(LabelsKey, true);
        set => EditorPrefs.SetBool(LabelsKey, value);
    }

    private static bool ShowPrefabCollider
    {
        get => SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Player) &&
               SkillGizmoUtility.IsLayerEnabled(SkillGizmoUtility.VisualLayer.Colliders) &&
               EditorPrefs.GetBool(ColliderKey, true);
        set => EditorPrefs.SetBool(ColliderKey, value);
    }

    public static void DrawOptions()
    {
        EditorGUILayout.Space(6f);
        EditorGUILayout.LabelField("Authoring Preview", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        ShowGeometry = EditorGUILayout.ToggleLeft("Show geometry", ShowGeometry);
        ShowLabels = EditorGUILayout.ToggleLeft("Show labels", ShowLabels);
        ShowPrefabCollider = EditorGUILayout.ToggleLeft("Show execution prefab collider", ShowPrefabCollider);
        if (EditorGUI.EndChangeCheck())
            SceneView.RepaintAll();
    }

    public static void DrawSkill(SkillDefinition skill)
    {
        if (skill == null) return;
        DrawSkillGeometry(
            skill.ExecutionPattern,
            skill.BaseCount,
            skill.BaseSpawnRadius,
            skill.BaseImpactRadius,
            skill.BaseRange,
            skill.ExecutionPrefab,
            skill.DisplayName);
    }

    public static void DrawPattern(SkillExecutionPatternDefinition pattern)
    {
        if (pattern == null) return;
        DrawSkillGeometry(pattern, 1, 0f, 0f, 0f, null, pattern.name);
    }

    private static void DrawSkillGeometry(
        SkillExecutionPatternDefinition pattern,
        int count,
        float spawnRadius,
        float impactRadius,
        float range,
        GameObject executionPrefab,
        string displayName)
    {
        if (!ShowGeometry && !ShowLabels) return;

        count = Mathf.Max(1, count);
        Vector3 origin = Vector3.zero;
        Vector3 forward = Vector3.forward;

        if (pattern is AreaStrikePatternDefinition area)
            DrawAreaStrike(area, count, spawnRadius, impactRadius, origin, forward);
        else if (pattern is ProjectilePatternDefinition projectile)
            DrawProjectilePattern(projectile, count, range, origin, forward);
        else if (pattern is OrbitalPatternDefinition orbital)
            DrawOrbital(orbital, count, executionPrefab, origin);
        else if (pattern is PersistentZonePatternDefinition zone)
            DrawPersistentZone(zone, count, spawnRadius, impactRadius, origin, forward);
        else if (pattern == null)
            DrawFallback(spawnRadius, impactRadius, range, origin);

        // Orbital preview draws its prefab collider at each orbit reference point.
        // Do not draw a second copy at the owner origin.
        if (!(pattern is OrbitalPatternDefinition) &&
            ShowGeometry && ShowPrefabCollider && executionPrefab != null)
            DrawExecutionPrefabCollider(executionPrefab, origin);

        if (ShowLabels)
        {
            DrawLabelColumn(
                new[]
                {
                    string.IsNullOrEmpty(displayName) ? "Skill Preview" : displayName,
                    $"Base count · {count}"
                },
                new Color(0.92f, 0.92f, 0.92f, 1f),
                new Vector3(1.0f, 0.25f, -0.9f));
        }
    }

    private static void DrawAreaStrike(
        AreaStrikePatternDefinition pattern,
        int count,
        float spawnRadius,
        float impactRadius,
        Vector3 origin,
        Vector3 forward)
    {
        Vector3 start = origin + forward * pattern.ForwardOffset;

        switch (pattern.Pattern)
        {
            case AreaStrikePatternType.SinglePoint:
                DrawImpactPoint(start, Mathf.Max(0.1f, impactRadius), "Impact footprint");
                break;

            case AreaStrikePatternType.MultiPoint:
                // Runtime chooses random positions inside this envelope.
                DrawCircle(origin, spawnRadius, SpawnColor);
                DrawImpactPoint(origin, Mathf.Max(0.1f, impactRadius), null);
                if (ShowLabels)
                {
                    DrawLabelColumn(
                        new[]
                        {
                            $"Random placement envelope · R {spawnRadius:0.##}",
                            $"Impact footprint · R {Mathf.Max(0.1f, impactRadius):0.##} (at each spawned strike)"
                        },
                        SpawnColor,
                        new Vector3(spawnRadius + 0.6f, 0.2f, 0.2f));
                }
                break;

            case AreaStrikePatternType.Ring:
                // AreaRadius is the random placement radius around the forward-offset center,
                // not a damage radius and not a deterministic ring of impacts.
                DrawCircle(start, pattern.AreaRadius, PatternColor);
                DrawImpactPoint(start, Mathf.Max(0.1f, impactRadius), null);
                if (ShowLabels)
                {
                    DrawLabelColumn(
                        new[]
                        {
                            $"Random placement envelope · R {pattern.AreaRadius:0.##}",
                            $"Center offset · {pattern.ForwardOffset:0.##}",
                            $"Impact footprint · R {Mathf.Max(0.1f, impactRadius):0.##}"
                        },
                        PatternColor,
                        start + new Vector3(pattern.AreaRadius + 0.6f, 0.2f, 0.2f));
                }
                break;

            case AreaStrikePatternType.Line:
                Vector3 end = start + forward * pattern.LineLength;
                DrawLineWithEndpoints(start, end, PatternColor);
                if (ShowLabels)
                {
                    DrawLabelColumn(
                        new[]
                        {
                            $"Line placement · {pattern.LineLength:0.##}",
                            $"Forward offset · {pattern.ForwardOffset:0.##}",
                            $"Impact footprint · R {Mathf.Max(0.1f, impactRadius):0.##}"
                        },
                        PatternColor,
                        end + new Vector3(0.6f, 0.2f, 0.2f));
                }
                break;
        }
    }

    private static void DrawProjectilePattern(
        ProjectilePatternDefinition pattern,
        int count,
        float range,
        Vector3 origin,
        Vector3 ownerForward)
    {
        Vector3 baseDirection = pattern.LocalDirection;
        if (pattern.Pattern != ProjectilePatternType.FixedDirection ||
            baseDirection.sqrMagnitude < 0.0001f)
            baseDirection = ownerForward;
        baseDirection.y = 0f;
        if (baseDirection.sqrMagnitude < 0.0001f) baseDirection = Vector3.forward;
        baseDirection.Normalize();

        float effectiveRange = Mathf.Max(0f, range);
        if (pattern.Pattern == ProjectilePatternType.TargetNearest)
        {
            DrawCircle(origin, effectiveRange, SpawnColor);
            if (ShowLabels)
                LabelAt(new Vector3(effectiveRange, 0.15f, 0f),
                    $"Target acquisition · R {effectiveRange:0.##}", SpawnColor);
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 direction = GetProjectileDirection(pattern, baseDirection, i, count);
            Vector3 end = origin + direction * effectiveRange;
            if (ShowGeometry)
            {
                Handles.color = ProjectileColor;
                Handles.DrawLine(origin, end);
                Handles.ConeHandleCap(0, end, Quaternion.LookRotation(direction),
                    Mathf.Clamp(effectiveRange * 0.035f, 0.12f, 0.45f), EventType.Repaint);
            }
        }

        if (ShowLabels)
        {
            DrawLabelColumn(
                new[]
                {
                    $"{pattern.Pattern} · {count} direction(s)",
                    $"Range · {effectiveRange:0.##}",
                    $"Aim at nearest · {(pattern.AimAtNearestTarget ? "Yes" : "No")}"
                },
                ProjectileColor,
                new Vector3(1f, 0.2f, 0.6f));
        }
    }

    private static Vector3 GetProjectileDirection(
        ProjectilePatternDefinition pattern,
        Vector3 baseDirection,
        int index,
        int count)
    {
        switch (pattern.Pattern)
        {
            case ProjectilePatternType.Fan:
                float fanAngle = count <= 1 ? 0f :
                    -pattern.SpreadAngle * 0.5f +
                    pattern.SpreadAngle * index / (count - 1);
                return Quaternion.Euler(0f, fanAngle, 0f) * baseDirection;

            case ProjectilePatternType.Radial:
                return Quaternion.Euler(0f, 360f * index / count, 0f) * baseDirection;

            case ProjectilePatternType.FixedDirection:
                Vector3 local = pattern.LocalDirection;
                return local.sqrMagnitude > 0.0001f ? local.normalized : Vector3.forward;

            case ProjectilePatternType.RandomSpread:
                // Preview shows the configured angular envelope, not a random runtime sample.
                float spread = pattern.RandomSpreadAngle;
                float envelopeAngle = count <= 1 ? 0f :
                    -spread + 2f * spread * index / (count - 1);
                return Quaternion.Euler(0f, envelopeAngle, 0f) * baseDirection;

            default:
                return baseDirection;
        }
    }

    private static void DrawOrbital(
        OrbitalPatternDefinition pattern,
        int count,
        GameObject prefab,
        Vector3 origin)
    {
        if (ShowGeometry)
        {
            Handles.color = OrbitColor;
            Handles.DrawWireDisc(origin + Vector3.up * pattern.OrbitHeight,
                Vector3.up, pattern.OrbitRadius);

            if (pattern.Pattern == OrbitalPatternType.ForwardArc)
            {
                Vector3 forward = Vector3.forward;
                Vector3 left = Quaternion.Euler(0f, -pattern.ForwardArcAngle * 0.5f, 0f) * forward;
                Vector3 right = Quaternion.Euler(0f, pattern.ForwardArcAngle * 0.5f, 0f) * forward;
                Handles.DrawLine(origin + Vector3.up * pattern.OrbitHeight,
                    origin + Vector3.up * pattern.OrbitHeight + left * pattern.OrbitRadius);
                Handles.DrawLine(origin + Vector3.up * pattern.OrbitHeight,
                    origin + Vector3.up * pattern.OrbitHeight + right * pattern.OrbitRadius);
                Handles.DrawWireArc(origin + Vector3.up * pattern.OrbitHeight,
                    Vector3.up, left, pattern.ForwardArcAngle, pattern.OrbitRadius);
            }

            for (int i = 0; i < count; i++)
            {
                float angle = 360f * i / count;
                if (pattern.Pattern == OrbitalPatternType.ForwardArc && count > 1)
                    angle = -pattern.ForwardArcAngle * 0.5f +
                            pattern.ForwardArcAngle * i / (count - 1);

                Vector3 offset = Quaternion.Euler(0f, angle, 0f) *
                    Vector3.forward * pattern.OrbitRadius;
                Vector3 point = origin + offset + Vector3.up * pattern.OrbitHeight;
                Handles.DrawWireDisc(point, Vector3.up, 0.18f);
                Handles.DrawLine(origin + Vector3.up * pattern.OrbitHeight, point);
            }
        }

        if (ShowLabels)
        {
            DrawLabelColumn(
                new[]
                {
                    $"{pattern.Pattern} · {count} orb(s)",
                    $"Orbit radius · {pattern.OrbitRadius:0.##}",
                    $"Orbit height · {pattern.OrbitHeight:0.##}",
                    $"Angular speed · {pattern.AngularSpeed:0.##}°/s"
                },
                OrbitColor,
                new Vector3(pattern.OrbitRadius + 0.65f, 0.2f,
                    pattern.OrbitRadius + 0.25f));
        }

        if (prefab != null && ShowGeometry && ShowPrefabCollider)
            DrawExecutionPrefabCollider(prefab, origin + Vector3.up * pattern.OrbitHeight);
    }

    private static void DrawPersistentZone(
        PersistentZonePatternDefinition pattern,
        int count,
        float spawnRadius,
        float impactRadius,
        Vector3 origin,
        Vector3 forward)
    {
        float effectiveDamageRadius = Mathf.Max(0.1f, impactRadius);
        Vector3 start = origin + forward * pattern.ForwardOffset;

        switch (pattern.Pattern)
        {
            case PersistentZonePatternType.SingleZone:
                // Runtime spawns SingleZone exactly at the player origin.
                DrawImpactPoint(origin, effectiveDamageRadius, "Damage area");
                break;

            case PersistentZonePatternType.MultiZone:
                // Runtime uses Random.insideUnitCircle within FinalSpawnRadius.
                DrawCircle(origin, spawnRadius, SpawnColor);
                DrawImpactPoint(origin, effectiveDamageRadius, null);
                if (ShowLabels)
                {
                    DrawLabelColumn(
                        new[]
                        {
                            $"Random placement envelope · R {spawnRadius:0.##}",
                            $"Damage radius per zone · R {effectiveDamageRadius:0.##}"
                        },
                        SpawnColor,
                        new Vector3(spawnRadius + 0.6f, 0.2f, 0.2f));
                }
                break;

            case PersistentZonePatternType.Ring:
                if (ShowGeometry)
                {
                    Handles.color = PatternColor;
                    Handles.DrawWireDisc(origin, Vector3.up, pattern.RingRadius);
                    for (int i = 0; i < count; i++)
                    {
                        float angle = 360f * i / count;
                        Vector3 point = origin + Quaternion.Euler(0f, angle, 0f) *
                            Vector3.forward * pattern.RingRadius;
                        DrawImpactPoint(point, effectiveDamageRadius, null);
                    }
                }
                if (ShowLabels)
                {
                    DrawLabelColumn(
                        new[]
                        {
                            $"Zone ring · R {pattern.RingRadius:0.##}",
                            $"Zones · {count}",
                            $"Damage radius per zone · R {effectiveDamageRadius:0.##}",
                            pattern.FollowPlayer ? "Follow player · Yes" : "Follow player · No"
                        },
                        PatternColor,
                        new Vector3(pattern.RingRadius + 0.6f, 0.2f, 0.2f));
                }
                break;

            case PersistentZonePatternType.Line:
                Vector3 end = start + forward * pattern.LineLength;
                DrawLineWithEndpoints(start, end, PatternColor);
                if (ShowGeometry)
                {
                    for (int i = 0; i < count; i++)
                    {
                        float t = count <= 1 ? 0f : (float)i / (count - 1);
                        DrawImpactPoint(Vector3.Lerp(start, end, t), effectiveDamageRadius, null);
                    }
                }
                if (ShowLabels)
                {
                    DrawLabelColumn(
                        new[]
                        {
                            $"Zone line · {pattern.LineLength:0.##}",
                            $"Forward offset · {pattern.ForwardOffset:0.##}",
                            $"Zones · {count}",
                            $"Damage radius per zone · R {effectiveDamageRadius:0.##}"
                        },
                        PatternColor,
                        end + new Vector3(0.6f, 0.2f, 0.2f));
                }
                break;
        }
    }

    private static void DrawFallback(
        float spawnRadius, float impactRadius, float range, Vector3 origin)
    {
        // No pattern means the executor owns placement semantics.
        // BaseSpawnRadius is used by the current AreaStrike/PersistentZone fallback
        // paths as a placement/search envelope. BaseImpactRadius is the per-effect
        // footprint; the runtime implementations clamp it to at least 0.1.
        float effectiveImpactRadius = Mathf.Max(0.1f, impactRadius);

        DrawCircle(origin, spawnRadius, SpawnColor);
        DrawImpactPoint(origin, effectiveImpactRadius, null);

        if (ShowLabels)
        {
            var labels = new System.Collections.Generic.List<string>();
            if (spawnRadius > 0f)
                labels.Add($"Placement/search envelope · R {spawnRadius:0.##}");
            labels.Add($"Impact footprint reference · R {effectiveImpactRadius:0.##}");
            labels.Add("Exact placement depends on the assigned executor");
            DrawLabelColumn(
                labels.ToArray(),
                SpawnColor,
                new Vector3(Mathf.Max(spawnRadius, effectiveImpactRadius) + 0.6f, 0.2f, 0.2f));
        }
    }

    private static void DrawLabelColumn(string[] labels, Color color, Vector3 anchor)
    {
        if (!ShowLabels || labels == null) return;

        Handles.color = color;
        const float verticalSpacing = 0.38f;
        for (int i = 0; i < labels.Length; i++)
        {
            Vector3 position = anchor + Vector3.up * (i * verticalSpacing);
            Handles.Label(position, labels[i]);
        }
    }

    private static void DrawImpactPoint(Vector3 center, float radius, string label)
    {
        DrawCircle(center, radius, ImpactColor);
        if (ShowGeometry)
        {
            Handles.color = ImpactColor;
            Handles.DrawSolidDisc(center, Vector3.up, 0.08f);
        }
        if (ShowLabels && !string.IsNullOrEmpty(label))
            LabelAt(center + Vector3.forward * (radius + 0.35f), label, ImpactColor);
    }

    private static void DrawCircle(Vector3 center, float radius, Color color)
    {
        if (!ShowGeometry || radius <= 0f) return;
        Handles.color = color;
        Handles.DrawWireDisc(center, Vector3.up, radius);
    }

    private static void DrawLineWithEndpoints(Vector3 start, Vector3 end, Color color)
    {
        if (!ShowGeometry) return;
        Handles.color = color;
        Handles.DrawLine(start, end);
        Handles.DrawWireDisc(start, Vector3.up, 0.12f);
        Handles.DrawWireDisc(end, Vector3.up, 0.12f);
    }

    private static void LabelAt(Vector3 position, string text, Color color)
    {
        if (!ShowLabels) return;
        Handles.color = color;
        Handles.Label(position, text);
    }

    private static void DrawExecutionPrefabCollider(GameObject prefab, Vector3 origin)
    {
        Collider[] colliders = prefab.GetComponentsInChildren<Collider>(true);
        if (colliders == null || colliders.Length == 0) return;

        Matrix4x4 oldMatrix = Handles.matrix;
        Color oldColor = Handles.color;
        Handles.color = new Color(1f, 0.85f, 0.25f, 0.9f);

        foreach (Collider collider in colliders)
        {
            if (collider == null) continue;
            Matrix4x4 relative = Matrix4x4.TRS(origin, Quaternion.identity, Vector3.one) *
                                 prefab.transform.worldToLocalMatrix *
                                 collider.transform.localToWorldMatrix;
            Handles.matrix = relative;

            if (collider is SphereCollider sphere)
            {
                Handles.DrawWireDisc(sphere.center, Vector3.up, sphere.radius);
                Handles.DrawWireDisc(sphere.center, Vector3.right, sphere.radius);
                Handles.DrawWireDisc(sphere.center, Vector3.forward, sphere.radius);
            }
            else if (collider is BoxCollider box)
            {
                Handles.DrawWireCube(box.center, box.size);
            }
            else if (collider is CapsuleCollider capsule)
            {
                Handles.DrawWireDisc(capsule.center, Vector3.up, capsule.radius);
                Handles.DrawWireDisc(capsule.center + Vector3.up *
                    Mathf.Max(0f, capsule.height * 0.5f - capsule.radius),
                    Vector3.up, capsule.radius);
            }
        }

        Handles.matrix = oldMatrix;
        Handles.color = oldColor;
    }
}

internal abstract class PlayerSkillPreviewEditorBase : UnityEditor.Editor
{
    protected void DrawOptions()
    {
        PlayerSkillAuthoringPreview.DrawOptions();
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

[CustomEditor(typeof(SkillDefinition))]
internal sealed class SkillDefinitionAuthoringPreviewEditor : PlayerSkillPreviewEditorBase
{
    private void OnEnable() => Subscribe(DrawPreview);
    private void OnDisable() => Unsubscribe(DrawPreview);

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawOptions();
        EditorGUILayout.HelpBox(
            "Uses base SkillDefinition values. Runtime modifiers can change final radii, range and count.",
            MessageType.Info);
    }

    private void DrawPreview(SceneView view)
    {
        if (SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Player) && target != null && Selection.activeObject == target)
            PlayerSkillAuthoringPreview.DrawSkill((SkillDefinition)target);
    }
}

[CustomEditor(typeof(AreaStrikePatternDefinition))]
internal sealed class AreaStrikePatternAuthoringPreviewEditor : PlayerSkillPreviewEditorBase
{
    private void OnEnable() => Subscribe(DrawPreview);
    private void OnDisable() => Unsubscribe(DrawPreview);
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawOptions();
        EditorGUILayout.HelpBox("MultiPoint is randomized at runtime; preview shows its configured spawn envelope.", MessageType.Info);
    }
    private void DrawPreview(SceneView view)
    {
        if (SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Player) && target != null && Selection.activeObject == target)
            PlayerSkillAuthoringPreview.DrawPattern((AreaStrikePatternDefinition)target);
    }
}

[CustomEditor(typeof(ProjectilePatternDefinition))]
internal sealed class ProjectilePatternAuthoringPreviewEditor : PlayerSkillPreviewEditorBase
{
    private void OnEnable() => Subscribe(DrawPreview);
    private void OnDisable() => Unsubscribe(DrawPreview);
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawOptions();
        EditorGUILayout.HelpBox("RandomSpread shows its configured angular envelope, not a random runtime sample. TargetNearest shows acquisition range on SkillDefinition.", MessageType.Info);
    }
    private void DrawPreview(SceneView view)
    {
        if (SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Player) && target != null && Selection.activeObject == target)
            PlayerSkillAuthoringPreview.DrawPattern((ProjectilePatternDefinition)target);
    }
}

[CustomEditor(typeof(OrbitalPatternDefinition))]
internal sealed class OrbitalPatternAuthoringPreviewEditor : PlayerSkillPreviewEditorBase
{
    private void OnEnable() => Subscribe(DrawPreview);
    private void OnDisable() => Unsubscribe(DrawPreview);
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawOptions();
        EditorGUILayout.HelpBox("Shows orbit radius, vertical offset and configured formation. Runtime starting phase may be randomized.", MessageType.Info);
    }
    private void DrawPreview(SceneView view)
    {
        if (SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Player) && target != null && Selection.activeObject == target)
            PlayerSkillAuthoringPreview.DrawPattern((OrbitalPatternDefinition)target);
    }
}

[CustomEditor(typeof(PersistentZonePatternDefinition))]
internal sealed class PersistentZonePatternAuthoringPreviewEditor : PlayerSkillPreviewEditorBase
{
    private void OnEnable() => Subscribe(DrawPreview);
    private void OnDisable() => Unsubscribe(DrawPreview);
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawOptions();
        EditorGUILayout.HelpBox("MultiZone uses random positions at runtime; preview shows its spawn envelope. Damage radius comes from SkillDefinition.BaseImpactRadius.", MessageType.Info);
    }
    private void DrawPreview(SceneView view)
    {
        if (SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Player) && target != null && Selection.activeObject == target)
            PlayerSkillAuthoringPreview.DrawPattern((PersistentZonePatternDefinition)target);
    }
}
#endif
