using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HealthOrbFillAnimator))]
public sealed class HealthOrbFillAnimatorEditor : Editor
{
    private SerializedProperty color;
    private SerializedProperty brightness;
    private SerializedProperty contrast;
    private SerializedProperty noiseBlend;

    private SerializedProperty fillTexture;
    private SerializedProperty tiling;
    private SerializedProperty scrollSpeed;

    private SerializedProperty fillTexture2;
    private SerializedProperty tiling2;
    private SerializedProperty scrollSpeed2;

    private SerializedProperty animate;

    private bool showAppearance = true;
    private bool showLayer1 = true;
    private bool showLayer2 = true;
    private bool showAnimation = true;

    private void OnEnable()
    {
        color =
            serializedObject.FindProperty(
                "color");

        brightness =
            serializedObject.FindProperty(
                "brightness");

        contrast =
            serializedObject.FindProperty(
                "contrast");

        noiseBlend =
            serializedObject.FindProperty(
                "noiseBlend");

        fillTexture =
            serializedObject.FindProperty(
                "fillTexture");

        tiling =
            serializedObject.FindProperty(
                "tiling");

        scrollSpeed =
            serializedObject.FindProperty(
                "scrollSpeed");

        fillTexture2 =
            serializedObject.FindProperty(
                "fillTexture2");

        tiling2 =
            serializedObject.FindProperty(
                "tiling2");

        scrollSpeed2 =
            serializedObject.FindProperty(
                "scrollSpeed2");

        animate =
            serializedObject.FindProperty(
                "animate");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(4);

        DrawAppearance();

        EditorGUILayout.Space(4);

        DrawLayer1();

        EditorGUILayout.Space(4);

        DrawLayer2();

        EditorGUILayout.Space(4);

        DrawAnimation();

        EditorGUILayout.Space(8);

        DrawQuickActions();

        bool changed =
            serializedObject.ApplyModifiedProperties();

        if (changed && Application.isPlaying)
        {
            HealthOrbFillAnimator animator =
                (HealthOrbFillAnimator)target;

            animator.RefreshMaterial();
        }
    }

    private void DrawAppearance()
    {
        showAppearance =
            EditorGUILayout.BeginFoldoutHeaderGroup(
                showAppearance,
                "Appearance");

        if (showAppearance)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(
                color,
                new GUIContent(
                    "Color"));

            EditorGUILayout.Slider(
                brightness,
                0f,
                3f,
                new GUIContent(
                    "Brightness"));

            EditorGUILayout.Slider(
                contrast,
                0f,
                3f,
                new GUIContent(
                    "Contrast"));

            EditorGUILayout.Slider(
                noiseBlend,
                0f,
                1f,
                new GUIContent(
                    "Noise Blend"));

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawLayer1()
    {
        showLayer1 =
            EditorGUILayout.BeginFoldoutHeaderGroup(
                showLayer1,
                "Layer 1");

        if (showLayer1)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(
                fillTexture,
                new GUIContent(
                    "Fill Texture"));

            EditorGUILayout.PropertyField(
                tiling,
                new GUIContent(
                    "Tiling"));

            EditorGUILayout.PropertyField(
                scrollSpeed,
                new GUIContent(
                    "Scroll Speed"));

            DrawVerticalOnlyHint(
                scrollSpeed);

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawLayer2()
    {
        showLayer2 =
            EditorGUILayout.BeginFoldoutHeaderGroup(
                showLayer2,
                "Layer 2");

        if (showLayer2)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(
                fillTexture2,
                new GUIContent(
                    "Fill Texture 2"));

            EditorGUILayout.PropertyField(
                tiling2,
                new GUIContent(
                    "Tiling"));

            EditorGUILayout.PropertyField(
                scrollSpeed2,
                new GUIContent(
                    "Scroll Speed"));

            DrawVerticalOnlyHint(
                scrollSpeed2);

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawAnimation()
    {
        showAnimation =
            EditorGUILayout.BeginFoldoutHeaderGroup(
                showAnimation,
                "Animation");

        if (showAnimation)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(
                animate,
                new GUIContent(
                    "Animate"));

            EditorGUILayout.HelpBox(
                "Animation is driven by the shader using the configured Scroll Speed values.",
                MessageType.Info);

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawVerticalOnlyHint(
        SerializedProperty speed)
    {
        Vector2 value =
            speed.vector2Value;

        if (!Mathf.Approximately(value.x, 0f))
        {
            EditorGUILayout.HelpBox(
                "X movement is enabled. For the current Health Orb design, X should normally remain 0.",
                MessageType.Warning);
        }
    }

    private void DrawQuickActions()
    {
        EditorGUILayout.LabelField(
            "Quick Actions",
            EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(
            "Stop Horizontal Movement"))
        {
            SetScrollX(
                scrollSpeed,
                0f);

            SetScrollX(
                scrollSpeed2,
                0f);
        }

        if (GUILayout.Button(
            "Stop Animation"))
        {
            animate.boolValue = false;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(
            "Enable Animation"))
        {
            animate.boolValue = true;
        }

        if (GUILayout.Button(
            "Reset Speeds"))
        {
            scrollSpeed.vector2Value =
                new Vector2(
                    0f,
                    0.08f);

            scrollSpeed2.vector2Value =
                new Vector2(
                    0f,
                    -0.045f);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void SetScrollX(
        SerializedProperty property,
        float value)
    {
        Vector2 current =
            property.vector2Value;

        current.x = value;

        property.vector2Value =
            current;
    }
}