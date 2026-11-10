#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Chaosbound.Gameplay.Debugging.SkillGizmos
{
    public sealed class SkillGizmoSettingsWindow : EditorWindow
    {
        private const string SelectedOnlyKey = "Chaosbound.SkillGizmos.SelectedOnly";
        private const string CategoryPlayerKey = "Chaosbound.SkillGizmos.Category.Player";
        private const string CategoryEnemyKey = "Chaosbound.SkillGizmos.Category.Enemy";
        private const string CategoryBossKey = "Chaosbound.SkillGizmos.Category.Boss";
        private const string GeometryKey = "Chaosbound.SkillGizmos.Layer.Geometry";
        private const string LabelsKey = "Chaosbound.SkillGizmos.Layer.Labels";
        private const string CollidersKey = "Chaosbound.SkillGizmos.Layer.Colliders";
        private const string TrajectoriesKey = "Chaosbound.SkillGizmos.Layer.Trajectories";

        [MenuItem("Chaosbound/Debugging/Skill Gizmo Settings")]
        public static void Open()
        {
            GetWindow<SkillGizmoSettingsWindow>("Skill Gizmos");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Skill Gizmos & Combat Debugging", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Central controls for Runtime Debug and Authoring Preview. These preferences are Editor-only and do not modify gameplay assets.",
                MessageType.Info);

            EditorGUI.BeginChangeCheck();
            SkillGizmoUtility.Enabled = EditorGUILayout.ToggleLeft(
                "Enable all debug visualization", SkillGizmoUtility.Enabled);
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Runtime selection", EditorStyles.boldLabel);
            EditorPrefs.SetBool(SelectedOnlyKey, EditorGUILayout.ToggleLeft(
                "Selected objects only", EditorPrefs.GetBool(SelectedOnlyKey, true)));

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Categories", EditorStyles.boldLabel);
            DrawPrefToggle(CategoryPlayerKey, "Player", true);
            DrawPrefToggle(CategoryEnemyKey, "Enemy", true);
            DrawPrefToggle(CategoryBossKey, "Boss", true);

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Visual layers", EditorStyles.boldLabel);
            DrawPrefToggle(GeometryKey, "Geometry", true);
            DrawPrefToggle(LabelsKey, "Labels", true);
            DrawPrefToggle(CollidersKey, "Colliders", true);
            DrawPrefToggle(TrajectoriesKey, "Trajectories", true);

            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();

            EditorGUILayout.Space(8f);
            if (GUILayout.Button("Restore defaults"))
            {
                SkillGizmoUtility.Enabled = true;
                EditorPrefs.SetBool(SelectedOnlyKey, true);
                EditorPrefs.SetBool(CategoryPlayerKey, true);
                EditorPrefs.SetBool(CategoryEnemyKey, true);
                EditorPrefs.SetBool(CategoryBossKey, true);
                EditorPrefs.SetBool(GeometryKey, true);
                EditorPrefs.SetBool(LabelsKey, true);
                EditorPrefs.SetBool(CollidersKey, true);
                EditorPrefs.SetBool(TrajectoriesKey, true);
                SceneView.RepaintAll();
            }
        }

        private static void DrawPrefToggle(string key, string label, bool defaultValue)
        {
            EditorPrefs.SetBool(key, EditorGUILayout.ToggleLeft(
                label, EditorPrefs.GetBool(key, defaultValue)));
        }
    }
}
#endif
