using UnityEditor;
using UnityEngine;

namespace Chaosbound.Content.World.Themes.TileSets
{
    [CustomPropertyDrawer(typeof(MinimapTileMask))]
    public sealed class MinimapTileMaskDrawer :
        PropertyDrawer
    {
        private const float CellSize = 28f;
        private const float Spacing = 2f;
        private const float LabelWidth = 24f;

        private const float HeaderHeight = 18f;
        private const float FooterHeight = 18f;

        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            SerializedProperty blockedCells =
                property.FindPropertyRelative(
                    "blockedCells");

            if (blockedCells == null)
            {
                EditorGUI.LabelField(
                    position,
                    "Invalid MinimapTileMask data.");

                return;
            }

            EditorGUI.BeginProperty(
                position,
                label,
                property);

            Rect contentRect =
                EditorGUI.PrefixLabel(
                    position,
                    label);

            float gridWidth =
                (CellSize * MinimapTileMask.Resolution) +
                (Spacing * (MinimapTileMask.Resolution - 1));

            float gridHeight =
                gridWidth;

            Rect gridRect =
                new Rect(
                    contentRect.x +
                    LabelWidth,
                    contentRect.y +
                    HeaderHeight,
                    gridWidth,
                    gridHeight);

            DrawOrientationLabels(
                contentRect,
                gridRect);

            DrawGrid(
                gridRect,
                blockedCells);

            EditorGUI.EndProperty();
        }

        private void DrawOrientationLabels(
            Rect contentRect,
            Rect gridRect)
        {
            GUIStyle centeredStyle =
                new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment =
                        TextAnchor.MiddleCenter
                };

            Rect northRect =
                new Rect(
                    gridRect.x,
                    contentRect.y,
                    gridRect.width,
                    HeaderHeight);

            GUI.Label(
                northRect,
                "+Z",
                centeredStyle);

            Rect southRect =
                new Rect(
                    gridRect.x,
                    gridRect.y +
                    gridRect.height,
                    gridRect.width,
                    FooterHeight);

            GUI.Label(
                southRect,
                "-Z",
                centeredStyle);

            Rect westRect =
                new Rect(
                    contentRect.x,
                    gridRect.y,
                    LabelWidth,
                    gridRect.height);

            GUI.Label(
                westRect,
                "-X",
                centeredStyle);

            Rect eastRect =
                new Rect(
                    gridRect.x +
                    gridRect.width,
                    gridRect.y,
                    LabelWidth,
                    gridRect.height);

            GUI.Label(
                eastRect,
                "+X",
                centeredStyle);
        }

        private void DrawGrid(
            Rect gridRect,
            SerializedProperty blockedCells)
        {
            int resolution =
                MinimapTileMask.Resolution;

            float cellSize =
                (gridRect.width -
                 (Spacing * (resolution - 1))) /
                resolution;

            for (int visualRow = 0;
                 visualRow < resolution;
                 visualRow++)
            {
                /*
                 * Visual row 0 represents +Z.
                 *
                 * Internal z=3 is +Z.
                 * Internal z=0 is -Z.
                 */
                int z =
                    resolution -
                    1 -
                    visualRow;

                for (int x = 0;
                     x < resolution;
                     x++)
                {
                    int index =
                        (z * resolution) +
                        x;

                    if (index < 0 ||
                        index >= blockedCells.arraySize)
                    {
                        continue;
                    }

                    Rect cellRect =
                        new Rect(
                            gridRect.x +
                            x * (cellSize + Spacing),
                            gridRect.y +
                            visualRow *
                            (cellSize + Spacing),
                            cellSize,
                            cellSize);

                    SerializedProperty cell =
                        blockedCells.GetArrayElementAtIndex(
                            index);

                    bool blocked =
                        cell.boolValue;

                    string text =
                        blocked
                            ? "■"
                            : "·";

                    if (
                        GUI.Button(
                            cellRect,
                            text))
                    {
                        cell.boolValue =
                            !cell.boolValue;
                    }
                }
            }
        }

        public override float GetPropertyHeight(
            SerializedProperty property,
            GUIContent label)
        {
            float gridHeight =
                (CellSize *
                 MinimapTileMask.Resolution) +
                (Spacing *
                 (MinimapTileMask.Resolution - 1));

            return
                EditorGUIUtility.singleLineHeight +
                HeaderHeight +
                gridHeight +
                FooterHeight +
                4f;
        }
    }
}