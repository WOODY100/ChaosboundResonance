#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Chaosbound.Gameplay.Debugging.SkillGizmos;

/// <summary>
/// Editor-only authoring preview for Enemy combat configuration.
/// Geometry is drawn around the Scene origin using +Z as the reference facing.
/// </summary>
internal static class EnemyAuthoringPreview
{
    private const string GeometryKey = "Chaosbound.EnemyAuthoringPreview.Geometry";
    private const string LabelsKey = "Chaosbound.EnemyAuthoringPreview.Labels";
    private const string ColliderKey = "Chaosbound.EnemyAuthoringPreview.ProjectileCollider";

    private static readonly Color MeleeColor = new Color(1f, 0.32f, 0.16f, 0.95f);
    private static readonly Color RangeColor = new Color(1f, 0.68f, 0.18f, 0.9f);
    private static readonly Color PreferredColor = new Color(0.2f, 0.85f, 1f, 0.9f);
    private static readonly Color ProjectileColor = new Color(0.25f, 0.9f, 1f, 0.95f);
    private static readonly Color ColliderColor = new Color(1f, 0.85f, 0.25f, 0.95f);

    public static bool ShowGeometry
    {
        get => SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Enemy) &&
               SkillGizmoUtility.IsLayerEnabled(SkillGizmoUtility.VisualLayer.Geometry) &&
               EditorPrefs.GetBool(GeometryKey, true);
        set => EditorPrefs.SetBool(GeometryKey, value);
    }

    public static bool ShowLabels
    {
        get => SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Enemy) &&
               SkillGizmoUtility.IsLayerEnabled(SkillGizmoUtility.VisualLayer.Labels) &&
               EditorPrefs.GetBool(LabelsKey, true);
        set => EditorPrefs.SetBool(LabelsKey, value);
    }

    public static bool ShowProjectileCollider
    {
        get => SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Enemy) &&
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
        ShowProjectileCollider = EditorGUILayout.ToggleLeft(
            "Show projectile prefab collider", ShowProjectileCollider);
        if (EditorGUI.EndChangeCheck())
            SceneView.RepaintAll();
    }

    public static void DrawVariant(EnemyVariantData variant)
    {
        if (variant == null) return;

        EnemyAttackDefinition attack = variant.Attack;
        if (attack == null)
        {
            DrawLabelColumn(new[]
            {
                variant.DisplayName,
                "No EnemyAttackDefinition assigned"
            }, Color.white, new Vector3(1f, 0.15f, 0.5f));
            return;
        }

        float attackRange = Mathf.Max(0f, attack.Range);
        float preferred = Mathf.Max(0f, variant.PreferredDistance);
        float tolerance = Mathf.Max(0f, variant.DistanceTolerance);

        if (ShowGeometry)
        {
            // EnemyCombat validates distance and angle; it does not use a circular damage overlap.
            DrawGroundCircle(Vector3.zero, attackRange, new Color(MeleeColor.r, MeleeColor.g, MeleeColor.b, 0.35f));
            DrawArc(Vector3.zero, Vector3.forward, attackRange, attack.Angle, MeleeColor);

            if (preferred > 0f)
            {
                DrawGroundCircle(Vector3.zero, preferred, PreferredColor);
                DrawGroundCircle(Vector3.zero, preferred + tolerance,
                    new Color(PreferredColor.r, PreferredColor.g, PreferredColor.b, 0.35f));
            }
        }

        if (attack.Ability != null &&
            attack.Ability.ExecutionType == EnemyAbilityExecutionType.Projectile)
        {
            Vector3 projectileOrigin = GetAbilityOriginLocalPosition(variant.SpawnPrefab);

            DrawProjectile(
                attack.Ability,
                attackRange,
                projectileOrigin,
                Vector3.forward,
                true);

            if (ShowLabels)
            {
                DrawLabelColumn(new[]
                {
                    $"Projectile origin · {FormatVector(projectileOrigin)}",
                    variant.SpawnPrefab != null
                        ? "Origin source · SpawnPrefab / EnemyCombat"
                        : "Origin source · Enemy root fallback"
                }, ProjectileColor, projectileOrigin + new Vector3(0.7f, 0.2f, 0.5f));
            }
        }

        if (ShowLabels)
        {
            DrawLabelColumn(new[]
            {
                string.IsNullOrEmpty(variant.DisplayName) ? variant.name : variant.DisplayName,
                $"Attack range · {attackRange:0.##}",
                $"Attack angle · {Mathf.Clamp(attack.Angle, 0f, 360f):0.##}°",
                $"Preferred distance · {preferred:0.##}",
                $"Distance tolerance · {tolerance:0.##}",
                $"Impact time · {attack.ImpactNormalizedTime * 100f:0.#}%"
            }, MeleeColor, new Vector3(attackRange + 0.8f, 0.2f, 0.4f));
        }
    }

    public static void DrawAbility(EnemyAbilityDefinition ability)
    {
        if (ability == null) return;

        if (ability.ExecutionType == EnemyAbilityExecutionType.Projectile)
        {
            // The ability asset does not own EnemyAttackDefinition.Range.
            // Therefore only its extension is known in this isolated preview.
            float extension = Mathf.Max(0f, ability.ProjectileRangeExtension);
            if (ShowGeometry)
            {
                DrawDirection(Vector3.zero, Vector3.forward, extension, ProjectileColor);
                DrawPrefabColliders(ability.ProjectilePrefab, Vector3.zero);
            }

            if (ShowLabels)
            {
                DrawLabelColumn(new[]
                {
                    string.IsNullOrEmpty(ability.DisplayName) ? ability.name : ability.DisplayName,
                    $"Projectile speed · {ability.ProjectileSpeed:0.##}",
                    $"Range extension · {extension:0.##}",
                    "Total travel requires owning EnemyAttackDefinition.Range"
                }, ProjectileColor, new Vector3(1f, 0.2f, 0.5f));
            }
        }
        else if (ShowLabels)
        {
            DrawLabelColumn(new[]
            {
                string.IsNullOrEmpty(ability.DisplayName) ? ability.name : ability.DisplayName,
                $"Execution type · {ability.ExecutionType}"
            }, Color.white, new Vector3(1f, 0.2f, 0.5f));
        }
    }

    private static Vector3 GetAbilityOriginLocalPosition(GameObject spawnPrefab)
    {
        if (spawnPrefab == null)
            return Vector3.zero;

        EnemyCombat combat = spawnPrefab.GetComponentInChildren<EnemyCombat>(true);
        if (combat == null)
            return Vector3.zero;

        // abilityOrigin is intentionally private on EnemyCombat. Read the serialized
        // prefab reference without changing runtime visibility or adding data fields.
        SerializedObject serializedCombat = new SerializedObject(combat);
        SerializedProperty originProperty = serializedCombat.FindProperty("abilityOrigin");

        if (originProperty == null ||
            originProperty.propertyType != SerializedPropertyType.ObjectReference)
            return Vector3.zero;

        Transform abilityOrigin = originProperty.objectReferenceValue as Transform;
        if (abilityOrigin == null)
            return Vector3.zero;

        return spawnPrefab.transform.InverseTransformPoint(abilityOrigin.position);
    }

    private static string FormatVector(Vector3 value)
    {
        return $"({value.x:0.##}, {value.y:0.##}, {value.z:0.##})";
    }

    private static void DrawProjectile(
        EnemyAbilityDefinition ability,
        float attackRange,
        Vector3 origin,
        Vector3 direction,
        bool drawTotalTravel)
    {
        float speed = Mathf.Max(0f, ability.ProjectileSpeed);
        float totalDistance = Mathf.Max(0f, attackRange + ability.ProjectileRangeExtension);
        float lifetime = speed > 0f ? totalDistance / speed : 0f;
        Vector3 end = origin + direction.normalized * totalDistance;

        if (ShowGeometry)
        {
            DrawDirection(origin, direction, totalDistance, ProjectileColor);
            DrawPrefabColliders(ability.ProjectilePrefab, origin);
        }

        if (ShowLabels)
        {
            DrawLabelColumn(new[]
            {
                $"Projectile speed · {speed:0.##}",
                $"Attack range · {attackRange:0.##}",
                $"Range extension · {ability.ProjectileRangeExtension:0.##}",
                $"Total travel · {totalDistance:0.##}",
                $"Lifetime · {lifetime:0.##} s"
            }, ProjectileColor, end + new Vector3(0.6f, 0.2f, 0.3f));
        }
    }

    private static void DrawGroundCircle(Vector3 center, float radius, Color color)
    {
        if (!ShowGeometry || radius <= 0f) return;
        Handles.color = color;
        Handles.DrawWireDisc(center, Vector3.up, radius);
    }

    private static void DrawArc(Vector3 origin, Vector3 forward, float radius, float angle, Color color)
    {
        if (!ShowGeometry || radius <= 0f) return;

        angle = Mathf.Clamp(angle, 0f, 360f);
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f) forward = Vector3.forward;
        forward.Normalize();

        Handles.color = color;
        Vector3 startDirection = Quaternion.Euler(0f, -angle * 0.5f, 0f) * forward;
        Handles.DrawWireArc(origin, Vector3.up, startDirection, angle, radius);
        Handles.DrawLine(origin, origin + startDirection * radius);
        Handles.DrawLine(origin,
            origin + Quaternion.Euler(0f, angle * 0.5f, 0f) * forward * radius);
    }

    private static void DrawDirection(Vector3 origin, Vector3 direction, float distance, Color color)
    {
        if (!ShowGeometry || direction.sqrMagnitude < 0.0001f || distance <= 0f) return;

        Vector3 end = origin + direction.normalized * distance;
        Handles.color = color;
        Handles.DrawLine(origin, end);
        Handles.ConeHandleCap(
            0, end, Quaternion.LookRotation(direction.normalized),
            Mathf.Clamp(distance * 0.035f, 0.12f, 0.45f),
            EventType.Repaint);
        Handles.DrawWireDisc(origin, Vector3.up, 0.16f);
    }

    private static void DrawPrefabColliders(GameObject prefab, Vector3 origin)
    {
        if (!ShowGeometry || !ShowProjectileCollider || prefab == null) return;

        Collider[] colliders = prefab.GetComponentsInChildren<Collider>(true);
        if (colliders == null || colliders.Length == 0) return;

        Transform prefabTransform = prefab.transform;
        Matrix4x4 oldMatrix = Handles.matrix;
        Color oldColor = Handles.color;
        Handles.color = ColliderColor;

        foreach (Collider collider in colliders)
        {
            if (collider == null) continue;

            Matrix4x4 relativeMatrix =
                Matrix4x4.TRS(origin, Quaternion.identity, Vector3.one) *
                prefabTransform.worldToLocalMatrix *
                collider.transform.localToWorldMatrix;

            Handles.matrix = relativeMatrix;

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
                Vector3 axis = capsule.direction == 0 ? Vector3.right :
                    capsule.direction == 2 ? Vector3.forward : Vector3.up;
                float halfSegment = Mathf.Max(0f, capsule.height * 0.5f - capsule.radius);
                Vector3 top = capsule.center + axis * halfSegment;
                Vector3 bottom = capsule.center - axis * halfSegment;

                Handles.DrawWireDisc(top, axis, capsule.radius);
                Handles.DrawWireDisc(bottom, axis, capsule.radius);
                Handles.DrawLine(top + Vector3.up * capsule.radius,
                    bottom + Vector3.up * capsule.radius);
                Handles.DrawLine(top - Vector3.up * capsule.radius,
                    bottom - Vector3.up * capsule.radius);
            }
        }

        Handles.matrix = oldMatrix;
        Handles.color = oldColor;
    }

    private static void DrawLabelColumn(string[] labels, Color color, Vector3 anchor)
    {
        if (!ShowLabels || labels == null) return;

        Handles.color = color;
        const float spacing = 0.38f;
        for (int i = 0; i < labels.Length; i++)
            Handles.Label(anchor + Vector3.up * (i * spacing), labels[i]);
    }
}

