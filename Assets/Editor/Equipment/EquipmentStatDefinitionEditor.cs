using UnityEditor;
using UnityEngine;
using Chaosbound.Gameplay.Equipment;

[CustomEditor(typeof(EquipmentStatDefinition))]
public sealed class EquipmentStatDefinitionEditor : Editor
{
    private SerializedProperty statType;
    private SerializedProperty modifierType;
    private SerializedProperty minimumValue;
    private SerializedProperty maximumValue;
    private SerializedProperty upgradeGrowth;
    private SerializedProperty weight;

    private void OnEnable()
    {
        statType =
            serializedObject.FindProperty("statType");

        modifierType =
            serializedObject.FindProperty("modifierType");

        minimumValue =
            serializedObject.FindProperty("minimumValue");

        maximumValue =
            serializedObject.FindProperty("maximumValue");

        upgradeGrowth =
            serializedObject.FindProperty("upgradeGrowth");

        weight =
            serializedObject.FindProperty("weight");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField(
            "Stat",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            statType);

        EditorGUILayout.PropertyField(
            modifierType);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Initial Roll",
            EditorStyles.boldLabel);

        bool isPercent =
            modifierType.enumValueIndex ==
            (int)ModifierType.Percent;

        if (isPercent)
        {
            DrawPercentProperty(
                minimumValue,
                "Minimum Value");

            DrawPercentProperty(
                maximumValue,
                "Maximum Value");
        }
        else
        {
            EditorGUILayout.PropertyField(
                minimumValue,
                new GUIContent("Minimum Value"));

            EditorGUILayout.PropertyField(
                maximumValue,
                new GUIContent("Maximum Value"));
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Upgrade Growth",
            EditorStyles.boldLabel);

        if (isPercent)
        {
            DrawPercentProperty(
                upgradeGrowth,
                "Upgrade Growth");
        }
        else
        {
            EditorGUILayout.PropertyField(
                upgradeGrowth,
                new GUIContent("Upgrade Growth"));
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Selection Weight",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            weight,
            new GUIContent("Weight"));

        serializedObject.ApplyModifiedProperties();
    }

    private static void DrawPercentProperty(
    SerializedProperty property,
    string label)
    {
        float internalValue =
            property.floatValue;

        float displayedValue =
            internalValue * 100f;

        EditorGUI.BeginChangeCheck();

        float newDisplayedValue =
            EditorGUILayout.FloatField(
                new GUIContent(label),
                displayedValue);

        if (EditorGUI.EndChangeCheck())
        {
            property.floatValue =
                newDisplayedValue / 100f;
        }
    }
}