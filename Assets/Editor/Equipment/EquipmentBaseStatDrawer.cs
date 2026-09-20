using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Equipment;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EquipmentBaseStat))]
public sealed class EquipmentBaseStatDrawer : PropertyDrawer
{
    private const float VerticalSpacing = 2f;

    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label)
    {
        return
            EditorGUIUtility.singleLineHeight * 3f
            + VerticalSpacing * 2f;
    }

    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        SerializedProperty statType =
            property.FindPropertyRelative("statType");

        SerializedProperty modifierType =
            property.FindPropertyRelative("modifierType");

        SerializedProperty value =
            property.FindPropertyRelative("value");

        float lineHeight =
            EditorGUIUtility.singleLineHeight;

        Rect statRect =
            new Rect(
                position.x,
                position.y,
                position.width,
                lineHeight);

        Rect modifierRect =
            new Rect(
                position.x,
                position.y + lineHeight + VerticalSpacing,
                position.width,
                lineHeight);

        Rect valueRect =
            new Rect(
                position.x,
                position.y +
                (lineHeight + VerticalSpacing) * 2f,
                position.width,
                lineHeight);

        EditorGUI.PropertyField(
            statRect,
            statType,
            new GUIContent("Stat Type"));

        EditorGUI.PropertyField(
            modifierRect,
            modifierType,
            new GUIContent("Modifier Type"));

        bool isPercent =
            modifierType.enumValueIndex ==
            (int)ModifierType.Percent;

        if (isPercent)
        {
            float internalValue =
                value.floatValue;

            float displayedValue =
                internalValue * 100f;

            EditorGUI.BeginChangeCheck();

            float newDisplayedValue =
                EditorGUI.FloatField(
                    valueRect,
                    new GUIContent("Value"),
                    displayedValue);

            if (EditorGUI.EndChangeCheck())
            {
                value.floatValue =
                    newDisplayedValue / 100f;
            }
        }
        else
        {
            EditorGUI.PropertyField(
                valueRect,
                value,
                new GUIContent("Value"));
        }
    }
}