internal abstract class EnemyAuthoringPreviewEditorBase : UnityEditor.Editor
{
    protected void DrawOptions() => EnemyAuthoringPreview.DrawOptions();
    protected void Subscribe(System.Action<SceneView> callback) => SceneView.duringSceneGui += callback;
    protected void Unsubscribe(System.Action<SceneView> callback) => SceneView.duringSceneGui -= callback;
}

[CustomEditor(typeof(EnemyVariantData))]
internal sealed class EnemyVariantDataAuthoringPreviewEditor : EnemyAuthoringPreviewEditorBase
{
    private void OnEnable() => Subscribe(DrawPreview);
    private void OnDisable() => Unsubscribe(DrawPreview);

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawOptions();
        EditorGUILayout.HelpBox(
            "Preview uses EnemyAttackDefinition values from this variant. Melee range is centered on the Enemy root, matching EnemyCombat distance checks. Projectile origin is read from the assigned SpawnPrefab's EnemyCombat. If no abilityOrigin is assigned, the Enemy root is used. +Z is the reference facing.",
            MessageType.Info);
    }

    private void DrawPreview(SceneView view)
    {
        if (SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Enemy) && target != null && Selection.activeObject == target)
            EnemyAuthoringPreview.DrawVariant((EnemyVariantData)target);
    }
}

[CustomEditor(typeof(EnemyAbilityDefinition))]
internal sealed class EnemyAbilityDefinitionAuthoringPreviewEditor : EnemyAuthoringPreviewEditorBase
{
    private void OnEnable() => Subscribe(DrawPreview);
    private void OnDisable() => Unsubscribe(DrawPreview);

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawOptions();
        EditorGUILayout.HelpBox(
            "This asset alone does not contain the owning EnemyAttackDefinition.Range. Its preview shows ProjectileRangeExtension only; select the owning EnemyVariantData to see total travel distance.",
            MessageType.Info);
    }

    private void DrawPreview(SceneView view)
    {
        if (SkillGizmoUtility.Enabled && SkillGizmoUtility.IsCategoryEnabled(SkillGizmoUtility.Category.Enemy) && target != null && Selection.activeObject == target)
            EnemyAuthoringPreview.DrawAbility((EnemyAbilityDefinition)target);
    }
}
#endif
