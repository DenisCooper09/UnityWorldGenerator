using UnityEditor;
using UnityEngine;

namespace ProceduralGeneration
{
    [CustomEditor(typeof(NoiseConfig))]
    internal sealed class NoiseConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var noiseConfig = (NoiseConfig)target;

            if (GUILayout.Button("Generate Preview"))
            {
                noiseConfig.GeneratePreviewTexture();
            }

            if (noiseConfig.PreviewTexture != null)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Preview");
                Rect previewRect = GUILayoutUtility.GetRect(noiseConfig.PreviewTexture.width * 3, noiseConfig.PreviewTexture.height * 3);
                GUI.DrawTexture(previewRect, noiseConfig.PreviewTexture, ScaleMode.ScaleToFit, false, 0f);
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}
