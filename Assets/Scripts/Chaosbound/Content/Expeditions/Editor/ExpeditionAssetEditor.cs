using UnityEditor;
using UnityEngine;
using Chaosbound.Content.Expeditions.Assets;
using Chaosbound.Shared.Enums;

[CustomEditor(typeof(ExpeditionAsset))]
public sealed class ExpeditionAssetEditor : Editor
{
    private static readonly string[] TierLabels =
    {
        "Tier 1",
        "Tier 2",
        "Tier 3",
        "Tier 4"
    };

    private static readonly EnemyTier[] Tiers =
    {
        EnemyTier.Tier1,
        EnemyTier.Tier2,
        EnemyTier.Tier3,
        EnemyTier.Tier4
    };

    private static readonly EnemyCombatType[] CombatTypes =
    {
        EnemyCombatType.Melee,
        EnemyCombatType.Ranged
    };

    private SerializedProperty m_ExpeditionProperty;
    private SerializedProperty m_EnemyContentProperty;
    private UnityEngine.Object[] m_AddSelections = new UnityEngine.Object[8];

    private bool[] m_TierFoldouts = new bool[4];
    private bool[,] m_CombatFoldouts = new bool[4, 2];
    private bool m_UnclassifiedFoldout;

    private void OnEnable()
    {
        m_ExpeditionProperty = serializedObject.FindProperty("m_expedition");

        if (m_ExpeditionProperty == null)
            return;

        SerializedProperty enemyProperty =
            m_ExpeditionProperty.FindPropertyRelative("m_enemy");

        if (enemyProperty != null)
        {
            m_EnemyContentProperty =
                enemyProperty.FindPropertyRelative("m_Content");
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (m_ExpeditionProperty == null)
        {
            EditorGUILayout.HelpBox(
                "No se encontró el campo m_expedition en ExpeditionAsset.",
                MessageType.Error);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        DrawExpeditionSections();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawExpeditionSections()
    {
        SerializedProperty iterator = m_ExpeditionProperty.Copy();
        SerializedProperty endProperty = iterator.GetEndProperty();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren) &&
               !SerializedProperty.EqualContents(iterator, endProperty))
        {
            enterChildren = false;

            if (iterator.name == "m_enemy")
            {
                DrawEnemyContent();
            }
            else
            {
                EditorGUILayout.PropertyField(iterator, true);
            }
        }
    }

    private void DrawEnemyContent()
    {
        EditorGUILayout.Space(4f);
        EditorGUILayout.LabelField("Enemy", EditorStyles.boldLabel);

        if (m_EnemyContentProperty == null)
        {
            EditorGUILayout.HelpBox(
                "No se encontró EnemyAuthoring.m_Content. " +
                "Comprueba la estructura de EnemyAuthoring.",
                MessageType.Error);
            return;
        }

        for (int tierIndex = 0; tierIndex < Tiers.Length; tierIndex++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            int tierCount = CountVariants(Tiers[tierIndex]);

            m_TierFoldouts[tierIndex] = EditorGUILayout.Foldout(
                m_TierFoldouts[tierIndex],
                TierLabels[tierIndex] + " (" + tierCount + ")",
                true,
                EditorStyles.foldoutHeader);

            if (m_TierFoldouts[tierIndex])
            {
                for (int combatIndex = 0;
                     combatIndex < CombatTypes.Length;
                     combatIndex++)
                {
                    DrawCombatGroup(
                        tierIndex,
                        combatIndex,
                        Tiers[tierIndex],
                        CombatTypes[combatIndex]);
                }
            }

            EditorGUILayout.EndVertical();
        }

        DrawUnclassifiedEntries();
    }

    private void DrawCombatGroup(
        int tierIndex,
        int combatIndex,
        EnemyTier tier,
        EnemyCombatType combatType)
    {
        EditorGUILayout.Space(2f);

        int matchingCount = CountVariants(tier, combatType);

        m_CombatFoldouts[tierIndex, combatIndex] =
            EditorGUILayout.Foldout(
                m_CombatFoldouts[tierIndex, combatIndex],
                combatType + " (" + matchingCount + ")",
                true);

        if (!m_CombatFoldouts[tierIndex, combatIndex])
            return;

        EditorGUI.indentLevel++;

        if (matchingCount == 0)
        {
            EditorGUILayout.LabelField(
                "Sin variantes asignadas.",
                EditorStyles.miniLabel);
        }
        else
        {
            for (int i = 0; i < m_EnemyContentProperty.arraySize; i++)
            {
                SerializedProperty element =
                    m_EnemyContentProperty.GetArrayElementAtIndex(i);

                EnemyVariantData variant =
                    element.objectReferenceValue as EnemyVariantData;

                if (variant != null && Matches(variant, tier, combatType))
                    DrawVariantRow(i, variant);
            }
        }

        DrawAddField(tierIndex, combatType);

        EditorGUI.indentLevel--;
    }

    private int CountVariants(EnemyTier tier)
    {
        int count = 0;

        for (int i = 0; i < m_EnemyContentProperty.arraySize; i++)
        {
            EnemyVariantData variant =
                m_EnemyContentProperty.GetArrayElementAtIndex(i)
                    .objectReferenceValue as EnemyVariantData;

            if (variant != null && variant.Tier == tier)
                count++;
        }

        return count;
    }

    private int CountVariants(
        EnemyTier tier,
        EnemyCombatType combatType)
    {
        int count = 0;

        for (int i = 0; i < m_EnemyContentProperty.arraySize; i++)
        {
            EnemyVariantData variant =
                m_EnemyContentProperty.GetArrayElementAtIndex(i)
                    .objectReferenceValue as EnemyVariantData;

            if (variant != null && Matches(variant, tier, combatType))
                count++;
        }

        return count;
    }

    private void DrawVariantRow(int arrayIndex, EnemyVariantData variant)
    {
        EditorGUILayout.BeginHorizontal();

        SerializedProperty element =
            m_EnemyContentProperty.GetArrayElementAtIndex(arrayIndex);

        UnityEngine.Object current = element.objectReferenceValue;
        UnityEngine.Object updated = EditorGUILayout.ObjectField(
            GetVariantLabel(variant),
            current,
            typeof(EnemyVariantData),
            false);

        if (updated != current)
            element.objectReferenceValue = updated;

        if (GUILayout.Button("−", GUILayout.Width(24f)))
        {
            element.objectReferenceValue = null;
            m_EnemyContentProperty.DeleteArrayElementAtIndex(arrayIndex);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawAddField(
        int tierIndex,
        EnemyCombatType combatType)
    {
        int selectionIndex = tierIndex * 2 +
            (combatType == EnemyCombatType.Ranged ? 1 : 0);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            "Añadir variante",
            GUILayout.Width(105f));

        UnityEngine.Object selection = EditorGUILayout.ObjectField(
            m_AddSelections[selectionIndex],
            typeof(EnemyVariantData),
            false);

        m_AddSelections[selectionIndex] = selection;

        if (selection != null)
        {
            EnemyVariantData variant = selection as EnemyVariantData;

            if (variant == null)
            {
                EditorGUILayout.HelpBox(
                    "El objeto debe ser un EnemyVariantData.",
                    MessageType.Warning);
            }
            else if (!Matches(variant, Tiers[tierIndex], combatType))
            {
                EditorGUILayout.HelpBox(
                    "El Tier o Combat Type del asset no coincide con este grupo.",
                    MessageType.Warning);
            }
            else
            {
                bool alreadyAssigned = IsAlreadyAssigned(variant);

                if (alreadyAssigned)
                {
                    EditorGUILayout.LabelField(
                        "Ya está asignado",
                        EditorStyles.miniLabel,
                        GUILayout.Width(95f));
                }
                else if (GUILayout.Button("Agregar", GUILayout.Width(70f)))
                {
                    int newIndex = m_EnemyContentProperty.arraySize;
                    m_EnemyContentProperty.InsertArrayElementAtIndex(newIndex);
                    m_EnemyContentProperty.GetArrayElementAtIndex(newIndex)
                        .objectReferenceValue = variant;

                    m_AddSelections[selectionIndex] = null;
                    GUI.FocusControl(null);
                }
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawUnclassifiedEntries()
    {
        bool hasUnclassified = false;

        for (int i = 0; i < m_EnemyContentProperty.arraySize; i++)
        {
            SerializedProperty element =
                m_EnemyContentProperty.GetArrayElementAtIndex(i);

            EnemyVariantData variant =
                element.objectReferenceValue as EnemyVariantData;

            if (variant == null ||
                !IsSupportedTier(variant.Tier) ||
                !IsSupportedCombatType(variant.CombatType))
            {
                hasUnclassified = true;
                break;
            }
        }

        if (!hasUnclassified)
            return;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        m_UnclassifiedFoldout = EditorGUILayout.Foldout(
            m_UnclassifiedFoldout,
            "Sin clasificar / Referencias vacías",
            true,
            EditorStyles.foldoutHeader);

        if (!m_UnclassifiedFoldout)
        {
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.HelpBox(
            "Estas entradas no pueden colocarse en los grupos Tier/Combat Type. " +
            "Asigna un asset válido y revisa su clasificación.",
            MessageType.Warning);

        for (int i = 0; i < m_EnemyContentProperty.arraySize; i++)
        {
            SerializedProperty element =
                m_EnemyContentProperty.GetArrayElementAtIndex(i);

            EnemyVariantData variant =
                element.objectReferenceValue as EnemyVariantData;

            if (variant != null &&
                IsSupportedTier(variant.Tier) &&
                IsSupportedCombatType(variant.CombatType))
                continue;

            DrawVariantRow(i, variant);
        }

        EditorGUILayout.EndVertical();
    }

    private bool IsAlreadyAssigned(EnemyVariantData candidate)
    {
        for (int i = 0; i < m_EnemyContentProperty.arraySize; i++)
        {
            SerializedProperty element =
                m_EnemyContentProperty.GetArrayElementAtIndex(i);

            if (element.objectReferenceValue == candidate)
                return true;
        }

        return false;
    }

    private static bool Matches(
        EnemyVariantData variant,
        EnemyTier tier,
        EnemyCombatType combatType)
    {
        return variant.Tier == tier &&
               variant.CombatType == combatType;
    }

    private static bool IsSupportedTier(EnemyTier tier)
    {
        return tier == EnemyTier.Tier1 ||
               tier == EnemyTier.Tier2 ||
               tier == EnemyTier.Tier3 ||
               tier == EnemyTier.Tier4;
    }

    private static bool IsSupportedCombatType(
        EnemyCombatType combatType)
    {
        return combatType == EnemyCombatType.Melee ||
               combatType == EnemyCombatType.Ranged;
    }

    private static string GetVariantLabel(EnemyVariantData variant)
    {
        if (variant == null)
            return "Missing reference";

        string roleLabel = GetRoleLabel(variant);
        return string.IsNullOrEmpty(roleLabel)
            ? variant.name
            : variant.name + "  [" + roleLabel + "]";
    }

    private static string GetRoleLabel(EnemyVariantData variant)
    {
        SerializedObject variantObject = new SerializedObject(variant);
        variantObject.Update();

        SerializedProperty rolesProperty =
            variantObject.FindProperty("m_Roles");

        if (rolesProperty == null || !rolesProperty.isArray ||
            rolesProperty.arraySize == 0)
            return "Role sin asignar";

        string[] roleNames = new string[rolesProperty.arraySize];

        for (int i = 0; i < rolesProperty.arraySize; i++)
        {
            SerializedProperty role =
                rolesProperty.GetArrayElementAtIndex(i);

            roleNames[i] = role.enumDisplayNames[role.enumValueIndex];
        }

        return string.Join(", ", roleNames);
    }
}