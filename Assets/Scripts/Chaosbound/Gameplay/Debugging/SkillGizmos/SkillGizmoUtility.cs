#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Chaosbound.Gameplay.Debugging.SkillGizmos
{
    /// <summary>Shared Editor-only geometry used by combat and skill gizmos.</summary>
    public static class SkillGizmoUtility
    {
        private const string EnabledKey = "Chaosbound.SkillGizmos.Enabled";
        private const string SelectedOnlyKey = "Chaosbound.SkillGizmos.SelectedOnly";

        public enum Category { Player, Enemy, Boss }
        public enum VisualLayer { Geometry, Labels, Colliders, Trajectories }

        public static bool Enabled
        {
            get => EditorPrefs.GetBool(EnabledKey, true);
            set => EditorPrefs.SetBool(EnabledKey, value);
        }

        public static bool SelectedOnly
        {
            get => EditorPrefs.GetBool(SelectedOnlyKey, true);
            set => EditorPrefs.SetBool(SelectedOnlyKey, value);
        }

        public static bool ShouldDraw(Component component, bool selectedCallback)
        {
            return ShouldDraw(component, selectedCallback, Category.Player);
        }

        public static bool ShouldDraw(Component component, bool selectedCallback, Category category)
        {
            if (!Enabled || component == null || !IsCategoryEnabled(category))
                return false;

            return !SelectedOnly || selectedCallback || Selection.Contains(component.gameObject);
        }

        public static bool IsCategoryEnabled(Category category)
        {
            switch (category)
            {
                case Category.Player: return EditorPrefs.GetBool("Chaosbound.SkillGizmos.Category.Player", true);
                case Category.Enemy: return EditorPrefs.GetBool("Chaosbound.SkillGizmos.Category.Enemy", true);
                case Category.Boss: return EditorPrefs.GetBool("Chaosbound.SkillGizmos.Category.Boss", true);
                default: return false;
            }
        }

        public static bool IsLayerEnabled(VisualLayer layer)
        {
            switch (layer)
            {
                case VisualLayer.Geometry: return EditorPrefs.GetBool("Chaosbound.SkillGizmos.Layer.Geometry", true);
                case VisualLayer.Labels: return EditorPrefs.GetBool("Chaosbound.SkillGizmos.Layer.Labels", true);
                case VisualLayer.Colliders: return EditorPrefs.GetBool("Chaosbound.SkillGizmos.Layer.Colliders", true);
                case VisualLayer.Trajectories: return EditorPrefs.GetBool("Chaosbound.SkillGizmos.Layer.Trajectories", true);
                default: return false;
            }
        }

        public static void DrawGroundCircle(Vector3 center, float radius, Color color, int segments = 64)
        {
            if (!IsLayerEnabled(VisualLayer.Geometry) || radius <= 0f) return;
            Gizmos.color = color;
            segments = Mathf.Max(12, segments);
            Vector3 previous = center + Vector3.right * radius;
            for (int i = 1; i <= segments; i++)
            {
                float angle = Mathf.PI * 2f * i / segments;
                Vector3 next = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                Gizmos.DrawLine(previous, next);
                previous = next;
            }
        }

        public static void DrawWireSphere(Vector3 center, float radius, Color color)
        {
            if (!IsLayerEnabled(VisualLayer.Geometry) || radius <= 0f) return;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(center, radius);
        }

        public static void DrawDirection(Vector3 origin, Vector3 direction, float length, Color color)
        {
            if (!IsLayerEnabled(VisualLayer.Trajectories) || direction.sqrMagnitude <= 0.0001f || length <= 0f) return;
            Gizmos.color = color;
            Vector3 end = origin + direction.normalized * length;
            Gizmos.DrawLine(origin, end);
            Gizmos.DrawSphere(end, Mathf.Max(0.04f, length * 0.025f));
        }

        public static void DrawArc(Vector3 origin, Vector3 forward, float radius, float angle, Color color, int segments = 32)
        {
            if (!IsLayerEnabled(VisualLayer.Geometry) || radius <= 0f || forward.sqrMagnitude <= 0.0001f) return;
            forward.y = 0f;
            forward.Normalize();
            Vector3 previous = origin + Quaternion.Euler(0f, -angle * 0.5f, 0f) * forward * radius;
            Gizmos.color = color;
            segments = Mathf.Max(4, segments);
            for (int i = 1; i <= segments; i++)
            {
                float t = (float)i / segments;
                Vector3 next = origin + Quaternion.Euler(0f, -angle * 0.5f + angle * t, 0f) * forward * radius;
                Gizmos.DrawLine(previous, next);
                previous = next;
            }
            Gizmos.DrawLine(origin, origin + Quaternion.Euler(0f, -angle * 0.5f, 0f) * forward * radius);
            Gizmos.DrawLine(origin, origin + Quaternion.Euler(0f, angle * 0.5f, 0f) * forward * radius);
        }

        public static void DrawCollider(Collider collider, Color color)
        {
            if (!IsLayerEnabled(VisualLayer.Colliders) || collider == null) return;
            Gizmos.color = color;
            if (collider is SphereCollider sphere)
            {
                Vector3 center = sphere.transform.TransformPoint(sphere.center);
                Vector3 scale = sphere.transform.lossyScale;
                float radius = sphere.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
                Gizmos.DrawWireSphere(center, radius);
            }
            else if (collider is BoxCollider box)
            {
                Matrix4x4 old = Gizmos.matrix;
                Gizmos.matrix = box.transform.localToWorldMatrix;
                Gizmos.DrawWireCube(box.center, box.size);
                Gizmos.matrix = old;
            }
            else if (collider is CapsuleCollider capsule)
            {
                Vector3 center = capsule.transform.TransformPoint(capsule.center);
                Vector3 scale = capsule.transform.lossyScale;
                float radius = capsule.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
                float height = Mathf.Max(capsule.height * Mathf.Abs(scale.y), radius * 2f);
                Gizmos.DrawWireSphere(center + Vector3.up * (height * 0.5f - radius), radius);
                Gizmos.DrawWireSphere(center - Vector3.up * (height * 0.5f - radius), radius);
                Gizmos.DrawLine(center + Vector3.up * (height * 0.5f - radius), center - Vector3.up * (height * 0.5f - radius));
            }
        }
    }
}
#endif
