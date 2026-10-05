using Chaosbound.SkillTree;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillTreeNodeDefinition))]
public sealed class SkillTreeNodeDefinitionEditor : Editor
{
    private SerializedProperty nodeId;
    private SerializedProperty displayName;
    private SerializedProperty description;
    private SerializedProperty icon;

    private SerializedProperty branch;
    private SerializedProperty nodeType;
    private SerializedProperty cost;

    private SerializedProperty requirement;
    private SerializedProperty effects;
    private SerializedProperty choiceOptions;

    private void OnEnable()
    {
        nodeId = serializedObject.FindProperty("NodeId");
        displayName = serializedObject.FindProperty("DisplayName");
        description = serializedObject.FindProperty("Description");
        icon = serializedObject.FindProperty("Icon");

        branch = serializedObject.FindProperty("Branch");
        nodeType = serializedObject.FindProperty("NodeType");
        cost = serializedObject.FindProperty("Cost");

        requirement = serializedObject.FindProperty("Requirement");
        effects = serializedObject.FindProperty("Effects");
        choiceOptions = serializedObject.FindProperty("ChoiceOptions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField(
            "Identity",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(nodeId);

        EditorGUILayout.Space(4);

        EditorGUILayout.LabelField(
            "Presentation",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(displayName);

        EditorGUILayout.PropertyField(description);

        EditorGUILayout.PropertyField(icon);

        EditorGUILayout.Space(4);

        EditorGUILayout.LabelField(
            "Structure",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(branch);

        EditorGUILayout.PropertyField(nodeType);

        EditorGUILayout.Space(4);

        EditorGUILayout.LabelField(
            "Cost",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(cost);

        EditorGUILayout.Space(4);

        DrawRequirement();

        EditorGUILayout.Space(8);

        DrawEffects();

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField(
            "Choice",
            EditorStyles.boldLabel);

        if (nodeType.enumValueIndex ==
            (int)SkillTreeNodeType.Choice)
        {
            EditorGUILayout.HelpBox(
                "Choice nodes store effects inside their Choice Options.",
                MessageType.Info);

            DrawChoiceOptions();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawRequirement()
    {
        EditorGUILayout.LabelField(
            "Requirement",
            EditorStyles.boldLabel);

        DrawRequirementProperty(
            requirement,
            "Requirement",
            true);
    }

    private void DrawRequirementProperty(
    SerializedProperty property,
    string label,
    bool allowRemove)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        if (property.managedReferenceValue == null)
        {
            EditorGUILayout.LabelField(
                label,
                EditorStyles.boldLabel);

            if (GUILayout.Button("Add Requirement"))
            {
                ShowRequirementMenu(property);
            }

            EditorGUILayout.EndVertical();
            return;
        }

        NodeRequirement requirementValue =
            property.managedReferenceValue as NodeRequirement;

        EditorGUILayout.LabelField(
            requirementValue.GetType().Name,
            EditorStyles.boldLabel);

        if (requirementValue is NodePurchasedRequirement)
        {
            SerializedProperty nodeIdProperty =
                property.FindPropertyRelative("NodeId");

            EditorGUILayout.PropertyField(
                nodeIdProperty,
                new GUIContent("Node Id"));

            if (allowRemove &&
                GUILayout.Button("Remove Requirement"))
            {
                property.managedReferenceValue = null;
            }
        }
        else if (requirementValue is AllRequirement ||
                 requirementValue is AnyRequirement)
        {
            DrawCompositeRequirement(
                property,
                requirementValue,
                allowRemove);
        }
        else
        {
            EditorGUILayout.HelpBox(
                $"Unsupported requirement type: {requirementValue.GetType().Name}",
                MessageType.Error);

            if (allowRemove &&
                GUILayout.Button("Remove Requirement"))
            {
                property.managedReferenceValue = null;
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawCompositeRequirement(
    SerializedProperty property,
    NodeRequirement requirementValue,
    bool allowRemove)
    {
        SerializedProperty requirements =
            property.FindPropertyRelative("Requirements");

        if (requirementValue is AllRequirement)
        {
            EditorGUILayout.LabelField(
                "All Requirements",
                EditorStyles.boldLabel);
        }
        else
        {
            EditorGUILayout.LabelField(
                "Any Requirements",
                EditorStyles.boldLabel);
        }

        if (requirements != null)
        {
            for (int i = 0; i < requirements.arraySize; i++)
            {
                SerializedProperty child =
                    requirements.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginVertical(
                    EditorStyles.helpBox);

                DrawRequirementProperty(
                    child,
                    $"Requirement {i + 1}",
                    true);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(4);

            if (GUILayout.Button("Add Requirement"))
            {
                ShowRequirementArrayMenu(requirements);
            }
        }

        if (allowRemove)
        {
            EditorGUILayout.Space(4);

            if (GUILayout.Button("Remove Requirement"))
            {
                property.managedReferenceValue = null;
            }
        }
    }

    private void DrawEffects()
    {
        EditorGUILayout.LabelField(
            "Effects",
            EditorStyles.boldLabel);

        DrawEffectArray(
            effects,
            "Node Effects");
    }

    private void DrawEffectArray(
    SerializedProperty effectsProperty,
    string label)
    {
        EditorGUILayout.BeginVertical(
            EditorStyles.helpBox);

        EditorGUILayout.LabelField(
            label,
            EditorStyles.boldLabel);

        if (effectsProperty.arraySize == 0)
        {
            EditorGUILayout.HelpBox(
                "No effects.",
                MessageType.None);
        }

        for (int i = 0;
             i < effectsProperty.arraySize;
             i++)
        {
            SerializedProperty element =
                effectsProperty.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical(
                EditorStyles.helpBox);

            EditorGUILayout.PropertyField(
                element,
                true);

            if (GUILayout.Button("Remove Effect"))
            {
                effectsProperty.DeleteArrayElementAtIndex(i);
                EditorGUILayout.EndVertical();
                break;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Effect"))
        {
            ShowEffectMenu(effectsProperty);
        }

        EditorGUILayout.EndVertical();
    }

    private void ShowRequirementMenu(
    SerializedProperty targetProperty)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(
            new GUIContent("Node Purchased Requirement"),
            false,
            () => CreateRequirement<NodePurchasedRequirement>(
                targetProperty));

        menu.AddItem(
            new GUIContent("All Requirement"),
            false,
            () => CreateRequirement<AllRequirement>(
                targetProperty));

        menu.AddItem(
            new GUIContent("Any Requirement"),
            false,
            () => CreateRequirement<AnyRequirement>(
                targetProperty));

        menu.ShowAsContext();
    }

    private void ShowRequirementArrayMenu(
    SerializedProperty requirements)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(
            new GUIContent("Node Purchased Requirement"),
            false,
            () => AddRequirementToArray<NodePurchasedRequirement>(
                requirements));

        menu.AddItem(
            new GUIContent("All Requirement"),
            false,
            () => AddRequirementToArray<AllRequirement>(
                requirements));

        menu.AddItem(
            new GUIContent("Any Requirement"),
            false,
            () => AddRequirementToArray<AnyRequirement>(
                requirements));

        menu.ShowAsContext();
    }

    private void AddRequirementToArray<T>(
    SerializedProperty requirements)
    where T : NodeRequirement
    {
        serializedObject.Update();

        int index = requirements.arraySize;

        requirements.InsertArrayElementAtIndex(index);

        SerializedProperty element =
            requirements.GetArrayElementAtIndex(index);

        element.managedReferenceValue =
            Activator.CreateInstance(typeof(T));

        serializedObject.ApplyModifiedProperties();
    }

    private void ShowEffectMenu(
    SerializedProperty targetProperty)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(
            new GUIContent("Player Stat Effect"),
            false,
            () => AddEffect<PlayerStatEffect>(
                targetProperty));

        menu.AddItem(
            new GUIContent("Global Skill Modifier Effect"),
            false,
            () => AddEffect<GlobalSkillModifierEffect>(
                targetProperty));

        menu.AddItem(
            new GUIContent("Rule Effect"),
            false,
            () => AddEffect<RuleEffect>(
                targetProperty));

        menu.ShowAsContext();
    }

    private void CreateRequirement<T>(
    SerializedProperty targetProperty)
    where T : NodeRequirement
    {
        serializedObject.Update();

        targetProperty.managedReferenceValue =
            Activator.CreateInstance(typeof(T));

        serializedObject.ApplyModifiedProperties();
    }

    private void AddEffect<T>(
    SerializedProperty effectsProperty)
    where T : SkillTreeEffect
    {
        serializedObject.Update();

        int index = effectsProperty.arraySize;

        effectsProperty.InsertArrayElementAtIndex(index);

        SerializedProperty element =
            effectsProperty.GetArrayElementAtIndex(index);

        element.managedReferenceValue =
            Activator.CreateInstance(typeof(T));

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawChoiceOptions()
    {
        EditorGUILayout.LabelField(
            "Choice Options",
            EditorStyles.boldLabel);

        if (choiceOptions.arraySize == 0)
        {
            EditorGUILayout.HelpBox(
                "A Choice node requires at least two options.",
                MessageType.Warning);
        }

        for (int i = 0;
             i < choiceOptions.arraySize;
             i++)
        {
            SerializedProperty option =
                choiceOptions.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical(
                EditorStyles.helpBox);

            EditorGUILayout.LabelField(
                $"Option {i + 1}",
                EditorStyles.boldLabel);

            SerializedProperty optionId =
                option.FindPropertyRelative("OptionId");

            SerializedProperty displayName =
                option.FindPropertyRelative("DisplayName");

            SerializedProperty description =
                option.FindPropertyRelative("Description");

            SerializedProperty icon =
                option.FindPropertyRelative("Icon");

            SerializedProperty optionEffects =
                option.FindPropertyRelative("Effects");

            EditorGUILayout.PropertyField(
                optionId);

            EditorGUILayout.PropertyField(
                displayName);

            EditorGUILayout.PropertyField(
                description);

            EditorGUILayout.PropertyField(
                icon);

            EditorGUILayout.Space(4);

            DrawEffectArray(
                optionEffects,
                "Option Effects");

            EditorGUILayout.Space(4);

            if (GUILayout.Button("Remove Choice Option"))
            {
                choiceOptions.DeleteArrayElementAtIndex(i);
                EditorGUILayout.EndVertical();
                break;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(4);
        }

        if (GUILayout.Button("Add Choice Option"))
        {
            AddChoiceOption();
        }
    }

    private void AddChoiceOption()
    {
        serializedObject.Update();

        int index = choiceOptions.arraySize;

        choiceOptions.InsertArrayElementAtIndex(index);

        SerializedProperty option =
            choiceOptions.GetArrayElementAtIndex(index);

        option.FindPropertyRelative("OptionId").stringValue = string.Empty;

        option.FindPropertyRelative("DisplayName").stringValue = string.Empty;

        option.FindPropertyRelative("Description").stringValue = string.Empty;

        option.FindPropertyRelative("Icon").objectReferenceValue = null;

        SerializedProperty effects =
            option.FindPropertyRelative("Effects");

        effects.ClearArray();

        serializedObject.ApplyModifiedProperties();
    }
}